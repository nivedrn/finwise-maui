using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace finwise.maui.Models
{
    public class ExpenseShare: BaseModel
    {
        public string personId { get; set; }
        public bool hasPaid { get; set; }
        public decimal paidAmount { get; set; }

        public bool hasShare { get; set; }
        public decimal shareAmount { get; set; }

        public decimal owedAmount { get; set; }

        public bool isAppUser{ get; set; }

        public ExpenseShare()
        {
            id = Guid.NewGuid().ToString(); 
            hasShare = true;

            createdDate = DateTime.Now;
            modifiedDate = DateTime.Now;
        }
    }

}
