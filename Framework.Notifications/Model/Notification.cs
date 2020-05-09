using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net.Mail;

namespace Framework.Notifications.Model
{
    public class Notification
    {
        public DateTime FechaYHoraNotificacion { get; set; }

        [Required]
        public string TituoNotificacion { get; set; }

        [Required]
        public string DetalleNotificacion { get; set; }

        [Required]
        public NotificationUser UsuarioEnvia { get; set; }

        public List<NotificationUser> Destinatarios { get; private set; } = new List<NotificationUser>();

        public List<NotificationUser> CC { get; private set; } = new List<NotificationUser>();

        public List<NotificationUser> CCO { get; private set; } = new List<NotificationUser>();

        public List<NotificationAttachement> Adjuntos { get; private set; } = new List<NotificationAttachement>();

        public ENotificationPriority Prioridad { get; set; } = ENotificationPriority.Normal;

        public ENotificationType TipoNotificacion { get; set; } = ENotificationType.General;


        public void AgregarDestinatario(NotificationUser pUsuario)
        {
            if (IsEmailValid(pUsuario.Email))
                Destinatarios.Add(pUsuario);
        }

        public void AgregarDestinatario(List<NotificationUser> pUsuarios)
        {
            foreach (NotificationUser person in pUsuarios)
            {
                AgregarDestinatario(person);
            }
        }

        public void AgregarCC(NotificationUser pUsuario)
        {
            if (IsEmailValid(pUsuario.Email))
                CC.Add(pUsuario);
        }

        public void AgregarCC(List<NotificationUser> pUsuarios)
        {
            foreach (NotificationUser usuario in pUsuarios)
            {
                AgregarCC(pUsuarios);
            }
        }

        public void AgregarCCO(NotificationUser pUsuario)
        {
            CCO.Add(pUsuario);
        }

        public void AgregarCCO(List<NotificationUser> pUsuarios)
        {
            foreach(NotificationUser usuario in pUsuarios)
            {
                AgregarCCO(usuario);
            }
        }

        public void AgregarAdjunto(string pPath)
        {
            Adjuntos.Add(new NotificationAttachement(pPath));
        }


        private bool IsEmailValid(string pEmail)
        {
            try
            {
                MailAddress m = new MailAddress(pEmail);
                return true;
            }
            catch (FormatException)
            {
                throw new FormatException(string.Format("El mail {0} no es válido", pEmail));
            }
        }
    }
}
