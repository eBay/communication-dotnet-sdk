# ChangeLog

## 0.4.0 (11/17/2026)

### Breaking changes

#### Negotiation API moved to the Offers and Leads SDK

The Negotiation API (`NegotiationClient`, `CommunicationClient.Negotiation`, `CommunicationConfiguration.NegotiationBaseUrl` and the `EBay.Communication.Generated.Negotiation` models) is no longer part of this SDK. It is now in `EBay.OffersAndLeads` 0.4.0 as `OffersAndLeadsClient.Negotiation`, with the same operations.

#### Simpler dependency injection

- `AddCommunicationSdk` now registers only the configuration and a singleton `CommunicationClient`. `FeedbackClient`, `MessageClient` and `NotificationClient` are no longer registered individually: inject `CommunicationClient` and use its `Feedback`, `Message` and `Notification` properties.
- The SDK no longer uses `IHttpClientFactory` or registers named `HttpClient`s, and no longer depends on `Microsoft.Extensions.Http`. To customise HTTP (proxy, client certificates, test doubles), use the new `CommunicationClient(CommunicationConfiguration, HttpMessageHandler)` constructor.

#### Trusted hosts

- The client constructors throw `InvalidOperationException` when a base URL uses a host that is not trusted (see Security). Add such hosts, for example a proxy, to `CommunicationConfiguration.AllowedHosts`.

### Security

- The access token is sent only to trusted hosts. Before, it was attached to requests for any HTTPS host configured as a base URL.

### Added

- `CommunicationConfiguration.AllowedHosts` for additional hosts trusted with the access token, and `CommunicationConfiguration.DefaultAllowedHosts`. The eBay production hosts and their sandbox counterparts (for example `api.sandbox.ebay.com`) are trusted by default, so the sandbox needs no extra configuration.
- Target framework `net472` (.NET Framework 4.7.2).

### Changed

- Dependency minimums are lowered to the versions Kiota itself requires, so projects can keep older compatible versions: `Microsoft.Kiota.Bundle` >= 1.22.0 and `Microsoft.Extensions.DependencyInjection.Abstractions` >= 6.0.0. Previously the SDK required the newest 10.0.x packages. On .NET Framework the SDK requires `System.Text.Json` >= 10.0.0, matching the oldest supported .NET LTS (.NET 10). None of these minimums has a known vulnerability, and each release is tested at both the minimum and the latest versions.
- On .NET Framework the SDK's HttpClients allow 64 connections per host instead of the `ServicePointManager` default of 2.
- `CommunicationConfiguration.HttpTimeout` and `RequestOptions.Headers` have `set` accessors instead of `init`, so projects compiled with C# 7.3 can assign them. Existing source compiles unchanged; recompile code that was built against 0.3.0.

## 0.3.0 (09/25/2026)

### Added

- Target frameworks `net8.0` and `net9.0`, in addition to `net10.0`.

## 0.2.0 (09/14/2026)

### Breaking changes

#### Changed signatures

`FeedbackClient`:

| Operation | Previous parameters | Current parameters |
| --- | --- | --- |
| `GetFeedbackAsync` | `(string userId, string feedbackType, string? accessToken, RequestOptions? requestOptions, CancellationToken cancellationToken)` | `(string feedbackType, string userId, string? feedbackId, string? filter, string? limit, string? listingId, string? offset, string? orderLineItemId, string? sort, string? transactionId, string? accessToken, RequestOptions? requestOptions, CancellationToken ct)` |
| `GetFeedbackRatingSummaryAsync` | `(string userId, string? accessToken, RequestOptions? requestOptions, CancellationToken cancellationToken)` | `(string filter, string userId, string? accessToken, RequestOptions? requestOptions, CancellationToken ct)` |
| `GetItemsAwaitingFeedbackAsync` | `(string? accessToken, RequestOptions? requestOptions, CancellationToken cancellationToken)` | `(string? filter, string? limit, string? offset, string? sort, string? accessToken, RequestOptions? requestOptions, CancellationToken ct)` |
| `LeaveFeedbackAsync` | `(string feedbackJson, string? accessToken, RequestOptions? requestOptions, CancellationToken cancellationToken)` | `(LeaveFeedbackRequest body, string? accessToken, RequestOptions? requestOptions, CancellationToken ct)` |
| `RespondToFeedbackAsync` | `(string responseJson, string? accessToken, RequestOptions? requestOptions, CancellationToken cancellationToken)` | `(RespondToFeedbackRequest body, string? accessToken, RequestOptions? requestOptions, CancellationToken ct)` |

`MessageClient`:

