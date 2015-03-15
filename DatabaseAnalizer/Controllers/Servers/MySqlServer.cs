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
            return "SERVER="+_serverAddress+";UID="+_userName+";PASSWORD='"+_userPassword+"';Convert Zero Datetime=True";
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
            ExtractTablesData();
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
            _connection.Open();// if there no mysql daabase here we are geting error so we can handle it with try catch
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
                foreach (string table in GetTablesNamesForDb(database.GetDBName())
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

        private void ExtractTablesData()
        {
            foreach (var db in _databases)
            {
                foreach (var table in db.GetTables())
                {
                    foreach(var column in table.Columns){
                  
                    List<string> cellsData = new List<string>();
                    _connection.Open();
                    MySqlCommand command = _connection.CreateCommand();
                    command.CommandText = "USE " + db.GetDBName() + "; select " + ConvertKey(column.Name) + " from " + table.Name;
                    MySqlDataReader cells = command.ExecuteReader();
              
                    foreach (var c in cells)
                    {
                        cellsData.Add(((DbDataRecord)c).GetValue(0).ToString());
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
    }
}
