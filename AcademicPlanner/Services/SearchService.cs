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
    private readonly PlannerItemService _plannerItemService;

    public SearchService(PlannerItemService plannerItemService)
    {
        _plannerItemService = plannerItemService;
    }

    public async Task<List<PlannerItem>> SearchAsync(string query)
    {
        query = query?.Trim() ?? string.Empty;

        var plannerItems = await _plannerItemService.GetPlannerItemsAsync();

        if (string.IsNullOrWhiteSpace(query))
        {
            return plannerItems
                .OrderBy(item => item.StartDate)
                .ToList();
        }

        return plannerItems
            .Where(item =>
                item.Title.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                item.ItemType.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                item.Subtitle.Contains(query, StringComparison.OrdinalIgnoreCase))
            .OrderBy(item => item.StartDate)
            .ToList();
    }
}