using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace finwise.maui.Models
{
    [Table("Expense")]
    public class ExpenseDB: BaseTable
    {
        [MaxLength(100)]
        public string description {  get; set; }

        public float amount { get; set; }

        public DateTime expenseDate { get; set; }

        [MaxLength(20)]
        public ExpenseType expenseType { get; set; }

        public RecurringUOM recurringUOM { get; set; }

        [MaxLength(400)]
        public string notes{  get; set; }

        public DateTime createdDate { get; set; }
        public DateTime modifiedDate{ get; set; }
        public bool isDeleted{ get; set; }

        public ExpenseDB()
        {
            this.expenseDate = DateTime.Now;
            this.amount = 0;
            this.expenseType = ExpenseType.OneTime;
            this.recurringUOM = RecurringUOM.NotApplicable;
            this.createdDate = DateTime.Now;
            this.modifiedDate = DateTime.Now;
            this.isDeleted = false;
        }
    }
}
