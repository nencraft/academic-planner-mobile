using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcademicPlanner.Services;

public interface INotificationManagerService
{
    event EventHandler? NotificationReceived;

    void SendNotification(int id, string title, string message, DateTime? notifyTime = null);

    void CancelNotification(int id);

    void ReceiveNotification(string title, string message);
}