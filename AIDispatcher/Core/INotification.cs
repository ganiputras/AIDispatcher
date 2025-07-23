namespace AIDispatcher.Core;

/// <summary>
/// Antarmuka penanda untuk objek notifikasi yang dapat dipublikasikan ke banyak handler.
/// Digunakan pada pola publish/subscribe agar suatu event atau pesan dapat diterima dan diproses oleh beberapa handler sekaligus.
/// </summary>
public interface INotification
{
}