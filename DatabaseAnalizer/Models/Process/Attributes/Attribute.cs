using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseAnalizer.Models.Process.Attributes
{
    public class Attribute
    {
        public string Value { set; get; }
        public string Key { set; get; }
        public string Type { set; get; }
        public List<Attribute> Attributes { set; get; }
        public Attribute(string key, string type, string value)
        {
            Value = value;
            Key = key;
            Type = type;
        }

        public string GetAttributeInXES()
        {
            return "<"+Type+" key="+Key+" value="+Value+" / >";
        }
    }
}
