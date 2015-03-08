using DatabaseAnalizer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseAnalizer.Controllers.Databases
{    
    public class Database
    {
        private List<Table> _tables;
        private string _name;
        public Database(string name){
            _name = name;

        }
        public string GetDBName(){
            return _name;
        }

        public List<Table> GetTables()
        {
            return _tables;
        }

        public void SetTables(List<Table> tables)
        {
            _tables = tables;
        }

    }
}
