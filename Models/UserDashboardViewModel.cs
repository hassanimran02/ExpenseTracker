using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ExpenseTracker.Models
{
    public class UserDashboardViewModel
    {
        public Budget CurrentBudget { get; set; }
        public BudgetSummaryViewModel BudgetSummary { get; set; }
        public List<Expense> RecentExpenses { get; set; }
        public string TopExpenseDay { get; set; }
        public decimal RemainingBudget { get; set; }
        public decimal BudgetSpentPercentage { get; set; }
        public Dictionary<string, decimal> ExpenseBreakdown { get; set; }  
        public string BudgetName { get; set; }
        public decimal TotalExpenses { get; set; }
    }

}