| Operation | Previous parameters | Current parameters |
| --- | --- | --- |
| `BulkUpdateConversationAsync` | `(string updateJson, string? accessToken, RequestOptions? requestOptions, CancellationToken cancellationToken)` | `(BulkUpdateConversationsRequest body, string? accessToken, RequestOptions? requestOptions, CancellationToken ct)` |
| `GetConversationAsync` | `(string conversationId, string conversationType, string? accessToken, RequestOptions? requestOptions, CancellationToken cancellationToken)` | `(string conversationId, string conversationType, string? limit, string? offset, string? accessToken, RequestOptions? requestOptions, CancellationToken ct)` |
| `GetConversationsAsync` | `(string conversationType, string? accessToken, RequestOptions? requestOptions, CancellationToken cancellationToken)` | `(string conversationType, string? conversationStatus, string? endTime, string? limit, string? offset, string? otherPartyUsername, string? referenceId, string? referenceType, string? startTime, string? accessToken, RequestOptions? requestOptions, CancellationToken ct)` |
| `SendMessageAsync` | `(string messageJson, string? accessToken, RequestOptions? requestOptions, CancellationToken cancellationToken)` | `(SendMessageRequest body, string? accessToken, RequestOptions? requestOptions, CancellationToken ct)` |
| `UpdateConversationAsync` | `(string updateJson, string? accessToken, RequestOptions? requestOptions, CancellationToken cancellationToken)` | `(UpdateConversationRequest body, string? accessToken, RequestOptions? requestOptions, CancellationToken ct)` |

`NegotiationClient`:

| Operation | Previous parameters | Current parameters |
| --- | --- | --- |
| `FindEligibleItemsAsync` | `(string? accessToken, RequestOptions? requestOptions, CancellationToken cancellationToken)` | `(string xEBAYCMARKETPLACEID, int? limit, int? offset, string? accessToken, RequestOptions? requestOptions, CancellationToken ct)` |
| `SendOfferToInterestedBuyersAsync` | `(string offerJson, string? accessToken, RequestOptions? requestOptions, CancellationToken cancellationToken)` | `(string xEBAYCMARKETPLACEID, CreateOffersRequest body, string? accessToken, RequestOptions? requestOptions, CancellationToken ct)` |

`NotificationClient`:

| Operation | Previous parameters | Current parameters |
| --- | --- | --- |
| `CreateDestinationAsync` | `(string destinationJson, string? accessToken, RequestOptions? requestOptions, CancellationToken cancellationToken)` | `(DestinationRequest body, string? accessToken, RequestOptions? requestOptions, CancellationToken ct)` |
| `CreateSubscriptionAsync` | `(string subscriptionJson, string? accessToken, RequestOptions? requestOptions, CancellationToken cancellationToken)` | `(CreateSubscriptionRequest body, string? accessToken, RequestOptions? requestOptions, CancellationToken ct)` |
| `CreateSubscriptionFilterAsync` | `(string subscriptionId, string filterJson, string? accessToken, RequestOptions? requestOptions, CancellationToken cancellationToken)` | `(string subscriptionId, CreateSubscriptionFilterRequest body, string? accessToken, RequestOptions? requestOptions, CancellationToken ct)` |
| `GetDestinationsAsync` | `(string? accessToken, RequestOptions? requestOptions, CancellationToken cancellationToken)` | `(string? continuationToken, string? limit, string? accessToken, RequestOptions? requestOptions, CancellationToken ct)` |
| `GetSubscriptionsAsync` | `(string? accessToken, RequestOptions? requestOptions, CancellationToken cancellationToken)` | `(string? continuationToken, string? limit, string? accessToken, RequestOptions? requestOptions, CancellationToken ct)` |
| `GetTopicsAsync` | `(string? accessToken, RequestOptions? requestOptions, CancellationToken cancellationToken)` | `(string? continuationToken, string? limit, string? accessToken, RequestOptions? requestOptions, CancellationToken ct)` |
| `UpdateConfigAsync` | `(string configJson, string? accessToken, RequestOptions? requestOptions, CancellationToken cancellationToken)` | `(Config body, string? accessToken, RequestOptions? requestOptions, CancellationToken ct)` |
| `UpdateDestinationAsync` | `(string destinationId, string destinationJson, string? accessToken, RequestOptions? requestOptions, CancellationToken cancellationToken)` | `(string destinationId, DestinationRequest body, string? accessToken, RequestOptions? requestOptions, CancellationToken ct)` |
| `UpdateSubscriptionAsync` | `(string subscriptionId, string subscriptionJson, string? accessToken, RequestOptions? requestOptions, CancellationToken cancellationToken)` | `(string subscriptionId, UpdateSubscriptionRequest body, string? accessToken, RequestOptions? requestOptions, CancellationToken ct)` |
