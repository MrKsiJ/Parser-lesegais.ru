using System.IO;

namespace ParseSQLTool.Loader
{
    public class LoadSettings
    {
        private const string PATH_TO_FILE_SETTINGS = "Settings.ini";

        public string DataFile { get; private set; }

        public Settings Settings { get; private set; }

        public LoadSettings()
        {
            DataFile = File.ReadAllText(PATH_TO_FILE_SETTINGS);
            Settings = ParseFile();
        }

        private Settings ParseFile()
        {
            var split = DataFile.Split(';');
            string hostServer = "";
            string userName = "";
            string dataBaseName = "";
            string password = "";
            string dataBaseTable = "";
            int minutesTimeCheck = 0;
            for(var i = 0; i < split.Length; i++)
            {
                var getIndex = split[i].IndexOf('=') + 1;
                split[i] = split[i].Remove(0, getIndex);
                ParseDataValues(split, ref hostServer, ref userName, ref dataBaseName, ref password, ref dataBaseTable, ref minutesTimeCheck, i);
            }
            var result = new Settings(hostServer, userName, dataBaseName, password, dataBaseTable, minutesTimeCheck);
            return result;
        }

        private void ParseDataValues(string[] split, ref string hostServer, ref string userName, ref string dataBaseName, ref string password, ref string dataBaseTable, ref int minutesTimeCheck, int i)
        {
            switch (i)
            {
                case 0:
                    hostServer = split[i];
                    break;
                case 1:
                    userName = split[i];
                    break;
                case 2:
                    dataBaseName = split[i];
                    break;
                case 3:
                    dataBaseTable = split[i];
                    break;
                case 4:
                    password = split[i];
                    break;
                case 5:
                    minutesTimeCheck = int.Parse(split[i]);
                    break;
            }
        }
    }
}
