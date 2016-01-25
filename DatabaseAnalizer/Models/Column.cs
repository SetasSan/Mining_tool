using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseAnalizer.Models
{
    public class Column
    {
        public string Name { set; get; }
        public string Type { set; get; }        
        public Dictionary<int, string> CellsData {set; get;}
        public Column(string name, string type)
        {
            this.Name = name;
            this.Type = type;
            this.CellsData = new Dictionary<int, string>();
        }
    }
}
