using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EBay.Communication;
using EBay.Communication.Configuration;
using EBay.Communication.Exceptions;

namespace EBay.Communication.CSharp73
{
    /// <summary>Typical consumer code written in C# 7.3; it only has to compile.</summary>
    public static class ConsumerUsage
    {
        public static async Task<string> RunAsync()
        {
            var config = new CommunicationConfiguration
            {
                AccessToken = "token",
                HttpTimeout = TimeSpan.FromSeconds(60),
                DiagnosticLogging = true,
                FeedbackBaseUrl = "https://ebay-proxy.example.com/commerce/feedback/v1",
                AllowedHosts = { "ebay-proxy.example.com" },
                DefaultRequestOptions = new RequestOptions
                {
                    Headers = new Dictionary<string, string> { { "X-EBAY-C-MARKETPLACE-ID", "EBAY_US" } }
                }
            };

            using (var client = new CommunicationClient(config))
            {
                try
                {
                    var result = await client.Feedback.GetItemsAwaitingFeedbackAsync();
                    return result != null ? "ok" : "empty";
                }
                catch (UnauthorizedException)
                {
                    return "unauthorized";
                }
            }
        }
    }
}
