using DatabaseAnalizer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DatabaseAnalizer.Helper;

namespace DatabaseAnalizer.Controllers
{
    public class Analizer
    {
        public Table Analize(List<Table> tables)
        {
            return CreateAnalizedTable(tables);
        }

        public Table FindMainTable(List<Table> tables)
        {
            return tables.Where(t => t.IsMainTable).FirstOrDefault();
        }

        private Table CreateAnalizedTable(List<Models.Table> tables)
        {
            Table analizedTable = new Table();

            analizedTable.Name = tables.Where(w => w.IsMainTable).SingleOrDefault().Name;
            foreach (var col in tables.Where(w => w.IsMainTable).SingleOrDefault().Columns)
                analizedTable.Columns.Add(new Column(tables.Where(w => w.IsMainTable).SingleOrDefault().Name + "." + col.Name, col.Type));

            foreach (var table in tables.Where(w => !w.IsMainTable))
                foreach (var col in table.Columns)
                    analizedTable.Columns.Add(new Column(table.Name + "." + col.Name, col.Type));

            return analizedTable;
        }
    }

      
}
