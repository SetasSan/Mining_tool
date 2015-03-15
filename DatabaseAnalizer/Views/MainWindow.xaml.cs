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

        
        private Controller _controller;       
        public MainWindow(Controller controller)
        {
            InitializeComponent();
            _controller = controller;
            CreateParametersTable();          
        }

        private void CreateParametersTable()
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

      
        private void Exit_click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        

        private void Databases_list_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            _controller.HandleDBChange((string)comboBox.SelectedValue);         
        }

        /// <summary>
        /// fill view with buttons of table by selected db
        /// </summary>
        /// <param name="tables"></param>
        public void FillButtons(List<Models.Table> tables)
        {
            List<Button> buttons = new List<Button>();
            ButtonsPanel.Children.Clear();
            foreach (Models.Table table in tables)
            {
                Button newButton = new Button();
                newButton.Click += (s, e) => { _controller.HandleTableButtonClick(table); };
                newButton.Name = table.Name + "_btn";
                newButton.Content = table.Name;
                newButton.Height = 50;
                ButtonsPanel.Children.Add(newButton);
            }

        }

        /// <summary>
        /// handle each button click (table button)
        /// </summary>
        /// <param name="Table_parametres"></param>
        /// <param name="line"></param>
        public void ShowTableParametres(Models.Table table)
        {

            Table_parametres.Items.Clear();
            foreach (var param in table.Columns)
            {
                Table_parametres.Items.Add(new Parameter(){Name=param.Name, Type=param.Type});
            }           
          
        }

        private class Parameter{
            public string Name {set; get;}
            public string Type {set;get;}
        }

        public void FillDataTable(Models.Table table)
        {
            table_data.Columns.Clear();            
            DataTable dt = new DataTable();           
            
            foreach(Column col in table.Columns)
            {
                DataColumn dc = new DataColumn(col.Name.Replace("_", "__"), typeof(string));
                dt.Columns.Add(dc);
            }

            for (int i = 0; i < table.Columns.ElementAt(0).CellsData.Count(); i++)
            {
                    DataRow dr = dt.NewRow();
                    int e = 0;
                    foreach (object l in table.Columns)
                    {
                        dr[e] = table.Columns.ElementAt(e).CellsData.ElementAt(i).ToString();      
                        e++;
                    }
                    dt.Rows.Add(dr);
            }

            DataView dw = new DataView(dt);
            table_data.ItemsSource = dw;
            PrintLog("filled table - "+table.Name);
            
        }

        public void PrintLog(String log)
        {
            data_displayer.AppendText(DateTime.Now.ToString("h:mm:ss tt") + " " + log + "\r\n");
            data_displayer.ScrollToEnd();
        }

    }
}
