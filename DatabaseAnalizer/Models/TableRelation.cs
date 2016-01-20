using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseAnalizer.Models
{
    public class TableRelation
    {
        public Table ForeignTable { set; get; }
        public List<RelationBeetweenTable> RelationFromTo { set; get; }

        public TableRelation()
        {
            RelationFromTo = new List<RelationBeetweenTable>();
        }

        public class RelationBeetweenTable
        {
            public Column ForeignKey { set; get; }
            public Column PrimaryKey { set; get; }
        }
    }
}
