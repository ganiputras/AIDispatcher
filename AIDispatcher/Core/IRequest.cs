namespace AIDispatcher.Core;

/// <summary>
///     Menandai sebuah objek sebagai permintaan yang menghasilkan respons.
/// </summary>
/// <typeparam name="TResponse">Tipe data yang akan dikembalikan sebagai respons.</typeparam>
public interface IRequest<TResponse>
{
}

/// <summary>
///     Menandai sebuah objek sebagai permintaan yang tidak menghasilkan respons.
///     Cocok digunakan untuk perintah (command).
/// </summary>
public interface IRequest
{
}