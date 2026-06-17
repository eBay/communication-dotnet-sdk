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


using Communication.Generated.Feedback;
using Communication.Generated.Message;
using Communication.Generated.Negotiation;
using Communication.Generated.Notification;
using EBay.Communication.Clients.Rest;
using EBay.Communication.Configuration;
using Microsoft.Kiota.Abstractions.Authentication;
using Microsoft.Kiota.Http.HttpClientLibrary;

namespace EBay.Communication;

/// <summary>
/// Main entry point for the Communication SDK
/// Provides access to Feedback, Message, Notification, and Negotiation APIs
/// </summary>
public sealed class CommunicationClient : IDisposable
{
    private readonly HttpClient _httpClient;
    private readonly bool _ownsHttpClient;

    /// <summary>
    /// Feedback API client
    /// </summary>
    public FeedbackClient Feedback { get; }

    /// <summary>
    /// Message API client (M2M Public API Service)
    /// </summary>
    public MessageClient Message { get; }

    /// <summary>
    /// Notification API client
    /// </summary>
    public NotificationClient Notification { get; }

    /// <summary>
    /// Negotiation API client
    /// </summary>
    public NegotiationClient Negotiation { get; }

    /// <summary>
    /// DI-friendly constructor: ITokenContext injected by the container.
    /// </summary>
    internal CommunicationClient(
        CommunicationConfiguration config,
        HttpClient httpClient,
        ITokenContext tokenContext)
    {
        ArgumentNullException.ThrowIfNull(config);
        ArgumentNullException.ThrowIfNull(httpClient);
        ArgumentNullException.ThrowIfNull(tokenContext);

        _httpClient = httpClient;
        _ownsHttpClient = false;

        var authProvider = new KiotaAuthProvider(config, tokenContext);
        var authenticationProvider = new BaseBearerTokenAuthenticationProvider(authProvider);

        // All REST API adapters share the single DI-provided HttpClient
        Feedback = new FeedbackClient(
            new FeedbackApiClient(new HttpClientRequestAdapter(
                authenticationProvider,
                httpClient: httpClient)),
            config,
            tokenContext);

        Message = new MessageClient(
            new MessageApiClient(new HttpClientRequestAdapter(
                authenticationProvider,
                httpClient: httpClient)),
            config,
            tokenContext);

        Notification = new NotificationClient(
            new NotificationApiClient(new HttpClientRequestAdapter(
                authenticationProvider,
                httpClient: httpClient)),
            config,
            tokenContext);

        Negotiation = new NegotiationClient(
            new NegotiationApiClient(new HttpClientRequestAdapter(
                authenticationProvider,
                httpClient: httpClient)),
            config,
            tokenContext);

    }

    /// <summary>
    /// Standalone constructor for use outside DI (tests, scripts).
    /// Creates its own HttpClient with DiagnosticLoggingHandler wired in.
    /// </summary>
    public CommunicationClient(CommunicationConfiguration config)
        : this(config, BuildStandaloneHttpClient(config), new AsyncLocalTokenContext())
    {
        _ownsHttpClient = true;
    }

    private static HttpClient BuildStandaloneHttpClient(CommunicationConfiguration config)
    {
        var handler = new DiagnosticLoggingHandler(config)
        {
            InnerHandler = new SocketsHttpHandler { PooledConnectionLifetime = TimeSpan.FromMinutes(2) }
        };
        return new HttpClient(handler) { Timeout = config.HttpTimeout };
    }

    public void Dispose()
    {
        if (_ownsHttpClient)
            _httpClient?.Dispose();
    }
}
