# eBay Communication SDK for .NET

Leave and respond to feedback, exchange messages with buyers, and subscribe to eBay notifications. This SDK wraps the eBay Feedback API, M2M Public API Service and Notification API behind one client, `CommunicationClient`.

- **Target frameworks:** .NET Framework 4.7.2, .NET 8, .NET 9 and .NET 10
- **C#:** usable from C# 7.3 and later

## Installation

The package `EBay.Communication` is not on nuget.org yet. Reference the project, or build the package and add it to a local feed:

```bash
dotnet pack EBay.Communication -c Release -o artifacts/packages
```

## Quick start

```csharp
using EBay.Communication;
using EBay.Communication.Configuration;

var config = new CommunicationConfiguration
{
    // A user or application access token from your own OAuth flow (see "Authentication").
    AccessToken = Environment.GetEnvironmentVariable("EBAY_ACCESS_TOKEN")
};

using var client = new CommunicationClient(config);

var awaiting = await client.Feedback.GetItemsAwaitingFeedbackAsync(limit: "10");
Console.WriteLine($"{awaiting?.ItemsAwaitingFeedbackCount} items awaiting feedback");
```

## APIs

| `CommunicationClient` property | API | Default base URL | Access token |
|---|---|---|---|
| `Feedback` | Feedback API | `https://api.ebay.com/commerce/feedback/v1` | User |
| `Message` | M2M Public API Service | `https://api.ebay.com/commerce/message/v1` | User |
| `Notification` | Notification API | `https://api.ebay.com/commerce/notification/v1` | User (some operations: application) |

"Access token" is what eBay's OpenAPI spec declares: an application token (client credentials) or a user token (authorization code). Scopes are listed per API below.

<details>
<summary><b>Feedback</b>: Feedback API, 5 operations</summary>

Scopes: `https://api.ebay.com/oauth/api_scope/commerce.feedback`, `https://api.ebay.com/oauth/api_scope/commerce.feedback.readonly`

| Method | Description |
|---|---|
| `GetItemsAwaitingFeedbackAsync` | This method retrieves line items awaiting feedback from the user's order partner. |
| `GetFeedbackAsync` | This method enables users to retrieve feedback for any specified user ID and feedback type (sent or received). |
| `LeaveFeedbackAsync` | This method creates and submits feedback to the user's order partner for a line item in the order. |
| `GetFeedbackRatingSummaryAsync` | This method provides a detailed overview of feedback ratings associated with a user in the eBay marketplace. |
| `RespondToFeedbackAsync` | This method allows users to respond to feedback provided by the order partner for a specific line item in an order. |

</details>

<details>
<summary><b>Message</b>: M2M Public API Service, 5 operations</summary>

Scopes: `https://api.ebay.com/oauth/api_scope/commerce.message`

| Method | Description |
|---|---|
| `BulkUpdateConversationAsync` | This method can be used to update the conversationStatus of up to 10 conversations. |
| `GetConversationAsync` | This method can be used to retrieve messages within a specific conversation. |
| `GetConversationsAsync` | This method can be used to retrieve one or more conversations associated with a user. |
| `SendMessageAsync` | This method can be used to start a conversation with another user or send a message in an existing conversation with another user based on the information provided in the request. |
| `UpdateConversationAsync` | This method can be used to update the conversationStatus or the read status of a specified conversation. |

</details>

<details>
<summary><b>Notification</b>: Notification API, 21 operations</summary>

Scopes: `https://api.ebay.com/oauth/api_scope`, `https://api.ebay.com/oauth/api_scope/commerce.notification.subscription`, `https://api.ebay.com/oauth/api_scope/commerce.notification.subscription.readonly`

