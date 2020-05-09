using Newtonsoft.Json;
using System;
using System.IO;

namespace Framework.ErrorLog
{
    public class LLog
    {
        private readonly Exception _exception;
        private static LLogConfiguration _logConfiguration;

        public static LLogConfiguration Configuration
        {
            get
            {
                if (_logConfiguration == null)
                {
                    _logConfiguration = new LLogConfiguration();
                }
                return _logConfiguration;
            }
        }

        public LLog()
        {
        }

        public LLog(Exception pException)
        {
            _exception = pException;
            AutoSave();
        }

        public LLog(Exception pException, string pOutputFile)
        {
            _exception = pException;
            FilePath = pOutputFile;
            AutoSave();
        }

        public string Application { get; set; }

        public string Description { get; set; }

        public string PilaDeLlamadas { get; set; }

        public string FilePath { get; set; }

        public DateTime Fecha { get; set; }

        public LLog InnerLog { get; set; }

        public string UsuarioAfectado { get; set; }

        public void Save()
        {
            if (_exception != null)
            {
                SaveToFile();
            }
        }

        public static LLog ReadLog(string pFilePath)
        {
            string log = File.ReadAllText(pFilePath);
            var llog = JsonConvert.DeserializeObject<LLog>(log);

            return llog;
        }

        private void Build(LLog pLlog, Exception pException)
        {
            pLlog.Description = pException.Message;
            pLlog.Application = pException.Source;
            pLlog.PilaDeLlamadas = pException.StackTrace;
            pLlog.Fecha = DateTime.Now;

            if (pException.InnerException != null)
            {
                pLlog.InnerLog = new LLog(pException.InnerException);
            }
        }

        private void AutoSave()
        {
            if (Configuration.AutoSave)
            {
                SaveToFile();
            }
        }  

        private void SaveToFile()
        {
            if (FilePath == null || FilePath == string.Empty)
            {
                FilePath = Path.Combine(Configuration.DefaultLogFilePath, string.Concat(DateTime.Now.ToString(Configuration.FileNameDateFormat), Configuration.FileExtension));
            }

            SaveToFile(FilePath);
        }

        private void SaveToFile(string pOutputFile)
        {
            Build(this, _exception);
            using (StreamWriter sw = new StreamWriter(pOutputFile, false))
            {
                var json = JsonConvert.SerializeObject(this);
                sw.Write(json);
            }
        }
    }
}
