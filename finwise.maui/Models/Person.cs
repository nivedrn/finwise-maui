using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace finwise.maui.Models
{
    public class Person: BaseModel
    {
        public string name { get; set; }
        public string email { get; set; }
        public string phoneNo { get; set; }

        public bool owesYou { get; set; }
        public decimal talliedAmount { get; set; }

        public string state { get; set; }

        public Person()
        {
            state = "No expenses";
            owesYou = false;
            talliedAmount = 0;
            isDeleted = false;
        }
    }
}
