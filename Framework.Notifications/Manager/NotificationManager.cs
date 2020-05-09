using Framework.Notifications.Contract;
using Framework.Notifications.Model;

using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.ComponentModel;
using System;

namespace Framework.Notifications.Manager
{
    public abstract class NotificationManager<T> : INotificationManager<T>
        where T : Notification
    {
        private readonly NotificationManagerConfiguration _configuration;

        public delegate void OnMailFinishedTaksEvent(NotificationDeliveryResult<T> pResult);
        public event OnMailFinishedTaksEvent OnMailFinishedTaks;

        public NotificationManager(NotificationManagerConfiguration pConfiguration)
        {
            _configuration = pConfiguration ?? throw new ArgumentException("La configuración no puede ser nula");
        }

        public virtual async Task PersistNotification(T pNotification)
        {
            await Task.Run(() => { });
        }

        public abstract Task<List<T>> GetPendingAsync();

        public void Send(T pItem)
        {
            if (IsValid(pItem))
            {
                MailMessage mail = new MailMessage
                {
                    Subject = pItem.TituoNotificacion,
                    Body = pItem.DetalleNotificacion,
                    From = new MailAddress(pItem.UsuarioEnvia.Email, pItem.UsuarioEnvia.NombreDisplay),
                    DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure, //Notificar cuando falla
                    IsBodyHtml = true
                };

                foreach (NotificationUser destinatario in pItem.Destinatarios)
                {
                    mail.To.Add(CreateMail(destinatario));
                }

                foreach (NotificationUser cc in pItem.CC)
                {
                    mail.CC.Add(CreateMail(cc));
                }

                foreach (NotificationUser cco in pItem.CCO)
                {
                    mail.Bcc.Add(CreateMail(cco));
                }

                foreach (NotificationAttachement adjunto in pItem.Adjuntos)
                {
                    mail.Attachments.Add(new Attachment(adjunto.Ruta));
                }

                switch (pItem.Prioridad)
                {
                    case ENotificationPriority.Alta:
                        mail.Priority = MailPriority.High;
                        break;
                    case ENotificationPriority.Baja:
                        mail.Priority = MailPriority.Low;
                        break;
                    case ENotificationPriority.Normal:
                        mail.Priority = MailPriority.Normal;
                        break;
                }

                SmtpClient smtp = new SmtpClient();
                smtp.SendCompleted += Smtp_SendCompleted;
                smtp.Host = _configuration.Servidor;
                smtp.EnableSsl = _configuration.UsaSSL;
                if (_configuration.UsaCredenciales)
                {
                    smtp.Credentials = new NetworkCredential(_configuration.MailCuentaSaliente, _configuration.PasswordCuentaMailSaliente);
                }
                smtp.Port = _configuration.Puerto;
                try
                {
                    smtp.Send(mail);
                }
                catch (Exception ex)
                {
                    var errores = new List<string>
                    {
                        ex.Message
                    };
                    OnMailFinishedTaks(ReturnResult(NotificationDeliveryResultType.SendError, errores, pItem));
                }
            }
        }

        private MailAddress CreateMail(NotificationUser pPersona)
        {
            return new MailAddress(pPersona.Email, pPersona.NombreDisplay);
        }

        public bool IsValid(T pItem)
        {
            try
            {
                List<string> errores = Framework.Utils.Utils.IsValid(_configuration);
                if (errores.Count > 0)
                {
                    OnMailFinishedTaks(ReturnResult(NotificationDeliveryResultType.ConfigurationValidationError, errores, pItem));
                    return false;
                }

                errores = Framework.Utils.Utils.IsValid(pItem);
                if (errores.Count > 0)
                {
                    OnMailFinishedTaks(ReturnResult(NotificationDeliveryResultType.MessageValidationErrors, errores, pItem));
                    return false;
                }

                return true;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        private void Smtp_SendCompleted(object sender, AsyncCompletedEventArgs e)
        {
            NotificationDeliveryResultType tipo = NotificationDeliveryResultType.OK;
            List<string> errores = new List<string>();

            if (e.Cancelled)
            {
                tipo = NotificationDeliveryResultType.Cancelado;
            }
            else if (e.Error != null)
            {
                tipo = NotificationDeliveryResultType.SendError;
                errores.Add(e.Error.Message);
            }

            OnMailFinishedTaks(ReturnResult(tipo, errores, sender as T));
        }

        private NotificationDeliveryResult<T> ReturnResult(NotificationDeliveryResultType pTipo, List<string> pErrores, T pItem)
        {
            return new NotificationDeliveryResult<T>()
            {
                Errores = pErrores,
                ResultType = pTipo,
                Item = pItem
            };
        }
    }
}