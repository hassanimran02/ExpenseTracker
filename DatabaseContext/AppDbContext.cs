using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using ExpenseTracker.Models;

namespace ExpenseTracker.DatabaseContext
{
    public class AppDbContext : DbContext
    {
        public AppDbContext() : base("SqlConn")
        {
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Expense> Expenses { get; set; }
        public DbSet<Budget> Budgets { get; set; }
        public DbSet<BudgetCategory> BudgetCategories { get; set; }

    }
}