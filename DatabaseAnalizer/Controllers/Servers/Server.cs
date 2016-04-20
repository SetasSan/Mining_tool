using DatabaseAnalizer.Controllers.Databases;
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
        public List<Database> _databases;
        public string _serverAddress;
        public string _userName;
        public string _userPassword;
         public Server(string name)
         {
            Name = name;
         }

         public string GetServerName()
         {
             return Name;
         }

         public void SetUserName(string userName)
         {
             _userName = userName;
         }

         public void SetUserPassword(string userPassword)
         {
             _userPassword = userPassword;
         }


         public void SetServerAddress(string serverAddress)
         {
             _serverAddress = serverAddress;
         }

         public List<Databases.Database> GetDatabases()
         {
             return _databases;
         }
    }
}
