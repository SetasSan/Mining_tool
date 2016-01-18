using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseAnalizer.Controllers.Servers
{
    public class Server
    {
        public string Name { private set; get; }
         public Server(string name)
         {
            Name = name;
         }  
    }
}
