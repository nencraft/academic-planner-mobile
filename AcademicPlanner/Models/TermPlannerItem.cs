using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcademicPlanner.Models;

public class TermPlannerItem : PlannerItem
{
    public override string ItemType => "Term";

    public override string Subtitle => $"Term dates: {StartDate:MM/dd/yyyy} - {EndDate:MM/dd/yyyy}";

    public override string NavigationRoute => $"{nameof(Views.TermOverviewPage)}?termId={SourceId}";
}
