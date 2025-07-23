namespace AIDispatcher.Core.Commons;

/// <summary>
/// Menentukan prioritas eksekusi handler notifikasi.
/// Semakin rendah nilainya, semakin tinggi prioritas eksekusi handler dalam pipeline.
/// Dapat diaplikasikan pada class atau interface handler.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
public class WithPriorityAttribute : Attribute
{
    /// <summary>
    /// Membuat atribut prioritas dengan nilai tertentu.
    /// </summary>
    /// <param name="priority">
    /// Nilai prioritas handler. Nilai lebih rendah berarti handler akan diproses lebih awal.
    /// Nilai default adalah 0 (prioritas tertinggi).
    /// </param>
    public WithPriorityAttribute(int priority = 0)
    {
        Priority = priority;
    }

    /// <summary>
    /// Nilai prioritas handler.
    /// Semakin rendah nilai ini, semakin tinggi prioritas eksekusi dalam pipeline.
    /// </summary>
    public int Priority { get; }
}