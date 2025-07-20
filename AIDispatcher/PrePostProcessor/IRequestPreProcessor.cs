namespace AIDispatcher.PrePostProcessor
{
    /// <summary>
    /// Interface untuk logika pemrosesan sebelum handler utama mengeksekusi request (pre-processing).
    /// </summary>
    /// <typeparam name="TRequest">Tipe request yang diproses.</typeparam>
    public interface IRequestPreProcessor<in TRequest>
    {
        /// <summary>
        /// Menjalankan proses pre-processing sebelum handler utama mengeksekusi request.
        /// </summary>
        /// <param name="request">Instance request yang akan diproses.</param>
        /// <param name="cancellationToken">Token pembatalan operasi asynchronous.</param>
        /// <returns>Task yang merepresentasikan operasi asynchronous pre-processing.</returns>
        Task ProcessAsync(TRequest request, CancellationToken cancellationToken);
    }
}