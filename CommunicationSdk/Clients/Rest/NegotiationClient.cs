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
using Communication.Generated.Negotiation;
using Communication.Generated.Negotiation.Models;
using EBay.Communication.Configuration;
using EBay.Communication.Exceptions;
using Microsoft.Kiota.Abstractions;

namespace EBay.Communication.Clients.Rest;

/// <summary>
/// Client for Negotiation API operations
/// </summary>
public sealed class NegotiationClient
{
    private readonly NegotiationApiClient _kiota;
    private readonly CommunicationConfiguration _config;
    private readonly ITokenContext _tokenContext;

    internal NegotiationClient(NegotiationApiClient kiota, CommunicationConfiguration config, ITokenContext tokenContext)
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
    /// Finds items eligible for sending offers to interested buyers
    /// </summary>
    public async Task<string?> FindEligibleItemsAsync(
        string? accessToken = null,
        RequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await _tokenContext.RunAsync(accessToken, async () =>
            {
                var result = await _kiota.Find_eligible_items.GetAsync(cfg =>
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
    /// Sends an offer to interested buyers
    /// </summary>
    public async Task<string?> SendOfferToInterestedBuyersAsync(
        string offerJson,
        string? accessToken = null,
        RequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await _tokenContext.RunAsync(accessToken, async () =>
            {
                var body = System.Text.Json.JsonSerializer.Deserialize<CreateOffersRequest>(offerJson);
                var result = await _kiota.Send_offer_to_interested_buyers.PostAsync(body, cfg =>
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
