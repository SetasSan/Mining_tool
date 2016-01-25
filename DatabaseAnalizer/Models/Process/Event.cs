using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseAnalizer.Models.Process
{
    public class Event
    {
        public List<Models.Process.Attributes.Attribute> atributes { set; get; }
        public string Name { set; get; }
        public string Time { set; get; }
        private string StartTag = "<event>";
        private string EndTag = "</event>";
        public Event()
        {
            atributes = new List<Models.Process.Attributes.Attribute>();
        }

        override
        public string ToString()
        {
            string result = StartTag;
            result += "<string key=\"concept:name\" value=\"" + Name + "\" />";
            result += "<date key=\"time:timestamp\" value=\"" + Time + "\" />";
            foreach (var atr in atributes)
                result += atr.ToString();
            return result + EndTag;
        }
    }
}
