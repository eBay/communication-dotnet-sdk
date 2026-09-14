# ChangeLog

## 0.2.0 (09/14/2026)

### Breaking changes

#### Changed signatures

`FeedbackClient`:

| Operation | Previous parameters | Current parameters |
| --- | --- | --- |
| `GetFeedbackAsync` | `(string, string, string?, RequestOptions?, CancellationToken)` | `(string, string, string?, string?, string?, string?, string?, string?, string?, string?, string?, RequestOptions?, CancellationToken)` |
| `GetFeedbackRatingSummaryAsync` | `(string, string?, RequestOptions?, CancellationToken)` | `(string, string, string?, RequestOptions?, CancellationToken)` |
| `GetItemsAwaitingFeedbackAsync` | `(string?, RequestOptions?, CancellationToken)` | `(string?, string?, string?, string?, string?, RequestOptions?, CancellationToken)` |
| `LeaveFeedbackAsync` | `(string, string?, RequestOptions?, CancellationToken)` | `(LeaveFeedbackRequest, string?, RequestOptions?, CancellationToken)` |
| `RespondToFeedbackAsync` | `(string, string?, RequestOptions?, CancellationToken)` | `(RespondToFeedbackRequest, string?, RequestOptions?, CancellationToken)` |

`MessageClient`:

| Operation | Previous parameters | Current parameters |
| --- | --- | --- |
| `BulkUpdateConversationAsync` | `(string, string?, RequestOptions?, CancellationToken)` | `(BulkUpdateConversationsRequest, string?, RequestOptions?, CancellationToken)` |
| `GetConversationAsync` | `(string, string, string?, RequestOptions?, CancellationToken)` | `(string, string, string?, string?, string?, RequestOptions?, CancellationToken)` |
| `GetConversationsAsync` | `(string, string?, RequestOptions?, CancellationToken)` | `(string, string?, string?, string?, string?, string?, string?, string?, string?, string?, RequestOptions?, CancellationToken)` |
| `SendMessageAsync` | `(string, string?, RequestOptions?, CancellationToken)` | `(SendMessageRequest, string?, RequestOptions?, CancellationToken)` |
| `UpdateConversationAsync` | `(string, string?, RequestOptions?, CancellationToken)` | `(UpdateConversationRequest, string?, RequestOptions?, CancellationToken)` |

`NegotiationClient`:

| Operation | Previous parameters | Current parameters |
| --- | --- | --- |
| `FindEligibleItemsAsync` | `(string?, RequestOptions?, CancellationToken)` | `(string, int?, int?, string?, RequestOptions?, CancellationToken)` |
| `SendOfferToInterestedBuyersAsync` | `(string, string?, RequestOptions?, CancellationToken)` | `(string, CreateOffersRequest, string?, RequestOptions?, CancellationToken)` |

`NotificationClient`:

| Operation | Previous parameters | Current parameters |
| --- | --- | --- |
| `CreateDestinationAsync` | `(string, string?, RequestOptions?, CancellationToken)` | `(DestinationRequest, string?, RequestOptions?, CancellationToken)` |
| `CreateSubscriptionAsync` | `(string, string?, RequestOptions?, CancellationToken)` | `(CreateSubscriptionRequest, string?, RequestOptions?, CancellationToken)` |
| `CreateSubscriptionFilterAsync` | `(string, string, string?, RequestOptions?, CancellationToken)` | `(string, CreateSubscriptionFilterRequest, string?, RequestOptions?, CancellationToken)` |
| `GetDestinationsAsync` | `(string?, RequestOptions?, CancellationToken)` | `(string?, string?, string?, RequestOptions?, CancellationToken)` |
| `GetSubscriptionsAsync` | `(string?, RequestOptions?, CancellationToken)` | `(string?, string?, string?, RequestOptions?, CancellationToken)` |
| `GetTopicsAsync` | `(string?, RequestOptions?, CancellationToken)` | `(string?, string?, string?, RequestOptions?, CancellationToken)` |
| `UpdateConfigAsync` | `(string, string?, RequestOptions?, CancellationToken)` | `(Config, string?, RequestOptions?, CancellationToken)` |
| `UpdateDestinationAsync` | `(string, string, string?, RequestOptions?, CancellationToken)` | `(string, DestinationRequest, string?, RequestOptions?, CancellationToken)` |
| `UpdateSubscriptionAsync` | `(string, string, string?, RequestOptions?, CancellationToken)` | `(string, UpdateSubscriptionRequest, string?, RequestOptions?, CancellationToken)` |
