using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseAnalizer.Models
{
    public class MySqlDbDataModel
    {
        public string dbName { set; get; }
        public string dbBackupPath { set; get; }
    }
}
