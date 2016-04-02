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
            n.TableTo = tables.Where(t => t.IsMainTable).SingleOrDefault();
            analizedTable = GetTableFromNode(n);

            return analizedTable;
        }

        private AnalizedTable GetTableFromNode(Node node)
        {
            AnalizedTable analizeTable = new AnalizedTable();


            analizeTable.table.Columns.AddRange(GetColumnsFromNode(node));

            List<Column> list = FillWithData(node, analizeTable.table.Columns);

            analizeTable.table.Columns = list;

            return analizeTable;
        }

        private List<Column> FillWithData(Node node, List<Column> list)
        {
            List<List<Column>> temporaryTables = new List<List<Column>>();
            List<Node> nodes = new List<Node>();
            Iterate(node, nodes);
            foreach (var rel in list)
            {
                //  rel.CellsData = nodes.First().RelationFromColumn.CellsData;//.Where(v=>v.TableFrom.Name == rel.Name.Split('.').First()).First().TableFrom.Columns.Where(c=>c.Name==rel.Name.Split('.').Last()).First().CellsData;

            }



            List<Column> temporaryTable = new List<Column>();

            temporaryTable.AddRange(list.Select(s => new Column(s.Name, s.Type)));
            foreach (var nod in nodes)
            {
                int index = 0;
                bool hasItemsInMainTable = temporaryTable.Where(w => w.CellsData.Count() != 0).Any();
                List<Column> temporaryFromCalculation = temporaryTable.Select(s => new Column(s.Name, s.Type)).ToList();
                foreach (var cellDataTo in nod.RelationToColumn.CellsData)
                {

                    if (hasItemsInMainTable)
                    {                     

                     
                        if (temporaryTable.Where(w => w.Name == nod.TableFrom.Name + " - " + nod.RelationFromColumn.Name).FirstOrDefault().CellsData.Any()
                            && !temporaryTable.Where(w => w.Name == nod.TableTo.Name + " - " + nod.RelationToColumn.Name).FirstOrDefault().CellsData.Any()
                            && !temporaryTable.Where(w => w.Name == nod.TableTo.Name + " - " + nod.RelationToColumn.Name).FirstOrDefault().CellsData.Where(w => w.Key == index).Any())
                        {

                            var arrowFrom = temporaryTable.SingleOrDefault(l => l.Name == nod.TableFrom.Name + " - " + nod.RelationFromColumn.Name);
                            if (arrowFrom.CellsData.Where(w => w.Value == cellDataTo.Value).Select(s => s.Key).Any())
                            {
                                foreach (var aFCell in arrowFrom.CellsData.Where(w => w.Value == cellDataTo.Value))
                                {
                                    foreach (var c in temporaryTable)
                                    {
                                        if (c.Name.Contains(nod.TableTo.Name + " - "))
                                            temporaryFromCalculation.Where(w => w.Name == c.Name).SingleOrDefault().CellsData.Add(index, cellDataTo.Value ?? null);
                                        else if (c.CellsData.Where(w => w.Key == aFCell.Key).Any())
                                            temporaryFromCalculation.Where(w => w.Name == c.Name).SingleOrDefault().CellsData.Add(index, c.CellsData[aFCell.Key]);
                                    }
                                    index++;
                                }
                                index--;
                            }
                            else
                            {
                                foreach (var c in temporaryTable)
                                {
                                    if (c.Name == nod.TableTo.Name + " - " + c.Name)
                                        temporaryFromCalculation.Where(w => w.Name == c.Name).SingleOrDefault().CellsData.Add(index, cellDataTo.Value);
                                    else
                                        temporaryFromCalculation.Where(w => w.Name == c.Name).SingleOrDefault().CellsData.Add(index, null);
                                }
                            }


                          
                        }                  
                        else
                        {


                            if (temporaryTable.Where(w => w.Name == nod.TableTo.Name + " - " + nod.RelationToColumn.Name).FirstOrDefault().CellsData.Any())
                            {

                                foreach (var cell in temporaryTable.Where(w => w.Name == nod.TableTo.Name + " - " + nod.RelationToColumn.Name).First().CellsData)
                                {
                                    var fKeyValue = cell.Value;
                                    var fKeyIndex = nod.RelationFromColumn.CellsData.Where(w => w.Value == fKeyValue).FirstOrDefault().Key;
                                    foreach (var secondTableCell in nod.TableFrom.Columns)
                                    {
                                        if (!temporaryTable.SingleOrDefault(l => l.Name == nod.TableFrom.Name + " - " + secondTableCell.Name).CellsData.Where(w => w.Key == cell.Key).Any())
                                            temporaryTable.SingleOrDefault(l => l.Name == nod.TableFrom.Name + " - " + secondTableCell.Name).CellsData.Add(cell.Key, secondTableCell.CellsData[fKeyIndex]);
                                    }
                                }

                            }
                            else
                            {

                            }
                        }

                      

                    }
                    else
                    {
                        foreach (var column in nod.TableTo.Columns)
                        {
                            var arrowTo = temporaryTable.SingleOrDefault(l => l.Name == nod.TableTo.Name + " - " + column.Name);
                            arrowTo.CellsData = arrowTo.CellsData ?? new Dictionary<int, string>();
                            arrowTo.CellsData.Add(index, column.CellsData[cellDataTo.Key]);
                        }

                        foreach (var column in nod.TableFrom.Columns)
                        {
                            var arrowTo = temporaryTable.SingleOrDefault(l => l.Name == nod.TableTo.Name + " - " + nod.RelationFromColumn.Name);
                            var valueIndex = arrowTo.CellsData.Where(w => w.Value == cellDataTo.Value).First().Key;
                            var arrowFrom = temporaryTable.SingleOrDefault(l => l.Name == nod.TableFrom.Name + " - " + column.Name);
                            arrowFrom.CellsData = arrowFrom.CellsData ?? new Dictionary<int, string>();

                            arrowFrom.CellsData.Add(index, column.CellsData[valueIndex]);
                        }
                    }
                    index++;


                }

                if (temporaryFromCalculation.Where(w => w.CellsData.Any()).Any())
                    temporaryTable = temporaryFromCalculation;
            }




            return temporaryTable;
        }


        private void Iterate(Node node, List<Node> nodes)
        {
            foreach (var n in node.Nodes)
            {
                if (!nodes.Where(w => w.RelationFromColumn.Name == n.RelationFromColumn.Name && w.RelationToColumn.Name == n.RelationToColumn.Name && w.TableFrom.Name == n.TableFrom.Name && w.TableTo.Name == w.TableTo.Name).Any())
                    nodes.Add(n);
                if (n.Nodes.Any())
                    Iterate(n, nodes);
            }
        }

        private List<Column> GetColumnsFromRelation(List<Table> tables, List<Column> cols)
        {
            foreach (var fTables in tables)
            {
                foreach (var fCol in fTables.Columns)
                {
                    if (!cols.Where(s => s.Name == (fTables.Name + " - " + fCol.Name)).Any())
                    {
                        Column newC = new Column(fTables.Name + " - " + fCol.Name, fCol.Type);
                        cols.Add(newC);
                    }
                }
            }
            return cols;

        }

        private IEnumerable<Column> GetColumnsFromNode(Node node)
        {
            List<Column> cols = new List<Column>();

            if (node.TableFrom.RelationsIn.Any() || node.TableFrom.RelationsFrom.Any() || node.TableTo.RelationsIn.Any() || node.TableTo.RelationsFrom.Any())
            {
                //add all cols for current node table
                foreach (var col in node.TableTo.Columns)
                {
                    Column newC = new Column(node.TableTo.Name + " - " + col.Name, col.Type);
                    cols.Add(newC);
                }

                List<Node> nodes = new List<Node>();
                Iterate(node, nodes);
                foreach (var nod in nodes)
                {
                    cols = GetColumnsFromRelation(nod.TableTo.RelationsIn.Select(s => s.ForeignTable).ToList(), cols);
                    cols = GetColumnsFromRelation(nod.TableTo.RelationsFrom.Select(s => s.ForeignTable).ToList(), cols);
                    cols = GetColumnsFromRelation(nod.TableFrom.RelationsIn.Select(s => s.ForeignTable).ToList(), cols);
                    cols = GetColumnsFromRelation(nod.TableFrom.RelationsFrom.Select(s => s.ForeignTable).ToList(), cols);

                    cols = GetColumnsFromRelation(nod.TableTo.RelationsIn.Select(s => s.PrimaryTable).ToList(), cols);
                    cols = GetColumnsFromRelation(nod.TableTo.RelationsFrom.Select(s => s.PrimaryTable).ToList(), cols);
                    cols = GetColumnsFromRelation(nod.TableFrom.RelationsIn.Select(s => s.PrimaryTable).ToList(), cols);
                    cols = GetColumnsFromRelation(nod.TableFrom.RelationsFrom.Select(s => s.PrimaryTable).ToList(), cols);
                }


                //foreach (var col in node.Table.Columns)
                //{
                //    foreach (var c in cols.Where(n => n.Name == node.Table.Name + " - " + col.Name))
                //    {
                //        foreach (var d in col.CellsData)
                //        {
                //            c.CellsData.Add(this.GetLastKeyAndIncrement(c), d.Value);
                //        }
                //    }
                //}


                //foreach (var rel in node.Table.Relations)
                //{
                //    foreach (var cpk in rel.PrimaryKey.CellsData)
                //    {                       
                //        foreach (var col in rel.ForeignTable.Columns)
                //        {
                //            foreach (var cell in cols.Where(n => n.Name == rel.ForeignTable.Name + " - " + col.Name))
                //            {
                //                var fValue = node.Table.Columns.Where(n => n.Name == rel.PrimaryKey.Name).Select(s => s.CellsData.Where(v => v.Key == cpk.Key)).SingleOrDefault().Select(s => s.Value).SingleOrDefault();
                //                var colCells = rel.ForeignTable.Columns.Where(n => n.Name == col.Name).SingleOrDefault();
                //                var indexes = rel.ForeignKey.CellsData.Where(w => w.Value == fValue).Select(v => v.Key).ToList();
                //                foreach (var index in indexes)
                //                {
                //                    var data = colCells.CellsData.Where(s => s.Key == index).Select(v => v.Value).SingleOrDefault();
                //                    cell.CellsData.Add(this.GetLastKeyAndIncrement(cell), data);
                //                }

                //            }
                //        }

                //    }
                //}
            }
            else
            {
                foreach (var col in node.TableTo.Columns)
                {
                    Column newC = new Column(node.TableTo.Name + " - " + col.Name, col.Type);
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

        private List<Node> IterateRelations(Table table, List<Node> INodes = null)
        {
            List<Node> nodes = new List<Node>();
            List<Node> allNodes = null;

            if (INodes == null)
                allNodes = new List<Node>();
            else
                allNodes = INodes;

            foreach (TableRelation rel in table.RelationsIn)
            {
                Node n = new Node();
                if (!(allNodes.Where(a => a.TableTo.Name == rel.ForeignTable.Name && a.TableFrom.Name == rel.PrimaryTable.Name && a.RelationToColumn.Name == rel.ForeignKey.Name).Any()))
                {

                    n.TableTo = rel.ForeignTable;
                    n.TableFrom = rel.PrimaryTable;
                    n.RelationToColumn = rel.ForeignKey;
                    n.RelationFromColumn = rel.PrimaryKey;
                    nodes.Add(n);
                    allNodes.Add(n);
                    List<Node> iNodeCont = IterateRelations(rel.ForeignTable, allNodes);
                    n.Nodes.AddRange(iNodeCont);
                }

            }
            foreach (TableRelation rel in table.RelationsFrom)
            {
                Node n = new Node();
                if (!(allNodes.Where(a => a.TableTo.Name == rel.ForeignTable.Name && a.TableFrom.Name == rel.PrimaryTable.Name && a.RelationToColumn.Name == rel.ForeignKey.Name).Any()))
                {
                    n.TableTo = rel.ForeignTable;
                    n.TableFrom = rel.PrimaryTable;
                    n.RelationToColumn = rel.ForeignKey;
                    n.RelationFromColumn = rel.PrimaryKey;
                    nodes.Add(n);
                    allNodes.Add(n);
                    List<Node> iNodeCont = IterateRelations(rel.ForeignTable, allNodes);
                    n.Nodes.AddRange(iNodeCont);
                }

            }

            return nodes;
        }
    }
    public class Node
    {
        public Table TableTo { set; get; }
        public Table TableFrom { set; get; }
        public Column RelationToColumn { set; get; }
        public Column RelationFromColumn { set; get; }
        public List<Node> Nodes { set; get; }
        public Node()
        {
            Nodes = new List<Node>();
            TableTo = new Table();
            TableFrom = new Table();
        }
    }
}
