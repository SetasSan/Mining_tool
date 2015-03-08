using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseAnalizer.Models
{
    public class Column
    {
        public string name { set; get; }
        public string type { set; get; }
        public List<string> cellsData {set; get;}
        public Column(string name, string type)
        {
            this.name = name;
            this.type = type;
        }
    }
}
