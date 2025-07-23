using AIDispatcher.Core.Interfaces;

namespace AIDispatcher.Core.Behaviors;

public class PrePostProcessorBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IRequestPostProcessor<TRequest, TResponse>> _postProcessors;
    private readonly IEnumerable<IRequestPreProcessor<TRequest>> _preProcessors;

    public PrePostProcessorBehavior(
        IEnumerable<IRequestPreProcessor<TRequest>> preProcessors,
        IEnumerable<IRequestPostProcessor<TRequest, TResponse>> postProcessors)
    {
        _preProcessors = preProcessors;
        _postProcessors = postProcessors;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        foreach (var processor in _preProcessors)
            await processor.Process(request, cancellationToken);

        var response = await next();

        foreach (var processor in _postProcessors)
            await processor.Process(request, response, cancellationToken);

        return response;
    }
}

//✅ Contoh PreProcessor Validasi
//public class ValidateMyRequest : IRequestPreProcessor<MyRequest>
//{
//    public Task Process(MyRequest request, CancellationToken cancellationToken)
//    {
//        if (string.IsNullOrWhiteSpace(request.Name))
//            throw new ValidationException("Nama tidak boleh kosong");

//        return Task.CompletedTask;
//    }
//}