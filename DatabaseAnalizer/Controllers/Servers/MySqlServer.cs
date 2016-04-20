using DatabaseAnalizer.Controllers.Interfaces;
using DatabaseAnalizer.Controllers.Servers;
using DatabaseAnalizer.Models;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DatabaseAnalizer.Controllers.Databases
{
    public class MySqlServer : Server, IServer
    {
        private MySqlConnection _connection;
        private List<Database> _databases;
        private string _serverAddress;
        private string _userName;
        private string _userPassword;

        public MySqlServer(string name)
            : base(name)
        {
            _databases = new List<Database>();
        }

        public string GetConnectionString()
        {
            return "SERVER=" + _serverAddress + ";UID=" + _userName + ";PASSWORD='" + _userPassword + "';Convert Zero Datetime=True";
        }

        public string GetServerName()
        {
            return Name;
        }

        public void Extract()
        {
            _connection = new MySqlConnection(GetConnectionString());
            _databases = ExtractDatabases();
            ExtractTablesForDbs();
            ExtractColumnNameAndTypes();
        }


        public List<Database> GetDatabases()
        {
            return _databases;
        }


        private List<Database> ExtractDatabases()
        {
            List<Database> databases = new List<Database>();

            foreach (string dbName in ExecuteSqlCommand("select SCHEMA_NAME from information_schema.SCHEMATA"))
            {
                databases.Add(new Database(dbName));
            }
            return databases;
        }

        /// <summary>
        /// Execute sql command and return result in List<string>
        /// </summary>
        /// <param name="sCommand"></param>
        /// <returns></returns>
        private List<string> ExecuteSqlCommand(string sCommand)
        {
            List<string> data = new List<string>();
            _connection.Open();// if there no mysql database here we are geting error so we can handle it with try catch
            MySqlCommand command = _connection.CreateCommand();
            command.CommandText = sCommand;
            MySqlDataReader tabs = command.ExecuteReader();
            while (tabs.Read())
            {
                data.Add(tabs.GetString(0));
            }
            _connection.Close();
            return data;
        }


        /// <summary>
        /// we filling all databases with tables because it will be faster go throught all of them
        /// </summary>
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

        private List<string> GetTablesNamesForDb(string dbName)
        {
            return ExecuteSqlCommand("show tables from " + dbName);
        }


        /// <summary>
        /// get table columns with type
        /// </summary>
        /// <param name="dbName"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        private void ExtractColumnNameAndTypes()
        {

            foreach (var db in _databases)
            {
                foreach (var table in db.GetTables())
                {
                    List<Column> columns = new List<Column>();
                    _connection.Open();
                    MySqlCommand command = _connection.CreateCommand();
                    command.CommandText = "select COLUMN_NAME, COLUMN_TYPE from information_schema.COLUMNS WHERE TABLE_NAME= '" + table.Name + "' AND TABLE_SCHEMA='" + db.GetDBName() + "'";
                    MySqlDataReader tabs = command.ExecuteReader();
                    while (tabs.Read())
                    {
                        columns.Add(new Column(tabs.GetString("COLUMN_NAME"), tabs.GetString("COLUMN_TYPE")));
                    }
                    _connection.Close();
                    table.Columns = columns;

                }
            }

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
                        MySqlCommand command = _connection.CreateCommand();
                        command.CommandText = "USE " + db.GetDBName() + "; select " + ConvertKey(column.Name) + " from " + table.Name;
                        MySqlDataReader cells = command.ExecuteReader();

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

        private string ConvertKey(string a)
        {
            string re = "";
            switch (a)
            {
                case "table":
                    {
                        re = "'table'";
                        break;
                    }
                case "KEY":
                    {
                        re = "'KEY'";
                        break;
                    }
                default:
                    {
                        re = a;
                        break;
                    }


            }
            return re;
        }


        public void SetUserName(string userName)
        {
            _userName = userName;
        }

        public void SetUserPassword(string userPassword)
        {
            _userPassword = userPassword;
        }


        public void SetServerAddress(string serverAddress)
        {
            _serverAddress = serverAddress;
        }


        public Table LeftJoinTables(List<Table> tablesForAnalize, Table analized, string selectedDb, List<Views.ConditionSetting> Filters)
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
            MySqlCommand command = _connection.CreateCommand();
            command.CommandText = query;
            MySqlDataReader rdr = command.ExecuteReader();
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
                _connection = new MySqlConnection(GetConnectionString());
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

            var sCommand = @"SELECT 
                                                 `TABLE_SCHEMA`,
                                                 `TABLE_NAME`,
                                                 `COLUMN_NAME`,
                                                 `REFERENCED_TABLE_SCHEMA`,
                                                 `REFERENCED_TABLE_NAME`,
                                                 `REFERENCED_COLUMN_NAME`
                                            FROM
                                                 `INFORMATION_SCHEMA`.`KEY_COLUMN_USAGE`
                                            WHERE
                                                 `TABLE_SCHEMA` = '" + selectedDb + "' AND `REFERENCED_TABLE_NAME` IS NOT NULL;";


            _connection.Open();// if there no mysql database here we are geting error so we can handle it with try catch
            MySqlCommand command = _connection.CreateCommand();
            command.CommandText = sCommand;
            MySqlDataReader tabs = command.ExecuteReader();
            while (tabs.Read())
            {
                result.Add(new RelationFromDb()
                {
                    TableName = tabs.GetString("TABLE_NAME"),
                    ColumnName = tabs.GetString("COLUMN_NAME"),
                    ReferenceColumnName = tabs.GetString("REFERENCED_COLUMN_NAME"),
                    ReferenceTableName = tabs.GetString("REFERENCED_TABLE_NAME")
                });
            }
            _connection.Close();


            return result;
        }
    }
    public class Relation
    {
        public string TableFrom { set; get; }
        public string TableTo { set; get; }
        public string ColumnFrom { set; get; }
        public string ColumnTo { set; get; }

        public bool IsEqual(Relation newRel)
        {
            return ((ColumnTo == newRel.ColumnTo && ColumnFrom == newRel.ColumnFrom) ||
                (ColumnFrom == newRel.ColumnTo && ColumnTo == newRel.ColumnFrom));
        }

    }
}
