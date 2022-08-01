using MySql.Data.MySqlClient;
using System;

namespace ParseSQLTool
{
    public class Connection
    {
        public string HostServer { get; private set; }
        public string UserName { get; private set; }
        public string DatabaseName { get; private set; }
        public string Password { get; private set; }

        public string ConnectionString { get; private set; }

        public MySqlConnection MySQLConnection { get; private set; }

        public Connection(string mySQLServer, string user, string database, string password)
        {
            HostServer = mySQLServer;
            UserName = user;
            DatabaseName = database;
            Password = password;

            ConnectionString = "server="+HostServer+";user="+UserName+";database="+DatabaseName+";password="+Password+";";
        }

        public bool Connect()
        {
            MySQLConnection = new MySqlConnection(ConnectionString);
            try
            {
                MySQLConnection.Open();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("[" + DateTime.Now.ToString() + "]" + " Соединение с базой установлено");
                return true;
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("[" + DateTime.Now.ToString() + "] " + ex.Message);
                Console.ReadLine();
                return false;
            }
        }
    }
}
