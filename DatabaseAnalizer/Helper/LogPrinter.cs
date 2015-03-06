using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseAnalizer.Helper
{
    public class LogPrinter
    {
        private System.Windows.Controls.TextBox data_displayer;

        public LogPrinter(System.Windows.Controls.TextBox data_displayer)
        {
            // TODO: Complete member initialization
            this.data_displayer = data_displayer;
        }
        public void printLog(String log)
        {
            data_displayer.AppendText(DateTime.Now.ToString("h:mm:ss tt")+" "+ log + "\r\n");
            data_displayer.ScrollToEnd();
        }
    }
}
