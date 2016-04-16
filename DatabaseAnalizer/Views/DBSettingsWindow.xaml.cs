using DatabaseAnalizer.Controllers;
using DatabaseAnalizer.Controllers.Databases;
using DatabaseAnalizer.Controllers.Interfaces;
using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace DatabaseAnalizer
{
    /// <summary>
    /// Interaction logic for DBSettingsWindow.xaml
    /// </summary>
    public partial class DBSettingsWindow : Window
    {

        private Controller _controller;
        private string _selectedServerName;

        public DBSettingsWindow(Controller controller)
        {
            InitializeComponent();
            _controller = controller;
            Init();
        }

        private void ConnectClick(object sender, RoutedEventArgs e)
        {

            String server = this.Server.Text;
            String username = this.UserName.Text;
            String pass = this.UserPassword.Text;

            if (_selectedServerName == null)
            {
                MessageBoxResult result = MessageBox.Show("Database not selected");
                if (result == MessageBoxResult.Yes)
                {
                    Application.Current.Shutdown();
                }

            }
            else
            {
                _controller.PrepareServer();
                var checkMessage = _controller.CheckDatabase();
                if (checkMessage != null)
                {
                    MessageBoxResult result = MessageBox.Show(checkMessage);
                    if (result == MessageBoxResult.Yes)
                    {
                        Application.Current.Shutdown();
                    }
                }
                else
                    _controller.Connect();
            }
        }

        /// <summary>
        /// create all databases
        /// </summary>
        private void Init()
        {
            this.DBSelector.ItemsSource = _controller.GetDatabasesNames();
        }

        private void DBSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = (ComboBox) sender;                    
            _selectedServerName = (string)comboBox.SelectedValue;
                       
        }

        public string GetSelectedServerName()
        {
            return _selectedServerName;
        }


    }
}
