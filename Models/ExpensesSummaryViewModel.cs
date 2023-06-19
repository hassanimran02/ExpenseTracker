using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ExpenseTracker.Models
{
    public class ExpensesSummaryViewModel
    {
        public List<ExpenseByCategory> ExpensesByCategory { get; set; }
        public List<ExpenseOverTime> ExpensesOverTime { get; set; }

        public class ExpenseByCategory
        {
            public string Category { get; set; }
            public decimal Amount { get; set; }
        }

        public class ExpenseOverTime
        {
            public string Date { get; set; }
            public decimal Amount { get; set; }
        }
    }
}