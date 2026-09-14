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
using EBay.Communication.Generated.Notification.Models;
using EBay.Communication.Generated.Notification;
using Microsoft.Kiota.Abstractions;

namespace EBay.Communication.Clients.Rest;

public sealed class NotificationClient
{
    private readonly NotificationApiClient _kiota;
    private readonly CommunicationConfiguration _config;
    private readonly ITokenContext _tokenContext;
    private readonly IRequestAdapter _adapter;

    internal NotificationClient(NotificationApiClient kiota, CommunicationConfiguration config, ITokenContext tokenContext, IRequestAdapter adapter)
    {
        _kiota = kiota;
        _config = config;
        _tokenContext = tokenContext;
        _adapter = adapter;
    }

    /// <summary>
    /// This method allows applications to retrieve a previously created configuration.
    /// </summary>
    public Task<Config?> GetConfigAsync(
        string? accessToken = null,
        RequestOptions? requestOptions = null,
        CancellationToken ct = default)
        => RestInvocation.CallAsync(
            _tokenContext,
            accessToken,
            () => _kiota.Config.GetAsync(cfg =>
        {
            RestInvocation.ApplyHeaders(cfg.Headers, _config, requestOptions);
        }, ct));

    /// <summary>
    /// This method allows applications to create a new configuration or update an existing configuration. This app-level configuration allows developers to set up alerts.
    /// </summary>
    public Task UpdateConfigAsync(
        Config body,
        string? accessToken = null,
        RequestOptions? requestOptions = null,
        CancellationToken ct = default)
        => RestInvocation.CallAsync(
            _tokenContext,
            accessToken,
            () => _kiota.Config.PutAsync(body, cfg =>
        {
            RestInvocation.ApplyHeaders(cfg.Headers, _config, requestOptions);
        }, ct));

    /// <summary>
    /// This method allows applications to retrieve a paginated collection of destination resources and related details. The details include the destination names, statuses, and configurations, including the endpoints and verification tokens.
    /// </summary>
    public Task<DestinationSearchResponse?> GetDestinationsAsync(
        string? continuationToken = null,
        string? limit = null,
        string? accessToken = null,
        RequestOptions? requestOptions = null,
        CancellationToken ct = default)
        => RestInvocation.CallAsync(
            _tokenContext,
            accessToken,
            () => _kiota.Destination.GetAsync(cfg =>
        {
            if (continuationToken is not null)
            {
                cfg.QueryParameters.ContinuationToken = continuationToken;
            }
            if (limit is not null)
            {
                cfg.QueryParameters.Limit = limit;
            }
            RestInvocation.ApplyHeaders(cfg.Headers, _config, requestOptions);
        }, ct));

    /// <summary>
    /// This method allows applications to create a destination. A destination is an endpoint that receives HTTP push notifications.
    /// A single destination for all topics is valid, as is individual destinations for each topic.
    /// To update a destination, use the <b>updateDestination</b> call.
    /// The destination created will need to be referenced while creating or updating a subscription to a topic.
    /// <b>Note:</b> The destination should be created and ready to respond with the expected <b>challengeResponse</b> for the endpoint to be registered successfully. Refer to the <see href="/api-docs/commerce/notification/overview.html">Notification API overview</see> for more information.
    /// </summary>
    public Task CreateDestinationAsync(
        DestinationRequest body,
        string? accessToken = null,
        RequestOptions? requestOptions = null,
        CancellationToken ct = default)
        => RestInvocation.CallAsync(
            _tokenContext,
            accessToken,
            () => _kiota.Destination.PostAsync(body, cfg =>
        {
            RestInvocation.ApplyHeaders(cfg.Headers, _config, requestOptions);
        }, ct));

