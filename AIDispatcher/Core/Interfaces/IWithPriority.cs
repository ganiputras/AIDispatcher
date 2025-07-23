namespace AIDispatcher.Core.Interfaces;

/// <summary>
///     Menandakan handler memiliki prioritas untuk dieksekusi terlebih dahulu.
///     Semakin kecil nilai Prioritas, semakin awal dieksekusi.
/// </summary>
public interface IWithPriority
{
    /// <summary>
    ///     Nilai prioritas (semakin kecil semakin awal dieksekusi).
    /// </summary>
    int Priority { get; }
}