namespace Framework.Notifications.Model
{
    public class NotificationAttachement
    {
        string _path = string.Empty;

        public NotificationAttachement(string pPath)
        {
            _path = pPath;
        }

        public string Ruta { get; set; }
    }
}
