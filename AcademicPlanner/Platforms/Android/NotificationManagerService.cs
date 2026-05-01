using Android.App;
using Android.Content;
using Android.OS;
using AndroidX.Core.App;
using AcademicPlanner.Services;

namespace AcademicPlanner.Platforms.Android;

public class NotificationManagerService : INotificationManagerService
{
    private const string ChannelId = "default";
    private const string ChannelName = "Default";
    private const string ChannelDescription = "Academic Planner notifications";

    public const string TitleKey = "title";
    public const string MessageKey = "message";
    public const string NotificationIdKey = "notificationId";

    private bool _channelInitialized;
    private NotificationManagerCompat? _compatManager;

    public event EventHandler? NotificationReceived;

    public static NotificationManagerService? Instance { get; private set; }

    public NotificationManagerService()
    {
        Instance = this;
    }

    public void SendNotification(int id, string title, string message, DateTime? notifyTime = null)
    {
        EnsureInitialized();

        if (notifyTime is null)
        {
            Show(id, title, message);
            return;
        }

        Intent intent = new Intent(Platform.AppContext, typeof(AlarmHandler));
        intent.PutExtra(NotificationIdKey, id);
        intent.PutExtra(TitleKey, title);
        intent.PutExtra(MessageKey, message);
        intent.SetFlags(ActivityFlags.SingleTop | ActivityFlags.ClearTop);

        PendingIntentFlags pendingIntentFlags = Build.VERSION.SdkInt >= BuildVersionCodes.S
            ? PendingIntentFlags.CancelCurrent | PendingIntentFlags.Immutable
            : PendingIntentFlags.CancelCurrent;

        PendingIntent? pendingIntent = PendingIntent.GetBroadcast(
            Platform.AppContext,
            id,
            intent,
            pendingIntentFlags);

        AlarmManager? alarmManager =
            Platform.AppContext.GetSystemService(Context.AlarmService) as AlarmManager;

        if (alarmManager is null || pendingIntent is null)
        {
            return;
        }

        long triggerTime = GetNotifyTime(notifyTime.Value);
        alarmManager.Set(AlarmType.RtcWakeup, triggerTime, pendingIntent);
    }

    public void CancelNotification(int id)
    {
        EnsureInitialized();
        _compatManager.Cancel(id);

        Intent intent = new Intent(Platform.AppContext, typeof(AlarmHandler));

        PendingIntentFlags pendingIntentFlags = Build.VERSION.SdkInt >= BuildVersionCodes.S
            ? PendingIntentFlags.NoCreate | PendingIntentFlags.Immutable
            : PendingIntentFlags.NoCreate;

        PendingIntent? pendingIntent = PendingIntent.GetBroadcast(
            Platform.AppContext,
            id,
            intent,
            pendingIntentFlags);

        if (pendingIntent is null)
        {
            return;
        }

        AlarmManager? alarmManager =
            Platform.AppContext.GetSystemService(Context.AlarmService) as AlarmManager;

        alarmManager?.Cancel(pendingIntent);
        pendingIntent.Cancel();
    }

    public void ReceiveNotification(string title, string message)
    {
        NotificationReceived?.Invoke(this, EventArgs.Empty);
    }

    public void Show(int id, string title, string message)
    {
        EnsureInitialized();

        Intent intent = new Intent(Platform.AppContext, typeof(MainActivity));
        intent.PutExtra(NotificationIdKey, id);
        intent.PutExtra(TitleKey, title);
        intent.PutExtra(MessageKey, message);
        intent.SetFlags(ActivityFlags.SingleTop | ActivityFlags.ClearTop);

        PendingIntentFlags pendingIntentFlags = Build.VERSION.SdkInt >= BuildVersionCodes.S
            ? PendingIntentFlags.UpdateCurrent | PendingIntentFlags.Immutable
            : PendingIntentFlags.UpdateCurrent;

        PendingIntent? pendingIntent = PendingIntent.GetActivity(
            Platform.AppContext,
            id,
            intent,
            pendingIntentFlags);

        if (pendingIntent is null || _compatManager is null)
        {
            return;
        }

        NotificationCompat.Builder builder = new NotificationCompat.Builder(
                Platform.AppContext,
                ChannelId)
            .SetContentIntent(pendingIntent)
            .SetContentTitle(title)
            .SetContentText(message)
            .SetSmallIcon(Resource.Mipmap.appicon)
            .SetAutoCancel(true);

        _compatManager.Notify(id, builder.Build());
    }

    private void CreateNotificationChannel()
    {
        if (_channelInitialized)
        {
            return;
        }

        if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
        {
            NotificationChannel channel = new NotificationChannel(
                ChannelId,
                ChannelName,
                NotificationImportance.Default)
            {
                Description = ChannelDescription
            };

            NotificationManager? manager =
                Platform.AppContext.GetSystemService(Context.NotificationService) as NotificationManager;

            manager?.CreateNotificationChannel(channel);
        }

        _channelInitialized = true;
    }

    private static long GetNotifyTime(DateTime notifyTime)
    {
        DateTime utcTime = TimeZoneInfo.ConvertTimeToUtc(notifyTime);
        DateTime epochStart = new DateTime(1970, 1, 1);
        TimeSpan timeSinceEpoch = utcTime - epochStart;

        return (long)timeSinceEpoch.TotalMilliseconds;
    }

    private void EnsureInitialized()
    {
        if (!_channelInitialized)
        {
            CreateNotificationChannel();
        }

        _compatManager ??= NotificationManagerCompat.From(Platform.AppContext);
    }
}