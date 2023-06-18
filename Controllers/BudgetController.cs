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
    public class BudgetController : Controller
    {
        private readonly AppDbContext _dbContext;

        public BudgetController()
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


        public ActionResult CreateCustomBudget()
        {
            var userId = (int)Session["UserId"];
            var categories = _dbContext.Expenses
                .Where(e => e.UserId == userId)
                .Select(e => e.Category)
                .Distinct()
                .ToList();

            var model = new CreateCustomBudgetViewModel
            {
                Categories = categories.Select(c => new BudgetCategoryViewModel
                {
                    Category = c,
                    AllocatedPercentage = 0.0m // Set initial allocation percentage to 0
                }).ToList(),
            };
            model.CategoryList = ExpenseCategoryHelper.GetExpenseCategories();

            return View(model);
        }

        [HttpPost]
        public ActionResult CreateCustomBudget(CreateCustomBudgetViewModel model)
        {
            if (ModelState.IsValid)
            {
                var userId = (int)Session["UserId"];

                // Calculate the total allocated percentage
                decimal totalAllocatedPercentage = model.Categories.Sum(c => c.AllocatedPercentage);

                if (totalAllocatedPercentage == 100)
                {
                    var budget = new Budget
                    {
                        UserId = userId,
                        Name = model.Name,
                        MonthlyIncome = model.MonthlyIncome,
                        StartDate = model.StartDate,
                        EndDate = model.EndDate
                    };
                    _dbContext.Budgets.Add(budget);
                    _dbContext.SaveChanges();

                    foreach (var category in model.Categories)
                    {
                        var budgetCategory = new BudgetCategory
                        {
                            BudgetId = budget.BudgetId,
                            Category = category.Category,
                            AllocatedPercentage = category.AllocatedPercentage
                        };
                        _dbContext.BudgetCategories.Add(budgetCategory);
                    }
                    _dbContext.SaveChanges();

                    return RedirectToAction("YourBudgets");
                }
                else
                {
                    ModelState.AddModelError("", "The total allocated percentage must be equal to 100.");
                }
            }

            model.CategoryList = ExpenseCategoryHelper.GetExpenseCategories(); // Retrieve the expense categories
            return View(model);
        }

        public ActionResult BudgetSummary(int? budgetId)
        {
            var userId = (int)Session["UserId"];
            var budgets = _dbContext.Budgets.Where(b => b.UserId == userId).ToList();

            if (budgetId.HasValue)
            {
                var budget = budgets.FirstOrDefault(b => b.BudgetId == budgetId);

                if (budget == null)
                {
                    // Redirect to the default budget summary if the selected budget does not exist
                    return RedirectToAction("BudgetSummary");
                }

                var expenses = _dbContext.Expenses
                    .Where(e => e.UserId == userId && e.Date >= budget.StartDate && e.Date <= budget.EndDate)
                    .ToList();

                decimal totalExpenses = expenses.Sum(e => e.Amount);
                decimal totalAllocatedAmount = budget.Categories.Sum(c => c.AllocatedPercentage / 100 * budget.MonthlyIncome);
                decimal savings = budget.MonthlyIncome - totalExpenses;
                decimal overspending = totalExpenses - budget.MonthlyIncome;
                var categoryWithHighestSpending = expenses
                    .GroupBy(e => e.Category)
                    .OrderByDescending(g => g.Sum(e => e.Amount))
                    .Select(g => g.Key)
                    .FirstOrDefault();
                var dayWithHighestSpending = expenses
                    .GroupBy(e => e.Date)
                    .OrderByDescending(g => g.Sum(e => e.Amount))
                    .Select(g => g.Key)
                    .FirstOrDefault();

                var budgetSummaryModel = new BudgetSummaryViewModel
                {
                    BudgetId = budget.BudgetId,
                    Name = budget.Name,
                    TotalExpenses = totalExpenses,
                    TotalAllocatedAmount = totalAllocatedAmount,
                    Savings = savings,
                    Overspending = overspending,
                    CategoryWithHighestSpending = categoryWithHighestSpending,
                    DayWithHighestSpending = dayWithHighestSpending
                };

                return View(budgetSummaryModel);
            }

            return View(budgets);
        }

        public ActionResult YourBudgets()
        {
            var userId = (int)Session["UserId"];
            var budgets = _dbContext.Budgets.Where(b => b.UserId == userId).ToList();

            return View(budgets);
        }

        public ActionResult UpdateBudget(int? budgetId)
        {
            if (!budgetId.HasValue)
            {
                return RedirectToAction("YourBudgets");
            }

            var userId = (int)Session["UserId"];
            var budget = _dbContext.Budgets.FirstOrDefault(b => b.BudgetId == budgetId && b.UserId == userId);

            if (budget == null)
            {
                return RedirectToAction("YourBudgets");
            }

            var categories = _dbContext.BudgetCategories.Where(c => c.BudgetId == budgetId).ToList();

            var model = new CreateCustomBudgetViewModel
            {
                BudgetId = budget.BudgetId,
                MonthlyIncome = budget.MonthlyIncome,
                StartDate = budget.StartDate,
                EndDate = budget.EndDate,
                Categories = categories.Select(c => new BudgetCategoryViewModel
                {
                    Category = c.Category,
                    AllocatedPercentage = c.AllocatedPercentage
                }).ToList()
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult UpdateBudget(CreateCustomBudgetViewModel model)
        {
            if (ModelState.IsValid)
            {
                var userId = (int)Session["UserId"];
                var budget = _dbContext.Budgets.FirstOrDefault(b => b.BudgetId == model.BudgetId && b.UserId == userId);

                if (budget != null)
                {
                    budget.MonthlyIncome = model.MonthlyIncome;
                    budget.StartDate = model.StartDate;
                    budget.EndDate = model.EndDate;

                    // Remove existing budget categories
                    var existingCategories = _dbContext.BudgetCategories.Where(c => c.BudgetId == budget.BudgetId);
                    _dbContext.BudgetCategories.RemoveRange(existingCategories);

                    // Add updated budget categories
                    foreach (var category in model.Categories)
                    {
                        var budgetCategory = new BudgetCategory
                        {
                            BudgetId = budget.BudgetId,
                            Category = category.Category,
                            AllocatedPercentage = category.AllocatedPercentage
                        };
                        _dbContext.BudgetCategories.Add(budgetCategory);
                    }

                    _dbContext.SaveChanges();

                    return RedirectToAction("YourBudgets");
                }

                return RedirectToAction("YourBudgets");
            }

            return View(model);
        }

        public ActionResult DeleteBudget(int? budgetId)
        {
            if (!budgetId.HasValue)
            {
                return RedirectToAction("YourBudgets");
            }

            var userId = (int)Session["UserId"];
            var budget = _dbContext.Budgets.FirstOrDefault(b => b.BudgetId == budgetId && b.UserId == userId);

            if (budget != null)
            {
                var categories = _dbContext.BudgetCategories.Where(c => c.BudgetId == budget.BudgetId);
                _dbContext.BudgetCategories.RemoveRange(categories);
                _dbContext.Budgets.Remove(budget);
                _dbContext.SaveChanges();
            }

            return RedirectToAction("YourBudgets");
        }
        [HttpPost]
        public ActionResult SetSelectedBudget(int? SelectedBudgetId)
        {
            if (SelectedBudgetId.HasValue)
            {
                Session["SelectedBudgetId"] = SelectedBudgetId.Value;
            }
            return RedirectToAction("Index", "Expense");
        }
        public ActionResult BudgetDetails(int budgetId)
        {
            // Get the current user's ID
            int userId = (int)Session["UserId"];

            // Retrieve the selected budget and budget categories from the database
            var budget = _dbContext.Budgets
                .Include(b => b.Categories)
                .FirstOrDefault(b => b.BudgetId == budgetId && b.UserId == userId);

            if (budget != null)
            {
                // Calculate the total allocated amount for each category
                foreach (var category in budget.Categories)
                {
                    category.AllocatedAmount = budget.MonthlyIncome * (category.AllocatedPercentage / 100);
                }

                // Calculate the total spent amount for each category
                var expensesByCategory = _dbContext.Expenses
                    .Where(e => e.UserId == userId && e.BudgetId == budgetId && e.Date >= budget.StartDate && e.Date <= budget.EndDate)
                    .GroupBy(e => e.Category)
                    .Select(g => new
                    {
                        Category = g.Key,
                        SpentAmount = g.Sum(e => e.Amount)
                    })
                    .ToList();

                // Update the spent amount for each category
                foreach (var category in budget.Categories)
                {
                    var expense = expensesByCategory.FirstOrDefault(e => e.Category == category.Category);
                    if (expense != null)
                    {
                        category.SpentAmount = expense.SpentAmount;
                    }
                    else
                    {
                        category.SpentAmount = 0;
                    }
                }

                // Calculate the remaining amount for each category
                foreach (var category in budget.Categories)
                {
                    category.RemainingAmount = category.AllocatedAmount - category.SpentAmount;
                }

                return View(budget);
            }

            // Budget not found
            return RedirectToAction("YourBudgets");
        }




    }
}