using AcademicPlanner.Data;
using AcademicPlanner.Models;

namespace AcademicPlanner.Views;

[QueryProperty(nameof(TermId), "termId")]
[QueryProperty(nameof(CourseId), "courseId")]
public partial class CourseEditPage : ContentPage
{
    private readonly AcademicPlannerDatabase _database;

    private int _termId;
    private int _courseId;

    public string TermId
    {
        set
        {
            if (int.TryParse(Uri.UnescapeDataString(value), out int id))
            {
                _termId = id;
            }
        }
    }

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

    public CourseEditPage(AcademicPlannerDatabase database)
    {
        InitializeComponent();
        _database = database;

        StartDatePicker.Date = DateTime.Today;
        EndDatePicker.Date = DateTime.Today.AddMonths(1);
    }

    private async Task LoadCourseAsync()
    {
        var course = await _database.GetCourseAsync(_courseId);
        if (course is null)
            return;

        _termId = course.TermId;

        TitleEntry.Text = course.Title;
        StartDatePicker.Date = course.StartDate;
        EndDatePicker.Date = course.EndDate;
    }

    private async void OnSaveClicked(object? sender, EventArgs e)
    {
        string title = TitleEntry.Text?.Trim() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(title))
        {
            await DisplayAlert("Validation Error", "Course title is required.", "OK");
            return;
        }

        if (StartDatePicker.Date > EndDatePicker.Date)
        {
            await DisplayAlert("Validation Error", "Start date cannot be after end date.", "OK");
            return;
        }

        Course course = new()
        {
            Id = _courseId,
            TermId = _termId,
            Title = title,
            StartDate = StartDatePicker.Date,
            EndDate = EndDatePicker.Date
        };

        await _database.SaveCourseAsync(course);
        await Shell.Current.GoToAsync("..");
    }

    private async void OnCancelClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }

    private async void OnBackClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }

    private async void OnAddNewClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync($"{nameof(CourseEditPage)}?termId={_termId}");
    }
}