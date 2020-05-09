using System.Collections.Generic;
using System.Threading.Tasks;

namespace Framework.Notifications.Contract
{
    public interface INotificationManager<T>
    {
        Task<List<T>> GetPendingAsync();

        void Send(T pItem);

        Task PersistNotification(T pNotification);
    }
}