| Method | Description |
|---|---|
| `GetConfigAsync` | This method allows applications to retrieve a previously created configuration. |
| `UpdateConfigAsync` | This method allows applications to create a new configuration or update an existing configuration. |
| `GetDestinationsAsync` | This method allows applications to retrieve a paginated collection of destination resources and related details. |
| `CreateDestinationAsync` | This method allows applications to create a destination. |
| `GetDestinationAsync` | This method allows applications to fetch the details for a destination. |
| `UpdateDestinationAsync` | This method allows applications to update a destination. |
| `DeleteDestinationAsync` | This method provides applications a way to delete a destination. |
| `GetPublicKeyAsync` | This method allows users to retrieve a public key using a specified key ID. |
| `GetSubscriptionsAsync` | This method allows applications to retrieve a list of all subscriptions. |
| `CreateSubscriptionAsync` | This method allows applications to create a subscription for a topic and supported schema version. |
| `CreateSubscriptionFilterAsync` | This method allows applications to create a filter for a subscription. |
| `GetSubscriptionAsync` | This method allows applications to retrieve subscription details for the specified subscription. |
| `UpdateSubscriptionAsync` | This method allows applications to update a subscription. |
| `DeleteSubscriptionAsync` | This method allows applications to delete a subscription. |
| `GetSubscriptionFilterAsync` | This method allows applications to retrieve the filter details for the specified subscription filter. |
| `DeleteSubscriptionFilterAsync` | This method allows applications to disable the active filter on a subscription, so that a new subscription filter may be added. |
| `DisableSubscriptionAsync` | This method disables a subscription, which prevents the subscription from providing notifications. |
| `EnableSubscriptionAsync` | This method allows applications to enable a disabled subscription. |
| `TestSubscriptionAsync` | This method triggers a mocked test payload that includes a notification ID, publish date, and so on. |
| `GetTopicAsync` | This method allows applications to retrieve details for the specified topic. |
| `GetTopicsAsync` | This method returns a paginated collection of all supported topics, along with the details for the topics. |

</details>

## Authentication

