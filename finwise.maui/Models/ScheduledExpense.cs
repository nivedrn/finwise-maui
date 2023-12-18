using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace finwise.maui.Models
{
    public class ScheduledExpense: BaseModel
    {
        public string expenseId { get; set; }   
        public DateTime lastProcessedDate { get; set; }
        public bool isActive { get; set; }
        
    }
}
