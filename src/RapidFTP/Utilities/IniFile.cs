namespace RapidFTP.Utilities
{
    using System.Runtime.InteropServices;
    using System.Text;

    public class IniFile
    {
        private readonly string path;

        public IniFile(string iniPath)
        {
            this.path = iniPath;
        }

        public static IniFile ReadFile(string fileName)
        {
            return new IniFile(fileName);
        }

        public void Write(string key, string value, string section = "")
        {
            WritePrivateProfileString(section, key, value, this.path);
        }

        public string Read(string key, string section = "")
        {
            var temp = new StringBuilder(255);
            int i = GetPrivateProfileString(section, key, string.Empty, temp, 255, this.path);
            return temp.ToString();
        }

        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);

        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(
            string section, 
            string key, 
            string def, 
            StringBuilder retVal, 
            int size, 
            string filePath);
    }
}