The SDK does not obtain or refresh tokens: get them with your own OAuth flow ([eBay OAuth guide](https://developer.ebay.com/develop/guides/sell/authorization)) and give them to the SDK in one of two ways:

- `CommunicationConfiguration.AccessToken`: used for every call that doesn't pass its own token.
- The `accessToken` parameter on every method: used for that call only, for example a user token where the configuration holds an application token.

```csharp
string userToken = Environment.GetEnvironmentVariable("EBAY_USER_TOKEN")!;
var awaiting = await client.Feedback.GetItemsAwaitingFeedbackAsync(limit: "10", accessToken: userToken);
```

A token is only ever sent to trusted hosts, see below.

## Configuration

| Property | Type | Default | Description |
|---|---|---|---|
| `FeedbackBaseUrl` | `string` | `https://api.ebay.com/commerce/feedback/v1` | Base URL of the Feedback API. |
| `MessageBaseUrl` | `string` | `https://api.ebay.com/commerce/message/v1` | Base URL of the M2M Public API Service. |
| `NotificationBaseUrl` | `string` | `https://api.ebay.com/commerce/notification/v1` | Base URL of the Notification API. |
| `AccessToken` | `string?` |  | Fallback bearer token used when no per-call accessToken is supplied. |
| `DiagnosticLogging` | `bool` | `false` | When true, logs the HTTP method, URL, status code, and response reason phrase for all non-success responses to stderr. |
| `HttpTimeout` | `TimeSpan` | 30 seconds | Timeout applied to all HTTP requests. |
| `DefaultRequestOptions` | `RequestOptions` | no headers | Default headers applied to every REST request. |
| `AllowedHosts` | `IList<string>` | empty | Additional hosts trusted with the access token, for example a proxy (see below). |

`CommunicationConfiguration.DefaultAllowedHosts` lists the hosts trusted by default.

## Sandbox and trusted hosts

The access token is only sent to trusted hosts: the hosts in `CommunicationConfiguration.DefaultAllowedHosts`, which are the eBay production hosts and their `*.sandbox.ebay.com` counterparts, plus any in `AllowedHosts`. Switching to the sandbox needs only the base URLs:

```csharp
var sandboxConfig = new CommunicationConfiguration
{
    AccessToken = Environment.GetEnvironmentVariable("EBAY_SANDBOX_ACCESS_TOKEN"),
    FeedbackBaseUrl = "https://api.sandbox.ebay.com/commerce/feedback/v1",
    MessageBaseUrl = "https://api.sandbox.ebay.com/commerce/message/v1",
    NotificationBaseUrl = "https://api.sandbox.ebay.com/commerce/notification/v1",
};
```

Any other host, for example a proxy that forwards to eBay, has to be added to `AllowedHosts`; the client constructor throws `InvalidOperationException` otherwise:

```csharp
var proxied = new CommunicationConfiguration
{
    FeedbackBaseUrl = "https://ebay-proxy.example.com/commerce/feedback/v1",
    AllowedHosts = { "ebay-proxy.example.com" },
};
```

## Dependency injection

`AddCommunicationSdk` registers the configuration and one shared `CommunicationClient` as a singleton. It owns its HTTP connections and is disposed with the container. Inject the client and use its properties for the individual APIs:

```csharp
using EBay.Communication;
using EBay.Communication.Configuration;
using EBay.Communication.Extensions;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCommunicationSdk(new CommunicationConfiguration
{
    AccessToken = builder.Configuration["EBay:AccessToken"]
});

var app = builder.Build();
app.MapGet("/example", async (CommunicationClient client) =>
{
    var awaiting = await client.Feedback.GetItemsAwaitingFeedbackAsync(limit: "10");
    return Results.Ok();
});
app.Run();
```

## Custom HTTP handler

To send requests through your own handler, for example for a proxy, client certificates or a test double, pass it to the constructor. The handler is shared by all APIs and is not disposed by the client:

```csharp
var handler = new HttpClientHandler { Proxy = new WebProxy("http://corporate-proxy:8080") };
using var proxiedClient = new CommunicationClient(config, handler);
```

## Error handling

```csharp
try
{
    var awaiting = await client.Feedback.GetItemsAwaitingFeedbackAsync(limit: "10");
}
catch (UnauthorizedException)
{
    // HTTP 401: the token is missing, expired, for the other environment, or lacks a scope.
}
catch (ApiException ex)
{
    // Any other REST error.
    Console.WriteLine($"HTTP {ex.ResponseStatusCode}");
}
```

- `UnauthorizedException` (HTTP 401) derives from `SdkException`.
- Other REST errors throw Kiota's `ApiException`, or a generated subclass named in eBay's spec for that operation, with `ResponseStatusCode`. eBay's error text is only available where the spec describes the error body correctly; set `DiagnosticLogging = true` to log failed requests to stderr.

## .NET Framework

The `net472` build is a classic package that `packages.config` projects can use. Such projects must list every dependency themselves. At the minimum versions the SDK supports, these are:

| Package | Version |
|---|---|
| Microsoft.Kiota.Abstractions, .Bundle, .Http.HttpClientLibrary, .Serialization.Form, .Serialization.Json, .Serialization.Multipart, .Serialization.Text | 1.22.0 |
| Std.UriTemplate | 2.0.8 |
| Microsoft.IO.RecyclableMemoryStream | 3.0.1 |
| Microsoft.Extensions.DependencyInjection.Abstractions | 6.0.0 |
| System.Diagnostics.DiagnosticSource, System.Net.Http.WinHttpHandler | 6.0.0 |
| System.Text.Json, System.Text.Encodings.Web, System.IO.Pipelines, Microsoft.Bcl.AsyncInterfaces | 10.0.0 |
| System.Runtime.CompilerServices.Unsafe | 6.1.2 |
| System.Memory, System.Threading.Tasks.Extensions | 4.6.3 |
| System.Buffers, System.Numerics.Vectors, System.ValueTuple | 4.6.1 |

With these versions the app needs binding redirects for Microsoft.Bcl.AsyncInterfaces (10.0.0.0), System.Buffers (4.0.5.0), System.Memory (4.0.5.0), System.Runtime.CompilerServices.Unsafe (6.0.3.0), System.Text.Encodings.Web (10.0.0.0), System.Text.Json (10.0.0.0) and System.Threading.Tasks.Extensions (4.2.4.0). There must be no redirect for System.ValueTuple: .NET Framework 4.7.1+ provides it. Newer package versions work too and change the redirect versions.

## Development

- .NET 10 SDK, plus the .NET 8 and .NET 9 SDKs so every target framework builds. `net472` builds on any OS; its tests run on Windows only.
- Build: `dotnet build EBay.Communication`
- Test: `dotnet test tests/EBay.Communication.Tests`
- Most of this repository is generated from eBay's API specs by the SDK generator (`api-sdk-definitions`): the API clients, `Configuration/`, `Extensions/`, `CommunicationClient.cs`, `specs/` and `generate.sh`. Change the generator, not the generated files. `./generate.sh` reruns Kiota for the specs in `specs/openapi`.

See [CONTRIBUTING.adoc](CONTRIBUTING.adoc).

## License

Apache 2.0, see [LICENSE](LICENSE).
