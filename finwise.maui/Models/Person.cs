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
        public bool hasDebts { get; set; }
        public bool isSettledUp { get; set; }
        public decimal talliedAmount { get; set; }

        public Person()
        {
            owesYou = true;
            hasDebts = false;
            isSettledUp = false;
            talliedAmount = 0;
            isDeleted = false;
        }
    }
}
