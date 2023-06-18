using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ExpenseTracker.DatabaseContext;
using ExpenseTracker.Models;

namespace ExpenseTracker.Controllers
{
    [HandleError(View = "Error")]
    public class ReportsController : Controller
    {

        private readonly AppDbContext _dbContext;

        public ReportsController()
        {
            _dbContext = new AppDbContext();
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            // Check if the user session is expired
            if (Session["UserId"] == null)
            {
                // Handle session expiration here, e.g., redirect to a login page
                filterContext.Result = RedirectToAction("Login", "Account");
                return;
            }

            base.OnActionExecuting(filterContext);
        }
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ExpensesByCategory()
        {
            var userId = (int)Session["UserId"];

            var expensesByCategory = _dbContext.Expenses
                .Where(e => e.UserId == userId) // Filter expenses for the current user
                .GroupBy(e => e.Category)
                .Select(g => new { Category = g.Key, TotalAmount = g.Sum(e => e.Amount) })
                .ToList();

            var expenses = expensesByCategory.Select(e => new ExpenseTracker.Models.Expense
            {
                Category = e.Category,
                Amount = e.TotalAmount
                // Set other properties as necessary
            }).ToList();

            return View(expenses);
        }

        public ActionResult ExpensesOverTime()
        {
            var userId = (int)Session["UserId"];

            var expensesOverTime = _dbContext.Expenses
                .Where(e => e.UserId == userId) // Filter expenses for the current user
                .GroupBy(e => new { Month = e.Date.Month, Year = e.Date.Year })
                .Select(g => new { Month = g.Key.Month, Year = g.Key.Year, TotalAmount = g.Sum(e => e.Amount) })
                .OrderBy(g => g.Year).ThenBy(g => g.Month)
                .ToList();

            var expenses = expensesOverTime.Select(e => new Expense
            {
                Date = new DateTime(e.Year, e.Month, 1), // Assuming you want to set the Date property to the first day of the month
                Amount = e.TotalAmount,
                Category = "", // Set the category and description as necessary
                Description = "",
                UserId = 0 // Set the user ID as necessary
            }).ToList();

            return View(expenses);
        }
    }
}