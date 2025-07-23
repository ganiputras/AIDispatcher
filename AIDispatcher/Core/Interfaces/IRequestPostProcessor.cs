namespace AIDispatcher.Core.Interfaces;

/// <summary>
///     Menangani logika setelah handler selesai dijalankan.
/// </summary>
public interface IRequestPostProcessor<in TRequest, in TResponse>
{
    Task Process(TRequest request, TResponse response, CancellationToken cancellationToken);
}