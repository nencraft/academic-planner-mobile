using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AcademicPlanner.Data;
using AcademicPlanner.Models;

namespace AcademicPlanner.Services;

public class SearchService
{
    private readonly AcademicPlannerDatabase _database;

    public SearchService(AcademicPlannerDatabase database)
    {
        _database = database;
    }

    public async Task<List<PlannerItem>> SearchAsync(string query)
    {
        query = query?.Trim().ToLowerInvariant() ?? string.Empty;

        var terms = await _database.GetTermsAsync();
        var courses = await _database.GetAllCoursesAsync();
        var assessments = await _database.GetAllAssessmentsAsync();

        var termResults = terms
            .Where(t => string.IsNullOrWhiteSpace(query) || t.Title.ToLowerInvariant().Contains(query))
            .Select(t => new TermPlannerItem
            {
                SourceId = t.Id,
                Title = t.Title,
                StartDate = t.StartDate,
                EndDate = t.EndDate
            });

        var courseResults = courses
            .Where(c =>
                string.IsNullOrWhiteSpace(query) ||
                c.Title.ToLowerInvariant().Contains(query) ||
                c.InstructorName.ToLowerInvariant().Contains(query) ||
                c.Status.ToLowerInvariant().Contains(query))
            .Select(c => new CoursePlannerItem
            {
                SourceId = c.Id,
                Title = c.Title,
                StartDate = c.StartDate,
                EndDate = c.EndDate,
                StatusValue = c.Status,
                InstructorName = c.InstructorName
            });

        var courseMap = courses.ToDictionary(c => c.Id, c => c.Title);

        var assessmentResults = assessments
            .Where(a =>
                string.IsNullOrWhiteSpace(query) ||
                a.Title.ToLowerInvariant().Contains(query) ||
                a.Type.ToLowerInvariant().Contains(query))
            .Select(a => new AssessmentPlannerItem
            {
                SourceId = a.Id,
                Title = a.Title,
                StartDate = a.StartDate,
                EndDate = a.EndDate,
                AssessmentType = a.Type,
                ParentCourseTitle = courseMap.TryGetValue(a.CourseId, out var courseTitle)
                    ? courseTitle
                    : "Unknown Course"
            });

        return termResults
            .Cast<PlannerItem>()
            .Concat(courseResults)
            .Concat(assessmentResults)
            .OrderBy(i => i.StartDate)
            .ToList();
    }
}