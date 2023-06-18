using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ExpenseTracker.Models
{
    public class Budget
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int BudgetId { get; set; }
        public string Name { get; set; }
        public int UserId { get; set; }
        public decimal MonthlyIncome { get; set; }
        public DateTime StartDate { get; set; } // Start date of the budget
        public DateTime EndDate { get; set; } // End date of the budget

        // Collection of budget categories
        public virtual ICollection<BudgetCategory> Categories { get; set; }

        // Navigation property for the associated user
        public virtual User User { get; set; }
    }
}