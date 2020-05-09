namespace Framework.Notifications.Model
{
    public class NotificationUser
    {
        public string Nombre { get; set; } = string.Empty;

        public string Apellido { get; set; } = string.Empty;

        public string Email { get; set; }


        public string NombreDisplay
        {
            get
            {
                if (string.IsNullOrEmpty(Nombre) || string.IsNullOrEmpty(Apellido))
                    return Email;
                else
                    return string.Format("{0} {1} <{2}>", Nombre, Apellido, Email);
            }
        }
    }
}
