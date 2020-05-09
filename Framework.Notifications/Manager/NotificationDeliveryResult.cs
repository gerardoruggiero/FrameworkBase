using Framework.Notifications.Model;
using System.Collections.Generic;

namespace Framework.Notifications.Manager
{
    public class NotificationDeliveryResult<T>
        where T : Notification
    {
        public T Item { get; set; }

        public List<string> Errores { get; set; } = new List<string>();

        public NotificationDeliveryResultType ResultType { get; set; } = NotificationDeliveryResultType.OK; 
    }
}
