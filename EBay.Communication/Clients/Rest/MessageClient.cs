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

using EBay.Communication.Configuration;
using EBay.Communication.Generated.Message.Models;
using EBay.Communication.Generated.Message;
using Microsoft.Kiota.Abstractions;

namespace EBay.Communication.Clients.Rest;

public sealed class MessageClient
{
    private readonly MessageApiClient _kiota;
    private readonly CommunicationConfiguration _config;
    private readonly ITokenContext _tokenContext;
    private readonly IRequestAdapter _adapter;

    internal MessageClient(MessageApiClient kiota, CommunicationConfiguration config, ITokenContext tokenContext, IRequestAdapter adapter)
    {
        _kiota = kiota;
        _config = config;
        _tokenContext = tokenContext;
        _adapter = adapter;
    }

    /// <summary>
    /// This method can be used to update the <b>conversationStatus</b> of up to 10 conversations.
    /// The <b>conversationId</b>, existing <b>conversationType</b>, and updated <b>conversationStatus</b> for each conversation to modify are required in the <b>conversations</b> array.
    /// Important! </b>"&gt;<b>Important!</b> Although it cannot be updated, the <b>conversationType</b> field is required for each conversation being updated.
    /// If the updates were successful, the <b>conversationId</b> of each conversation will be returned with an associated <b>updateStatus</b> value of <c>SUCCESSFUL</c>.
    /// </summary>
    public Task<BulkUpdateConversationsResponse?> BulkUpdateConversationAsync(
        BulkUpdateConversationsRequest body,
        string? accessToken = null,
        RequestOptions? requestOptions = null,
        CancellationToken ct = default)
        => RestInvocation.CallAsync(
            _tokenContext,
            accessToken,
            () => _kiota.Bulk_update_conversation.PostAsync(body, cfg =>
        {
            RestInvocation.ApplyHeaders(cfg.Headers, _config, requestOptions);
        }, ct));

    /// <summary>
    /// This method can be used to retrieve messages within a specific conversation.
    /// The <b>conversation_id</b> of the conversation for which to retrieve messages is required as a path parameter and the <b>conversation_type</b> of the conversation is required as a query parameter.
    /// </summary>
    public Task<GetMessagesByConversationIdResponse?> GetConversationAsync(
        string conversationId,
        string conversationType,
        string? limit = null,
        string? offset = null,
        string? accessToken = null,
        RequestOptions? requestOptions = null,
        CancellationToken ct = default)
        => RestInvocation.CallAsync(
            _tokenContext,
            accessToken,
            () => _kiota.Conversation[conversationId].GetAsync(cfg =>
        {
            cfg.QueryParameters.ConversationType = conversationType;
            if (limit is not null)
            {
                cfg.QueryParameters.Limit = limit;
            }
            if (offset is not null)
            {
                cfg.QueryParameters.Offset = offset;
            }
            RestInvocation.ApplyHeaders(cfg.Headers, _config, requestOptions);
        }, ct));

    /// <summary>
    /// This method can be used to retrieve one or more conversations associated with a user.
    /// The <b>conversation_type</b> query parameter is required when using this method to specify if the retrieved conversations are from eBay or from members.
    /// The result set can also optionally be filtered by conversation status, reference, username, and/or time range. The <b>limit</b> and <b>offset</b> path parameters can be used to paginate the result set and control how many conversations are returned in the response.
    /// </summary>
    public Task<GetAllMyConversationsResponse?> GetConversationsAsync(
        string conversationType,
        string? conversationStatus = null,
        string? endTime = null,
        string? limit = null,
        string? offset = null,
        string? otherPartyUsername = null,
        string? referenceId = null,
        string? referenceType = null,
        string? startTime = null,
        string? accessToken = null,
        RequestOptions? requestOptions = null,
        CancellationToken ct = default)
        => RestInvocation.CallAsync(
            _tokenContext,
            accessToken,
            () => _kiota.Conversation.GetAsync(cfg =>
        {
            cfg.QueryParameters.ConversationType = conversationType;
            if (conversationStatus is not null)
            {
                cfg.QueryParameters.ConversationStatus = conversationStatus;
            }
            if (endTime is not null)
            {
                cfg.QueryParameters.EndTime = endTime;
            }
            if (limit is not null)
            {
                cfg.QueryParameters.Limit = limit;
            }
            if (offset is not null)
            {
                cfg.QueryParameters.Offset = offset;
            }
            if (otherPartyUsername is not null)
            {
                cfg.QueryParameters.OtherPartyUsername = otherPartyUsername;
            }
            if (referenceId is not null)
            {
                cfg.QueryParameters.ReferenceId = referenceId;
            }
            if (referenceType is not null)
            {
                cfg.QueryParameters.ReferenceType = referenceType;
            }
            if (startTime is not null)
            {
                cfg.QueryParameters.StartTime = startTime;
            }
            RestInvocation.ApplyHeaders(cfg.Headers, _config, requestOptions);
        }, ct));

    /// <summary>
    /// This method can be used to start a conversation with another user or send a message in an existing conversation with another user based on the information provided in the request.
    /// To send a message, one of the <b>conversationId</b> or <b>otherPartyUsername</b> request fields is required. The <b>conversationId</b> must be used when sending a message in an existing conversation and specifies the conversation for which to send the message. For a new conversation, the <b>otherPartyUsername</b> field must be used to send the message to a specific user. In addition, the <b>messageText</b> field is required as it contains the body text of the message.
    /// Optionally, media (such as images or documents) can be attached to the message using the <b>messageMedia</b> container. The <b>reference</b> container can also be used to associate a message with a listing.
    /// </summary>
    public Task<SendMessageResponse?> SendMessageAsync(
        SendMessageRequest body,
        string? accessToken = null,
        RequestOptions? requestOptions = null,
        CancellationToken ct = default)
        => RestInvocation.CallAsync(
            _tokenContext,
            accessToken,
            () => _kiota.Send_message.PostAsync(body, cfg =>
        {
            RestInvocation.ApplyHeaders(cfg.Headers, _config, requestOptions);
        }, ct));

    /// <summary>
    /// This method can be used to update the <b>conversationStatus</b> or the <b>read</b> status of a specified conversation.
    /// <b>Note:</b> Only one of these statuses can be updated at a time using this method. If both fields are included, only the <b>read</b> status of the specified conversation will be updated and the <b>conversationStatus</b> field will be ignored.
    /// Both the <b>conversationId</b> of the conversation to modify and the existing <b>conversationType</b> of the specified conversation are required as part of the request payload.
    /// Important! </b>"&gt;<b>Important!</b> Although it cannot be updated, the existing <b>conversationType</b> of the specified conversation to be updated is required in the request payload. If this value is not provided, an error will occur.
    /// To update a conversation's status (e.g., updating an <c>ACTIVE</c> conversation to <c>ARCHIVE</c>), include the <b>conversationStatus</b> field in the request with the updated value. To update a conversation's read status (e.g., updating an <c>UNREAD</c> conversation to <c>READ</c>), include the <b>read</b> boolean in the request with the updated value.
    /// </summary>
    public Task UpdateConversationAsync(
        UpdateConversationRequest body,
        string? accessToken = null,
        RequestOptions? requestOptions = null,
        CancellationToken ct = default)
        => RestInvocation.CallAsync(
            _tokenContext,
            accessToken,
            () => _kiota.Update_conversation.PostAsync(body, cfg =>
        {
            RestInvocation.ApplyHeaders(cfg.Headers, _config, requestOptions);
        }, ct));
}
