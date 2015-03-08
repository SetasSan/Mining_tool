using DatabaseAnalizer.Controllers.Interfaces;
using DatabaseAnalizer.Controllers.Servers;
using DatabaseAnalizer.Models;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
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
            return _name;
        }
         
        public void Extract()
        {
            _connection = new MySqlConnection(GetConnectionString());
            _databases = ExtractDatabases();
            ExtractTablesForDbs();
            ExtractColumnNameAndType();
        }


        public List<Database> GetDatabases()
        {
            return _databases;
        }


        private List<Database> ExtractDatabases()
        {
            List<Database> databases = new List<Database>();

            foreach (string dbName in GetStringList("select SCHEMA_NAME from information_schema.SCHEMATA"))
            {
                databases.Add(new Database(dbName));
            }
            return databases;
        }

        private List<string> GetStringList(string sCommand)
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
                List<string> tablesNames = GetStringList("show tables from " + database.GetDBName());
                List<Table> tables = new List<Table>();
                foreach (string table in tablesNames)
                {
                    tables.Add(new Table(table));
                }
                database.SetTables(tables);
            }
        }

       
         /// <summary>
         /// get table columns with type
         /// </summary>
         /// <param name="dbName"></param>
         /// <param name="tableName"></param>
         /// <returns></returns>
        public void ExtractColumnNameAndType()
        {

            foreach (var db in _databases)
            {
                foreach (var table in db.GetTables())
                {
                    List<Column> columns = new List<Column>();
                    _connection.Open();
                    MySqlCommand command = _connection.CreateCommand();
                    //TODO: perkelti i DBcommanderi
                    command.CommandText = "select COLUMN_NAME, COLUMN_TYPE from information_schema.COLUMNS WHERE TABLE_NAME= '" + table.name + "' AND TABLE_SCHEMA='" + db.GetDBName() + "'";
                    MySqlDataReader tabs = command.ExecuteReader();
                    while (tabs.Read())
                    {
                        columns.Add(new Column(tabs.GetString("COLUMN_NAME"), tabs.GetString("COLUMN_TYPE")));
                    }
                    _connection.Close();
                    table.columns = columns;

                }
            }
            
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
