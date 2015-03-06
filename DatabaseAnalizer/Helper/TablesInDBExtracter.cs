using DatabaseAnalizer.Models;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;

namespace DatabaseAnalizer.Helper
{
    public class TablesInDBExtracter
    {
        private MySqlConnection connection;
        private MySqlDbDataModel dbModel;
        private LogPrinter loger;
        private string selectedTableNam { set; get; }
        private DataGrid table_data;
        private Dictionary<string, DatabaseAnalizer.Models.Table> tables;


        public TablesInDBExtracter(MySqlConnection connection, ref MySqlDbDataModel dbModel, ref LogPrinter loger, DataGrid table_data, ref Dictionary<string, DatabaseAnalizer.Models.Table> tables)
        {
            this.loger = loger;
            this.connection = connection;
            this.dbModel = dbModel;
            this.table_data = table_data;
            this.tables = tables;
        }
        public void generateTableButtuns(StackPanel ButtonsPanel, DataGrid Table_parametres)
        {

            connection.Open();
            connection.ChangeDatabase(dbModel.dbName);
            connection.Close();
            List<string> tables = (new DBCommandGenerator(connection)).getStringList("show tables from " + dbModel.dbName);

            List<Button> buttons = new List<Button>();
            ButtonsPanel.Children.Clear();
            foreach (string line in tables)
            {
                Button newButton = new Button();
                newButton.Click += (s, e) => { createTableParametres(Table_parametres, line); };
                newButton.Name = line + "_btn";
                newButton.Content = line;
                newButton.Height = 50;
                ButtonsPanel.Children.Add(newButton);
            }
        }

        private void createTableParametres(DataGrid Table_parametres, String line)
        {
            selectedTableNam = line;
            connection.Open();
            MySqlCommand command = connection.CreateCommand();
            //TODO: perkelti i DBcommanderi
            command.CommandText = "select COLUMN_NAME, COLUMN_TYPE from information_schema.COLUMNS WHERE TABLE_NAME= '" + selectedTableNam + "' AND TABLE_SCHEMA='" + dbModel.dbName + "'";
            MySqlDataReader tabs = command.ExecuteReader();
            Table_parametres.Items.Clear();
            while (tabs.Read())
            {
                Table_parametres.Items.Add(new DbTableData { Name = tabs.GetString("COLUMN_NAME"), Type = tabs.GetString("COLUMN_TYPE") });
            }
            connection.Close();
            fillDataTable(selectedTableNam);
            loger.printLog("selected table - " + selectedTableNam);
        }

        private class DbTableData
        {
            public string Name { get; set; }
            public string Type { get; set; }
        }

        public void getTablesStructure()
        {

            MySqlDataReader tabs;
            MySqlCommand command;
            tables = new Dictionary<string, DatabaseAnalizer.Models.Table>();

            //select all tables in DB
            connection.Open();
            command = connection.CreateCommand();
            command.CommandText = "select distinct TABLE_NAME from information_schema.COLUMNS WHERE TABLE_SCHEMA = '" + dbModel.dbName + "'";
            tabs = command.ExecuteReader();
            while (tabs.Read())
            {
                tables.Add(tabs.GetString("TABLE_NAME"), new Table());
            }
            connection.Close();


            //select all row names in DB
            foreach (string tableName in tables.Select(a => a.Key))
            {
                connection.Open();
                command.CommandText = "select distinct COLUMN_NAME from information_schema.COLUMNS WHERE TABLE_NAME= '" + tableName + "' AND TABLE_SCHEMA='" + dbModel.dbName + "'";
                tabs = command.ExecuteReader();
                while (tabs.Read())
                {
                    tables.Where(a => a.Key == tableName).Select(k => k.Value).SingleOrDefault().cols.Add(tabs.GetString("COLUMN_NAME"), new List<object>());
                }
                connection.Close();

            }

            //add all rows data in 
            foreach (string tableName in tables.Select(a => a.Key))
            {
                foreach (Table col in tables.Where(a => a.Key == tableName).Select(k => k.Value))
                {
                    foreach (string s in col.cols.Select(t => t.Key))
                    {
                        connection.Open();
                       
                        command.CommandText = "select " + convertKey(s)+ " from " + tableName;
                        tabs = command.ExecuteReader();
                        while (tabs.Read())
                        {
                            tables.Where(a => a.Key == tableName).Select(k => k.Value).SingleOrDefault().cols.Where(p => p.Key == s).Single().Value.Add(tabs.GetValue(0));
                        }
                        connection.Close();
                    }
                }
            }

            loger.printLog("selected table - " + selectedTableNam);


        }
        private string convertKey(string a)
        {
            string re= "";
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
                default: {
                        re=a;
                        break;
                }
                        
                    
            }
            return re; 
        }

        public void fillDataTable(string tableName)
        {
            table_data.Columns.Clear();
            DataTable dt = new DataTable();
            int o = 0;


            List<List<object>> listas = new List<List<object>>();
           

            foreach (string s in tables.Where(a => a.Key == tableName).Select(k => k.Value).Single().cols.Select(t => t.Key))
            {
               
                DataColumn dc = new DataColumn(s, typeof(string));
                dt.Columns.Add(dc);
                o++;
               
                KeyValuePair<string, List<object>> kk = tables.Where(a => a.Key == tableName).Select(k => k.Value).Single().cols.Where(z => z.Key == s).Single();
              
                List<object> l = new List<object>();
                foreach (object obj in kk.Value)
                {
                    //dr[i] = obj.ToString();
                    l.Add(obj);                    
                }
                listas.Add(l);

            }

            for (int i = 0; i < ((List<object>)listas.ElementAt(0)).Count; i++)
            {
                DataRow dr = dt.NewRow();
                int e= 0;
                foreach (object l in listas)
                {
                     dr[e] = ((List<object>)listas.ElementAt(e)).ElementAt(i).ToString();
                    //dr[e] = "takas";
                    e++;
                }                
                dt.Rows.Add(dr);
            }

            DataView dw = new DataView(dt);
            table_data.ItemsSource = dw;
            
            

        }
    }
}
