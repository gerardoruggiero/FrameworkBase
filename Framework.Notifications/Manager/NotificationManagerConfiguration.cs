using System.ComponentModel.DataAnnotations;

namespace Framework.Notifications.Manager
{
    public class NotificationManagerConfiguration
    {
        [Required]
        public string Servidor { get; set; }

        public bool UsaSSL { get; set; } = false;

        public string MailCuentaSaliente { get; set; }

        public string PasswordCuentaMailSaliente { get; set; }

        [Range(0, 65535)]
        public int Puerto { get; set; } = 25;


        public bool UsaCredenciales
        {
            get
            {
                return (string.IsNullOrEmpty(MailCuentaSaliente) == false 
                    && string.IsNullOrEmpty(PasswordCuentaMailSaliente) == false);
            }
        }
    }
}
