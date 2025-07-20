namespace AIDispatcher.Notification;

/// <summary>
/// Menandakan bahwa handler memiliki prioritas.
/// Handler dengan nilai lebih rendah dijalankan lebih awal.
/// </summary>
public interface INotificationPriority
{
    int Priority { get; }
}