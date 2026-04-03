using AcademicPlanner.Data;
using AcademicPlanner.Models;

namespace AcademicPlanner.Views;

public partial class TermsPage : ContentPage
{
    private readonly AcademicPlannerDatabase _database;

    public TermsPage(AcademicPlannerDatabase database)
    {
        InitializeComponent();
        _database = database;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadTermsAsync();
    }

    private async Task LoadTermsAsync()
    {
        TermsCollectionView.ItemsSource = await _database.GetTermsAsync();
    }

    private async void OnAddTermClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(TermEditPage));
    }

    private async void OnTermSelectedClicked(object? sender, EventArgs e)
    {
        if (sender is not ImageButton button || button.CommandParameter is not int termId)
            return;

        await Shell.Current.GoToAsync($"{nameof(TermOverviewPage)}?termId={termId}");
    }

    private async void OnBackClicked(object? sender, EventArgs e)
    {
        await Task.CompletedTask;
    }
}