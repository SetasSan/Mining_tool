using DatabaseAnalizer.Controllers.Databases;
using DatabaseAnalizer.Controllers.Interfaces;
using DatabaseAnalizer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DatabaseAnalizer.Controllers
{
    public class Controller
    {
        private MainWindow _mainWindow;
        private DBSettingsWindow _settingsWindow;
        private List<IServer> _servers;
        private IServer _selectedServer;
        private Analizer _analizer;

        public Controller()
        {
            _servers = new List<IServer>();
            AddServers();
            _mainWindow = new MainWindow(this);
            _settingsWindow = new DBSettingsWindow(this);
            SetUpWindows();
            _analizer = new Analizer();
        }

        private void AddServers()
        {
            IServer mySqlServer = new MySqlServer("MySql");
            _servers.Add(mySqlServer);
        }

        private void SetUpWindows()
        {
            _mainWindow.Visibility = System.Windows.Visibility.Hidden;
            _settingsWindow.Visibility = System.Windows.Visibility.Visible;
            _settingsWindow.Activate();
        }

        public void Connect()
        {
            _selectedServer = _servers.Single(s => s.GetServerName() == _settingsWindow.GetSelectedServerName());
            if (_selectedServer != null)
            {
                _settingsWindow.Visibility = System.Windows.Visibility.Hidden;
                _mainWindow.Visibility = System.Windows.Visibility.Visible;
                _mainWindow.Activate();
                _selectedServer.SetUserName(_settingsWindow.UserName.Text);
                _selectedServer.SetUserPassword(_settingsWindow.UserPassword.Text);
                _selectedServer.SetServerAddress(_settingsWindow.Server.Text);
                _selectedServer.Extract();
                FillDbDropDownBox();

            }
            else
            {
                throw new Exception("Server has been not selected");
            }

        }

        public string[] GetDatabasesNames()
        {
            return _servers.Select(n => n.GetServerName()).ToArray();
        }

        private void FillDbDropDownBox()
        {
            foreach (var database in _selectedServer.GetDatabases())
            {
                _mainWindow.Databases_list.Items.Add(database.GetDBName());
            }
        }

       


        /// <summary>
        /// fill with buttons by db name
        /// </summary>
        /// <param name="dbName"></param>
        public void HandleDBChange(string dbName)
        {
            _mainWindow.FillButtons(_selectedServer.GetDatabases().Single(db => db.GetDBName() == dbName).GetTables());
        }


        public void HandleTableButtonClick(Table table)
        {
            _mainWindow.PrintLog("Selected table - "+table.Name);
            _mainWindow.ShowTableParametres(table);
            _mainWindow.FillDataTable(table);
        }


    }
}
