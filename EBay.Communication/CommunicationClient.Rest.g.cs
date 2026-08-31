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

namespace EBay.Communication;

public partial class CommunicationClient
{
    /// <summary>
    /// Feedback API client
    /// </summary>
    public FeedbackClient Feedback { get; private set; } = null!;

    /// <summary>
    /// Message API client (M2M Public API Service)
    /// </summary>
    public MessageClient Message { get; private set; } = null!;

    /// <summary>
    /// Notification API client
    /// </summary>
    public NotificationClient Notification { get; private set; } = null!;

    /// <summary>
    /// Negotiation API client
    /// </summary>
    public NegotiationClient Negotiation { get; private set; } = null!;

    private void InitializeRestClients(
        IAuthenticationProvider authenticationProvider,
        HttpClient httpClient,
        CommunicationConfiguration config,
        ITokenContext tokenContext)
    {
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
}
