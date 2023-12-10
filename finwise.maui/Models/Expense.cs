using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace finwise.maui.Models
{
    public enum ExpenseType
    {
        OneTime,
        Recurring
    }

    public enum RecurringUOM
    {
        NotApplicable,
        Daily,
        Weekly,
        Monthly,
        Yearly
    }

    [XmlRoot("Expenses")]
    public class Expense: BaseModel
    {
        public string description {  get; set; }
        public float amount { get; set; }
        public ExpenseType expenseType { get; set; }
        public RecurringUOM recurringUOM { get; set; }

        public List<Tag> tags { get; set; }
        public string notes { get; set; }

        public List<Person> persons { get; set; }
        public bool isShared { get; set; }

        public bool isDeleted { get; set; }
        public DateTime createdDate { get; set; }
        public DateTime modifiedDate { get; set; }

        public Expense()
        {
            this.amount = 0;
            this.expenseType = ExpenseType.OneTime;
            this.recurringUOM = RecurringUOM.NotApplicable;

            this.tags = new List<Tag>();

            this.isShared = false;
            this.isDeleted = false;
            this.createdDate = DateTime.Now;
            this.modifiedDate = DateTime.Now;

        }
    }
}
