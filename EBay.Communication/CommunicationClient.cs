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
using EBay.Communication.Generated.Negotiation;
using EBay.Communication.Generated.Notification;
using Microsoft.Kiota.Abstractions.Authentication;
using Microsoft.Kiota.Http.HttpClientLibrary;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace EBay.Communication;

public class CommunicationClient : IDisposable
{
    internal const string FeedbackClientName = "CommunicationClient.Feedback";
    internal const string MessageClientName = "CommunicationClient.Message";
    internal const string NegotiationClientName = "CommunicationClient.Negotiation";
    internal const string NotificationClientName = "CommunicationClient.Notification";

    private readonly HttpClient[] _ownedClients;

    public FeedbackClient Feedback { get; private set; } = null!;
    public MessageClient Message { get; private set; } = null!;
    public NegotiationClient Negotiation { get; private set; } = null!;
    public NotificationClient Notification { get; private set; } = null!;

    /// <summary>
    /// DI-friendly constructor. Named HttpClients come from <see cref="IHttpClientFactory"/>.
    /// </summary>
    internal CommunicationClient(
        CommunicationConfiguration config,
        IHttpClientFactory factory,
        ITokenContext tokenContext)
    {
        _ownedClients = [];
        var auth = new BaseBearerTokenAuthenticationProvider(new KiotaAuthProvider(config, tokenContext));
        InitializeRestClients(auth, config, tokenContext, endpoint => factory.CreateClient(endpoint.HttpClientName));
    }

    /// <summary>
    /// Standalone constructor for use outside DI (tests, scripts).
    /// Creates its own HttpClients with DiagnosticLoggingHandler.
    /// </summary>
    public CommunicationClient(CommunicationConfiguration config)
    {
        var tokenContext = new AsyncLocalTokenContext();
        var auth = new BaseBearerTokenAuthenticationProvider(new KiotaAuthProvider(config, tokenContext));
        var owned = new List<HttpClient>();
        InitializeRestClients(auth, config, tokenContext, endpoint =>
        {
            var client = BuildStandaloneHttpClient(config, endpoint.BaseUrl);
            owned.Add(client);
            return client;
        });
        _ownedClients = [.. owned];
    }

    private static HttpClient BuildStandaloneHttpClient(CommunicationConfiguration config, string baseUrl)
    {
        var handler = new DiagnosticLoggingHandler(config)
        {
            InnerHandler = new SocketsHttpHandler { PooledConnectionLifetime = TimeSpan.FromMinutes(2) }
        };
        return new HttpClient(handler) { BaseAddress = new Uri(baseUrl), Timeout = config.HttpTimeout };
    }

    internal static IReadOnlyList<(string ApiId, string HttpClientName)> RestEndpointNames { get; } =
    [
        ("feedback", FeedbackClientName),
        ("message", MessageClientName),
        ("negotiation", NegotiationClientName),
        ("notification", NotificationClientName)
    ];

    internal static IReadOnlyList<RestEndpoint> RestEndpoints(CommunicationConfiguration config) =>
        [.. RestEndpointNames.Select(name => new RestEndpoint(name.ApiId, name.HttpClientName, config.RestBaseUrl(name.ApiId)))];

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
        var negotiationAdapter = new HttpClientRequestAdapter(auth, httpClient: httpClientFor(endpoints.First(endpoint => endpoint.ApiId == "negotiation")));
        Negotiation = new NegotiationClient(new NegotiationApiClient(negotiationAdapter), config, tokenContext, negotiationAdapter);
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

internal sealed record RestEndpoint(string ApiId, string HttpClientName, string BaseUrl);

internal delegate HttpClient RestHttpClientProvider(RestEndpoint endpoint);
