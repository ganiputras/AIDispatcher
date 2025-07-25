﻿using AIDispatcher.Core.Commons;
using AIDispatcher.Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace AIDispatcher.Core;

/// <summary>
/// Dispatcher utama yang bertanggung jawab mengarahkan permintaan (request/command/query) dan notifikasi ke handler yang sesuai,
/// serta menjalankan seluruh pipeline behavior dan notification pipeline secara terurut sesuai konfigurasi.
/// </summary>
public class Dispatcher : IDispatcher
{
    private readonly DispatcherOptions _options;
    private readonly IServiceProvider _serviceProvider;

    /// <summary>
    /// Membuat instance baru dari <see cref="Dispatcher"/> dengan dependency injection.
    /// </summary>
    /// <param name="serviceProvider">Penyedia layanan untuk resolve handler dan pipeline behavior.</param>
    /// <param name="options">Opsi konfigurasi dispatcher. Jika null, akan digunakan nilai default.</param>
    public Dispatcher(IServiceProvider serviceProvider, DispatcherOptions? options = null)
    {
        _serviceProvider = serviceProvider;
        _options = options ?? new DispatcherOptions();
    }

    /// <summary>
    /// Mengirim permintaan (request/command/query) dengan response ke handler yang sesuai,
    /// beserta seluruh pipeline behavior yang telah diregistrasi.
    /// </summary>
    /// <typeparam name="TRequest">Tipe permintaan.</typeparam>
    /// <typeparam name="TResponse">Tipe hasil yang diharapkan dari handler.</typeparam>
    /// <param name="request">Objek permintaan yang akan diproses.</param>
    /// <param name="cancellationToken">Token untuk membatalkan operasi asynchronous.</param>
    /// <returns>Hasil dari handler utama setelah seluruh pipeline dijalankan.</returns>
    public async Task<TResponse> Send<TRequest, TResponse>(
        TRequest request,
        CancellationToken cancellationToken = default)
        where TRequest : IRequest<TResponse>
    {
        var handler = _serviceProvider.GetRequiredService<IRequestHandler<TRequest, TResponse>>();
        var behaviors = _serviceProvider.GetServices<IPipelineBehavior<TRequest, TResponse>>()
            .Reverse()
            .ToList();

        RequestHandlerDelegate<TResponse> handlerDelegate = () => handler.Handle(request, cancellationToken);

        foreach (var behavior in behaviors)
        {
            var next = handlerDelegate;
            handlerDelegate = () => behavior.Handle(request, next, cancellationToken);
        }

        return await handlerDelegate();
    }

    /// <summary>
    /// Mengirim permintaan tanpa hasil (void command) ke handler yang sesuai,
    /// beserta seluruh pipeline behavior yang telah diregistrasi.
    /// </summary>
    /// <typeparam name="TRequest">Tipe permintaan.</typeparam>
    /// <param name="request">Objek permintaan yang akan diproses.</param>
    /// <param name="cancellationToken">Token untuk membatalkan operasi asynchronous.</param>
    /// <returns>Task asynchronous yang merepresentasikan proses pengiriman permintaan.</returns>
    public async Task Send<TRequest>(TRequest request, CancellationToken cancellationToken = default)
        where TRequest : IRequest
    {
        var handler = _serviceProvider.GetRequiredService<IRequestHandler<TRequest>>();
        var behaviors = _serviceProvider.GetServices<IPipelineBehavior<TRequest>>()
            .Reverse()
            .ToList();

        RequestHandlerDelegate handlerDelegate = () => handler.Handle(request, cancellationToken);

        foreach (var behavior in behaviors)
        {
            var next = handlerDelegate;
            handlerDelegate = () => behavior.Handle(request, next, cancellationToken);
        }

        await handlerDelegate();
    }

    /// <summary>
    /// Mempublikasikan notifikasi ke semua handler yang terdaftar,
    /// beserta seluruh notification pipeline behavior, dengan strategi eksekusi sesuai konfigurasi (sequential atau parallel).
    /// Handler dengan prioritas lebih rendah akan dieksekusi lebih awal.
    /// </summary>
    /// <typeparam name="TNotification">Tipe notifikasi.</typeparam>
    /// <param name="notification">Objek notifikasi yang akan diproses.</param>
    /// <param name="cancellationToken">Token untuk membatalkan operasi asynchronous.</param>
    /// <returns>Task asynchronous yang merepresentasikan proses publish notifikasi ke seluruh handler.</returns>
    public async Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default)
        where TNotification : INotification
    {
        var handlers = _serviceProvider.GetServices<INotificationHandler<TNotification>>();
        var pipelines = _serviceProvider.GetServices<INotificationPipelineBehavior<TNotification>>()
            .Reverse()
            .ToList();

        var orderedHandlers = handlers
            .OrderBy(h =>
            {
                var attr = h.GetType().GetCustomAttributes(typeof(WithPriorityAttribute), true)
                    .FirstOrDefault() as WithPriorityAttribute;
                return attr?.Priority ?? 0;
            })
            .ToList();

        if (_options.PublishStrategy == PublishStrategy.Parallel)
        {
            var tasks = orderedHandlers.Select(handler =>
            {
                NotificationHandlerDelegate handlerDelegate = () => handler.Handle(notification, cancellationToken);

                foreach (var pipeline in pipelines)
                {
                    var next = handlerDelegate;
                    handlerDelegate = () => pipeline.Handle(notification, next, cancellationToken);
                }

                return handlerDelegate();
            });

            await Task.WhenAll(tasks);
        }
        else // Sequential
        {
            foreach (var handler in orderedHandlers)
            {
                NotificationHandlerDelegate handlerDelegate = () => handler.Handle(notification, cancellationToken);

                foreach (var pipeline in pipelines)
                {
                    var next = handlerDelegate;
                    handlerDelegate = () => pipeline.Handle(notification, next, cancellationToken);
                }

                await handlerDelegate();
            }
        }
    }
}
