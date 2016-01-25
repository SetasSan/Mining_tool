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
        public AnalizedTable Analize(List<Table> tables)
        {
            return CreateAnalizedTable(tables);
        }

        public Table FindMainTable(List<Table> tables)
        {
            return tables.Where(t => t.IsMainTable).FirstOrDefault();
        }

        private AnalizedTable CreateAnalizedTable(List<Models.Table> tables)
        {
            Models.AnalizedTable analizedTable = new AnalizedTable();

            foreach (var table in tables.GroupBy(c => c.Name).ToList())
            {
                string name = table.Select(s => s.Name).SingleOrDefault();
                analizedTable.Header.Add(name, tables.Where(s => s.Name == name).Count());
            }


            //foreach (Models.Table table in tables)
            //{
            //    foreach (Column col in table.Columns)
            //    {
            //        analizedTable.table.Columns.Add(new Column(table.Name + " - " + col.Name, col.Type));
            //    }
            //}

            Node n = new Node();
            n.Nodes = IterateRelations(tables.Where(t => t.IsMainTable).SingleOrDefault());
            n.Table = tables.Where(t => t.IsMainTable).SingleOrDefault();
            analizedTable = GetTableFromNode(n);
            return analizedTable;
        }

        private AnalizedTable GetTableFromNode(Node node)
        {
            AnalizedTable analizeTable = new AnalizedTable();


            analizeTable.table.Columns.AddRange(GetColumnsFromNode(node));


            return analizeTable;
        }

        private IEnumerable<Column> GetColumnsFromNode(Node node)
        {
            List<Column> cols = new List<Column>();

            if (node.Table.Relations.Any())
            {
                foreach (var col in node.Table.Columns)
                {
                    Column newC = new Column(node.Table.Name + " - " + col.Name, col.Type);
                    cols.Add(newC);
                }

                if (node.Table.Relations.Any() && node.Table.Relations.Select(ft => ft.ForeignTable).Any())
                {
                    foreach (var fTables in node.Table.Relations.Select(s => s.ForeignTable).ToList())
                    {
                        foreach (var fCol in fTables.Columns)
                        {
                            Column newC = new Column(fTables.Name + " - " + fCol.Name, fCol.Type);
                            cols.Add(newC);
                        }
                    }
                }

                foreach (var col in node.Table.Columns)
                {
                    foreach (var c in cols.Where(n => n.Name == node.Table.Name + " - " + col.Name))
                    {
                        foreach (var d in col.CellsData)
                        {
                            c.CellsData.Add(this.GetLastKeyAndIncrement(c), d.Value);
                        }
                    }
                }


                foreach (var rel in node.Table.Relations)
                {
                    foreach (var cpk in rel.PrimaryKey.CellsData)
                    {                       
                        foreach (var col in rel.ForeignTable.Columns)
                        {
                            foreach (var cell in cols.Where(n => n.Name == rel.ForeignTable.Name + " - " + col.Name))
                            {
                                var fValue = node.Table.Columns.Where(n => n.Name == rel.PrimaryKey.Name).Select(s => s.CellsData.Where(v => v.Key == cpk.Key)).SingleOrDefault().Select(s => s.Value).SingleOrDefault();
                                var colCells = rel.ForeignTable.Columns.Where(n => n.Name == col.Name).SingleOrDefault();
                                int index = rel.ForeignKey.CellsData.Where(w => w.Value == fValue).Select(v => v.Key).SingleOrDefault();
                                var data = colCells.CellsData.Where(s => s.Key == index).Select(v => v.Value).SingleOrDefault();
                                cell.CellsData.Add(this.GetLastKeyAndIncrement(cell), data);

                            }
                        }

                    }
                }
            }
            else
            {
                foreach (var col in node.Table.Columns)
                {
                    Column newC = new Column(node.Table.Name + " - " + col.Name, col.Type);
                    cols.Add(newC);
                }

            }

            return cols;
        }

        private int GetLastKeyAndIncrement(Column col)
        {
            if (col == null || col.CellsData == null)
                return 0;

            if (col.CellsData.Any())
            {
                return col.CellsData.Max(s => s.Key) + 1;
            }
            else
            {
                return 0;
            }
        }

        private List<Node> IterateRelations(Table table)
        {
            List<Node> nodes = new List<Node>();
            foreach (TableRelation rel in table.Relations)
            {
                Node n = new Node();
                n.Table = rel.ForeignTable;
                n.Nodes.AddRange(IterateRelations(rel.ForeignTable));
                nodes.Add(n);
            }
            return nodes;
        }
    }
    public class Node
    {
        public Table Table { set; get; }
        public List<Node> Nodes { set; get; }
        public Node()
        {
            Nodes = new List<Node>();
            Table = new Table();
        }
    }
}
