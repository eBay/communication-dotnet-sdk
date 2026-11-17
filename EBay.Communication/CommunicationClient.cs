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
using EBay.Communication.Generated.Feedback;
using EBay.Communication.Generated.Message;
using EBay.Communication.Generated.Notification;
using Microsoft.Kiota.Abstractions.Authentication;
using Microsoft.Kiota.Http.HttpClientLibrary;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace EBay.Communication;

/// <summary>
/// Entry point for the Communication APIs. The client owns its HTTP connections:
/// create one per application (or register it with <c>AddCommunicationSdk</c>, which adds it
/// as a singleton) and dispose it on shutdown. Reach the individual APIs through its properties.
/// </summary>
public class CommunicationClient : IDisposable
{
    private readonly HttpClient[] _ownedClients;

    public FeedbackClient Feedback { get; private set; } = null!;
    public MessageClient Message { get; private set; } = null!;
    public NotificationClient Notification { get; private set; } = null!;

    /// <summary>Creates a client with its own HTTP connections.</summary>
    public CommunicationClient(CommunicationConfiguration config)
        : this(config, CreateDefaultPrimaryHandler, disposeHandlers: true)
    {
    }

    /// <summary>
    /// Creates a client that sends every request through <paramref name="primaryHandler"/>, for example to
    /// configure a proxy or client certificates, or to substitute a test double. The handler is shared by
    /// all APIs and is not disposed by this client.
    /// </summary>
    public CommunicationClient(CommunicationConfiguration config, HttpMessageHandler primaryHandler)
        : this(config, RequireHandler(primaryHandler), disposeHandlers: false)
    {
    }

    private CommunicationClient(
        CommunicationConfiguration config,
        Func<HttpMessageHandler> primaryHandler,
        bool disposeHandlers)
    {
        config.ValidateTrustedHosts();
        var tokenContext = new AsyncLocalTokenContext();
        var auth = new BaseBearerTokenAuthenticationProvider(new KiotaAuthProvider(config, tokenContext));
        var owned = new List<HttpClient>();
        HttpClient Build(DelegatingHandler logging, string baseUrl)
        {
            logging.InnerHandler = primaryHandler();
            var client = new HttpClient(logging, disposeHandlers)
            {
                BaseAddress = new Uri(baseUrl),
                Timeout = config.HttpTimeout
            };
            owned.Add(client);
            return client;
        }
        InitializeRestClients(auth, config, tokenContext, endpoint => Build(new DiagnosticLoggingHandler(config), endpoint.BaseUrl));
        _ownedClients = [.. owned];
    }

    private static HttpMessageHandler CreateDefaultPrimaryHandler() =>
#if NETFRAMEWORK
        // .NET Framework defaults to 2 connections per host outside ASP.NET.
        new HttpClientHandler { MaxConnectionsPerServer = 64 };
#else
        new SocketsHttpHandler { PooledConnectionLifetime = TimeSpan.FromMinutes(2) };
#endif

    private static Func<HttpMessageHandler> RequireHandler(HttpMessageHandler primaryHandler)
    {
#if NETFRAMEWORK
        if (primaryHandler is null)
        {
            throw new ArgumentNullException(nameof(primaryHandler));
        }
#else
        ArgumentNullException.ThrowIfNull(primaryHandler);
#endif
        return () => primaryHandler;
    }

    internal static IReadOnlyList<RestEndpoint> RestEndpoints(CommunicationConfiguration config) =>
    [
        new RestEndpoint("feedback", config.RestBaseUrl("feedback")),
        new RestEndpoint("message", config.RestBaseUrl("message")),
        new RestEndpoint("notification", config.RestBaseUrl("notification"))
    ];

    private void InitializeRestClients(
        IAuthenticationProvider auth,
        CommunicationConfiguration config,
        ITokenContext tokenContext,
        RestHttpClientProvider httpClientFor)
    {
        var endpoints = RestEndpoints(config);
        var feedbackAdapter = new HttpClientRequestAdapter(auth, httpClient: httpClientFor(endpoints.First(endpoint => endpoint.ApiId == "feedback")));
        Feedback = new FeedbackClient(new FeedbackApiClient(feedbackAdapter), config, tokenContext, feedbackAdapter);
        var messageAdapter = new HttpClientRequestAdapter(auth, httpClient: httpClientFor(endpoints.First(endpoint => endpoint.ApiId == "message")));
        Message = new MessageClient(new MessageApiClient(messageAdapter), config, tokenContext, messageAdapter);
        var notificationAdapter = new HttpClientRequestAdapter(auth, httpClient: httpClientFor(endpoints.First(endpoint => endpoint.ApiId == "notification")));
        Notification = new NotificationClient(new NotificationApiClient(notificationAdapter), config, tokenContext, notificationAdapter);
    }

    public void Dispose()
    {
        foreach (var client in _ownedClients)
        {
            client.Dispose();
        }
        GC.SuppressFinalize(this);
    }
}

internal sealed record RestEndpoint(string ApiId, string BaseUrl);

internal delegate HttpClient RestHttpClientProvider(RestEndpoint endpoint);
