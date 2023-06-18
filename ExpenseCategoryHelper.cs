using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExpenseTracker
{
    public class ExpenseCategoryHelper
    {
        public static List<SelectListItem> GetExpenseCategories()
        {
            var categories = new List<string>
        {
            "Food",
            "Transportation",
            "Shopping",
            "Bills",
            "Entertainment",
            "Education",
            "Other"
        };

            var categoryList = categories.Select(c => new SelectListItem
            {
                Value = c,
                Text = c
            }).ToList();

            return categoryList;
        }
    }
}