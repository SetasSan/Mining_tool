using DatabaseAnalizer.Helper;
using DatabaseAnalizer.Helper.Enums;
using DatabaseAnalizer.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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

namespace DatabaseAnalizer.Views
{
    /// <summary>
    /// Interaction logic for Filter.xaml
    /// </summary>
    public partial class Filter : Window
    {
        public string FilterFieldName { set; get; }
        public string FilterFieldType { set; get; }
        public CompareValue SelectedCompareValue { set; get; }

        public ObservableCollection<string> CompareValues { get; set; }
        public ObservableCollection<string> CompareColumns { get; set; }
        public List<Column> Columns { set; get; }
        public List<ConditionSetting> AddedFilters { set; get; }      
        public List<ConditionSetting> RemovedFilters { set; get; }
        public MainWindow MainWindow { get; set; }

        public Filter(MainWindow window)
        {
            MainWindow = window;
            InitializeComponent();
            Closing += ChildWindowClosing;
            AddedFilters = new List<ConditionSetting>();        
            RemovedFilters = new List<ConditionSetting>();
            SelectedCompareValue = CompareValue.Equal;
            CompareValues = new ObservableCollection<string>();
            CompareValues.Add(EnumHelper.GetDescription(CompareValue.Equal));
            CompareValues.Add(EnumHelper.GetDescription(CompareValue.Greater));
            CompareValues.Add(EnumHelper.GetDescription(CompareValue.GreaterOrEqual));
            CompareValues.Add(EnumHelper.GetDescription(CompareValue.GreaterOrLess));
            CompareValues.Add(EnumHelper.GetDescription(CompareValue.Less));
            CompareValues.Add(EnumHelper.GetDescription(CompareValue.NotEqual));
            ConditionBox.ItemsSource = CompareValues;
            Columns = new List<Column>();
        }

        public void FillFilter()
        {
            CompareColumns = new ObservableCollection<string>();
            foreach (var col in Columns)
                CompareColumns.Add(col.Name);
            ConditionBoxFields.ItemsSource = CompareColumns;

            Filter_conditions.Items.Clear();
            foreach (var item in AddedFilters)
                Filter_conditions.Items.Add(item);

        }
        private void ChildWindowClosing(object sender, CancelEventArgs e)
        {            
            CloseWindow();
            e.Cancel = true;
        }

        private void Filter_Clicked(object sender, RoutedEventArgs e)
        {

            CleanFilter(false);         
            MainWindow.Filter(AddedFilters);         

        }


        public ConditionSetting SelectedItem;

        private void AddFilterInBox(object sender, RoutedEventArgs e)
        {
            var filt = new ConditionSetting()
            {
                Name = (string)ConditionBoxFields.SelectedValue,
                Condition = (string)ConditionBox.SelectedValue,
                Value = Condition_value.Text,
                Action = new Button()
                {
                    Content = "X"
                },
                Id = Guid.NewGuid()
            };
            AddedFilters.Add(filt);        
            Filter_conditions.Items.Add(filt);
        }

        private void Delete(object sender, RoutedEventArgs e)
        {
            SelectedItem = ((FrameworkElement)sender).DataContext as ConditionSetting;
            if (null != SelectedItem)
            {
                RemovedFilters.Add(SelectedItem);
                Filter_conditions.Items.Remove(SelectedItem);
            }
        }

        private void Cancell_Click(object sender, RoutedEventArgs e)
        {
            CloseWindow();
        }

        private void CloseWindow()
        {           
            CleanFilter(true);          
            Hide();
        }

        private void CleanFilter(bool cleanDisplay)
        {
            foreach (var item in RemovedFilters)
            {
                if (AddedFilters.Where(w => w.Id == item.Id).Any())
                {
                    var items = AddedFilters.Where(w => w.Id == item.Id).ToList();
                    foreach (var it in items)
                        AddedFilters.Remove(it);
                }
            }

            if (cleanDisplay)
            {
                RemovedFilters = new List<ConditionSetting>();
                Filter_conditions.Items.Clear();             
            }
        }

    }



    public class ConditionSetting
    {
        public Guid Id { set; get; }
        public string Name { set; get; }
        public string Condition { set; get; }
        public string Value { set; get; }
        public Button Action { set; get; }
    }
}
