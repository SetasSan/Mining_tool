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
            this.name = name;
        }
        public string name {set; get;}
        public List<Column> columns { set; get; }
    }
}
