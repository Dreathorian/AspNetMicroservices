using Shopping.Aggregator.Extensions;
using Shopping.Aggregator.Models;

namespace Shopping.Aggregator.Services;

class CatalogService : ICatalogService
{
    private readonly HttpClient _client;

    public CatalogService(HttpClient client) => _client = client ?? throw new ArgumentNullException(nameof(client));

    public async Task<List<CatalogModel>?> GetCatalog()
    {
        var response = await _client.GetAsync("/api/v1/Catalog");
        return await response.ReadContentAs<List<CatalogModel>>();
    }

    public async Task<CatalogModel?> GetCatalog(string id)
    {
        var response = await _client.GetAsync($"/api/v1/Catalog/{id}");
        return await response.ReadContentAs<CatalogModel>();
    }

    public async Task<List<CatalogModel>?> GetCatalogByCategory(string category)
    {
        var response = await _client.GetAsync($"api/v1/Catalog/GetProductByCategory/{category}");
        return await response.ReadContentAs<List<CatalogModel>>();
    }
}
