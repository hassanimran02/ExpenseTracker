using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExpenseTracker.Models
{
    public class CreateCustomBudgetViewModel
    {
        public int BudgetId { get; set; }
        public string Name { get; set; }
        public decimal MonthlyIncome { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<BudgetCategoryViewModel> Categories { get; set; }
        public List<SelectListItem> CategoryList { get; set; }

    }

}