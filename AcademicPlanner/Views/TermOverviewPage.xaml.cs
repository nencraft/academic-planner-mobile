using AcademicPlanner.Data;
using AcademicPlanner.Models;
using AcademicPlanner.Services;

namespace AcademicPlanner.Views;

[QueryProperty(nameof(TermId), "termId")]
public partial class TermOverviewPage : ContentPage
{
    private readonly AcademicPlannerDatabase _database;
    private readonly AcademicNotificationService _academicNotificationService;
    private int _termId;
    private Term? _currentTerm;

    public string TermId
    {
        set
        {
            if (int.TryParse(Uri.UnescapeDataString(value), out int id))
            {
                _termId = id;
                _ = LoadPageAsync();
            }
        }
    }

    public TermOverviewPage(
        AcademicPlannerDatabase database,
        AcademicNotificationService academicNotificationService)
    {
        InitializeComponent();
        _database = database;
        _academicNotificationService = academicNotificationService;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (_termId != 0)
        {
            await LoadPageAsync();
        }
    }

    private async Task LoadPageAsync()
    {
        _currentTerm = await _database.GetTermAsync(_termId);
        if (_currentTerm is null)
            return;

        TermTitleLabel.Text = $"{_currentTerm.Title} Overview";
        TermDatesLabel.Text = $"{_currentTerm.StartDate:MM/dd/yyyy} - {_currentTerm.EndDate:MM/dd/yyyy}";

        CoursesCollectionView.ItemsSource = await _database.GetCoursesByTermAsync(_termId);
    }

    private async void OnBackClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }

    private async void OnAddCourseClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync($"{nameof(CourseEditPage)}?termId={_termId}");
    }

    private async void OnEditTermClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync($"{nameof(TermEditPage)}?termId={_termId}");
    }

    private async void OnDeleteTermClicked(object? sender, EventArgs e)
    {
        if (_currentTerm is null)
            return;

        bool confirm = await DisplayAlert(
            "Delete Term",
            $"Delete '{_currentTerm.Title}'?",
            "Yes",
            "No");

        if (!confirm)
            return;

        await _academicNotificationService.CancelTermNotificationsAsync(_termId);
        await _database.DeleteTermCascadeAsync(_termId);
        await Shell.Current.GoToAsync("..");
    }

    private async void OnCourseSelectedClicked(object? sender, EventArgs e)
    {
        if (sender is not ImageButton button || button.CommandParameter is not int courseId)
            return;

        await Shell.Current.GoToAsync($"{nameof(CourseOverviewPage)}?courseId={courseId}");
    }
    private async void OnHomeClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//TermsPage");
    }
}