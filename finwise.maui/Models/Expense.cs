using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace finwise.maui.Models
{
    public class Expense: BaseModel
    {
        public string description {  get; set; }
        public decimal amount { get; set; }
        public string category { get; set; }

        public List<string> tags { get; set; }
        public string notes { get; set; }

        public List<ExpenseShare> expenseShares { get; set; }
        public List<ExpenseDebt> expenseDebts { get; set; }
        public bool isShared { get; set; }
        public string paidByType { get; set; }
        public string shareType { get; set; }

        public DateTime expenseDate { get; set; }
        public string repeatExpense { get; set; }
        public string scheduleId { get; set; }

        public Expense()
        {
            this.repeatExpense = "Off";
            this.category = "Others";
            this.expenseDate = DateTime.Now;

            this.tags = new List<string>();
            this.expenseShares = new List<ExpenseShare>();
            this.expenseDebts = new List<ExpenseDebt>();

            this.paidByType = "Paid By You";
            this.shareType = "Equally";

            this.isShared = false;
            this.isDeleted = false;
        }

    }
}
