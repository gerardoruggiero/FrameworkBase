using Framework.Security.Encryption;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Data.Entity;
using System.IO;

namespace EntityFramework.Context
{
    public abstract class DbContextBase : DbContext
    {
        public DbContextBase()
        {
            Database.Connection.ConnectionString = GetConnectionString();
        }

        public DbContextBase(string pConnectionString)
        {
            Database.Connection.ConnectionString = pConnectionString;
        }

        private string GetConnectionString()
        {
            JObject conexiones = null;

            string machine = Environment.MachineName;
            string folder = AppDomain.CurrentDomain.RelativeSearchPath;

            if (string.IsNullOrEmpty(folder))
            {
                folder = AppDomain.CurrentDomain.BaseDirectory;
            } 
            string archivo = Path.Combine(folder, "Config.connection");

            if (!File.Exists(archivo))
                throw new FileNotFoundException("El archivo de conexiones no existe");

            try
            {
                var conexionesEncriptadas = File.ReadAllText(archivo);
                var encriptado = DataEncryptor.Decrypt(conexionesEncriptadas);
                conexiones = (JObject)JsonConvert.DeserializeObject(encriptado);
            }
            catch (FormatException ex)
            {
                throw new FormatException("No se puede leer el archivo de conexiones", ex);
            }

            if (conexiones == null)
                throw new FormatException("El archivo de conexiones no tiene connection strings configurados");

            var conexion = (JValue)conexiones[machine];

            if (conexion == null)
                conexion = (JValue)conexiones["DEFAULT"];

            if (conexion == null)
                throw new Exception("El archivo está mal armado");

            return conexion.Value.ToString();
        }
    }
}
