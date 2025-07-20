namespace AIDispatcher.Dispatcher;

/// <summary>
///     Menandai sebuah class sebagai permintaan (request) yang akan menghasilkan respons bertipe <typeparamref name="TResponse" />.
///     Interface ini digunakan oleh sistem dispatcher untuk memetakan permintaan ke handler yang sesuai.
/// </summary>
/// <typeparam name="TResponse">Tipe respons yang diharapkan dari handler.</typeparam>
public interface IRequest<TResponse>
{
}