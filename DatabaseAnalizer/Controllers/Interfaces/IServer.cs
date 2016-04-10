using DatabaseAnalizer.Controllers.Databases;
using DatabaseAnalizer.Models;
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
        void ExtractTablesData(string tableName);

        void SetUserName(string userName);
        void SetUserPassword(string userPassword);
        void SetServerAddress(string serverAddress);
        List<Database> GetDatabases();
        Table LeftJoinTables(List<Table> tablesForAnalize, Table analized, string selectedDb);
    }
}
