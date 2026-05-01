using AcademicPlanner.Models;
using AcademicPlanner.Services;

namespace AcademicPlanner.Views;

public partial class SearchPage : ContentPage
{
    private readonly SearchService _searchService;
    private bool _hasSearched;

    public SearchPage(SearchService searchService)
    {
        InitializeComponent();
        _searchService = searchService;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        ResultsCollectionView.ItemsSource = null;
        ResultsCollectionView.IsVisible = false;
        NoResultsLabel.IsVisible = false;
        _hasSearched = false;
    }

    private async void OnSearchClicked(object sender, EventArgs e)
    {
        await LoadResultsAsync(markAsSearched: true);
    }

    private async void OnSearchCompleted(object sender, EventArgs e)
    {
        await LoadResultsAsync(markAsSearched: true);
    }

    private void OnClearSearchClicked(object sender, EventArgs e)
    {
        SearchEntry.Text = string.Empty;
        ResultsCollectionView.ItemsSource = null;
        ResultsCollectionView.IsVisible = false;
        NoResultsLabel.IsVisible = false;
        _hasSearched = false;
    }

    private async Task LoadResultsAsync(bool markAsSearched)
    {
        if (markAsSearched)
        {
            _hasSearched = true;
        }

        string query = SearchEntry.Text?.Trim() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(query))
        {
            ResultsCollectionView.ItemsSource = null;
            ResultsCollectionView.IsVisible = false;
            NoResultsLabel.IsVisible = false;
            return;
        }

        List<PlannerItem> results = await _searchService.SearchAsync(query);

        ResultsCollectionView.ItemsSource = results;

        bool hasResults = results.Count > 0;
        ResultsCollectionView.IsVisible = hasResults;
        NoResultsLabel.IsVisible = _hasSearched && !hasResults;
    }

    private async void OnOpenResultClicked(object sender, EventArgs e)
    {
        if (sender is not Button button || button.CommandParameter is not string route)
            return;

        await Shell.Current.GoToAsync(route);
    }

    private async void OnBackClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }

    private async void OnHomeClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//TermsPage");
    }
}