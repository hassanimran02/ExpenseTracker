using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ExpenseTracker.Models
{
    public class BudgetTrackingViewModel
    {
        public Budget Budget { get; set; }
        public List<CategoryExpense> CategoryExpenses { get; set; }
        public decimal RemainingBudget { get; set; }
    }
}