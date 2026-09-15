# ChangeLog

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
