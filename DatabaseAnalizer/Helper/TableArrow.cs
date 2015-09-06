using DatabaseAnalizer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace DatabaseAnalizer.Helper
{
    public class TableArrow
    {

        public Table startTable { set; get; }
        public Column startColumn { set; get; }
        public ListBox startMovableElement { set; get; }

        public double xDif { set; get; }
        public double yDif { set; get; }

        public Table endTable { set; get; }
        public Column endColumn { set; get; }
        public ListBox endMovableElement { set; get; }

        public Line line;

        public TableArrow(Table table, Column col)
            : base()
        {
            startTable = table;
            startColumn = col;
            line = new Line();
            line.Stroke = System.Windows.Media.Brushes.LightSteelBlue;
            line.StrokeThickness = 2;
            xDif = 0;
            yDif = 0;
        }

    }
}
