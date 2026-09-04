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


using EBay.Communication.Configuration;
using Microsoft.Kiota.Abstractions.Authentication;

namespace EBay.Communication;

/// <summary>
/// Main entry point for the Communication SDK
/// Provides access to Feedback, Message, Notification, and Negotiation APIs
/// </summary>
public partial class CommunicationClient : IDisposable
{
    internal const string FeedbackClientName = "CommunicationClient.Feedback";
    internal const string MessageClientName = "CommunicationClient.Message";
    internal const string NegotiationClientName = "CommunicationClient.Negotiation";
    internal const string NotificationClientName = "CommunicationClient.Notification";

    private readonly HttpClient[] _ownedClients;

    /// <summary>
    /// DI-friendly constructor: each HttpClient is a named client from IHttpClientFactory
    /// with DiagnosticLoggingHandler and BaseAddress already configured.
    /// </summary>
    internal CommunicationClient(
        CommunicationConfiguration config,
        HttpClient feedbackClient,
        HttpClient messageClient,
        HttpClient negotiationClient,
        HttpClient notificationClient,
        ITokenContext tokenContext)
    {
        _ownedClients = [];

        var auth = new BaseBearerTokenAuthenticationProvider(new KiotaAuthProvider(config, tokenContext));

        InitializeRestClients(auth, feedbackClient, messageClient, negotiationClient, notificationClient, config, tokenContext);
    }

    /// <summary>
    /// Standalone constructor for use outside DI (tests, scripts).
    /// Creates its own HttpClients with DiagnosticLoggingHandler.
    /// </summary>
    public CommunicationClient(CommunicationConfiguration config)
    {
        var tokenContext = new AsyncLocalTokenContext();
        var auth = new BaseBearerTokenAuthenticationProvider(new KiotaAuthProvider(config, tokenContext));

        var feedbackClient = BuildStandaloneHttpClient(config, config.FeedbackBaseUrl);
        var messageClient = BuildStandaloneHttpClient(config, config.MessageBaseUrl);
        var negotiationClient = BuildStandaloneHttpClient(config, config.NegotiationBaseUrl);
        var notificationClient = BuildStandaloneHttpClient(config, config.NotificationBaseUrl);
        _ownedClients = [feedbackClient, messageClient, negotiationClient, notificationClient];

        InitializeRestClients(auth, feedbackClient, messageClient, negotiationClient, notificationClient, config, tokenContext);
    }

    private static HttpClient BuildStandaloneHttpClient(CommunicationConfiguration config, string baseUrl)
    {
        var handler = new DiagnosticLoggingHandler(config)
        {
            InnerHandler = new SocketsHttpHandler { PooledConnectionLifetime = TimeSpan.FromMinutes(2) }
        };
        return new HttpClient(handler) { BaseAddress = new Uri(baseUrl), Timeout = config.HttpTimeout };
    }

    public void Dispose()
    {
        foreach (var client in _ownedClients)
            client.Dispose();
    }
}
