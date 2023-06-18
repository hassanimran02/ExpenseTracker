using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ExpenseTracker.Models
{
    public class Expense
    {
        public int ExpenseId { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        public string Category { get; set; }

        [Required]
        public decimal Amount { get; set; }

        [Required]
        public string Description { get; set; }

        // Foreign key for the associated user
        public int UserId { get; set; }
        public virtual User User { get; set; }

        // Foreign key for the associated budget
        public int BudgetId { get; set; }
        public virtual Budget Budget { get; set; }
    }
}