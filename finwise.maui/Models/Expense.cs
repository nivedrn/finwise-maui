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
        public float amount { get; set; }
        public string category { get; set; }

        public List<string> tags { get; set; }
        public string notes { get; set; }

        public List<Person> members { get; set; }
        public List<ExpenseShare> expenseShares { get; set; }
        public bool isShared { get; set; }
        public string sharingType { get; set; }

        public DateTime expenseDate { get; set; }
        public string repeatExpense { get; set; }
        public string scheduleId { get; set; }

        public Expense()
        {
            this.repeatExpense = "Off";
            this.category = "Others";
            this.expenseDate = DateTime.Now;

            this.tags = new List<string>();
            this.members = new List<Person>();
            this.expenseShares = new List<ExpenseShare>();

            this.isShared = false;
            this.isDeleted = false;
        }

    }
}
