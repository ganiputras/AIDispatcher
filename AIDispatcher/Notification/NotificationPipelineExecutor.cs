namespace AIDispatcher.Notification;

internal static class NotificationPipelineExecutor
{
    public static async Task ExecuteAsync<TNotification>(
        TNotification notification,
        IEnumerable<INotificationBehavior<TNotification>> behaviors,
        Func<Task> finalHandler,
        CancellationToken cancellationToken)
        where TNotification : notnull
    {
        var enumerator = behaviors.Reverse().GetEnumerator();

        Task Handler()
        {
            if (!enumerator.MoveNext())
                return finalHandler();

            return enumerator.Current.HandleAsync(notification, cancellationToken, Handler);
        }

        await Handler();
    }
}