namespace AIDispatcher.Core.Interfaces;

/// <summary>
/// Antarmuka penanda untuk handler yang memiliki prioritas eksekusi.
/// Semakin kecil nilai <see cref="Priority"/>, semakin awal handler dieksekusi dalam pipeline notifikasi.
/// </summary>
public interface IWithPriority
{
    /// <summary>
    /// Nilai prioritas handler.
    /// Semakin kecil nilainya, semakin awal handler dieksekusi dalam urutan pipeline.
    /// </summary>
    int Priority { get; }
}