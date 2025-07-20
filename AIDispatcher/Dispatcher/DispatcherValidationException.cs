using FluentValidation.Results;

namespace AIDispatcher.Dispatcher;

/// <summary>
///     Exception standar untuk validasi yang digunakan dalam pipeline dispatcher.
///     Exception ini membungkus daftar kesalahan validasi dari FluentValidation,
///     dan menyederhanakan penyajian pesan error kepada konsumen (pengguna atau developer).
/// </summary>
public class DispatcherValidationException : Exception
{
    /// <summary>
    ///     Membuat instance baru dari <see cref="DispatcherValidationException"/>.
    /// </summary>
    /// <param name="failures">
    ///     Daftar hasil validasi yang gagal. Biasanya berasal dari FluentValidation.
    /// </param>
    public DispatcherValidationException(IEnumerable<ValidationFailure> failures)
        : base("One or more validation failures have occurred.")
    {
        Errors = failures?.ToList() ?? new List<ValidationFailure>();
    }

    /// <summary>
    ///     Daftar lengkap kesalahan validasi yang terjadi.
    /// </summary>
    public IReadOnlyList<ValidationFailure> Errors { get; }

    /// <summary>
    ///     Mengembalikan pesan error dalam format teks yang mudah dibaca.
    ///     Setiap kesalahan ditampilkan dalam format <c>- NamaProperti: PesanError</c>.
    /// </summary>
    public override string ToString()
    {
        var messages = Errors.Select(e => $"- {e.PropertyName}: {e.ErrorMessage}");
        return $"{Message}{Environment.NewLine}{string.Join(Environment.NewLine, messages)}";
    }
}