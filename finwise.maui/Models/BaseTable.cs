using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace finwise.maui.Models
{
    public class BaseTable
    {
        [PrimaryKey, AutoIncrement]
        public int id { get; set; }
    }
}