    /// <summary>
    /// This method allows applications to fetch the details for a destination. The details include the destination name, status, and configuration, including the endpoint and verification token.
    /// </summary>
    public Task<Destination?> GetDestinationAsync(
        string destinationId,
        string? accessToken = null,
        RequestOptions? requestOptions = null,
        CancellationToken ct = default)
        => RestInvocation.CallAsync(
            _tokenContext,
            accessToken,
            () => _kiota.Destination[destinationId].GetAsync(cfg =>
        {
            RestInvocation.ApplyHeaders(cfg.Headers, _config, requestOptions);
        }, ct));

    /// <summary>
    /// This method allows applications to update a destination.
    /// <b>Note:</b> The destination should be created and ready to respond with the expected <b>challengeResponse</b> for the endpoint to be registered successfully. Refer to the <see href="/api-docs/commerce/notification/overview.html">Notification API overview</see> for more information.
    /// </summary>
    public Task UpdateDestinationAsync(
        string destinationId,
        DestinationRequest body,
        string? accessToken = null,
        RequestOptions? requestOptions = null,
        CancellationToken ct = default)
        => RestInvocation.CallAsync(
            _tokenContext,
            accessToken,
            () => _kiota.Destination[destinationId].PutAsync(body, cfg =>
        {
            RestInvocation.ApplyHeaders(cfg.Headers, _config, requestOptions);
        }, ct));

    /// <summary>
    /// This method provides applications a way to delete a destination.
    /// The same destination ID can be used by many destinations.
    /// Trying to delete an active destination results in an error. You can disable a subscription, and when the destination is no longer in use, you can delete it.
    /// </summary>
    public Task DeleteDestinationAsync(
        string destinationId,
        string? accessToken = null,
        RequestOptions? requestOptions = null,
        CancellationToken ct = default)
        => RestInvocation.CallAsync(
            _tokenContext,
            accessToken,
            () => _kiota.Destination[destinationId].DeleteAsync(cfg =>
        {
            RestInvocation.ApplyHeaders(cfg.Headers, _config, requestOptions);
        }, ct));

    /// <summary>
    /// This method allows users to retrieve a public key using a specified key ID. The public key that is returned in the response payload is used to process and validate eBay notifications.
    /// The public key ID, which is a required request parameter for this method, is retrieved from the Base64-encoded <b>X-EBAY-SIGNATURE</b> header that is included in the eBay notification.
    /// Important! </b>"&gt;<b>Important!</b> The retrieved public key value should be cached for a temporary — but reasonable — amount of time (e.g., one-hour is recommended.) This key should not be requested for every notification since doing so can result in exceeding <see href="/develop/apis/api-call-limits">API call limits</see> if a large number of notification requests is received.
    /// <b>Note:</b> For more details about how to process eBay push notifications and validate notification message payloads, see the <see href="/api-docs/commerce/notification/overview.html">Notification API overview</see>.
    /// </summary>
    public Task<PublicKey?> GetPublicKeyAsync(
        string publicKeyId,
        string? accessToken = null,
        RequestOptions? requestOptions = null,
        CancellationToken ct = default)
        => RestInvocation.CallAsync(
            _tokenContext,
            accessToken,
            () => _kiota.Public_key[publicKeyId].GetAsync(cfg =>
        {
            RestInvocation.ApplyHeaders(cfg.Headers, _config, requestOptions);
        }, ct));

    /// <summary>
    /// This method allows applications to retrieve a list of all subscriptions. The list returned is a paginated collection of subscription resources.
    /// Subscriptions allow applications to express interest in notifications and keep receiving the information relevant to their business.
    /// </summary>
    public Task<SubscriptionSearchResponse?> GetSubscriptionsAsync(
        string? continuationToken = null,
        string? limit = null,
        string? accessToken = null,
        RequestOptions? requestOptions = null,
        CancellationToken ct = default)
        => RestInvocation.CallAsync(
            _tokenContext,
            accessToken,
            () => _kiota.Subscription.GetAsync(cfg =>
        {
            if (continuationToken is not null)
            {
                cfg.QueryParameters.ContinuationToken = continuationToken;
            }
            if (limit is not null)
            {
                cfg.QueryParameters.Limit = limit;
            }
            RestInvocation.ApplyHeaders(cfg.Headers, _config, requestOptions);
        }, ct));

