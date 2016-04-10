using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseAnalizer.Helper.Enums
{
    public enum CompareValue
    {
        [Description(" = ")]
        Equal,
        [Description(" != ")]
        NotEqual,
        [Description(" > ")]
        Greater,
        [Description(" < ")]
        Less,
        [Description(" => ")]
        GreaterOrEqual,
        [Description(" =< ")]
        GreaterOrLess
    }
}
