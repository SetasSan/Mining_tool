using DatabaseAnalizer.Controllers.Databases;
using DatabaseAnalizer.Controllers.Interfaces;
using DatabaseAnalizer.Models;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseAnalizer.Controllers.Servers
{
    public class MsSqlServer : Server, IServer
    {

        public SqlConnection _connection;

        public MsSqlServer(string name)
            : base(name)
        {
            _databases = new List<Database>();
        }

        public string GetConnectionString()
        {
            string con = "Server=" + _serverAddress + ";";//.\SQLEXPRESS
            con += !string.IsNullOrWhiteSpace(_userName) ? "User Id='" + _userName + "';" : "";
            con += !string.IsNullOrWhiteSpace(_userPassword) ? "Password='" + _userPassword + "';" : "";
            con += string.IsNullOrWhiteSpace(_userName) ? "Trusted_Connection=Yes;" : "";
            return con;
        }


        public void Extract()
        {
            _connection = new SqlConnection(GetConnectionString());
            _databases = ExtractDatabases();
            ExtractTablesForDbs();
            ExtractColumnNameAndTypes();
        }

        private List<Database> ExtractDatabases()
        {
            List<Database> databases = new List<Database>();

            foreach (string dbName in ExecuteSqlCommand(" SELECT name FROM master.dbo.sysdatabases"))
            {
                databases.Add(new Database(dbName));
            }
            return databases;
        }

        private IEnumerable<string> ExecuteSqlCommand(string sCommand)
        {
            List<string> data = new List<string>();
            _connection.Open();// if there no mysql database here we are geting error so we can handle it with try catch
            SqlCommand command = _connection.CreateCommand();
            command.CommandText = sCommand;
            SqlDataReader tabs = command.ExecuteReader();
            while (tabs.Read())
            {
                data.Add(tabs.GetString(0));
            }
            _connection.Close();
            return data;
        }

        private void ExtractColumnNameAndTypes()
        {
            foreach (var db in _databases)
            {
                foreach (var table in db.GetTables())
                {
                    List<Column> columns = new List<Column>();
                    _connection.Open();
                    SqlCommand command = _connection.CreateCommand();
                    command.CommandText = "select COLUMN_NAME, DATA_TYPE from " + db.GetDBName() + ".information_schema.COLUMNS WHERE TABLE_NAME= '" + table.Name + "';";
                    SqlDataReader tabs = command.ExecuteReader();
                    while (tabs.Read())
                    {
                        columns.Add(new Column(tabs["COLUMN_NAME"].ToString(), tabs["DATA_TYPE"].ToString()));
                    }
                    _connection.Close();
                    table.Columns = columns;

                }
            }
        }

        private void ExtractTablesForDbs()
        {
            foreach (var database in GetDatabases())
            {
                List<Table> tables = new List<Table>();
                foreach (string table in GetTablesNamesForDb(database.GetDBName()))
                {
                    tables.Add(new Table(table));
                }
                database.SetTables(tables);
            }
        }

        private IEnumerable<string> GetTablesNamesForDb(string dbName)
        {
            return ExecuteSqlCommand("SELECT TABLE_NAME FROM " + dbName + ".INFORMATION_SCHEMA.Tables;");

        }

        public void ExtractTablesData(string tableName)
        {
            foreach (var db in _databases)
            {
                foreach (var table in db.GetTables().Where(t => t.Name == tableName))
                {
                    foreach (var column in table.Columns)
                    {

                        Dictionary<int, string> cellsData = new Dictionary<int, string>();
                        _connection.Open();
                        SqlCommand command = _connection.CreateCommand();
                        command.CommandText = "USE " + db.GetDBName() + "; select " + column.Name + " from " + table.Name;
                        SqlDataReader cells = command.ExecuteReader();

                        int i = 0;
                        foreach (var c in cells)
                        {
                            cellsData.Add(i, ((DbDataRecord)c).GetValue(0).ToString());
                            i++;
                        }
                        _connection.Close();
                        column.CellsData = cellsData;
                    }

                }
            }

        }


        public Models.Table LeftJoinTables(List<Models.Table> tablesForAnalize, Models.Table analized, string selectedDb, List<Views.ConditionSetting> Filters)
        {
            List<Relation> relations = new List<Relation>();
            List<Relation> quaredRelations = new List<Relation>();
            List<string> JoinedTables = new List<string>();
            List<string> notIncluded = new List<string>();
            JoinedTables.Add(analized.Name);
            List<string> selections = analized.Columns.Select(s => s.Name + " as '" + s.Name + "'").ToList();
            string query = "USE " + selectedDb + "; select " + string.Join(", ", selections) + " from " + analized.Name;

            foreach (var tab in tablesForAnalize)
            {
                var relFromTo = tab.RelationsFrom;
                relFromTo.AddRange(tab.RelationsIn);
                foreach (var rel in relFromTo)
                {
                    Relation newRel = new Relation()
                    {
                        TableFrom = rel.ForeignTable.Name,
                        TableTo = rel.PrimaryTable.Name,
                        ColumnFrom = rel.ForeignKey.Name,
                        ColumnTo = rel.PrimaryKey.Name
                    };
                    if (!relations.Where(w => w.IsEqual(newRel)).Any())
                        relations.Add(newRel);
                }
            }



            var tablesNamesList = relations.Select(s => s.TableFrom).ToList();
            tablesNamesList.AddRange(relations.Select(s => s.TableTo).ToList());

            notIncluded = tablesNamesList.Distinct().ToList();
            notIncluded.Remove(analized.Name);
            while (notIncluded.Any())
            {
                var tableName = notIncluded.First();
                bool changePositon = false;
                if (!JoinedTables.Contains(tableName) && relations.Where(w => w.TableFrom == tableName || w.TableTo == tableName).Any())
                {


                    bool add = true;
                    foreach (var rel in relations.Where(w => w.TableFrom == tableName || w.TableTo == tableName))
                    {
                        if ((JoinedTables.Contains(rel.TableFrom) || tableName == rel.TableFrom) && (JoinedTables.Contains(rel.TableTo) || tableName == rel.TableTo))
                        {
                            if (!quaredRelations.Where(w => w.IsEqual(rel)).Any())
                            {
                                if (JoinedTables.Contains(rel.TableFrom) && JoinedTables.Contains(rel.TableTo))
                                {
                                    query += " AND ";
                                }


                                if (add)
                                {
                                    query += " LEFT JOIN " + tableName + " ON ";
                                    add = false;
                                    notIncluded.Remove(tableName);
                                    JoinedTables.Add(tableName);
                                }
                                quaredRelations.Add(rel);
                                query += rel.TableFrom + "." + rel.ColumnFrom + " = " + rel.TableTo + "." + rel.ColumnTo + " ";
                                var relFromTo = relations.Where(w => w.TableFrom == tableName || w.TableTo == tableName);
                            }
                        }
                        else
                        {
                            changePositon = true;
                        }
                    }
                    add = false;

                }
                else
                {
                    changePositon = true;
                }
                if (changePositon)
                {
                    changePositon = false;
                    var item = notIncluded.First();
                    notIncluded.Remove(notIncluded.First());
                    notIncluded.Add(item);
                }


            }

            if (Filters != null && Filters.Any())
            {
                query += " WHERE ";
                foreach (var where in Filters)
                {
                    query += where.Name + " " + where.Condition + " " + where.Value;
                    if (where != Filters.Last())
                        query += " AND ";
                }
            }

            _connection.Open();
            SqlCommand command = _connection.CreateCommand();
            command.CommandText = query;
            SqlDataReader rdr = command.ExecuteReader();
            int index = 0;
            while (rdr.Read())
            {
                foreach (var col in analized.Columns)
                {
                    col.CellsData.Add(index, rdr[col.Name].ToString());
                }
                index++;
            }
            _connection.Close();



            return analized;
        }

        public string IsServerWorking()
        {
            try
            {
                _connection = new SqlConnection(GetConnectionString());
                _connection.Open();
                _connection.Close();
                return null;
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }


        public List<RelationFromDb> GetTablesRelations(string selectedDb)
        {
            List<RelationFromDb> result = new List<RelationFromDb>();

            var sCommand = @"
use " + selectedDb + @";
SELECT f.name AS ForeignKey,
OBJECT_NAME(f.parent_object_id) AS TABLE_NAME,
COL_NAME(fc.parent_object_id,
fc.parent_column_id) AS COLUMN_NAME,
OBJECT_NAME (f.referenced_object_id) AS REFERENCED_TABLE_NAME,
COL_NAME(fc.referenced_object_id,
fc.referenced_column_id) AS REFERENCED_COLUMN_NAME
FROM sys.foreign_keys AS f
INNER JOIN sys.foreign_key_columns AS fc
ON f.OBJECT_ID = fc.constraint_object_id
";


            _connection.Open();// if there no mysql database here we are geting error so we can handle it with try catch
            SqlCommand command = _connection.CreateCommand();
            command.CommandText = sCommand;
            SqlDataReader tabs = command.ExecuteReader();
            while (tabs.Read())
            {
                if (tabs["TABLE_NAME"] != null &&
                    tabs["COLUMN_NAME"] != null &&
                    tabs["REFERENCED_COLUMN_NAME"] != null &&
                    tabs["REFERENCED_TABLE_NAME"] != null)
                    result.Add(new RelationFromDb()
                    {
                        TableName = tabs["TABLE_NAME"].ToString(),
                        ColumnName = tabs["COLUMN_NAME"].ToString(),
                        ReferenceColumnName = tabs["REFERENCED_COLUMN_NAME"].ToString(),
                        ReferenceTableName = tabs["REFERENCED_TABLE_NAME"].ToString()
                    });
            }
            _connection.Close();


            return result;
        }
    }
}
