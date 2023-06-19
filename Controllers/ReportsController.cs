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

        public ActionResult ExpensesSummary()
        {
            var userId = (int)Session["UserId"];

            var expensesByCategory = _dbContext.Expenses
                .Where(e => e.UserId == userId)
                .GroupBy(e => e.Category)
                .Select(g => new { Category = g.Key, TotalAmount = g.Sum(e => e.Amount) })
                .ToList()
                .Select(e => new ExpensesSummaryViewModel.ExpenseByCategory
                {
                    Category = e.Category,
                    Amount = e.TotalAmount
                })
                .ToList();

            var expensesOverTime = _dbContext.Expenses
                .Where(e => e.UserId == userId)
                .GroupBy(e => new { Month = e.Date.Month, Year = e.Date.Year })
                .Select(g => new { Month = g.Key.Month, Year = g.Key.Year, TotalAmount = g.Sum(e => e.Amount) })
                .OrderBy(g => g.Year).ThenBy(g => g.Month)
                .ToList()
                .Select(e => new ExpensesSummaryViewModel.ExpenseOverTime
                {
                    Date = new DateTime(e.Year, e.Month, 1).ToString("MMM yyyy"),
                    Amount = e.TotalAmount
                })
                .ToList();

            var viewModel = new ExpensesSummaryViewModel
            {
                ExpensesByCategory = expensesByCategory,
                ExpensesOverTime = expensesOverTime
            };

            return View(viewModel);
        }
    }
}