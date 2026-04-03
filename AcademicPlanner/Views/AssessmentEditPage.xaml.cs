using AcademicPlanner.Data;
using AcademicPlanner.Models;

namespace AcademicPlanner.Views;

[QueryProperty(nameof(CourseId), "courseId")]
[QueryProperty(nameof(AssessmentId), "assessmentId")]
public partial class AssessmentEditPage : ContentPage
{
    private readonly AcademicPlannerDatabase _database;

    private int _courseId;
    private int _assessmentId;

    public string CourseId
    {
        set
        {
            if (int.TryParse(Uri.UnescapeDataString(value), out int id))
            {
                _courseId = id;
            }
        }
    }

    public string AssessmentId
    {
        set
        {
            if (int.TryParse(Uri.UnescapeDataString(value), out int id))
            {
                _assessmentId = id;
                _ = LoadAssessmentAsync();
            }
        }
    }

    public AssessmentEditPage(AcademicPlannerDatabase database)
    {
        InitializeComponent();
        _database = database;

        StartDatePicker.Date = DateTime.Today;
        EndDatePicker.Date = DateTime.Today.AddMonths(1);

        AssessmentTypePicker.SelectedIndex = -1;
        AlertSettingPicker.SelectedIndex = 0; 
    }

    private async Task LoadAssessmentAsync()
    {
        var assessment = await _database.GetAssessmentAsync(_assessmentId);
        if (assessment is null)
            return;

        _courseId = assessment.CourseId;

        BannerTitleLabel.Text = "Edit Assessment";
        TitleEntry.Text = assessment.Title;
        StartDatePicker.Date = assessment.StartDate;
        EndDatePicker.Date = assessment.EndDate;

        if (!string.IsNullOrWhiteSpace(assessment.Type))
            AssessmentTypePicker.SelectedItem = assessment.Type;

        if (!string.IsNullOrWhiteSpace(assessment.AlertSetting))
            AlertSettingPicker.SelectedItem = assessment.AlertSetting;

        DeleteButton.IsVisible = true;
    }

    private async void OnSaveClicked(object? sender, EventArgs e)
    {
        string title = TitleEntry.Text?.Trim() ?? string.Empty;
        string type = AssessmentTypePicker.SelectedItem?.ToString() ?? string.Empty;
        string alertSetting = AlertSettingPicker.SelectedItem?.ToString() ?? "None";

        if (string.IsNullOrWhiteSpace(title))
        {
            await DisplayAlert("Validation Error", "Assessment title is required.", "OK");
            return;
        }

        if (string.IsNullOrWhiteSpace(type))
        {
            await DisplayAlert("Validation Error", "Assessment type is required.", "OK");
            return;
        }

        if (StartDatePicker.Date > EndDatePicker.Date)
        {
            await DisplayAlert("Validation Error", "Start date cannot be after end date.", "OK");
            return;
        }

        Assessment assessment = new()
        {
            Id = _assessmentId,
            CourseId = _courseId,
            Title = title,
            Type = type,
            StartDate = StartDatePicker.Date,
            EndDate = EndDatePicker.Date,
            AlertSetting = alertSetting
        };

        await _database.SaveAssessmentAsync(assessment);
        await Shell.Current.GoToAsync("..");
    }

    private async void OnDeleteClicked(object? sender, EventArgs e)
    {
        if (_assessmentId == 0)
            return;

        var assessment = await _database.GetAssessmentAsync(_assessmentId);
        if (assessment is null)
            return;

        bool confirm = await DisplayAlert(
            "Delete Assessment",
            $"Delete '{assessment.Title}'?",
            "Yes",
            "No");

        if (!confirm)
            return;

        await _database.DeleteAssessmentAsync(assessment);
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
        await Shell.Current.GoToAsync($"{nameof(AssessmentEditPage)}?courseId={_courseId}");
    }
}