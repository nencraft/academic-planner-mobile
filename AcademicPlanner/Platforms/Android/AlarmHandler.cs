using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.Content;

namespace AcademicPlanner.Platforms.Android;

[BroadcastReceiver(Enabled = true, Label = "Local Notifications Broadcast Receiver")]
public class AlarmHandler : BroadcastReceiver
{
    public override void OnReceive(Context? context, Intent? intent)
    {
        if (intent?.Extras != null)
        {
            string? title = intent.GetStringExtra(NotificationManagerService.TitleKey);
            string? message = intent.GetStringExtra(NotificationManagerService.MessageKey);

            NotificationManagerService manager =
                NotificationManagerService.Instance ?? new NotificationManagerService();

            int notificationId = intent.GetIntExtra(NotificationManagerService.NotificationIdKey, 0);

            if (notificationId == 0)
            {
                notificationId = DateTime.Now.GetHashCode();
            }

            manager.Show(notificationId, title ?? "Reminder", message ?? string.Empty);
        }
    }
}
