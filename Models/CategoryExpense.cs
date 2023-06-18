using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ExpenseTracker.Models
{
    public class CategoryExpense
    {
        public string Category { get; set; }
        public decimal TotalExpense { get; set; }
    }
}