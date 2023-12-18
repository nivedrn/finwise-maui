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
        public float paidAmount { get; set; }

        public bool hasShare { get; set; }
        public float shareAmount { get; set; }

        public bool isAppUser{ get; set; }

        public ExpenseShare(string pId, bool appUser)
        {
            id = Guid.NewGuid().ToString(); 
            personId = pId;
            isAppUser = appUser;
            hasPaid = appUser;
            hasShare = true;
        }
    }
}
