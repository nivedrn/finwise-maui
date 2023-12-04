using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace finwise.maui.Models
{
    public class Expense
    {
        [PrimaryKey, AutoIncrement]
        public string id {  get; set; }
        
        [MaxLength(20)]
        public string expenseType { get; set; }

        [MaxLength(100)]
        public string description {  get; set; }

        public float amount { get; set; }

        public DateTime expenseDate { get; set; }

        [MaxLength(400)]
        public string notes{  get; set; }

        public DateTime createdDate { get; set; }
        public DateTime modifiedDate{ get; set; }
        public bool isDeleted{ get; set; }

    }
}
