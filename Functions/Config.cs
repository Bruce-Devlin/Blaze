using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace Blaze.Functions
{
    class Config
    {
        static string configFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Blaze\\blaze.config");
        public static void storeVariable(string name, string data)
        {
            ExeConfigurationFileMap fileMap = new ExeConfigurationFileMap();
            fileMap.ExeConfigFilename = configFile;
            Configuration configuration = ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);
            KeyValueConfigurationCollection settings = configuration.AppSettings.Settings;
            if (settings[name] == null) settings.Add(name, data);
            else settings[name].Value = data;
            configuration.Save(ConfigurationSaveMode.Modified);
        }

        public static string getVariable(string variable)
        {
            ExeConfigurationFileMap fileMap = new ExeConfigurationFileMap();
            fileMap.ExeConfigFilename = configFile;
            Configuration configuration = ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);
            KeyValueConfigurationCollection settings = configuration.AppSettings.Settings;
            if (settings[variable] == null) return "";
            else return settings[variable].Value;
        }
    }
}
