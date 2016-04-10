using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseAnalizer.Helper.Enums
{
    public enum DataTypes
    {
        [Description("int")]
        Int,
        [Description("string")]
        String,
        [Description("date")]
        Date,
        [Description("float")]
        Float,
        [Description("boolean")]
        Boolean
    }
}
