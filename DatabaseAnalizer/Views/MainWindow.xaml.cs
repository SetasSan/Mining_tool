using DatabaseAnalizer.Controllers;
using DatabaseAnalizer.Helper;
using DatabaseAnalizer.Models;
using Microsoft.Win32;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DatabaseAnalizer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public LogPrinter loger;
        private Controller _controller;

        private Analizer _analizer;
        public MainWindow(Controller controller)
        {
            InitializeComponent();
            _controller = controller;
            CreatePatametersTable();
            //    this.Visibility = System.Windows.Visibility.Hidden;

            //    tables = new Dictionary<string, DatabaseAnalizer.Models.Table>();
            //    connection = new MySqlConnection(ConfigurationSettings.AppSettings["MySqlConnectionString"]);
            //    mysqlModel = new MySqlDbDataModel();            
               
            //    RefreshDatabaseList();
            //    tide = new TablesInDBExtracter(connection, ref mysqlModel, ref loger, Table_data, ref tables); 
        }

        private void CreatePatametersTable()
        {
            var nameC = new DataGridTextColumn();           
            nameC.Header = "Name";
            nameC.Width = 120;
            nameC.Binding = new Binding("Name");
            Table_parametres.Columns.Add(nameC);
            var typeC = new DataGridTextColumn();
            typeC.Header = "Type";
            typeC.Width = 190;
            typeC.Binding = new Binding("Type");
            Table_parametres.Columns.Add(typeC);

        }

        public void ActivateMainWindow()
        {
            this.Visibility = System.Windows.Visibility.Visible;
        }

        private void Select_DB_Click(object sender, RoutedEventArgs e)
        {
            //OpenFileDialog ofd = new OpenFileDialog();
            //ofd.ShowDialog();
            //if ((ofd.OpenFile()) != null)//TODO: here i get error
            //{
            //    mysqlModel.dbBackupPath = ofd.FileName;
            //    string dbName;
            //    new MysqlDBGeneratorFromBackup(connection, mysqlModel).createDbFormBackup(out dbName);
            //    mysqlModel.dbName = dbName;
            //    tide.generateTableButtuns(ButtonsPanel, Table_parametres);                                    
            //}            
        }

        private void Exit_click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }




        public void RefreshDatabaseList()
        {
            //Databases_list.Items.Clear();
            //Databases_list.Items.Add("Select");
            //Databases_list.SelectedIndex = 0;

            //foreach (string db in (new SqlCommandGenerator(connection)).getStringList("select SCHEMA_NAME from information_schema.SCHEMATA"))
            //{
            //    Databases_list.Items.Add(db);
            //}

        }


        private void Databases_list_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            _controller.HandleDBChange((string)comboBox.SelectedValue);
            //if (comboBox.SelectedIndex!=0)
            //{               
            //    string selectedDb = (string)comboBox.SelectedItem;               
            //    mysqlModel.dbName = selectedDb;               
            //    tide.generateTableButtuns(ButtonsPanel, Table_parametres);
            //    loger.printLog("db selected - " + selectedDb);                
            //    tide.getTablesStructure();
            //}

        }

        public void FillButtons(List<Models.Table> tables)
        {
            List<Button> buttons = new List<Button>();
            ButtonsPanel.Children.Clear();
            foreach (Models.Table table in tables)
            {
                Button newButton = new Button();
                newButton.Click += (s, e) => { _controller.HandleTableButtonClick(table); };
                newButton.Name = table.name + "_btn";
                newButton.Content = table.name;
                newButton.Height = 50;
                ButtonsPanel.Children.Add(newButton);
            }

        }

        /// <summary>
        /// handle each button click (table button)
        /// </summary>
        /// <param name="Table_parametres"></param>
        /// <param name="line"></param>
        public void ShowTableParametres(Models.Table tables)
        {

            Table_parametres.Items.Clear();
            foreach (var param in tables.columns)
            {
                Table_parametres.Items.Add(new Parameter(){Name=param.name, Type=param.type});
            }           
          
        }

        private class Parameter{
            public string Name {set; get;}
            public string Type {set;get;}
        }

        public void fillDataTable(Models.Table table)
        {
            //DataGrid tableData = new DataGrid();
            //DataTable dt = new DataTable();
            //int o = 0;


            //List<List<object>> listas = new List<List<object>>();


            //foreach(string s in table.columns.Select(n=>n.name).ToList())
            //{
            //    DataColumn dc = new DataColumn(s, typeof(string));
            //    dt.Columns.Add(dc);
            //    o++;

            //    List<object> l = new List<object>();
            //    foreach (object obj in kk.Value)
            //    {                  
            //        l.Add(obj);
            //    }
            //    listas.Add(l);

            //}

            //for (int i = 0; i < ((List<object>)listas.ElementAt(0)).Count; i++)
            //{
            //    DataRow dr = dt.NewRow();
            //    int e = 0;
            //    foreach (object l in listas)
            //    {
            //        dr[e] = ((List<object>)listas.ElementAt(e)).ElementAt(i).ToString();
            //        //dr[e] = "takas";
            //        e++;
            //    }
            //    dt.Rows.Add(dr);
            //}

            //DataView dw = new DataView(dt);
            //tableData.ItemsSource = dw;



        }

    }
}
