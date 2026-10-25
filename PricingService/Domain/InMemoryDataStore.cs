using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PricingService.Domain;

public sealed class InMemoryDataStore : IDataStore
{
    public InMemoryDataStore()
    {
        Tariffs = new InMemoryTariffRepository(DefaultTariffs.Create());
    }

    public ITariffRepository Tariffs { get; }

    public void Commit()
    {
    }
}

internal sealed class InMemoryTariffRepository : ITariffRepository
{
    private readonly Dictionary<string, Tariff> tariffs;

    internal InMemoryTariffRepository(IEnumerable<Tariff> tariffs)
    {
        this.tariffs = tariffs.ToDictionary(tariff => tariff.Code);
    }

    public Task<Tariff?> this[string code] =>
        Task.FromResult(tariffs.TryGetValue(code, out var tariff) ? tariff : null);

    public void Add(Tariff tariff)
    {
        tariffs.Add(tariff.Code, tariff);
    }

    public Task<bool> Exists(string code)
    {
        return Task.FromResult(tariffs.ContainsKey(code));
    }
}
