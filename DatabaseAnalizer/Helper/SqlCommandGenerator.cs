using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseAnalizer.Helper
{
    public class SqlCommandGenerator
    {
        private MySqlConnection connection;
        public SqlCommandGenerator(MySqlConnection connection)
        {
            this.connection = connection;
        }
        public List<string> getStringList(string sCommand)
        {
            List<string> data = new List<string>();
            connection.Open();
            MySqlCommand command = connection.CreateCommand();
            command.CommandText = sCommand;
            MySqlDataReader tabs = command.ExecuteReader();
            while (tabs.Read())
            {
                data.Add(tabs.GetString(0));
            }
            connection.Close();
            return data;
        }
    }
}
