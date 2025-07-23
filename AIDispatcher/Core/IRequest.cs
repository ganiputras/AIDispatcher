namespace AIDispatcher.Core;

/// <summary>
/// Antarmuka penanda untuk objek permintaan (request) yang menghasilkan respons.
/// Digunakan pada pola request/response agar dispatcher dapat mengarahkan request ke handler yang tepat dan mengembalikan hasil.
/// </summary>
/// <typeparam name="TResponse">Tipe data yang akan dikembalikan sebagai respons dari handler.</typeparam>
public interface IRequest<TResponse>
{
}

/// <summary>
/// Antarmuka penanda untuk objek permintaan (request) yang tidak menghasilkan respons (void).
/// Cocok digunakan untuk perintah (command) yang hanya memicu aksi tanpa perlu return value.
/// </summary>
public interface IRequest
{
}