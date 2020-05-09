using System;
using System.Configuration;
using System.IO;

namespace Framework.ErrorLog
{
    public class LLogConfiguration
    {
        public string FileExtension { get; private set; }
        public string FileNameDateFormat { get; private set; }
        public string DefaultLogFilePath { get; private set; }
        public bool AutoSave { get; private set; } = true;

        public LLogConfiguration()
        {
            ReadConfigurations();
        }

        private void ReadConfigurations()
        {
            string configFile = Path.Combine(AppDomain.CurrentDomain.RelativeSearchPath, "LLogConfig.config");
            if (!File.Exists(configFile))
            {
                throw new Exception("No existe el archivo de configuración del logger");
            }

            ExeConfigurationFileMap customConfigFileMap = new ExeConfigurationFileMap
            {
                ExeConfigFilename = configFile
            };

            Configuration customConfig = ConfigurationManager.OpenMappedExeConfiguration(customConfigFileMap, ConfigurationUserLevel.None);
            AppSettingsSection appSettings = (customConfig.GetSection("appSettings") as AppSettingsSection);

            FileExtension = appSettings.Settings["FileExtension"] != null? appSettings.Settings["FileExtension"].Value : ".txt";
            DefaultLogFilePath = appSettings.Settings["DefaultLogPath"]!= null ? appSettings.Settings["DefaultLogPath"].Value : Path.Combine(AppDomain.CurrentDomain.RelativeSearchPath, "Logs");
            FileNameDateFormat = appSettings.Settings["FileNameDateFormat"] != null ? appSettings.Settings["FileNameDateFormat"].Value : "dd_MM_yyyy_HH_mm_ss";
            AutoSave = appSettings.Settings["AutoSaveLogs"] == null || appSettings.Settings["AutoSaveLogs"].Value == "true";

            if (!Directory.Exists(DefaultLogFilePath))
            {
                try
                {
                    Directory.CreateDirectory(DefaultLogFilePath);
                }
                catch { }
            }
        }
    }
}
