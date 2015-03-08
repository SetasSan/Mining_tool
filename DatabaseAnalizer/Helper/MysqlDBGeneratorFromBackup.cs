using DatabaseAnalizer.Models;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseAnalizer.Helper
{
    public class MysqlDBGeneratorFromBackup
    {
        //private MySqlConnection connection;
        //private MySqlDbDataModel model;

        //public MysqlDBGeneratorFromBackup(MySqlConnection connection, MySqlDbDataModel model)
        //{
        //    this.model = model;
        //    this.connection = connection;
        //}

        //public void createDbFormBackup(out string dbName)
        //{
        //    MySqlCommand cmd = new MySqlCommand();
        //    MySqlBackup bcp = new MySqlBackup(cmd);            
        //    List<string> sDbs1 = (new SqlCommandGenerator(connection)).getStringList("select SCHEMA_NAME from information_schema.SCHEMATA");
        //    cmd.Connection = connection;
        //    connection.Open();
        //    bcp.ImportFromFile(this.model.dbBackupPath);
        //    connection.Close();
        //    List<string> sDbs2 = (new SqlCommandGenerator(connection)).getStringList("select SCHEMA_NAME from information_schema.SCHEMATA");
        //    connection.Close();
        //    dbName = sDbs2.Except(sDbs1).Single();           
        //}
       
    }
}
