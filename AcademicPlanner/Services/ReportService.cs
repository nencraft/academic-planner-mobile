using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AcademicPlanner.Data;
using AcademicPlanner.Models;

namespace AcademicPlanner.Services;

public class ReportService
{
    private readonly PlannerItemService _plannerItemService;

    public ReportService(PlannerItemService plannerItemService)
    {
        _plannerItemService = plannerItemService;
    }

    public async Task<List<ReportRow>> GetUpcomingAcademicActivityReportAsync()
    {
        var plannerItems = await _plannerItemService.GetPlannerItemsAsync();
        DateTime today = DateTime.Today;

        return plannerItems
            .Where(item => item.EndDate.Date >= today)
            .OrderBy(item => item.StartDate)
            .Select(item => item.ToReportRow())
            .ToList();
    }

    public async Task<string> ExportReportRowsToCsvAsync(IEnumerable<ReportRow> rows)
    {
        var safeRows = rows?.ToList() ?? new List<ReportRow>();

        string timestamp = DateTime.Now.ToString("yyyyMMdd-HHmmss");
        string fileName = $"upcoming-academic-activity-report-{timestamp}.csv";
        string filePath = Path.Combine(FileSystem.CacheDirectory, fileName);

        string csv = BuildCsv(safeRows);

        await File.WriteAllTextAsync(filePath, csv);

        return filePath;
    }

    public static string BuildCsv(IEnumerable<ReportRow> rows)
    {
        var sb = new StringBuilder();

        sb.AppendLine("Type,Title,Context,Start Date,End/Due Date,Status");

        foreach (var row in rows)
        {
            sb.Append(EscapeCsv(row.ItemType)).Append(",");
            sb.Append(EscapeCsv(row.Title)).Append(",");
            sb.Append(EscapeCsv(row.Context)).Append(",");
            sb.Append(EscapeCsv(row.StartDate)).Append(",");
            sb.Append(EscapeCsv(row.EndDate)).Append(",");
            sb.Append(EscapeCsv(row.Status)).AppendLine();
        }

        return sb.ToString();
    }

    public static string EscapeCsv(string? value)
    {
        value ??= string.Empty;

        if (value.Contains(",") || value.Contains("\"") || value.Contains("\n"))
        {
            value = value.Replace("\"", "\"\"");
            return $"\"{value}\"";
        }

        return value;
    }
}