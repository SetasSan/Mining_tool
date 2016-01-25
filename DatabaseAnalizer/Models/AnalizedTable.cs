using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseAnalizer.Models
{
    public class AnalizedTable
    {
        public Dictionary<string, int> Header { set; get; }
        public Table table { set; get; }

        public AnalizedTable()
        {
            Header = new Dictionary<string, int>();
            table = new Table();
        }
    }
}
