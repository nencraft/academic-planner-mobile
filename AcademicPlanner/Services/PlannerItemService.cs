using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AcademicPlanner.Data;
using AcademicPlanner.Models;

namespace AcademicPlanner.Services;

public class PlannerItemService
{
    private readonly AcademicPlannerDatabase _database;

    public PlannerItemService(AcademicPlannerDatabase database)
    {
        _database = database;
    }

    public async Task<List<PlannerItem>> GetPlannerItemsAsync()
    {
        var terms = await _database.GetTermsAsync();
        var courses = await _database.GetAllCoursesAsync();
        var assessments = await _database.GetAllAssessmentsAsync();

        var validTermIds = terms
            .Select(term => term.Id)
            .ToHashSet();

        var validCourses = courses
            .Where(course => validTermIds.Contains(course.TermId))
            .ToList();

        var validCourseIds = validCourses
            .Select(course => course.Id)
            .ToHashSet();

        var validAssessments = assessments
            .Where(assessment => validCourseIds.Contains(assessment.CourseId))
            .ToList();

        var courseTitleById = validCourses.ToDictionary(
            course => course.Id,
            course => course.Title);

        var plannerItems = new List<PlannerItem>();

        plannerItems.AddRange(terms.Select(term => new TermPlannerItem
        {
            SourceId = term.Id,
            Title = term.Title,
            StartDate = term.StartDate,
            EndDate = term.EndDate
        }));

        plannerItems.AddRange(validCourses.Select(course => new CoursePlannerItem
        {
            SourceId = course.Id,
            Title = course.Title,
            StartDate = course.StartDate,
            EndDate = course.EndDate,
            StatusValue = course.Status,
            InstructorName = course.InstructorName
        }));

        plannerItems.AddRange(validAssessments.Select(assessment => new AssessmentPlannerItem
        {
            SourceId = assessment.Id,
            Title = assessment.Title,
            StartDate = assessment.StartDate,
            EndDate = assessment.EndDate,
            AssessmentType = assessment.Type,
            ParentCourseTitle = courseTitleById[assessment.CourseId]
        }));

        return plannerItems;
    }
}