    /// <summary>
    /// This method allows applications to create a subscription for a topic and supported schema version. Subscriptions allow applications to express interest in notifications and keep receiving the information relevant to their business.
    /// Each application and topic-schema pairing to a subscription should have a 1:1 cardinality.
    /// You can create the subscription in disabled mode, test it (see the <b>test</b> method), and when everything is ready, you can enable the subscription (see the <b>enableSubscription</b> method).
    /// <b>Note:</b> If an application is not authorized to subscribe to a topic, for example, if your authorization does not include the list of scopes required for the topic, an error code of 195011 is returned.
    /// </summary>
    public Task CreateSubscriptionAsync(
        CreateSubscriptionRequest body,
        string? accessToken = null,
        RequestOptions? requestOptions = null,
        CancellationToken ct = default)
        => RestInvocation.CallAsync(
            _tokenContext,
            accessToken,
            () => _kiota.Subscription.PostAsync(body, cfg =>
        {
            RestInvocation.ApplyHeaders(cfg.Headers, _config, requestOptions);
        }, ct));

    /// <summary>
    /// This method allows applications to create a filter for a subscription. Filters allow applications to only be sent notifications that match a provided criteria. Notifications that do not match this criteria will not be sent to the destination.
    /// The <b>filterSchema</b> value must be a valid <see href="https://json-schema.org">JSON Schema Core document</see> (version 2020-12 or later). The <b>filterSchema</b> provided must describe the subscription's notification payload such that it supplies valid criteria to filter the subscription's notifications. The user does not need to provide <c>$schema</c> and <c>$id</c> definitions.
    /// When a filter is first created, it is not immediately active on the subscription. If the request has a valid JSON body, the successful call returns the HTTP status code <b>201 Created</b>. Newly created filters are in <c>PENDING</c> status until they are reviewed. If a filter is valid, it will move from <c>PENDING</c> status to <c>ENABLED</c> status. You can find the status of a filter using the <see href="/api-docs/commerce/notification/resources/subscription/methods/getSubscriptionFilter">getSubscriptionFilter</see> method. See <see href="/api-docs/commerce/notification/overview.html#create-filter">Creating a subscription filter for a topic</see> for additional information.
    /// <b>Note:</b> Only one filter can be in <b>ENABLED</b> (which means active) status on a subscription at a time. If an <b>ENABLED</b> filter is overwritten by a new call to <b>CREATE</b> a filter for the subscription, it stays in <b>ENABLED</b> status until the new <b>PENDING</b> filter becomes the <b>ENABLED</b> filter, and the existing filter then becomes <b>DISABLED</b>.
    /// </summary>
    public Task CreateSubscriptionFilterAsync(
        string subscriptionId,
        CreateSubscriptionFilterRequest body,
        string? accessToken = null,
        RequestOptions? requestOptions = null,
        CancellationToken ct = default)
        => RestInvocation.CallAsync(
            _tokenContext,
            accessToken,
            () => _kiota.Subscription[subscriptionId].Filter.PostAsync(body, cfg =>
        {
            RestInvocation.ApplyHeaders(cfg.Headers, _config, requestOptions);
        }, ct));

    /// <summary>
    /// This method allows applications to retrieve subscription details for the specified subscription.
    /// Specify the subscription to retrieve using the <b>subscription_id</b>. Use the <b>getSubscriptions</b> method to browse all subscriptions if you do not know the <b>subscription_id</b>.
    /// Subscriptions allow applications to express interest in notifications and keep receiving the information relevant to their business.
    /// </summary>
    public Task<Subscription?> GetSubscriptionAsync(
        string subscriptionId,
        string? accessToken = null,
        RequestOptions? requestOptions = null,
        CancellationToken ct = default)
        => RestInvocation.CallAsync(
            _tokenContext,
            accessToken,
            () => _kiota.Subscription[subscriptionId].GetAsync(cfg =>
        {
            RestInvocation.ApplyHeaders(cfg.Headers, _config, requestOptions);
        }, ct));

