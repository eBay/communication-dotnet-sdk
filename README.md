# Communication .NET SDK

A production-ready .NET SDK for eBay Communication APIs, providing unified access to Feedback, Message (M2M), Notification, and Negotiation REST APIs plus GraphQL support.

## Features

- **Feedback API**: Seller feedback and rating management (5 operations)
- **Message API (M2M)**: Member-to-member messaging and conversation management (5 operations)
- **Notification API**: Event notification subscriptions and delivery (22 operations)
- **Negotiation API**: Send offers to interested buyers (2 operations)
- **GraphQL Support**: Dynamic client for arbitrary GraphQL queries
- **OAuth Authentication**: Token refresh handler interface for seamless authentication
- **Dependency Injection**: First-class support for ASP.NET Core DI
- **Strong Typing**: Internal generated types with public abstraction layer

## Architecture

```
CommunicationSdk/
├── Generated/                    # Kiota-generated REST clients (internal)
│   ├── Feedback/
│   ├── Message/
│   ├── Notification/
│   └── Negotiation/
├── Clients/
│   ├── Rest/
│   │   ├── FeedbackClient.cs    # REST client wrappers (public API)
│   │   ├── MessageClient.cs
│   │   ├── NotificationClient.cs
│   │   └── NegotiationClient.cs
│   └── GraphQL/
│       └── DynamicGraphQLClient.cs  # GraphQL escape hatch
├── Configuration/                # SDK configuration and auth
├── Exceptions/                   # SDK exceptions
└── CommunicationClient.cs        # Main SDK facade
```

## Installation

```bash
# From source
dotnet pack CommunicationSdk/CommunicationSdk.csproj
dotnet add reference path/to/CommunicationSdk.1.0.0.nupkg

# Or add project reference
dotnet add reference CommunicationSdk/CommunicationSdk.csproj
```

## Quick Start

### 1. Implement Token Refresh Handler

```csharp
using Communication.Configuration;

public class MyTokenRefreshHandler : ITokenRefreshHandler
{
    private readonly string _clientId;
    private readonly string _clientSecret;
    private string? _accessToken;
    private DateTime _tokenExpiry;

    public MyTokenRefreshHandler(string clientId, string clientSecret)
    {
        _clientId = clientId;
        _clientSecret = clientSecret;
    }

    public async Task<string> GetAccessTokenAsync(CancellationToken cancellationToken = default)
    {
        if (_accessToken != null && DateTime.UtcNow < _tokenExpiry.AddMinutes(-5))
        {
            return _accessToken;
        }

        // Refresh token using eBay OAuth API
        using var client = new HttpClient();
        var request = new HttpRequestMessage(HttpMethod.Post, "https://api.ebay.com/identity/v1/oauth2/token");
        request.Headers.Authorization = new AuthenticationHeaderValue(
            "Basic",
            Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_clientId}:{_clientSecret}")));
        
        request.Content = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("grant_type", "client_credentials"),
            new KeyValuePair<string, string>("scope", "https://api.ebay.com/oauth/api_scope")
        });

        var response = await client.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync(cancellationToken);
        var tokenResponse = JsonSerializer.Deserialize<TokenResponse>(json);

        _accessToken = tokenResponse.AccessToken;
        _tokenExpiry = DateTime.UtcNow.AddSeconds(tokenResponse.ExpiresIn);

        return _accessToken;
    }
}
```

### 2. Configure and Use the SDK

```csharp
using Communication;
using Communication.Configuration;

// Create configuration
var config = new CommunicationConfiguration
{
    TokenRefreshHandler = new MyTokenRefreshHandler(clientId, secret),
    FeedbackBaseUrl = "https://api.ebay.com/commerce/feedback/v1",
    MessageBaseUrl = "https://api.ebay.com/commerce/message/v1",
    NotificationBaseUrl = "https://api.ebay.com/commerce/notification/v1",
    NegotiationBaseUrl = "https://api.ebay.com/sell/negotiation/v1",
    GraphQLBaseUrl = "https://api.ebay.com/graphql"
};

// Create SDK client
using var client = new CommunicationClient(config);

// Use Feedback API
var feedback = await client.Feedback.GetFeedbackAsync();
var summary = await client.Feedback.GetFeedbackRatingSummaryAsync();
await client.Feedback.LeaveFeedbackAsync(feedbackJson);

// Use Message API
var conversations = await client.Message.GetConversationsAsync();
var conversation = await client.Message.GetConversationAsync(conversationId);
await client.Message.SendMessageAsync(messageJson);

// Use Notification API
var subscriptions = await client.Notification.GetSubscriptionsAsync();
await client.Notification.CreateSubscriptionAsync(subscriptionJson);
await client.Notification.EnableSubscriptionAsync(subscriptionId);

// Use Negotiation API
var eligibleItems = await client.Negotiation.FindEligibleItemsAsync();
await client.Negotiation.SendOfferToInterestedBuyersAsync(offerJson);

// Use GraphQL (escape hatch for arbitrary queries)
var result = await client.GraphQL.ExecuteAsync(@"
    query {
      userBiddingActivity(input: {}) {
        auctions {
          itemId
          auctionDataSummary {
            currentPrice { value currency }
          }
        }
      }
    }");
```

