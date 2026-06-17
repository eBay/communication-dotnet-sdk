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


using System.Net;
using EBay.Communication.Generated.Notification;
using EBay.Communication.Generated.Notification.Models;
using EBay.Communication.Configuration;
using EBay.Communication.Exceptions;
using Microsoft.Kiota.Abstractions;

namespace EBay.Communication.Clients.Rest;

/// <summary>
/// Client for Notification API operations
/// </summary>
public sealed class NotificationClient
{
    private readonly NotificationApiClient _kiota;
    private readonly CommunicationConfiguration _config;
    private readonly ITokenContext _tokenContext;

    internal NotificationClient(NotificationApiClient kiota, CommunicationConfiguration config, ITokenContext tokenContext)
    {
        _kiota = kiota;
        _config = config;
        _tokenContext = tokenContext;
    }

    private void ApplyHeaders(RequestHeaders headers, RequestOptions? callOptions)
    {
        foreach (var (k, v) in _config.DefaultRequestOptions.Headers)
            headers.TryAdd(k, v);
        if (callOptions is null) return;
        foreach (var (k, v) in callOptions.Headers)
            headers[k] = new[] { v };
    }

    // ── Config Operations ─────────────────────────────────────────────────────────

    /// <summary>
    /// Gets the notification configuration
    /// </summary>
    public async Task<string?> GetConfigAsync(
        string? accessToken = null,
        RequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await _tokenContext.RunAsync(accessToken, async () =>
            {
                var result = await _kiota.Config.GetAsync(cfg =>
                    ApplyHeaders(cfg.Headers, requestOptions), cancellationToken);
                return result != null ? System.Text.Json.JsonSerializer.Serialize(result) : null;
            });
        }
        catch (ApiException ex) when (ex.ResponseStatusCode == (int)HttpStatusCode.Unauthorized)
        {
            throw new UnauthorizedException(ex);
        }
    }

    /// <summary>
    /// Updates the notification configuration
    /// </summary>
    public async Task UpdateConfigAsync(
        string configJson,
        string? accessToken = null,
        RequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await _tokenContext.RunAsync(accessToken, async () =>
            {
                var body = System.Text.Json.JsonSerializer.Deserialize<Config>(configJson);
                await _kiota.Config.PutAsync(body, cfg =>
                    ApplyHeaders(cfg.Headers, requestOptions), cancellationToken);
            });
        }
        catch (ApiException ex) when (ex.ResponseStatusCode == (int)HttpStatusCode.Unauthorized)
        {
            throw new UnauthorizedException(ex);
        }
    }

    // ── Destination Operations ────────────────────────────────────────────────────

    /// <summary>
    /// Creates a notification destination
    /// </summary>
    public async Task<string?> CreateDestinationAsync(
        string destinationJson,
        string? accessToken = null,
        RequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await _tokenContext.RunAsync(accessToken, async () =>
            {
                var body = System.Text.Json.JsonSerializer.Deserialize<DestinationRequest>(destinationJson);
                var result = await _kiota.Destination.PostAsync(body, cfg =>
                    ApplyHeaders(cfg.Headers, requestOptions), cancellationToken);
                return result != null ? System.Text.Json.JsonSerializer.Serialize(result) : null;
            });
        }
        catch (ApiException ex) when (ex.ResponseStatusCode == (int)HttpStatusCode.Unauthorized)
        {
            throw new UnauthorizedException(ex);
        }
    }

    /// <summary>
    /// Gets a specific destination by ID
    /// </summary>
    public async Task<string?> GetDestinationAsync(
        string destinationId,
        string? accessToken = null,
        RequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await _tokenContext.RunAsync(accessToken, async () =>
            {
                var result = await _kiota.Destination[destinationId].GetAsync(cfg =>
                    ApplyHeaders(cfg.Headers, requestOptions), cancellationToken);
                return result != null ? System.Text.Json.JsonSerializer.Serialize(result) : null;
            });
        }
        catch (ApiException ex) when (ex.ResponseStatusCode == (int)HttpStatusCode.Unauthorized)
        {
            throw new UnauthorizedException(ex);
        }
    }

    /// <summary>
    /// Gets all destinations
    /// </summary>
    public async Task<string?> GetDestinationsAsync(
        string? accessToken = null,
        RequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await _tokenContext.RunAsync(accessToken, async () =>
            {
                var result = await _kiota.Destination.GetAsync(cfg =>
                    ApplyHeaders(cfg.Headers, requestOptions), cancellationToken);
                return result != null ? System.Text.Json.JsonSerializer.Serialize(result) : null;
            });
        }
        catch (ApiException ex) when (ex.ResponseStatusCode == (int)HttpStatusCode.Unauthorized)
        {
            throw new UnauthorizedException(ex);
        }
    }

    /// <summary>
    /// Updates a destination
    /// </summary>
    public async Task UpdateDestinationAsync(
        string destinationId,
        string destinationJson,
        string? accessToken = null,
        RequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await _tokenContext.RunAsync(accessToken, async () =>
            {
                var body = System.Text.Json.JsonSerializer.Deserialize<DestinationRequest>(destinationJson);
                await _kiota.Destination[destinationId].PutAsync(body, cfg =>
                    ApplyHeaders(cfg.Headers, requestOptions), cancellationToken);
            });
        }
        catch (ApiException ex) when (ex.ResponseStatusCode == (int)HttpStatusCode.Unauthorized)
        {
            throw new UnauthorizedException(ex);
        }
    }

    /// <summary>
    /// Deletes a destination
    /// </summary>
    public async Task DeleteDestinationAsync(
        string destinationId,
        string? accessToken = null,
        RequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await _tokenContext.RunAsync(accessToken, async () =>
            {
                await _kiota.Destination[destinationId].DeleteAsync(cfg =>
                    ApplyHeaders(cfg.Headers, requestOptions), cancellationToken);
            });
        }
        catch (ApiException ex) when (ex.ResponseStatusCode == (int)HttpStatusCode.Unauthorized)
        {
            throw new UnauthorizedException(ex);
        }
    }

    // ── Subscription Operations ───────────────────────────────────────────────────

    /// <summary>
    /// Creates a notification subscription
    /// </summary>
    public async Task<string?> CreateSubscriptionAsync(
        string subscriptionJson,
        string? accessToken = null,
        RequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await _tokenContext.RunAsync(accessToken, async () =>
            {
                var body = System.Text.Json.JsonSerializer.Deserialize<CreateSubscriptionRequest>(subscriptionJson);
                var result = await _kiota.Subscription.PostAsync(body, cfg =>
                    ApplyHeaders(cfg.Headers, requestOptions), cancellationToken);
                return result != null ? System.Text.Json.JsonSerializer.Serialize(result) : null;
            });
        }
        catch (ApiException ex) when (ex.ResponseStatusCode == (int)HttpStatusCode.Unauthorized)
        {
            throw new UnauthorizedException(ex);
        }
    }

    /// <summary>
    /// Gets a specific subscription by ID
    /// </summary>
    public async Task<string?> GetSubscriptionAsync(
        string subscriptionId,
        string? accessToken = null,
        RequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await _tokenContext.RunAsync(accessToken, async () =>
            {
                var result = await _kiota.Subscription[subscriptionId].GetAsync(cfg =>
                    ApplyHeaders(cfg.Headers, requestOptions), cancellationToken);
                return result != null ? System.Text.Json.JsonSerializer.Serialize(result) : null;
            });
        }
        catch (ApiException ex) when (ex.ResponseStatusCode == (int)HttpStatusCode.Unauthorized)
        {
            throw new UnauthorizedException(ex);
        }
    }

    /// <summary>
    /// Gets all subscriptions
    /// </summary>
    public async Task<string?> GetSubscriptionsAsync(
        string? accessToken = null,
        RequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await _tokenContext.RunAsync(accessToken, async () =>
            {
                var result = await _kiota.Subscription.GetAsync(cfg =>
                    ApplyHeaders(cfg.Headers, requestOptions), cancellationToken);
                return result != null ? System.Text.Json.JsonSerializer.Serialize(result) : null;
            });
        }
        catch (ApiException ex) when (ex.ResponseStatusCode == (int)HttpStatusCode.Unauthorized)
        {
            throw new UnauthorizedException(ex);
        }
    }

    /// <summary>
    /// Updates a subscription
    /// </summary>
    public async Task UpdateSubscriptionAsync(
        string subscriptionId,
        string subscriptionJson,
        string? accessToken = null,
        RequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await _tokenContext.RunAsync(accessToken, async () =>
            {
                var body = System.Text.Json.JsonSerializer.Deserialize<UpdateSubscriptionRequest>(subscriptionJson);
                await _kiota.Subscription[subscriptionId].PutAsync(body, cfg =>
                    ApplyHeaders(cfg.Headers, requestOptions), cancellationToken);
            });
        }
        catch (ApiException ex) when (ex.ResponseStatusCode == (int)HttpStatusCode.Unauthorized)
        {
            throw new UnauthorizedException(ex);
        }
    }

    /// <summary>
    /// Deletes a subscription
    /// </summary>
    public async Task DeleteSubscriptionAsync(
        string subscriptionId,
        string? accessToken = null,
        RequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await _tokenContext.RunAsync(accessToken, async () =>
            {
                await _kiota.Subscription[subscriptionId].DeleteAsync(cfg =>
                    ApplyHeaders(cfg.Headers, requestOptions), cancellationToken);
            });
        }
        catch (ApiException ex) when (ex.ResponseStatusCode == (int)HttpStatusCode.Unauthorized)
        {
            throw new UnauthorizedException(ex);
        }
    }

    /// <summary>
    /// Enables a subscription
    /// </summary>
    public async Task EnableSubscriptionAsync(
        string subscriptionId,
        string? accessToken = null,
        RequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await _tokenContext.RunAsync(accessToken, async () =>
            {
                await _kiota.Subscription[subscriptionId].Enable.PostAsync(cfg =>
                    ApplyHeaders(cfg.Headers, requestOptions), cancellationToken);
            });
        }
        catch (ApiException ex) when (ex.ResponseStatusCode == (int)HttpStatusCode.Unauthorized)
        {
            throw new UnauthorizedException(ex);
        }
    }

    /// <summary>
    /// Disables a subscription
    /// </summary>
    public async Task DisableSubscriptionAsync(
        string subscriptionId,
        string? accessToken = null,
        RequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await _tokenContext.RunAsync(accessToken, async () =>
            {
                await _kiota.Subscription[subscriptionId].Disable.PostAsync(cfg =>
                    ApplyHeaders(cfg.Headers, requestOptions), cancellationToken);
            });
        }
        catch (ApiException ex) when (ex.ResponseStatusCode == (int)HttpStatusCode.Unauthorized)
        {
            throw new UnauthorizedException(ex);
        }
    }

    /// <summary>
    /// Tests a subscription
    /// </summary>
    public async Task TestSubscriptionAsync(
        string subscriptionId,
        string? accessToken = null,
        RequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await _tokenContext.RunAsync(accessToken, async () =>
            {
                await _kiota.Subscription[subscriptionId].Test.PostAsync(cfg =>
                    ApplyHeaders(cfg.Headers, requestOptions), cancellationToken);
            });
        }
        catch (ApiException ex) when (ex.ResponseStatusCode == (int)HttpStatusCode.Unauthorized)
        {
            throw new UnauthorizedException(ex);
        }
    }

    // ── Subscription Filter Operations ────────────────────────────────────────────

    /// <summary>
    /// Creates a subscription filter
    /// </summary>
    public async Task<string?> CreateSubscriptionFilterAsync(
        string subscriptionId,
        string filterJson,
        string? accessToken = null,
        RequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await _tokenContext.RunAsync(accessToken, async () =>
            {
                var body = System.Text.Json.JsonSerializer.Deserialize<CreateSubscriptionFilterRequest>(filterJson);
                var result = await _kiota.Subscription[subscriptionId].Filter.PostAsync(body, cfg =>
                    ApplyHeaders(cfg.Headers, requestOptions), cancellationToken);
                return result != null ? System.Text.Json.JsonSerializer.Serialize(result) : null;
            });
        }
        catch (ApiException ex) when (ex.ResponseStatusCode == (int)HttpStatusCode.Unauthorized)
        {
            throw new UnauthorizedException(ex);
        }
    }

    /// <summary>
    /// Gets a subscription filter
    /// </summary>
    public async Task<string?> GetSubscriptionFilterAsync(
        string subscriptionId,
        string filterId,
        string? accessToken = null,
        RequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await _tokenContext.RunAsync(accessToken, async () =>
            {
                var result = await _kiota.Subscription[subscriptionId].Filter[filterId].GetAsync(cfg =>
                    ApplyHeaders(cfg.Headers, requestOptions), cancellationToken);
                return result != null ? System.Text.Json.JsonSerializer.Serialize(result) : null;
            });
        }
        catch (ApiException ex) when (ex.ResponseStatusCode == (int)HttpStatusCode.Unauthorized)
        {
            throw new UnauthorizedException(ex);
        }
    }

    /// <summary>
    /// Deletes a subscription filter
    /// </summary>
    public async Task DeleteSubscriptionFilterAsync(
        string subscriptionId,
        string filterId,
        string? accessToken = null,
        RequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await _tokenContext.RunAsync(accessToken, async () =>
            {
                await _kiota.Subscription[subscriptionId].Filter[filterId].DeleteAsync(cfg =>
                    ApplyHeaders(cfg.Headers, requestOptions), cancellationToken);
            });
        }
        catch (ApiException ex) when (ex.ResponseStatusCode == (int)HttpStatusCode.Unauthorized)
        {
            throw new UnauthorizedException(ex);
        }
    }

    // ── Topic Operations ──────────────────────────────────────────────────────────

    /// <summary>
    /// Gets a specific topic by ID
    /// </summary>
    public async Task<string?> GetTopicAsync(
        string topicId,
        string? accessToken = null,
        RequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await _tokenContext.RunAsync(accessToken, async () =>
            {
                var result = await _kiota.Topic[topicId].GetAsync(cfg =>
                    ApplyHeaders(cfg.Headers, requestOptions), cancellationToken);
                return result != null ? System.Text.Json.JsonSerializer.Serialize(result) : null;
            });
        }
        catch (ApiException ex) when (ex.ResponseStatusCode == (int)HttpStatusCode.Unauthorized)
        {
            throw new UnauthorizedException(ex);
        }
    }

    /// <summary>
    /// Gets all topics
    /// </summary>
    public async Task<string?> GetTopicsAsync(
        string? accessToken = null,
        RequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await _tokenContext.RunAsync(accessToken, async () =>
            {
                var result = await _kiota.Topic.GetAsync(cfg =>
                    ApplyHeaders(cfg.Headers, requestOptions), cancellationToken);
                return result != null ? System.Text.Json.JsonSerializer.Serialize(result) : null;
            });
        }
        catch (ApiException ex) when (ex.ResponseStatusCode == (int)HttpStatusCode.Unauthorized)
        {
            throw new UnauthorizedException(ex);
        }
    }

    // ── Public Key Operations ─────────────────────────────────────────────────────

    /// <summary>
    /// Gets a public key by ID
    /// </summary>
    public async Task<string?> GetPublicKeyAsync(
        string publicKeyId,
        string? accessToken = null,
        RequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await _tokenContext.RunAsync(accessToken, async () =>
            {
                var result = await _kiota.Public_key[publicKeyId].GetAsync(cfg =>
                    ApplyHeaders(cfg.Headers, requestOptions), cancellationToken);
                return result != null ? System.Text.Json.JsonSerializer.Serialize(result) : null;
            });
        }
        catch (ApiException ex) when (ex.ResponseStatusCode == (int)HttpStatusCode.Unauthorized)
        {
            throw new UnauthorizedException(ex);
        }
    }
}