    /// <summary>
    /// This method allows applications to update a subscription. Subscriptions allow applications to express interest in notifications and keep receiving the information relevant to their business.
    /// <b>Note:</b> This call returns an error if an application is not authorized to subscribe to a topic.
    /// You can pause and restart a subscription. See the <b>disableSubscription</b> and <b>enableSubscription</b> methods.
    /// </summary>
    public Task UpdateSubscriptionAsync(
        string subscriptionId,
        UpdateSubscriptionRequest body,
        string? accessToken = null,
        RequestOptions? requestOptions = null,
        CancellationToken ct = default)
        => RestInvocation.CallAsync(
            _tokenContext,
            accessToken,
            () => _kiota.Subscription[subscriptionId].PutAsync(body, cfg =>
        {
            RestInvocation.ApplyHeaders(cfg.Headers, _config, requestOptions);
        }, ct));

    /// <summary>
    /// This method allows applications to delete a subscription. Subscriptions can be deleted regardless of status.
    /// </summary>
    public Task DeleteSubscriptionAsync(
        string subscriptionId,
        string? accessToken = null,
        RequestOptions? requestOptions = null,
        CancellationToken ct = default)
        => RestInvocation.CallAsync(
            _tokenContext,
            accessToken,
            () => _kiota.Subscription[subscriptionId].DeleteAsync(cfg =>
        {
            RestInvocation.ApplyHeaders(cfg.Headers, _config, requestOptions);
        }, ct));

    /// <summary>
    /// This method allows applications to retrieve the filter details for the specified subscription filter.
    /// Specify the subscription filter to retrieve by using the <b>subscription_id</b> and the <b>filter_id</b> associated with the subscription filter. The <b>filter_id</b> can be found in the response body for the <b>getSubscription</b> method, if there is a filter applied on the subscription.
    /// Filters allow applications to only be sent notifications that match a provided criteria. Notifications that do not match this criteria will not be sent to the destination.
    /// </summary>
    public Task<SubscriptionFilter?> GetSubscriptionFilterAsync(
        string filterId,
        string subscriptionId,
        string? accessToken = null,
        RequestOptions? requestOptions = null,
        CancellationToken ct = default)
        => RestInvocation.CallAsync(
            _tokenContext,
            accessToken,
            () => _kiota.Subscription[subscriptionId].Filter[filterId].GetAsync(cfg =>
        {
            RestInvocation.ApplyHeaders(cfg.Headers, _config, requestOptions);
        }, ct));

    /// <summary>
    /// This method allows applications to disable the active filter on a subscription, so that a new subscription filter may be added.
    /// <b>Note:</b> Subscription filters in <b>PENDING</b> status can not be disabled. However, a new filter can be created instead with the <b>createSubscriptionFilter</b> method and this new filter will override the <b>PENDING</b> filter.
    /// </summary>
    public Task DeleteSubscriptionFilterAsync(
        string filterId,
        string subscriptionId,
        string? accessToken = null,
        RequestOptions? requestOptions = null,
        CancellationToken ct = default)
        => RestInvocation.CallAsync(
            _tokenContext,
            accessToken,
            () => _kiota.Subscription[subscriptionId].Filter[filterId].DeleteAsync(cfg =>
        {
            RestInvocation.ApplyHeaders(cfg.Headers, _config, requestOptions);
        }, ct));

    /// <summary>
    /// This method disables a subscription, which prevents the subscription from providing notifications. To restart a subscription, call <b>enableSubscription</b>.
    /// </summary>
    public Task DisableSubscriptionAsync(
        string subscriptionId,
        string? accessToken = null,
        RequestOptions? requestOptions = null,
        CancellationToken ct = default)
        => RestInvocation.CallAsync(
            _tokenContext,
            accessToken,
            () => _kiota.Subscription[subscriptionId].Disable.PostAsync(cfg =>
        {
            RestInvocation.ApplyHeaders(cfg.Headers, _config, requestOptions);
        }, ct));

