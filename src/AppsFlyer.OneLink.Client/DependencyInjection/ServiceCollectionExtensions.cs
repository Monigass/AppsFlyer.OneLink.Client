// <copyright file="OneLinkClient.cs">
// Copyright (c) Monigass.
// Licensed under the MIT License.
// </copyright>

namespace AppsFlyer.OneLink.Client.DependencyInjection;

using System;
using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;
using AppsFlyer.OneLink.Client;

/// <summary>
/// Extension methods for registering the AppsFlyer OneLink client.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers a typed AppsFlyer client and allows access to the service provider when configuring the underlying HTTP client.
    /// </summary>
    /// <typeparam name="TClient">The client contract type.</typeparam>
    /// <typeparam name="TImplementation">The client implementation type.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <param name="configureClient">Callback used to configure the <see cref="HttpClient"/>.</param>
    /// <returns>The same service collection.</returns>
    public static IServiceCollection AddAppsFlyerClient<TClient, TImplementation>(
        this IServiceCollection services,
        Action<IServiceProvider, HttpClient> configureClient)
        where TClient : class
        where TImplementation : class, TClient
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configureClient);

        services
            .AddHttpClient<TClient, TImplementation>((provider, client) => configureClient(provider, client))
            .AddPolicyHandler(HttpPolicyExtensions
                .HandleTransientHttpError()
                .WaitAndRetryAsync(3, attempt => TimeSpan.FromMilliseconds(100 * Math.Pow(2, attempt))));

        return services;
    }

    /// <summary>
    /// Registers the AppsFlyer OneLink client as a typed HTTP client.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configure">Optional configuration for the client options.</param>
    /// <returns>The same service collection.</returns>
    public static IServiceCollection AddAppsFlyerOneLinkClient(
        this IServiceCollection services,
        Action<AppsFlyerOneLinkClientOptions> configure = null)
    {
        ArgumentNullException.ThrowIfNull(services);

        var options = new AppsFlyerOneLinkClientOptions();
        configure?.Invoke(options);

        services.AddAppsFlyerClient<IAppsFlyerClient, AppsFlyerClient>((_, client) =>
        {
            if (options.BaseAddress is not null)
            {
                client.BaseAddress = options.BaseAddress;
            }
        });

        return services;
    }
}