### 3. Dependency Injection (ASP.NET Core)

```csharp
using Communication.Extensions;
using Communication.Configuration;

// In Program.cs or Startup.cs
builder.Services.AddSingleton<ITokenRefreshHandler, MyTokenRefreshHandler>();

builder.Services.AddCommunicationSdk(config =>
{
    config.TokenRefreshHandler = sp.GetRequiredService<ITokenRefreshHandler>();
});

// Inject the SDK or sub-clients
public class CommunicationController : ControllerBase
{
    private readonly CommunicationClient _sdk;
    private readonly FeedbackClient _feedback;
    private readonly MessageClient _message;

    public CommunicationController(
        CommunicationClient sdk,
        FeedbackClient feedback,
        MessageClient message)
    {
        _sdk = sdk;
        _feedback = feedback;
        _message = message;
    }

    [HttpGet("feedback")]
    public async Task<IActionResult> GetFeedback()
    {
        var feedback = await _feedback.GetFeedbackAsync();
        return Ok(feedback);
    }

    [HttpGet("conversations")]
    public async Task<IActionResult> GetConversations()
    {
        var conversations = await _message.GetConversationsAsync();
        return Ok(conversations);
    }
}
```

## API Operations

### Feedback API (5 operations)

#### Get Feedback
```csharp
var feedback = await client.Feedback.GetFeedbackAsync();
```
Retrieves feedback left for a seller.

#### Get Feedback Rating Summary
```csharp
var summary = await client.Feedback.GetFeedbackRatingSummaryAsync();
```
Gets the feedback rating summary for a seller.

#### Get Items Awaiting Feedback
```csharp
var items = await client.Feedback.GetItemsAwaitingFeedbackAsync();
```
Gets items awaiting feedback from the seller.

#### Leave Feedback
```csharp
await client.Feedback.LeaveFeedbackAsync(feedbackJson);
```
Leave feedback for a buyer or seller.

#### Respond to Feedback
```csharp
await client.Feedback.RespondToFeedbackAsync(responseJson);
```
Respond to feedback left by a buyer.

### Message API - M2M Public API Service (5 operations)

#### Get Conversation
```csharp
var conversation = await client.Message.GetConversationAsync(conversationId);
```
Gets a specific conversation by ID.

#### Get Conversations
```csharp
var conversations = await client.Message.GetConversationsAsync();
```
Gets all conversations for the authenticated user.

#### Send Message
```csharp
await client.Message.SendMessageAsync(messageJson);
```
Sends a message in a conversation.

#### Update Conversation
```csharp
await client.Message.UpdateConversationAsync(updateJson);
```
Updates a conversation.

#### Bulk Update Conversation
```csharp
await client.Message.BulkUpdateConversationAsync(updateJson);
```
Bulk updates multiple conversations.

### Negotiation API (2 operations)

#### Find Eligible Items
```csharp
var items = await client.Negotiation.FindEligibleItemsAsync();
```
Finds items eligible for sending offers to interested buyers.

#### Send Offer to Interested Buyers
```csharp
await client.Negotiation.SendOfferToInterestedBuyersAsync(offerJson);
```
Sends an offer to interested buyers.

### Notification API (22 operations)

#### Config Operations
```csharp
var config = await client.Notification.GetConfigAsync();
await client.Notification.UpdateConfigAsync(configJson);
```

#### Destination Operations
```csharp
await client.Notification.CreateDestinationAsync(destinationJson);
var destination = await client.Notification.GetDestinationAsync(destinationId);
var destinations = await client.Notification.GetDestinationsAsync();
await client.Notification.UpdateDestinationAsync(destinationId, destinationJson);
await client.Notification.DeleteDestinationAsync(destinationId);
```

