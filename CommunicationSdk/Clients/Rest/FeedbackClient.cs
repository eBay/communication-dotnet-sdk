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
using Communication.Generated.Feedback;
using Communication.Generated.Feedback.Feedback;
using Communication.Generated.Feedback.Models;
using EBay.Communication.Configuration;
using EBay.Communication.Exceptions;
using Microsoft.Kiota.Abstractions;

namespace EBay.Communication.Clients.Rest;

/// <summary>
/// Client for Feedback API operations
/// </summary>
public sealed class FeedbackClient
{
    private readonly FeedbackApiClient _kiota;
    private readonly CommunicationConfiguration _config;
    private readonly ITokenContext _tokenContext;

    internal FeedbackClient(FeedbackApiClient kiota, CommunicationConfiguration config, ITokenContext tokenContext)
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
    /// Retrieves feedback for a specified user ID and feedback type.
    /// </summary>
    public async Task<string?> GetFeedbackAsync(
        string userId,
        string feedbackType,
        string? accessToken = null,
        RequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await _tokenContext.RunAsync(accessToken, async () =>
            {
                var result = await _kiota.Feedback.GetAsync(cfg =>
                {
                    cfg.QueryParameters.UserId = userId;
                    cfg.QueryParameters.FeedbackType = feedbackType;
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
    /// Gets the feedback rating summary for a seller
    /// </summary>
    public async Task<string?> GetFeedbackRatingSummaryAsync(
        string userId,
        string? accessToken = null,
        RequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await _tokenContext.RunAsync(accessToken, async () =>
            {
                var result = await _kiota.Feedback_rating_summary.GetAsync(cfg =>
                {
                    cfg.QueryParameters.UserId = userId;
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
    /// Gets items awaiting feedback from the seller
    /// </summary>
    public async Task<string?> GetItemsAwaitingFeedbackAsync(
        string? accessToken = null,
        RequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await _tokenContext.RunAsync(accessToken, async () =>
            {
                var result = await _kiota.Awaiting_feedback.GetAsync(cfg =>
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
    /// Leave feedback for a buyer or seller
    /// </summary>
    public async Task<string?> LeaveFeedbackAsync(
        string feedbackJson,
        string? accessToken = null,
        RequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await _tokenContext.RunAsync(accessToken, async () =>
            {
                var body = System.Text.Json.JsonSerializer.Deserialize<LeaveFeedbackRequest>(feedbackJson);
                var result = await _kiota.Feedback.PostAsync(body, cfg => ApplyHeaders(cfg.Headers, requestOptions), cancellationToken);
                return result != null ? System.Text.Json.JsonSerializer.Serialize(result) : null;
            });
        }
        catch (ApiException ex) when (ex.ResponseStatusCode == (int)HttpStatusCode.Unauthorized)
        {
            throw new UnauthorizedException(ex);
        }
    }

    /// <summary>
    /// Respond to feedback left by a buyer
    /// </summary>
    public async Task<string?> RespondToFeedbackAsync(
        string responseJson,
        string? accessToken = null,
        RequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await _tokenContext.RunAsync(accessToken, async () =>
            {
                var body = System.Text.Json.JsonSerializer.Deserialize<RespondToFeedbackRequest>(responseJson);
                var result = await _kiota.Respond_to_feedback.PostAsync(body, cfg => ApplyHeaders(cfg.Headers, requestOptions), cancellationToken);
                return result != null ? System.Text.Json.JsonSerializer.Serialize(result) : null;
            });
        }
        catch (ApiException ex) when (ex.ResponseStatusCode == (int)HttpStatusCode.Unauthorized)
        {
            throw new UnauthorizedException(ex);
        }
    }
}
