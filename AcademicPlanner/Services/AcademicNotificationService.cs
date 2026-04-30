using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AcademicPlanner.Data;
using AcademicPlanner.Models;

namespace AcademicPlanner.Services;

public class AcademicNotificationService
{
    private const int CourseStartNotificationBase = 100_000;
    private const int CourseEndNotificationBase = 200_000;
    private const int AssessmentStartNotificationBase = 300_000;
    private const int AssessmentEndNotificationBase = 400_000;

    private readonly AcademicPlannerDatabase _database;
    private readonly INotificationManagerService _notificationManager;

    public AcademicNotificationService(
        AcademicPlannerDatabase database,
        INotificationManagerService notificationManager)
    {
        _database = database;
        _notificationManager = notificationManager;
    }

    public async Task ApplyCourseNotificationsAsync(Course course)
    {
#if ANDROID
        if (course.Id == 0)
            return;

        int startId = GetCourseStartNotificationId(course.Id);
        int endId = GetCourseEndNotificationId(course.Id);

        CancelNotificationIfPresent(course.StartNotificationId);
        CancelNotificationIfPresent(course.EndNotificationId);
        _notificationManager.CancelNotification(startId);
        _notificationManager.CancelNotification(endId);

        course.StartNotificationId = null;
        course.EndNotificationId = null;

        if (course.AlertSetting == "None")
        {
            await _database.SaveCourseAsync(course);
            return;
        }

        var permission = await Permissions.RequestAsync<AcademicPlanner.Platforms.Android.NotificationPermission>();

        if (permission != PermissionStatus.Granted)
        {
            await _database.SaveCourseAsync(course);
            return;
        }

        if (course.AlertSetting == "Start" || course.AlertSetting == "Start and End")
        {
            _notificationManager.SendNotification(
                startId,
                "Course Start Reminder",
                $"{course.Title} starts today.",
                course.StartDate.Date.AddHours(9));

            course.StartNotificationId = startId;
        }

        if (course.AlertSetting == "End" || course.AlertSetting == "Start and End")
        {
            _notificationManager.SendNotification(
                endId,
                "Course End Reminder",
                $"{course.Title} ends today.",
                course.EndDate.Date.AddHours(9));

            course.EndNotificationId = endId;
        }

        await _database.SaveCourseAsync(course);
#else
        await Task.CompletedTask;
#endif
    }

    public async Task ApplyAssessmentNotificationsAsync(Assessment assessment)
    {
#if ANDROID
        if (assessment.Id == 0)
            return;

        int startId = GetAssessmentStartNotificationId(assessment.Id);
        int endId = GetAssessmentEndNotificationId(assessment.Id);

        CancelNotificationIfPresent(assessment.StartNotificationId);
        CancelNotificationIfPresent(assessment.EndNotificationId);
        _notificationManager.CancelNotification(startId);
        _notificationManager.CancelNotification(endId);

        assessment.StartNotificationId = null;
        assessment.EndNotificationId = null;

        if (assessment.AlertSetting == "None")
        {
            await _database.SaveAssessmentAsync(assessment);
            return;
        }

        var permission = await Permissions.RequestAsync<AcademicPlanner.Platforms.Android.NotificationPermission>();

        if (permission != PermissionStatus.Granted)
        {
            await _database.SaveAssessmentAsync(assessment);
            return;
        }

        if (assessment.AlertSetting == "Start" || assessment.AlertSetting == "Start and End")
        {
            _notificationManager.SendNotification(
                startId,
                "Assessment Start Reminder",
                $"{assessment.Title} starts today.",
                assessment.StartDate.Date.AddHours(9));

            assessment.StartNotificationId = startId;
        }

        if (assessment.AlertSetting == "End" || assessment.AlertSetting == "Start and End")
        {
            _notificationManager.SendNotification(
                endId,
                "Assessment Due Reminder",
                $"{assessment.Title} is due today.",
                assessment.EndDate.Date.AddHours(9));

            assessment.EndNotificationId = endId;
        }

        await _database.SaveAssessmentAsync(assessment);
#else
        await Task.CompletedTask;
#endif
    }

    public async Task CancelCourseNotificationsAsync(int courseId)
    {
        var course = await _database.GetCourseAsync(courseId);

        if (course is null)
            return;

        CancelCourseNotifications(course);

        var assessments = await _database.GetAssessmentsByCourseAsync(courseId);

        foreach (var assessment in assessments)
        {
            CancelAssessmentNotifications(assessment);
        }
    }

    public async Task CancelTermNotificationsAsync(int termId)
    {
        var courses = await _database.GetCoursesByTermAsync(termId);

        foreach (var course in courses)
        {
            await CancelCourseNotificationsAsync(course.Id);
        }
    }

    public async Task CancelAssessmentNotificationsAsync(int assessmentId)
    {
        var assessment = await _database.GetAssessmentAsync(assessmentId);

        if (assessment is null)
            return;

        CancelAssessmentNotifications(assessment);
    }

    private void CancelCourseNotifications(Course course)
    {
        CancelNotificationIfPresent(course.StartNotificationId);
        CancelNotificationIfPresent(course.EndNotificationId);

        if (course.Id != 0)
        {
            _notificationManager.CancelNotification(GetCourseStartNotificationId(course.Id));
            _notificationManager.CancelNotification(GetCourseEndNotificationId(course.Id));
        }
    }

    private void CancelAssessmentNotifications(Assessment assessment)
    {
        CancelNotificationIfPresent(assessment.StartNotificationId);
        CancelNotificationIfPresent(assessment.EndNotificationId);

        if (assessment.Id != 0)
        {
            _notificationManager.CancelNotification(GetAssessmentStartNotificationId(assessment.Id));
            _notificationManager.CancelNotification(GetAssessmentEndNotificationId(assessment.Id));
        }
    }

    private void CancelNotificationIfPresent(int? notificationId)
    {
        if (notificationId.HasValue)
        {
            _notificationManager.CancelNotification(notificationId.Value);
        }
    }

    private static int GetCourseStartNotificationId(int courseId) =>
        CourseStartNotificationBase + courseId;

    private static int GetCourseEndNotificationId(int courseId) =>
        CourseEndNotificationBase + courseId;

    private static int GetAssessmentStartNotificationId(int assessmentId) =>
        AssessmentStartNotificationBase + assessmentId;

    private static int GetAssessmentEndNotificationId(int assessmentId) =>
        AssessmentEndNotificationBase + assessmentId;
}
