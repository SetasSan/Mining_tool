using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseAnalizer.Models
{
    public class RelationFromDb
    {
        public string TableName { set; get; }
        public string ColumnName { set; get; }
        public string ReferenceTableName { set; get; }
        public string ReferenceColumnName { set; get; }
    }
}
