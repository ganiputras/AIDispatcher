using AIDispatcher.Dispatcher;
using Microsoft.Extensions.DependencyInjection;

namespace AIDispatcher.PrePostProcessor
{
    /// <summary>
    /// Behavior yang mengeksekusi pre-processor dan post-processor yang terdaftar 
    /// di sekitar eksekusi handler utama sebuah request.
    /// </summary>
    /// <typeparam name="TRequest">Tipe request yang diproses.</typeparam>
    /// <typeparam name="TResponse">Tipe response yang dihasilkan handler utama.</typeparam>
    public class PrePostProcessorBehavior<TRequest, TResponse> : IDispatcherBehavior<TRequest, TResponse>
    {
        private readonly IEnumerable<IRequestPostProcessor<TRequest, TResponse>> _postProcessors;
        private readonly IEnumerable<IRequestPreProcessor<TRequest>> _preProcessors;

        /// <summary>
        /// Membuat instance PrePostProcessorBehavior dengan mengambil semua pre- dan post-processor dari service provider.
        /// </summary>
        /// <param name="serviceProvider">Service provider untuk resolve semua pre- dan post-processor yang terdaftar.</param>
        public PrePostProcessorBehavior(IServiceProvider serviceProvider)
        {
            _preProcessors = serviceProvider.GetServices<IRequestPreProcessor<TRequest>>();
            _postProcessors = serviceProvider.GetServices<IRequestPostProcessor<TRequest, TResponse>>();
        }

        /// <summary>
        /// Menjalankan semua pre-processor, kemudian handler utama, dan akhirnya semua post-processor secara asynchronous.
        /// </summary>
        /// <param name="request">Instance request yang diproses.</param>
        /// <param name="next">Delegate untuk mengeksekusi handler utama berikutnya di pipeline.</param>
        /// <param name="cancellationToken">Token pembatalan operasi asynchronous.</param>
        /// <returns>Task yang menghasilkan response dari handler utama setelah semua pre- dan post-processor selesai dijalankan.</returns>
        public async Task<TResponse> HandleAsync(TRequest request,
            DispatcherHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            foreach (var pre in _preProcessors)
                await pre.ProcessAsync(request, cancellationToken);

            var response = await next(cancellationToken);

            foreach (var post in _postProcessors)
                await post.ProcessAsync(request, response, cancellationToken);

            return response;
        }
    }
}
