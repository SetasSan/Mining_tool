using DatabaseAnalizer.Controllers.Databases;
using DatabaseAnalizer.Controllers.Interfaces;
using DatabaseAnalizer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace DatabaseAnalizer.Controllers
{
    public class Controller
    {
        private MainWindow mainWindow;
        private DBSettingsWindow settingsWindow;
        private List<IServer> servers;
        private IServer selectedServer;
        private Analizer analizer;
        private List<Table> tablesForAnalize;



        public Controller()
        {
            servers = new List<IServer>();
            AddServers();
            mainWindow = new MainWindow(this);
            settingsWindow = new DBSettingsWindow(this);
            SetUpWindows();
            analizer = new Analizer();
            tablesForAnalize = new List<Table>();
        }

        private void AddServers()
        {
            IServer mySqlServer = new MySqlServer("MySql");
            servers.Add(mySqlServer);
        }

        private void SetUpWindows()
        {
            mainWindow.Visibility = System.Windows.Visibility.Hidden;
            settingsWindow.Visibility = System.Windows.Visibility.Visible;
            settingsWindow.Activate();
        }

        public void Connect()
        {
            selectedServer = servers.Single(s => s.GetServerName() == settingsWindow.GetSelectedServerName());
            if (selectedServer != null)
            {
                settingsWindow.Visibility = System.Windows.Visibility.Hidden;
                mainWindow.Visibility = System.Windows.Visibility.Visible;
                mainWindow.Activate();
                selectedServer.SetUserName(settingsWindow.UserName.Text);
                selectedServer.SetUserPassword(settingsWindow.UserPassword.Text);
                selectedServer.SetServerAddress(settingsWindow.Server.Text);
                selectedServer.Extract();
                FillDbDropDownBox();

            }
            else
            {
                throw new Exception("Server has been not selected");
            }

        }

        public string[] GetDatabasesNames()
        {
            return servers.Select(n => n.GetServerName()).ToArray();
        }

        private void FillDbDropDownBox()
        {
            foreach (var database in selectedServer.GetDatabases())
            {
                mainWindow.Databases_list.Items.Add(database.GetDBName());
            }
        }

        /// <summary>
        /// fill with buttons by db name
        /// </summary>
        /// <param name="dbName"></param>
        public void HandleDBChange(string dbName)
        {
            mainWindow.FillButtons(selectedServer.GetDatabases().Single(db => db.GetDBName() == dbName).GetTables());
        }



        public void AddTableRelation(Helper.TableArrow tableArrow)
        {
            foreach (var table in this.tablesForAnalize)
            {
                if (table.Name == tableArrow.endMovableElement.Name)
                {
                    TableRelation relation = new TableRelation()
                    {
                        ForeignTable = tableArrow.startTable,
                    };
                    relation.RelationFromTo.Add(new TableRelation.RelationBeetweenTable
                    {
                        ForeignKey = tableArrow.startColumn,
                        PrimaryKey = tableArrow.endColumn
                    });
                    table.Relations.Add(relation);
                }
            }
        }

        internal void AddTable(Table dragingTable)
        {
            this.tablesForAnalize.Add(dragingTable);
        }

        internal void SetMainTable(Table table)
        {
            foreach (var tab in this.tablesForAnalize)
            {
                tab.IsMainTable = false;
                if (tab.Name == table.Name)
                    tab.IsMainTable = true;
            }
        }

        internal void AnalizeData()
        {
            this.mainWindow.FillGeneratedDataTable(analizer.Analize(tablesForAnalize));
        }
    }
}
