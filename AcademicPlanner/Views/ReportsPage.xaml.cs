using AcademicPlanner.Models;
using AcademicPlanner.Services;
using Microsoft.Maui.ApplicationModel.DataTransfer;

namespace AcademicPlanner.Views;

public partial class ReportsPage : ContentPage
{
    private readonly ReportService _reportService;
    private List<ReportRow> _currentReportRows = new();

    public ReportsPage(ReportService reportService)
    {
        InitializeComponent();
        _reportService = reportService;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadReportAsync();
    }

    private async Task LoadReportAsync()
    {
        TimestampLabel.Text = $"Generated on: {DateTime.Now:MM/dd/yyyy hh:mm tt}";

        _currentReportRows = await _reportService.GetUpcomingAcademicActivityReportAsync();
        ReportCollectionView.ItemsSource = _currentReportRows;

        int count = _currentReportRows.Count;
        SummaryLabel.Text = count == 1
            ? "1 item displayed"
            : $"{count} items displayed";

        bool hasRows = count > 0;
        ReportCollectionView.IsVisible = hasRows;
        EmptyReportLabel.IsVisible = !hasRows;
        ExportButton.IsEnabled = hasRows;
    }

    private async void OnBackClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }

    private async void OnHomeClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//TermsPage");
    }

    private async void OnExportReportClicked(object sender, EventArgs e)
    {
        if (_currentReportRows is null || !_currentReportRows.Any())
        {
            await DisplayAlert("Export Report", "There is no report data to export.", "OK");
            return;
        }

        string filePath = await _reportService.ExportReportRowsToCsvAsync(_currentReportRows);

        await DisplayAlert(
            "Export Report",
            $"Report created: {Path.GetFileName(filePath)}",
            "OK");

        await Share.Default.RequestAsync(new ShareFileRequest
        {
            Title = "Export Report",
            File = new ShareFile(filePath)
        });
    }
}