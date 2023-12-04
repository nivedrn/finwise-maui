using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace finwise.maui.Models
{
    internal class ExpenseMember
    {
        public Expense item { get; set; }
        public Person member { get; set; }  
    }
}
