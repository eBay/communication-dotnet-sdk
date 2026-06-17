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
using EBay.Communication.Generated.Message;
using EBay.Communication.Generated.Message.Conversation;
using EBay.Communication.Generated.Message.Models;
using EBay.Communication.Configuration;
using EBay.Communication.Exceptions;
using Microsoft.Kiota.Abstractions;

namespace EBay.Communication.Clients.Rest;

/// <summary>
/// Client for Message API operations (M2M Public API Service)
/// </summary>
public sealed class MessageClient
{
    private readonly MessageApiClient _kiota;
    private readonly CommunicationConfiguration _config;
    private readonly ITokenContext _tokenContext;

    internal MessageClient(MessageApiClient kiota, CommunicationConfiguration config, ITokenContext tokenContext)
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

    /// <summary>
    /// Gets a specific conversation by ID
    /// </summary>
    public async Task<string?> GetConversationAsync(
        string conversationId,
        string conversationType,
        string? accessToken = null,
        RequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await _tokenContext.RunAsync(accessToken, async () =>
            {
                var result = await _kiota.Conversation[conversationId].GetAsync(cfg =>
                {
                    cfg.QueryParameters.ConversationType = conversationType;
                    ApplyHeaders(cfg.Headers, requestOptions);
                }, cancellationToken);
                return result != null ? System.Text.Json.JsonSerializer.Serialize(result) : null;
            });
        }
        catch (ApiException ex) when (ex.ResponseStatusCode == (int)HttpStatusCode.Unauthorized)
        {
            throw new UnauthorizedException(ex);
        }
    }

    /// <summary>
    /// Gets all conversations for the authenticated user
    /// </summary>
    public async Task<string?> GetConversationsAsync(
        string conversationType,
        string? accessToken = null,
        RequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await _tokenContext.RunAsync(accessToken, async () =>
            {
                var result = await _kiota.Conversation.GetAsync(cfg =>
                {
                    cfg.QueryParameters.ConversationType = conversationType;
                    ApplyHeaders(cfg.Headers, requestOptions);
                }, cancellationToken);
                return result != null ? System.Text.Json.JsonSerializer.Serialize(result) : null;
            });
        }
        catch (ApiException ex) when (ex.ResponseStatusCode == (int)HttpStatusCode.Unauthorized)
        {
            throw new UnauthorizedException(ex);
        }
    }

    /// <summary>
    /// Sends a message in a conversation
    /// </summary>
    public async Task<string?> SendMessageAsync(
        string messageJson,
        string? accessToken = null,
        RequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await _tokenContext.RunAsync(accessToken, async () =>
            {
                var body = System.Text.Json.JsonSerializer.Deserialize<SendMessageRequest>(messageJson);
                var result = await _kiota.Send_message.PostAsync(body, cfg =>
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
    /// Updates a conversation
    /// </summary>
    public async Task UpdateConversationAsync(
        string updateJson,
        string? accessToken = null,
        RequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await _tokenContext.RunAsync(accessToken, async () =>
            {
                var body = System.Text.Json.JsonSerializer.Deserialize<UpdateConversationRequest>(updateJson);
                await _kiota.Update_conversation.PostAsync(body, cfg =>
                    ApplyHeaders(cfg.Headers, requestOptions), cancellationToken);
            });
        }
        catch (ApiException ex) when (ex.ResponseStatusCode == (int)HttpStatusCode.Unauthorized)
        {
            throw new UnauthorizedException(ex);
        }
    }

    /// <summary>
    /// Bulk updates multiple conversations
    /// </summary>
    public async Task<string?> BulkUpdateConversationAsync(
        string updateJson,
        string? accessToken = null,
        RequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await _tokenContext.RunAsync(accessToken, async () =>
            {
                var body = System.Text.Json.JsonSerializer.Deserialize<BulkUpdateConversationsRequest>(updateJson);
                var result = await _kiota.Bulk_update_conversation.PostAsync(body, cfg =>
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
