using EBay.Communication.Configuration;
using Xunit;

namespace EBay.Communication.Tests;

public sealed class ApiRosterTests
{
    [Fact]
    public void Negotiation_api_moved_to_the_offers_and_leads_sdk()
    {
        Assert.Null(typeof(CommunicationClient).GetProperty("Negotiation"));
        Assert.Null(typeof(CommunicationConfiguration).GetProperty("NegotiationBaseUrl"));
    }
}
