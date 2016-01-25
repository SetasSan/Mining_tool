using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseAnalizer.Models.Process
{
    public class Trace
    {
        private string startTag = "<trace>";
        private string endTag = "</trace>";
        public string traceName { set; get; }
        public List<Event> events { set; get; }

        public Trace()
        {
            events = new List<Event>();
        }

        override
        public string ToString()
        {
            string result = startTag;
            result += "<string key=\"concept:name\" value=\"" + traceName + "\" />";
            foreach (var even in events)
                result += even.ToString();

            result += endTag;
            return result;
        }
    }
}
