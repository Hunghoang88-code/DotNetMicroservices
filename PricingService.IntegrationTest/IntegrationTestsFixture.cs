using System.Threading.Tasks;
using Alba;
using Xunit;


namespace PricingService.IntegrationTest;

public class IntegrationTestsFixture : IAsyncLifetime
{
    public IAlbaHost SystemUnderTest { get; private set; }

    public async Task InitializeAsync()
    {
        SystemUnderTest = await AlbaHost.For<Program>();
    }

    public async Task DisposeAsync()
    {
        await SystemUnderTest.DisposeAsync();
    }
}