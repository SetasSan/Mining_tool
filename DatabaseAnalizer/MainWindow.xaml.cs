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
        private MySqlConnection connection;        
        private MySqlDbDataModel mysqlModel;
        private Dictionary<string, DatabaseAnalizer.Models.Table> tables;
        public LogPrinter loger;
        private TablesInDBExtracter tide;
        public MainWindow()
        {
            InitializeComponent();
            tables = new Dictionary<string, DatabaseAnalizer.Models.Table>();
            connection = new MySqlConnection(ConfigurationSettings.AppSettings["MySqlConnectionString"]);
            mysqlModel = new MySqlDbDataModel();            
            var nameC = new DataGridTextColumn();
            loger = new LogPrinter(data_displayer);
            nameC.Header = "Name";
            nameC.Width = 120;
            nameC.Binding = new Binding("Name");
            Table_parametres.Columns.Add(nameC);
            var typeC = new DataGridTextColumn();
            typeC.Header = "Type";
            typeC.Width = 190;
            typeC.Binding = new Binding("Type");
            Table_parametres.Columns.Add(typeC);
            RefreshDatabaseList();
            tide = new TablesInDBExtracter(connection, ref mysqlModel, ref loger, Table_data, ref tables); 
        }


        private void Select_DB_Click(object sender, RoutedEventArgs e)
        {          
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.ShowDialog();
            if ((ofd.OpenFile()) != null)
            {
                mysqlModel.dbBackupPath = ofd.FileName;
                string dbName;
                new MysqlDBGeneratorFromBackup(connection, mysqlModel).createDbFormBackup(out dbName);
                mysqlModel.dbName = dbName;
                tide.generateTableButtuns(ButtonsPanel, Table_parametres);                                    
            }            
        }

        private void Exit_click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }


        public void RefreshDatabaseList()
        {            
            Databases_list.Items.Clear();
            Databases_list.Items.Add("Select");
            Databases_list.SelectedIndex = 0;

            foreach (string db in (new DBCommandGenerator(connection)).getStringList("select SCHEMA_NAME from information_schema.SCHEMATA"))
            {
                Databases_list.Items.Add(db);
            }
             
        }


        private void Databases_list_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = (ComboBox) sender;
            if (comboBox.SelectedIndex!=0)
            {               
                string selectedDb = (string)comboBox.SelectedItem;               
                mysqlModel.dbName = selectedDb;               
                tide.generateTableButtuns(ButtonsPanel, Table_parametres);
                loger.printLog("db selected - " + selectedDb);                
                tide.getTablesStructure();
            }
       
        }        
       
    }
}
