namespace AIDispatcher.Core.Interfaces;

/// <summary>
///     Menangani logika sebelum handler dijalankan.
/// </summary>
public interface IRequestPreProcessor<in TRequest>
{
    Task Process(TRequest request, CancellationToken cancellationToken);
}