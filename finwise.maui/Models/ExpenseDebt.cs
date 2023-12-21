using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace finwise.maui.Models
{
    public class ExpenseDebt: BaseModel
    {
        public string fromPersonId {  get; set; }
        public string toPersonId { get; set;}
        public decimal debtAmount { get; set; }
        public bool debtSettled { get; set; }
        public bool isSettlement { get; set; }

        public ExpenseDebt() { }
    }
}
