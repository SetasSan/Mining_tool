using DatabaseAnalizer.Controllers;
using DatabaseAnalizer.Helper;
using DatabaseAnalizer.Models;
using DatabaseAnalizer.Views;
using Microsoft.Win32;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
        private DatabaseAnalizer.Models.Table dragingTable;
        private double subX = 0;
        private double subY = 0;
        private bool isElementMoving = false;
        private ListBox movingElement;
        private bool isDrawingLine = false;
        private TableArrow currentTableArrow;
        public List<TableArrow> tableArrows = new List<TableArrow>();
        private List<ComboBoxValues> ColumnNames { set; get; }
        private Filter filter;
        private Models.Table selectedTable { set; get; }
        private Models.Table joinedTable { set; get; }
        private List<ConditionSetting> Filters { set; get; }

        public MainWindow(Controller controller)
        {
            filter = new Filter(this);
            InitializeComponent();
            _controller = controller;
            CreateParametersTable();
            this.Closing += CloseMainWindow;

        }

        private void CloseMainWindow(object sender, CancelEventArgs e)
        {
            filter.Close();
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

        private void ExportLogFile_click(object sender, RoutedEventArgs e)
        {
            string result = _controller.getXESString();
            if (result != "")
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "XES file|*.xes";
                saveFileDialog.Title = "Save an XES file";
                saveFileDialog.FileName = _controller.SelectedDb + DateTime.Now.ToString("yyyy-mm-dd-HH-mm-ss") + ".xes";
                if (saveFileDialog.ShowDialog() == true)
                {
                    StreamWriter writer = new StreamWriter(saveFileDialog.OpenFile());
                    writer.WriteLine(_controller.getXESString());
                    writer.Dispose();
                    writer.Close();
                }
            }
            else
            {
                PrintLog("XES log not formated");
            }
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
                newButton.PreviewMouseDown += (s, e) => { HandleMouseDown(table); };
                newButton.PreviewMouseDown += newButton_MouseDown;
                newButton.Name = table.Name + "_btn";
                newButton.Content = table.Name;
                newButton.Height = 50;
                ButtonsPanel.Children.Add(newButton);
            }

        }

        private void newButton_MouseDown(object sender, MouseEventArgs e)
        {
            Button lblFrom = e.Source as Button;

            if (e.LeftButton == MouseButtonState.Pressed)
                DragDrop.DoDragDrop(lblFrom, lblFrom.Content, DragDropEffects.Copy);
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
                Table_parametres.Items.Add(new Parameter() { Name = param.Name, Type = param.Type });
            }
        }

        private class Parameter
        {
            public string Name { set; get; }
            public string Type { set; get; }
        }
        private class ParameterForSettings
        {
            public string Name { set; get; }
            public DataGridComboBoxColumn Type { set; get; }
        }

        public void FillDataTable(Models.Table table)
        {
            FillDataTable(table, table_data);
        }

        public void FillGeneratedDataTable(Models.Table table)
        {
            joinedTable = table;
            generated_table_data.Columns.Clear();
            FillDataTable(table, generated_table_data);
        }

        private void FillDataTable(Models.Table table, DataGrid grid)
        {
            selectedTable = table;
            table_data.Columns.Clear();
            DataTable dt = new DataTable();
            grid.IsReadOnly = true;
            grid.AutoGenerateColumns = false;

            foreach (Column column in table.Columns)
            {
                DataGridTextColumn col = new DataGridTextColumn();
                col.Binding = new Binding(column.Name.Replace(".", ""));
                var spHeader = new StackPanel() { Orientation = Orientation.Horizontal };
                spHeader.Children.Add(new Label() { Content = column.Name.Replace("_", "__") });
                col.Header = spHeader;
                grid.Columns.Add(col);
            }

            foreach (Column column in table.Columns)
            {
                DataColumn dc = new DataColumn(column.Name.Replace(".", ""), typeof(string));
                dt.Columns.Add(dc);
            }

            if (table.Columns.First().CellsData != null)
                for (int i = 0; i < table.Columns.Select(s => s.CellsData.Count()).Max(); i++)
                {
                    DataRow dr = dt.NewRow();
                    int e = 0;
                    foreach (object l in table.Columns)
                    {
                        if (table.Columns.ElementAt(e).CellsData.Count() > i)
                            dr[e] = table.Columns.ElementAt(e).CellsData.ElementAt(i).Value != "" ? table.Columns.ElementAt(e).CellsData.ElementAt(i).Value.ToString() : "null";
                        else
                            dr[e] = "-";
                        e++;
                    }
                    dt.Rows.Add(dr);

                }

            DataView dw = new DataView(dt);
            grid.ItemsSource = dw;
        }


        public void PrintLog(String log)
        {
            data_displayer.AppendText(DateTime.Now.ToString("h:mm:ss tt") + " " + log + "\r\n");
            data_displayer.ScrollToEnd();
        }

        private void DropHandler(object sender, DragEventArgs e)
        {
            Canvas canvas = sender as Canvas;
            Models.Table currentTable = new Models.Table();
            foreach (var item in canvas.Children)
            {
                if (item.GetType() == typeof(ListBox))
                {
                    if (((ListBox)item).Name == dragingTable.Name)
                    {
                        PrintLog("Table - " + this.dragingTable.Name + " has been already added !");
                        return;
                    }
                }
            }
            _controller.AddTable(dragingTable);
            currentTable = _controller.GetAllTables().Where(s => s.Name == dragingTable.Name).SingleOrDefault();


            ListBox mainItem = new ListBox();
            ListBox listBox = new ListBox();
            int charsize = 10;
            int charcount = currentTable.Name.Count() < 6 ? 6 : currentTable.Name.Count();

            //calculate labels width
            foreach (var col in currentTable.Columns)
            {
                charcount = col.Name.Count() > charcount ? col.Name.Count() : charcount;
            }

            //add table columns
            foreach (var col in currentTable.Columns)
            {
                Label lab = new Label() { Content = col.Name };
                lab.Width = charcount * charsize - charsize * 2;
                listBox.Items.Add(lab);

                lab.PreviewMouseLeftButtonDown += (s, er) => { HandlelistBoxClick(currentTable, col, s); };
                lab.PreviewMouseLeftButtonDown += lab_PreviewMouseLeftButtonDown;
            }

            Canvas.SetLeft(mainItem, e.GetPosition(canvas).X);
            Canvas.SetTop(mainItem, e.GetPosition(canvas).Y);

            if (canvas.Width < e.GetPosition(canvas).X + charcount * charsize)
                canvas.Width = e.GetPosition(canvas).X + charcount * charsize;

            if (canvas.Height < e.GetPosition(canvas).Y + (currentTable.Columns.Count + 1) * 30)
                canvas.Height = e.GetPosition(canvas).Y + (currentTable.Columns.Count + 1) * 30;

            //table header
            Label header = new Label()
            {
                Name = dragingTable.Name,
                Content = dragingTable.Name.Replace("_", "__"),
                Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(System.Drawing.Color.Gray.A, System.Drawing.Color.Gray.R, System.Drawing.Color.Gray.G, System.Drawing.Color.Gray.B)),
                Width = charcount * charsize - charsize
            };

            ContextMenu tableHeaderMeniu = new System.Windows.Controls.ContextMenu();
            MenuItem makeMainTable = new MenuItem()
            {
                Header = "Make this table main",
            };
            MenuItem removeTable = new MenuItem()
            {
                Header = "Remove this table",
            };
            DatabaseAnalizer.Models.Table tempTable = new DatabaseAnalizer.Models.Table();
            tempTable = dragingTable;

            makeMainTable.PreviewMouseLeftButtonDown += (s, er) => { makeMainTable_PreviewMouseLeftButtonDown(tempTable, header); };
            removeTable.PreviewMouseLeftButtonDown += (s, er) => { removeTableFromCanvas_CLick(tempTable, header); };

            tableHeaderMeniu.Items.Add(makeMainTable);
            tableHeaderMeniu.Items.Add(removeTable);
            header.ContextMenu = tableHeaderMeniu;
            mainItem.Name = dragingTable.Name;

            mainItem.Items.Add(header);

            header.PreviewMouseLeftButtonDown += mainItem_PreviewMouseLeftButtonDown;
            header.PreviewMouseLeftButtonUp += mainItem_PreviewMouseLeftButtonUp;


            mainItem.Width = charcount * charsize;
            listBox.Width = charcount * charsize - charsize;
            mainItem.Items.Add(listBox);
            canvas.Children.Add(mainItem);
        }

        private void removeTableFromCanvas_CLick(Models.Table tempTable, Label header)
        {
            string tableName = header.Content.ToString();
            

            ListBox listBoxForRemove = null;
            foreach (var canItem in Relation_Canvas.Children)
            {
                if (canItem.GetType() == typeof(ListBox) && ((ListBox)canItem).Name == header.Content.ToString())
                {
                    listBoxForRemove = canItem as ListBox;


                }

            }

            //cremove visual table
            if (listBoxForRemove != null)
                Relation_Canvas.Children.Remove(listBoxForRemove);

            //remove arrows
            var tableArrowsForRemoving = tableArrows.Where(s => s.endMovableElement.Name == header.Content.ToString() || s.startMovableElement.Name == header.Content.ToString()).ToList();
        
            foreach (var arrow in tableArrowsForRemoving)
            {
                tableArrows.Remove(arrow);
                Relation_Canvas.Children.Remove(arrow.line);

            }


            _controller.removeTable(tableName);

        }

        public void makeMainTable_PreviewMouseLeftButtonDown(DatabaseAnalizer.Models.Table table, Label sender)
        {
            ListBox lb = sender.Parent as ListBox;
            Canvas mainCanvas = lb.Parent as Canvas;
            foreach (var canItem in mainCanvas.Children)
            {
                if (canItem.GetType() == typeof(ListBox))
                {
                    ListBox lbInCanvas = canItem as ListBox;
                    foreach (var lbItem in lbInCanvas.Items)
                    {
                        if (lbItem.GetType() == typeof(Label))
                        {
                            Label labelInTable = lbItem as Label;
                            labelInTable.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(System.Drawing.Color.Gray.A, System.Drawing.Color.Gray.R, System.Drawing.Color.Gray.G, System.Drawing.Color.Gray.B));
                        }
                    }
                }
            }

            sender.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(System.Drawing.Color.Green.A, System.Drawing.Color.Green.R, System.Drawing.Color.Green.G, System.Drawing.Color.Green.B));
            _controller.SetMainTable(table);
        }

        private void canvas_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (this.isDrawingLine)
            {
                Canvas mainCanvas = sender as Canvas;
                this.isDrawingLine = false;
                mainCanvas.Children.Remove(currentTableArrow.line);
                this.currentTableArrow.line = null;
            }
        }

        private void lab_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Canvas mainCanvas = getMainCanvasFromTableListItem(sender);
            ListBox moovingLabel = getMovingLabelFromTableItem(sender);
            if (this.isDrawingLine && currentTableArrow.startMovableElement.Name != moovingLabel.Name)
            {
                SetArrowEndPosition(this.currentTableArrow, e.MouseDevice.GetPosition(mainCanvas).X, e.MouseDevice.GetPosition(mainCanvas).Y);
                this.currentTableArrow.endMovableElement = moovingLabel;
                this.tableArrows.Add(this.currentTableArrow);
                _controller.AddTableRelation(this.currentTableArrow);
                this.isDrawingLine = false;
            }
            else
            {
                mainCanvas.Children.Add(this.currentTableArrow.line);
                SetArrowStartPosition(this.currentTableArrow, e.MouseDevice.GetPosition(mainCanvas).X, e.MouseDevice.GetPosition(mainCanvas).Y);
                this.currentTableArrow.startMovableElement = moovingLabel;
                this.isDrawingLine = true;
            }
        }

        private void mainItem_PreviewMouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            isElementMoving = false;
        }

        public void HandlelistBoxClick(DatabaseAnalizer.Models.Table table, Column col, object s)
        {

            if (!this.isDrawingLine)
            {
                this.currentTableArrow = new TableArrow(table, col);
            }
            else if (this.currentTableArrow != null)
            {
                this.currentTableArrow.endTable = table;
                this.currentTableArrow.endColumn = col;
            }

        }

        public Canvas getMainCanvasFromTableListItem(object sender)
        {
            Label lab = sender as Label;
            ListBox lb1 = lab.Parent as ListBox;
            ListBox lb2 = lb1.Parent as ListBox;
            return lb2.Parent as Canvas;
        }

        public ListBox getMovingLabelFromTableItem(object sender)
        {
            Label lab = sender as Label;
            ListBox lb1 = lab.Parent as ListBox;
            return lb1.Parent as ListBox;
        }



        private void mainItem_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            isElementMoving = true;

            if (sender.GetType() == typeof(Label))
                this.movingElement = ((Label)sender).Parent as ListBox;
            Canvas mainCanvas = this.movingElement.Parent as Canvas;
            this.subX = e.MouseDevice.GetPosition(this.movingElement).X;
            this.subY = e.MouseDevice.GetPosition(this.movingElement).Y;

            foreach (var arrow in this.tableArrows)
            {
                if (arrow.startMovableElement != null && this.movingElement.Name == arrow.startMovableElement.Name)
                {
                    arrow.xDif = arrow.line.X1 - e.MouseDevice.GetPosition(mainCanvas).X;
                    arrow.yDif = arrow.line.Y1 - e.MouseDevice.GetPosition(mainCanvas).Y;
                }
                if (arrow.endMovableElement != null && this.movingElement.Name == arrow.endMovableElement.Name)
                {
                    arrow.xDif = arrow.line.X2 - e.MouseDevice.GetPosition(mainCanvas).X;
                    arrow.yDif = arrow.line.Y2 - e.MouseDevice.GetPosition(mainCanvas).Y;
                }
            }
        }

        private void SetArrowEndPosition(TableArrow currentTableArrow, double x, double y)
        {
            currentTableArrow.line.X2 = x;
            currentTableArrow.line.Y2 = y;
        }

        private void SetArrowStartPosition(TableArrow tableArrow, double x, double y)
        {
            tableArrow.line.X1 = x;
            tableArrow.line.Y1 = y;
        }


        /// <summary>
        /// move arrows then table is moving in screen
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        private void MoveArrows(double x, double y)
        {
            foreach (var arrow in this.tableArrows)
            {
                if (this.movingElement.Name == arrow.startMovableElement.Name)
                {
                    SetArrowStartPosition(arrow, x + arrow.xDif, y + arrow.yDif);
                }
                if (this.movingElement.Name == arrow.endMovableElement.Name)
                {
                    SetArrowEndPosition(arrow, x + arrow.xDif, y + arrow.yDif);
                }
            }
        }


        /// <summary>
        /// draging table from table list
        /// </summary>
        /// <param name="table"></param>
        public void HandleMouseDown(DatabaseAnalizer.Models.Table table)
        {
            _controller.ExtractData(table.Name);
            ShowTableParametres(table);
            FillDataTable(table);
            dragingTable = table;
        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (isElementMoving)
            {
                Canvas mainCanvas = sender as Canvas;
                Canvas.SetLeft(movingElement, e.GetPosition(mainCanvas).X - this.subX);
                Canvas.SetTop(movingElement, e.GetPosition(mainCanvas).Y - this.subY);

                if (mainCanvas.Width < e.GetPosition(mainCanvas).X + movingElement.ActualWidth - this.subX)
                    mainCanvas.Width = e.GetPosition(mainCanvas).X + movingElement.ActualWidth - this.subX;


                if (mainCanvas.Height < e.GetPosition(mainCanvas).Y + movingElement.ActualHeight - this.subY)
                    mainCanvas.Height = e.GetPosition(mainCanvas).Y + movingElement.ActualHeight - this.subY;

                MoveArrows(e.GetPosition(mainCanvas).X, e.GetPosition(mainCanvas).Y);
            }

            if (this.isDrawingLine)
            {
                Canvas mainCanvas = sender as Canvas;
                if (this.currentTableArrow.line != null)
                {
                    int x = this.currentTableArrow.line.X1 > e.GetPosition(mainCanvas).X ? 2 : -2;
                    int y = this.currentTableArrow.line.Y1 > e.GetPosition(mainCanvas).Y ? 2 : -2;
                    SetArrowEndPosition(currentTableArrow, e.GetPosition(mainCanvas).X + x, e.GetPosition(mainCanvas).Y + y);
                }
            }
        }

        private void Generate_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            _controller.AnalizeData(Filters);
        }

        private void Analize_Click(object sender, RoutedEventArgs e)
        {
            if (_controller.ExistMainTable())
                _controller.AnalizeData(Filters);
            else
                PrintLog("No main table selected");
        }

        public void AddLogSettings(Models.Table analized)
        {
            ColumnNames = new List<ComboBoxValues>();
            foreach (var item in analized.Columns.Select(s => s.Name))
            {
                ComboBoxValues cbv = new ComboBoxValues() { Name = item };
                cbv.Types = new ObservableCollection<ProcessTypes>();
                cbv.Types.Add(ProcessTypes.Actor);
                cbv.Types.Add(ProcessTypes.Atribute);
                cbv.Types.Add(ProcessTypes.Event);
                cbv.Types.Add(ProcessTypes.Trace);
                cbv.Types.Add(ProcessTypes.Time);
                ColumnNames.Add(cbv);
            }
            Table_settings.DataContext = ColumnNames;
        }

        public class ComboBoxValues
        {
            public string Name { set; get; }
            public ObservableCollection<ProcessTypes> Types { get; set; }
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox cb = sender as ComboBox;
            string name = cb.ToolTip.ToString();
            ProcessTypes selected = (ProcessTypes)cb.SelectedValue;
            _controller.AddColumnRole(name, selected);
        }

        private void GenerateLog_Click(object sender, RoutedEventArgs e)
        {
            geberated_table_log.Text = _controller.GetGeneratedLog();
        }

        private void r2_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.PropertyName.Contains('.') && e.Column is DataGridBoundColumn)
            {
                DataGridBoundColumn dataGridBoundColumn = e.Column as DataGridBoundColumn;
                dataGridBoundColumn.Binding = new Binding("[" + e.PropertyName + "]");
            }
        }

        private void OpenFilterClick(object sender, RoutedEventArgs e)
        {

            if (joinedTable != null && joinedTable.Columns.Any())
            {
                filter.Owner = this;
                filter.Columns = new List<Column>();
                filter.Columns.AddRange(joinedTable.Columns);
                filter.FillFilter();
                filter.ShowDialog();
            }

            else
                PrintLog("No table analized");

        }


        internal void Filter(List<ConditionSetting> AddedFilters)
        {
            Filters = AddedFilters;
            _controller.AnalizeData(Filters);
        }

        private void Find_relations_from_schema_Click(object sender, RoutedEventArgs e)
        {
            _controller.CreateRelationsByDb();
        }

        public ArrowElements MakeArrowElemets(RelationFromDb relation)
        {

            ArrowElements arrowElements = new ArrowElements();
            arrowElements.line = new Line();
            arrowElements.line.Stroke = System.Windows.Media.Brushes.LightSteelBlue;
            arrowElements.line.StrokeThickness = 2;


            bool foundEnd = false;

            foreach (var item in Relation_Canvas.Children)
            {

                if (item.GetType() == typeof(ListBox) && ((ListBox)item).Name == relation.TableName)
                {
                    var listBox = ((ListBox)item);
                    foreach (var columnsListBox in listBox.Items)//canvase esantis pagrindinis listboxas
                    {
                        if (columnsListBox.GetType() == typeof(ListBox))
                        {
                            int i = 0;
                            foreach (var colInTableList in ((ListBox)columnsListBox).Items)
                            {//lenteleje esantis headeris ir listboxas
                                i++;
                                if (((Label)colInTableList).Content.ToString() == relation.ColumnName)
                                {
                                    var label = (Label)colInTableList;
                                    UIElement container = VisualTreeHelper.GetParent(listBox) as UIElement;
                                    Point relativeLocation = listBox.TranslatePoint(new Point(0, 0), container);
                                    arrowElements.line.X1 = relativeLocation.X + 20;
                                    arrowElements.line.Y1 = relativeLocation.Y + i * 30 + 7;
                                    arrowElements.startMovableElement = listBox;
                                }
                            }
                        }

                    }

                }

                if (item.GetType() == typeof(ListBox) && ((ListBox)item).Name == relation.ReferenceTableName)
                {
                    var listBox = ((ListBox)item);
                    Label mainTableLabel = null;
                    foreach (var columnsListBox in listBox.Items)//canvase esantis pagrindinis listboxas
                    {
                        if (columnsListBox.GetType() == typeof(Label))
                        {
                            mainTableLabel = (Label)columnsListBox;
                        }
                        if (columnsListBox.GetType() == typeof(ListBox))
                        {
                            int i = 0;
                            foreach (var colInTableList in ((ListBox)columnsListBox).Items)
                            {//lenteleje esantis headeris ir listboxas
                                if (((Label)colInTableList).Content.ToString() == relation.ReferenceColumnName)
                                {
                                    i++;
                                    var label = (Label)colInTableList;
                                    UIElement container = VisualTreeHelper.GetParent(listBox) as UIElement;
                                    Point relativeLocation = listBox.TranslatePoint(new Point(0, 0), container);
                                    arrowElements.line.X2 = relativeLocation.X + 20;
                                    arrowElements.line.Y2 = relativeLocation.Y + i * 30 + 15;
                                    arrowElements.endMovableElement = listBox;
                                    arrowElements.tableLabel = mainTableLabel;
                                    foundEnd = true;

                                }
                            }
                        }

                    }

                }

            }

            if (!foundEnd)
                return null;

            Relation_Canvas.Children.Add(arrowElements.line);
            return arrowElements;
        }
    }

    public class ArrowElements
    {
        public Line line { set; get; }
        public ListBox startMovableElement { set; get; }
        public ListBox endMovableElement { set; get; }
        public Label tableLabel { set; get; }
    }
}
