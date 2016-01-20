using DatabaseAnalizer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseAnalizer.Controllers
{
    public class Analizer
    {
        //List<Table> analizingTable;
        //public Analizer(List<Table> tables)
        //{
        //    analizingTable = tables;
        //}
        public Table Analize(List<Table> tables)
        {
            Table result = new Table();
            return tables.First();
        }
    }
}
