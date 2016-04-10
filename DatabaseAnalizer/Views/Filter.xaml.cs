using DatabaseAnalizer.Helper.Enums;
using System;
using System.Collections.Generic;
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
        public CompareValue SelectedCompareValue {set;get;}
        public List<FilerValue> FilterValues { set; get; }
        public Filter()
        {
            InitializeComponent();
            Closing += ChildWindowClosing;
            FilterValues = new List<FilerValue>();
            SelectedCompareValue = CompareValue.Equal;
        }

        public void FillFilter(){
            TypeField.Content = FilterFieldType;
            NameField.Text = FilterFieldName;
            FromField.Content = FilterFieldName;
            
            ToField.Content = FilterFieldName;
        }
        private void ChildWindowClosing(object sender, CancelEventArgs e)
        {
            e.Cancel = true;
            Visibility = Visibility.Hidden;
        }

        private void AddFilterValue(object sender, RoutedEventArgs e)
        {
            if(FilterValues.Where(w=>w.FieldName == FilterFieldName).Any()){

            }else{
                FilterValues.Add( new FilerValue(){
                    CopmareValue = SelectedCompareValue,
                    FieldName = FilterFieldName,                                        
                });
            }
        }
    }

    public class FilerValue
    {
        public string FieldName { set; get; }
        public CompareValue CopmareValue { set; get; }
        public string Valuefrom { set; get; }
        public string ValueTo { set; get; }
    }
}