    /// <summary>
    /// This method allows applications to enable a disabled subscription. To pause (or disable) an enabled subscription, call <b>disableSubscription</b>.
    /// </summary>
    public Task EnableSubscriptionAsync(
        string subscriptionId,
        string? accessToken = null,
        RequestOptions? requestOptions = null,
        CancellationToken ct = default)
        => RestInvocation.CallAsync(
            _tokenContext,
            accessToken,
            () => _kiota.Subscription[subscriptionId].Enable.PostAsync(cfg =>
        {
            RestInvocation.ApplyHeaders(cfg.Headers, _config, requestOptions);
        }, ct));

    /// <summary>
    /// This method triggers a mocked test payload that includes a notification ID, publish date, and so on. Use this method to test your subscription end-to-end.
    /// You can create the subscription in disabled mode, test it using this method, and when everything is ready, you can enable the subscription (see the <b>enableSubscription</b> method).
    /// <b>Note:</b> Use the <b>notificationId</b> to tell the difference between a test payload and a real payload.
    /// </summary>
    public Task TestSubscriptionAsync(
        string subscriptionId,
        string? accessToken = null,
        RequestOptions? requestOptions = null,
        CancellationToken ct = default)
        => RestInvocation.CallAsync(
            _tokenContext,
            accessToken,
            () => _kiota.Subscription[subscriptionId].Test.PostAsync(cfg =>
        {
            RestInvocation.ApplyHeaders(cfg.Headers, _config, requestOptions);
        }, ct));

    /// <summary>
    /// This method allows applications to retrieve details for the specified topic. This information includes supported schema versions, formats, and other metadata for the topic.
    /// Applications can subscribe to any of the topics for a supported schema version and format, limited by the authorization scopes required to subscribe to the topic.
    /// A topic specifies the type of information to be received and the data types associated with an event. An event occurs in the eBay system, such as when a user requests deletion or revokes access for an application. An event is an instance of an event type (topic).
    /// Specify the topic to retrieve using the <b>topic_id</b> URI parameter.
    /// <b>Note:</b> Use the <see href="/api-docs/commerce/notification/resources/topic/methods/getTopics">getTopics</see> method to find a topic if you do not know the topic ID.
    /// </summary>
    public Task<Topic?> GetTopicAsync(
        string topicId,
        string? accessToken = null,
        RequestOptions? requestOptions = null,
        CancellationToken ct = default)
        => RestInvocation.CallAsync(
            _tokenContext,
            accessToken,
            () => _kiota.Topic[topicId].GetAsync(cfg =>
        {
            RestInvocation.ApplyHeaders(cfg.Headers, _config, requestOptions);
        }, ct));

    /// <summary>
    /// This method returns a paginated collection of all supported topics, along with the details for the topics. This information includes supported schema versions, formats, and other metadata for the topics.
    /// Applications can subscribe to any of the topics for a supported schema version and format, limited by the authorization scopes required to subscribe to the topic.
    /// A topic specifies the type of information to be received and the data types associated with an event. An event occurs in the eBay system, such as when a user requests deletion or revokes access for an application. An event is an instance of an event type (topic).
    /// </summary>
    public Task<TopicSearchResponse?> GetTopicsAsync(
        string? continuationToken = null,
        string? limit = null,
        string? accessToken = null,
        RequestOptions? requestOptions = null,
        CancellationToken ct = default)
        => RestInvocation.CallAsync(
            _tokenContext,
            accessToken,
            () => _kiota.Topic.GetAsync(cfg =>
        {
            if (continuationToken is not null)
            {
                cfg.QueryParameters.ContinuationToken = continuationToken;
            }
            if (limit is not null)
            {
                cfg.QueryParameters.Limit = limit;
            }
            RestInvocation.ApplyHeaders(cfg.Headers, _config, requestOptions);
        }, ct));
}
