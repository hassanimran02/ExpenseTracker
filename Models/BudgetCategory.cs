using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ExpenseTracker.Models
{
    public class BudgetCategory
    {
        public int BudgetCategoryId { get; set; }
        public string Category { get; set; }
        public decimal AllocatedPercentage { get; set; }

        // Foreign key for the associated budget
        public int BudgetId { get; set; }
        public virtual Budget Budget { get; set; }

        // Calculated property for allocated amount
        [NotMapped]
        public decimal AllocatedAmount { get; set; }

        // Calculated property for spent amount
        [NotMapped]
        public decimal SpentAmount { get; set; }

        // Calculated property for remaining amount
        [NotMapped]
        public decimal RemainingAmount { get; set; }
    }
}