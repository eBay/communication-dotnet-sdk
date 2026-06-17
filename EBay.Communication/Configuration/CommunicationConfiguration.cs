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


namespace EBay.Communication.Configuration;

/// <summary>
/// Configuration for the Communication SDK
/// </summary>
public sealed class CommunicationConfiguration
{
    private string _feedbackBaseUrl = "https://api.ebay.com/commerce/feedback/v1";
    private string _messageBaseUrl = "https://api.ebay.com/commerce/message/v1";
    private string _notificationBaseUrl = "https://api.ebay.com/commerce/notification/v1";
    private string _negotiationBaseUrl = "https://api.ebay.com/sell/negotiation/v1";

    /// <summary>Fallback bearer token used when no per-call accessToken is supplied.</summary>
    public string? AccessToken { get; set; }

    /// <summary>
    /// Default request options (headers) applied to all SDK calls.
    /// Per-call RequestOptions will override these.
    /// </summary>
    public RequestOptions DefaultRequestOptions { get; set; } = new();

    /// <summary>Timeout applied to all HTTP requests. Defaults to 30 seconds.</summary>
    public TimeSpan HttpTimeout { get; init; } = TimeSpan.FromSeconds(30);

    /// <summary>
    /// When true, logs the HTTP method, URL, and status code of non-success responses to stderr.
    /// Response bodies are intentionally excluded to avoid leaking PII.
    /// <para>
    /// <b>WARNING — PII risk.</b> URLs may contain item IDs and query parameters that are
    /// sensitive in context. Never enable this in production. Ensure logs are access-controlled
    /// and not persisted to any external system when enabled in non-production environments.
    /// </para>
    /// </summary>
    public bool DiagnosticLogging { get; set; }

    /// <summary>
    /// Base URL for Feedback API
    /// </summary>
    public string FeedbackBaseUrl
    {
        get => _feedbackBaseUrl;
        set => _feedbackBaseUrl = RequireHttps(value, nameof(FeedbackBaseUrl));
    }

    /// <summary>
    /// Base URL for Message API
    /// </summary>
    public string MessageBaseUrl
    {
        get => _messageBaseUrl;
        set => _messageBaseUrl = RequireHttps(value, nameof(MessageBaseUrl));
    }

    /// <summary>
    /// Base URL for Notification API
    /// </summary>
    public string NotificationBaseUrl
    {
        get => _notificationBaseUrl;
        set => _notificationBaseUrl = RequireHttps(value, nameof(NotificationBaseUrl));
    }

    /// <summary>
    /// Base URL for Negotiation API
    /// </summary>
    public string NegotiationBaseUrl
    {
        get => _negotiationBaseUrl;
        set => _negotiationBaseUrl = RequireHttps(value, nameof(NegotiationBaseUrl));
    }

    /// <summary>
    /// Gets all allowed hosts for the auth provider
    /// </summary>
    internal IEnumerable<string> GetAllowedHosts() =>
        ["api.ebay.com", "apim.ebay.com", "apiz.ebay.com"];

    private static string RequireHttps(string value, string paramName)
    {
        if (!value.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            throw new ArgumentException("Only HTTPS endpoints are allowed.", paramName);
        return value;
    }
}