#### Subscription Operations
```csharp
await client.Notification.CreateSubscriptionAsync(subscriptionJson);
var subscription = await client.Notification.GetSubscriptionAsync(subscriptionId);
var subscriptions = await client.Notification.GetSubscriptionsAsync();
await client.Notification.UpdateSubscriptionAsync(subscriptionId, subscriptionJson);
await client.Notification.DeleteSubscriptionAsync(subscriptionId);
await client.Notification.EnableSubscriptionAsync(subscriptionId);
await client.Notification.DisableSubscriptionAsync(subscriptionId);
await client.Notification.TestSubscriptionAsync(subscriptionId);
```

#### Subscription Filter Operations
```csharp
await client.Notification.CreateSubscriptionFilterAsync(subscriptionId, filterJson);
var filter = await client.Notification.GetSubscriptionFilterAsync(subscriptionId, filterId);
await client.Notification.DeleteSubscriptionFilterAsync(subscriptionId, filterId);
```

#### Topic Operations
```csharp
var topic = await client.Notification.GetTopicAsync(topicId);
var topics = await client.Notification.GetTopicsAsync();
```

#### Public Key Operations
```csharp
var publicKey = await client.Notification.GetPublicKeyAsync(publicKeyId);
```

### GraphQL API

#### Execute Arbitrary Queries
```csharp
var result = await client.GraphQL.ExecuteAsync(@"
    query GetUserInfo {
      userBiddingActivity(input: {}) {
        auctions {
          itemId
          auctionDataSummary {
            currentPrice { value currency }
            bidCount
            endTime
          }
        }
        paginationOutput {
          total
          limit
        }
      }
    }");

// Parse the response
var data = result.RootElement.GetProperty("data");
var auctions = data.GetProperty("userBiddingActivity").GetProperty("auctions");
```

## Error Handling

```csharp
using Communication.Exceptions;

try
{
    var feedback = await client.Feedback.GetFeedbackAsync();
}
catch (GraphQLException ex)
{
    foreach (var error in ex.Errors)
    {
        Console.WriteLine($"GraphQL Error: {error.Message}");
        if (error.Path != null)
        {
            Console.WriteLine($"Path: {string.Join(" > ", error.Path)}");
        }
    }
}
catch (SdkException ex)
{
    Console.WriteLine($"SDK Error: {ex.Message}");
}
catch (HttpRequestException ex)
{
    Console.WriteLine($"HTTP Error: {ex.Message}");
}
```

## Regenerating the SDK

When OpenAPI specs or GraphQL schemas change:

```bash
# Run the regeneration script
./generate.sh

# Or manually:
dotnet tool restore
dotnet kiota generate -l csharp -d specs/commerce_feedback_v1_beta_oas3.yaml ...
dotnet build CommunicationSdk/CommunicationSdk.csproj
```

## Configuration Options

| Property | Default | Description |
|----------|---------|-------------|
| `FeedbackBaseUrl` | `https://api.ebay.com/commerce/feedback/v1` | Feedback API base URL |
| `MessageBaseUrl` | `https://api.ebay.com/commerce/message/v1` | Message API base URL |
| `NotificationBaseUrl` | `https://api.ebay.com/commerce/notification/v1` | Notification API base URL |
| `NegotiationBaseUrl` | `https://api.ebay.com/sell/negotiation/v1` | Negotiation API base URL |
| `GraphQLBaseUrl` | `https://api.ebay.com/graphql` | GraphQL API endpoint |
| `TokenRefreshHandler` | (required) | OAuth token refresh handler |

## Project Structure

- **Generated/**: Kiota-generated REST clients (internal, committed to git)
- **Clients/Rest/**: Hand-crafted wrappers exposing public API
- **Clients/GraphQL/**: GraphQL dynamic client for arbitrary queries
- **Configuration/**: SDK config, auth providers
- **Exceptions/**: SDK exceptions
- **Extensions/**: DI extension methods

## Development

### Prerequisites

- .NET 10.0 SDK
- Kiota CLI tool (auto-installed via dotnet tools)

### Building

```bash
dotnet build CommunicationSdk/CommunicationSdk.csproj
```

### Packaging

```bash
dotnet pack CommunicationSdk/CommunicationSdk.csproj
```

## Dependencies

- **Microsoft.Kiota.Bundle**: REST client generation and runtime
- **Microsoft.Extensions.DependencyInjection.Abstractions**: DI support
- **Microsoft.Extensions.Http**: HttpClient factory integration

## License

[Apache 2.0](LICENSE) © 2026 eBay Inc.

## Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Run `./generate.sh` if specs changed
5. Ensure `dotnet build` succeeds
6. Submit a pull request
