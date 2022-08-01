namespace ParseSQLTool.Loader
{
    [System.Serializable]
    public class Settings
    {
        public string HostServer;
        public string UserName;
        public string DataBaseName;
        public string DatabaseTable;
        public string Password;
        public int MinutesTimeCheck;

        public Settings(string hostServer, string userName, string dataBaseName, string password, string databaseTable, int minutesTimeCheck)
        {
            HostServer = hostServer;
            UserName = userName;
            DataBaseName = dataBaseName;
            Password = password;
            DatabaseTable = databaseTable;
            MinutesTimeCheck = minutesTimeCheck;
        }
    }
}
