using AcademicPlanner.Data;
using AcademicPlanner.Models;

namespace AcademicPlanner.Views;

[QueryProperty(nameof(CourseId), "courseId")]
public partial class CourseOverviewPage : ContentPage
{
    private readonly AcademicPlannerDatabase _database;
    private int _courseId;
    private Course? _course;

    public string CourseId
    {
        set
        {
            if (int.TryParse(Uri.UnescapeDataString(value), out int id))
            {
                _courseId = id;
                _ = LoadCourseAsync();
            }
        }
    }

    public CourseOverviewPage(AcademicPlannerDatabase database)
    {
        InitializeComponent();
        _database = database;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (_courseId != 0)
            await LoadCourseAsync();
    }

    private async Task LoadCourseAsync()
    {
        _course = await _database.GetCourseAsync(_courseId);
        if (_course is null)
            return;

        CourseTitleLabel.Text = _course.Title;
        CourseStatusLabel.Text = $"Status: {_course.Status}";
        CourseStartDateLabel.Text = $"Start Date: {_course.StartDate:MM/dd/yyyy}";
        CourseEndDateLabel.Text = $"Anticipated End / Due Date: {_course.EndDate:MM/dd/yyyy}";
        InstructorNameLabel.Text = $"Instructor Name: {_course.InstructorName}";
        InstructorPhoneLabel.Text = $"Phone: {_course.InstructorPhone}";
        InstructorEmailLabel.Text = $"Email: {_course.InstructorEmail}";
    }

    private async void OnBackClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }

    private async void OnEditCourseClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync($"{nameof(CourseEditPage)}?courseId={_courseId}");
    }

    private async void OnDeleteCourseClicked(object? sender, EventArgs e)
    {
        if (_course is null)
            return;

        bool confirm = await DisplayAlert(
            "Delete Course",
            $"Delete '{_course.Title}'?",
            "Yes",
            "No");

        if (!confirm)
            return;

        await _database.DeleteCourseAsync(_course);
        await Shell.Current.GoToAsync("..");
    }

    private async void OnAlertClicked(object? sender, EventArgs e)
    {
        await DisplayAlert("Alerts", "To do: alerts", "OK");
    }
}