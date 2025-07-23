namespace AIDispatcher.Core.Commons;

/// <summary>
///     Menentukan prioritas eksekusi handler notifikasi.
///     Semakin rendah nilainya, semakin tinggi prioritas.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
public class WithPriorityAttribute : Attribute
{
    /// <summary>
    ///     Membuat atribut prioritas dengan nilai tertentu.
    /// </summary>
    /// <param name="priority">Nilai prioritas (default: 0).</param>
    public WithPriorityAttribute(int priority = 0)
    {
        Priority = priority;
    }

    /// <summary>
    ///     Nilai prioritas. Lebih rendah = lebih cepat diproses.
    /// </summary>
    public int Priority { get; }
}