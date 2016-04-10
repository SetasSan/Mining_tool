using DatabaseAnalizer.Controllers.Databases;
using DatabaseAnalizer.Controllers.Interfaces;
using DatabaseAnalizer.Models;
using DatabaseAnalizer.Models.Process;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Xml;

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
        private Dictionary<string, ProcessTypes> logSettings;
        private Table analized;
        private string XESString = "";
        public string SelectedDb { set; get; }


        public Controller()
        {
            servers = new List<IServer>();
            AddServers();
            mainWindow = new MainWindow(this);
            settingsWindow = new DBSettingsWindow(this);
            SetUpWindows();
            analizer = new Analizer();
            tablesForAnalize = new List<Table>();
            logSettings = new Dictionary<string, ProcessTypes>();
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
            SelectedDb = dbName;
            mainWindow.FillButtons(selectedServer.GetDatabases().Single(db => db.GetDBName() == dbName).GetTables());
        }



        public void AddTableRelation(Helper.TableArrow tableArrow)
        {
            foreach (var table in this.tablesForAnalize)
            {
                //Relation then draging happens to selected table
                if (table.Name == tableArrow.endMovableElement.Name)
                {

                    TableRelation startRelation = new TableRelation()
                    {
                        PrimaryTable = tableArrow.startTable,
                        ForeignTable = tableArrow.endTable,
                        ForeignKey = tableArrow.endColumn,
                        PrimaryKey = tableArrow.startColumn
                    };


                    table.RelationsIn.Add(startRelation);
                }

                //Relation then draging happens from selected table
                if (table.Name == tableArrow.startMovableElement.Name)
                {
                    TableRelation endRelation = new TableRelation()
                    {
                        PrimaryTable = tableArrow.startTable,
                        ForeignTable = tableArrow.endTable,
                        ForeignKey = tableArrow.endColumn,
                        PrimaryKey = tableArrow.startColumn
                    };
                    table.RelationsFrom.Add(endRelation);
                }
            }
        }

        internal void AddTable(Table dragingTable)
        {
            this.tablesForAnalize.Add(dragingTable);            
        }

        internal List<Models.Table> GetAllTables()
        {
            return this.tablesForAnalize;
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
            analized = analizer.Analize(tablesForAnalize);
            analized = this.selectedServer.LeftJoinTables(tablesForAnalize, analized, this.SelectedDb);
            this.mainWindow.FillGeneratedDataTable(analized);            
            this.mainWindow.AddLogSettings(analized);
        }

        public bool ExistMainTable()
        {
            return tablesForAnalize.Where(t => t.IsMainTable).Any();
        }

        internal void AddColumnRole(string name, ProcessTypes selected)
        {
            if (logSettings.Where(l => l.Key == name).Any())
                logSettings[name] = selected;
            else
                logSettings.Add(name, selected);
        }

        internal string GetGeneratedLog()
        {
            Log log = new Log();
            if (analized != null)
            {
                var traces = logSettings.Where(s => s.Value == ProcessTypes.Trace).Select(s => s.Key);
                var eventColName = logSettings.Where(s => s.Value == ProcessTypes.Event).Select(s => s.Key).SingleOrDefault();
                var timeColName = logSettings.Where(s => s.Value == ProcessTypes.Time).Select(s => s.Key).SingleOrDefault();
                var traceCols = analized.Columns.Where(s => traces.Contains(s.Name));

                Dictionary<string, List<string>> distinctedCols = new Dictionary<string, List<string>>();
                foreach (var traceCol in traceCols)
                    distinctedCols.Add(traceCol.Name, traceCol.CellsData.Select(s => s.Value).Distinct().ToList());

                for (int i = 0; i < analized.Columns.FirstOrDefault().CellsData.Count(); i++)
                {
                    Event even = new Event();
                    string traceName = "";
                    foreach (var cols in analized.Columns)
                    {
                        foreach (var dist in distinctedCols)
                        {
                            if (cols.Name == dist.Key)
                                traceName += cols.CellsData.Where(k => k.Key == i).SingleOrDefault().Value;
                        }
                        if (cols.Name == eventColName)
                            even.Name = cols.CellsData.Where(k => k.Key == i).SingleOrDefault().Value;
                        if (cols.Name == timeColName)
                            even.Time = Convert.ToDateTime(cols.CellsData.Where(k => k.Key == i).SingleOrDefault().Value).ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss");
                    }

                    if (!log.traces.Where(n => n.traceName == traceName).Any())
                    {
                        log.traces.Add(new Trace()
                        {
                            traceName = traceName,
                        });
                    }
                    log.traces.Where(t => t.traceName == traceName).SingleOrDefault().events.Add(even);
                }
            }

            //for (int index = 0; index < analized.table.Columns.FirstOrDefault().CellsData.Count(); index++)
            //{
            //    Event even = new Event();
            //    foreach (var col in analized.table.Columns)
            //    {
            //        even.atributes.Add(new Models.Process.Attributes.Attribute(col.Name, getTypeForXml(col.Type), col.CellsData.Where(w => w.Key == index).SingleOrDefault().Value));
            //    }
            //    log.trace.events.Add(even);
            //}
            return PrintXML(log.ToString());
        }

        private string getTypeForXml(string type)
        {
            if (checkType("int"))
                return type.Substring(0, 3);
            else if (checkType("bigint"))
                return "int";
            else if (checkType("varchar"))
                return "string";
            else if (checkType("longtext"))
                return "string";
            else if (checkType("datetime"))
                return "date";
            else if (checkType("double"))
                return "float";
            else if (checkType("tinyint"))
                return "boolean";
            else if (checkType("mediumtext"))
                return "string";
            else if (checkType("timestamp"))
                return "date";
            else if (checkType("smallint"))
                return "int";
            else if (checkType("char"))
                return "string";
            else if (checkType("text"))
                return "string";
            else if (checkType("float"))
                return "float";

            else
                return "string";
        }

        private bool checkType(string type)
        {
            return type.Substring(0, type.Length) == type;
        }


        public String PrintXML(String XML)
        {

            try
            {
                MemoryStream mStream = new MemoryStream();
                XmlTextWriter writer = new XmlTextWriter(mStream, Encoding.Unicode);
                XmlDocument document = new XmlDocument();
                document.LoadXml(XML);
                writer.Formatting = Formatting.Indented;
                document.WriteContentTo(writer);
                writer.Flush();
                mStream.Flush();
                mStream.Position = 0;
                StreamReader sReader = new StreamReader(mStream);
                String FormattedXML = sReader.ReadToEnd();
                XESString = FormattedXML;
                mStream.Close();
                writer.Close();
            }
            catch (XmlException)
            {
            }

            return XESString;
        }

        internal string getXESString()
        {
            return XESString;
        }

        internal void ExtractData(string tableName)
        {
            selectedServer.ExtractTablesData(tableName);
        }
    }
}
