using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace finwise.maui.Models
{
    [XmlRoot("Groups")]
    public class Group: BaseModel
    {
        public string groupName { get; set; }
        public List<string> personIds { get; set; }
    }
}
