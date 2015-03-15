using DatabaseAnalizer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseAnalizer.Models
{
    public class Table
    {
        public Table(string name)
        {
            this.Name = name;
        }
        public string Name {set; get;}
        public List<Column> Columns { set; get; }
    }
}
