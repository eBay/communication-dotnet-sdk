// Copyright 2026 eBay Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using EBay.Communication.Clients.Rest;
using EBay.Communication.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EBay.Communication.Extensions;

/// <summary>
/// Extension methods for registering Communication SDK with dependency injection
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers the Communication SDK with the service collection
    /// </summary>
    public static IServiceCollection AddCommunicationSdk(
        this IServiceCollection services,
        CommunicationConfiguration config)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(config);

        services.AddSingleton(config);
        services.AddSingleton<ITokenContext, AsyncLocalTokenContext>();

        services.AddHttpClient(nameof(CommunicationClient))
            .ConfigureHttpClient(c => c.Timeout = config.HttpTimeout)
            .AddHttpMessageHandler(sp => new DiagnosticLoggingHandler(sp.GetRequiredService<CommunicationConfiguration>()));
        services.AddScoped<CommunicationClient>(sp =>
            new CommunicationClient(
                sp.GetRequiredService<CommunicationConfiguration>(),
                sp.GetRequiredService<IHttpClientFactory>().CreateClient(nameof(CommunicationClient)),
                sp.GetRequiredService<ITokenContext>()));

        services.AddScoped(sp => sp.GetRequiredService<CommunicationClient>().Feedback);
        services.AddScoped(sp => sp.GetRequiredService<CommunicationClient>().Message);
        services.AddScoped(sp => sp.GetRequiredService<CommunicationClient>().Notification);
        services.AddScoped(sp => sp.GetRequiredService<CommunicationClient>().Negotiation);

        return services;
    }

    /// <summary>
    /// Registers the Communication SDK with a configuration builder
    /// </summary>
    public static IServiceCollection AddCommunicationSdk(
        this IServiceCollection services,
        Action<CommunicationConfiguration> configBuilder)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configBuilder);

        var config = new CommunicationConfiguration();
        configBuilder(config);

        return services.AddCommunicationSdk(config);
    }
}
