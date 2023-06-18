using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ExpenseTracker.Models
{
    public class BudgetCategoryViewModel
    {
        public string Category { get; set; }
        public decimal AllocatedPercentage { get; set; }
    }
}