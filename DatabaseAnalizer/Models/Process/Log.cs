using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseAnalizer.Models.Process
{
    public class Log
    {
        private const string _version="2.0";
        private const string xesFeatures = "nested-attributes";
        private string startTag = "<log>";
        private string endTag = "</log>";
        public List<Trace> traces { set; get; }

        public Log()
        {
            traces = new List<Trace>();
        }

        override
        public string ToString()
        {
            string trac = "";
            foreach (Trace trace in traces)
            {
                trac += trace.ToString();
            }
            return "<?xml version=\"1.0\" encoding=\"UTF-8\" ?><log xes.version=\"" + _version + "\" xes.features=\"" + xesFeatures + "\">"+trac+"</log>";
        }
    }
}
