using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ExpenseTracker.DatabaseContext;
using ExpenseTracker.Models;

namespace ExpenseTracker.Controllers
{
    [HandleError(View = "Error")]
    public class ExpenseController : Controller
    {
        private readonly AppDbContext _dbContext;

        public ExpenseController()
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
            // Get the logged-in user's expenses from the database and pass them to the view
            var userId = (int)Session["UserId"];
            var expenses = _dbContext.Expenses.Where(e => e.UserId == userId).ToList();

            return View(expenses);
        }

        [HttpGet]
        public ActionResult Create()
        {
            ViewBag.CategoryList = ExpenseCategoryHelper.GetExpenseCategories();
            var selectedBudgetId = (int?)Session["SelectedBudgetId"];

            if (!selectedBudgetId.HasValue)
            {
                TempData["Message"] = "Please select a budget first.";
                return RedirectToAction("YourBudgets", "Budget");
            }

            ViewBag.SelectedBudgetId = selectedBudgetId.Value;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Expense expense, string customCategory)
        {
            if (ModelState.IsValid)
            {
                expense.UserId = (int)Session["UserId"];

                if (expense.Category == "Other" && !string.IsNullOrEmpty(customCategory))
                {
                    expense.Category = customCategory;
                }

                var selectedBudgetId = (int?)Session["SelectedBudgetId"];

                if (!selectedBudgetId.HasValue)
                {
                    TempData["Message"] = "Please select a budget first.";
                    return RedirectToAction("YourBudgets", "Budget");
                }

                var userId = (int)Session["UserId"];
                var budget = _dbContext.Budgets
                    .Include(b => b.Categories)
                    .FirstOrDefault(b => b.BudgetId == selectedBudgetId && b.UserId == userId);

                if (budget != null)
                {
                    var category = budget.Categories.FirstOrDefault(c => c.Category == expense.Category);

                    if (category != null && category.AllocatedPercentage > 0)
                    {
                        decimal? allocatedAmount = budget.MonthlyIncome * (category.AllocatedPercentage / 100);
                        decimal? spentAmount = _dbContext.Expenses
                            .Where(e => e.UserId == userId && e.Category == expense.Category)
                            .Sum(e => (decimal?)e.Amount); // Cast the amount to nullable decimal

                        if ((spentAmount ?? 0) + expense.Amount > (allocatedAmount ?? 0))
                        {
                            // The expense exceeds the allocated budget for the category
                            ModelState.AddModelError("", "Warning: You are exceeding the allocated budget for this category.");
                            ViewBag.CategoryList = ExpenseCategoryHelper.GetExpenseCategories();
                            return View(expense);
                        }
                    }
                }

                expense.BudgetId = selectedBudgetId.Value; // Assign the selected budget ID to the expense object

                _dbContext.Expenses.Add(expense);
                _dbContext.SaveChanges();

                return RedirectToAction("Index");
            }

            ViewBag.CategoryList = ExpenseCategoryHelper.GetExpenseCategories();
            return View(expense);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Expense expense)
        {
            if (ModelState.IsValid)
            {
                _dbContext.Entry(expense).State = EntityState.Modified;
                _dbContext.SaveChanges();

                return RedirectToAction("Index");
            }

            ViewBag.CategoryList = ExpenseCategoryHelper.GetExpenseCategories();
            return View(expense);
        }

        [HttpGet]
        public ActionResult Delete(int id)
        {
            int userId = (int)Session["UserId"];
            var expense = _dbContext.Expenses.SingleOrDefault(e => e.ExpenseId == id && e.UserId == userId);

            if (expense == null)
            {
                return HttpNotFound();
            }

            return View(expense);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, Expense expense)
        {
            int userId = (int)Session["UserId"];
            var expenseToDelete = _dbContext.Expenses.SingleOrDefault(e => e.ExpenseId == id && e.UserId == userId);

            if (expenseToDelete == null)
            {
                return HttpNotFound();
            }

            _dbContext.Expenses.Remove(expenseToDelete);
            _dbContext.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}