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

        services.AddHttpClient(CommunicationClient.FeedbackClientName,
                (sp, c) => c.BaseAddress = new Uri(sp.GetRequiredService<CommunicationConfiguration>().FeedbackBaseUrl))
            .ConfigureHttpClient((sp, c) => c.Timeout = sp.GetRequiredService<CommunicationConfiguration>().HttpTimeout)
            .AddHttpMessageHandler(sp => new DiagnosticLoggingHandler(sp.GetRequiredService<CommunicationConfiguration>()));
        services.AddHttpClient(CommunicationClient.MessageClientName,
                (sp, c) => c.BaseAddress = new Uri(sp.GetRequiredService<CommunicationConfiguration>().MessageBaseUrl))
            .ConfigureHttpClient((sp, c) => c.Timeout = sp.GetRequiredService<CommunicationConfiguration>().HttpTimeout)
            .AddHttpMessageHandler(sp => new DiagnosticLoggingHandler(sp.GetRequiredService<CommunicationConfiguration>()));
        services.AddHttpClient(CommunicationClient.NegotiationClientName,
                (sp, c) => c.BaseAddress = new Uri(sp.GetRequiredService<CommunicationConfiguration>().NegotiationBaseUrl))
            .ConfigureHttpClient((sp, c) => c.Timeout = sp.GetRequiredService<CommunicationConfiguration>().HttpTimeout)
            .AddHttpMessageHandler(sp => new DiagnosticLoggingHandler(sp.GetRequiredService<CommunicationConfiguration>()));
        services.AddHttpClient(CommunicationClient.NotificationClientName,
                (sp, c) => c.BaseAddress = new Uri(sp.GetRequiredService<CommunicationConfiguration>().NotificationBaseUrl))
            .ConfigureHttpClient((sp, c) => c.Timeout = sp.GetRequiredService<CommunicationConfiguration>().HttpTimeout)
            .AddHttpMessageHandler(sp => new DiagnosticLoggingHandler(sp.GetRequiredService<CommunicationConfiguration>()));

        services.AddScoped<CommunicationClient>(sp =>
        {
            var factory = sp.GetRequiredService<IHttpClientFactory>();
            return new CommunicationClient(
                sp.GetRequiredService<CommunicationConfiguration>(),
                factory.CreateClient(CommunicationClient.FeedbackClientName),
                factory.CreateClient(CommunicationClient.MessageClientName),
                factory.CreateClient(CommunicationClient.NegotiationClientName),
                factory.CreateClient(CommunicationClient.NotificationClientName),
                sp.GetRequiredService<ITokenContext>());
        });

        services.AddScoped(sp => sp.GetRequiredService<CommunicationClient>().Feedback);
        services.AddScoped(sp => sp.GetRequiredService<CommunicationClient>().Message);
        services.AddScoped(sp => sp.GetRequiredService<CommunicationClient>().Negotiation);
        services.AddScoped(sp => sp.GetRequiredService<CommunicationClient>().Notification);

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
