using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ExpenseTracker.Models
{
    public class BudgetSummaryViewModel
    {
        public int BudgetId { get; set; }
        public string Name { get; set; }
        public decimal TotalExpenses { get; set; }
        public decimal TotalAllocatedAmount { get; set; }
        public decimal RemainingBudget { get; set; }
        public decimal BudgetSpentPercentage { get; set; }
        public decimal Savings { get; set; }
        public decimal Overspending { get; set; }
        public string CategoryWithHighestSpending { get; set; }
        public DateTime DayWithHighestSpending { get; set; }
    }
}