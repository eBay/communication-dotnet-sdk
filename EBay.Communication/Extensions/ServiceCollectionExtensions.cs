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

using EBay.Communication;
using EBay.Communication.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EBay.Communication.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCommunicationSdk(
        this IServiceCollection services,
        CommunicationConfiguration config)
    {
        services.AddSingleton(config);
        services.AddSingleton<ITokenContext, AsyncLocalTokenContext>();

        foreach (var endpoint in CommunicationClient.RestEndpointNames)
        {
            var id = endpoint.ApiId;
            services.AddHttpClient(endpoint.HttpClientName, (sp, c) =>
                    c.BaseAddress = new Uri(sp.GetRequiredService<CommunicationConfiguration>().RestBaseUrl(id)))
                .ConfigureHttpClient((sp, c) =>
                    c.Timeout = sp.GetRequiredService<CommunicationConfiguration>().HttpTimeout)
                .AddHttpMessageHandler(sp =>
                    new DiagnosticLoggingHandler(sp.GetRequiredService<CommunicationConfiguration>()));
        }

        services.AddScoped<CommunicationClient>(sp =>
        {
            var factory = sp.GetRequiredService<IHttpClientFactory>();
            return new CommunicationClient(
                sp.GetRequiredService<CommunicationConfiguration>(),
                factory,
                sp.GetRequiredService<ITokenContext>());
        });

        services.AddScoped(sp => sp.GetRequiredService<CommunicationClient>().Feedback);
        services.AddScoped(sp => sp.GetRequiredService<CommunicationClient>().Message);
        services.AddScoped(sp => sp.GetRequiredService<CommunicationClient>().Negotiation);
        services.AddScoped(sp => sp.GetRequiredService<CommunicationClient>().Notification);
        return services;
    }
}
