namespace AIDispatcher.PrePostProcessor
{
    /// <summary>
    /// Interface untuk logika pemrosesan setelah handler utama selesai dieksekusi (post-processing).
    /// </summary>
    /// <typeparam name="TRequest">Tipe request yang diproses.</typeparam>
    /// <typeparam name="TResponse">Tipe response yang dihasilkan handler utama.</typeparam>
    public interface IRequestPostProcessor<in TRequest, in TResponse>
    {
        /// <summary>
        /// Menjalankan proses post-processing setelah handler utama mengeksekusi request.
        /// </summary>
        /// <param name="request">Instance request yang telah diproses.</param>
        /// <param name="response">Response yang dihasilkan oleh handler utama.</param>
        /// <param name="cancellationToken">Token pembatalan operasi asynchronous.</param>
        /// <returns>Task yang merepresentasikan operasi asynchronous post-processing.</returns>
        Task ProcessAsync(TRequest request, TResponse response, CancellationToken cancellationToken);
    }
}