using DatabaseAnalizer.Controllers.Databases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseAnalizer.Controllers.Interfaces
{
    public interface IServer
    {        
        string GetConnectionString();
        string GetServerName();
        void Extract();

        void SetUserName(string userName);
        void SetUserPassword(string userPassword);
        void SetServerAddress(string serverAddress);
        List<Database> GetDatabases(); 
        
    }
}
