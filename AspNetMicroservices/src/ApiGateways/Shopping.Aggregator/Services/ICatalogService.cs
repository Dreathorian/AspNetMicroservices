using Shopping.Aggregator.Models;

namespace Shopping.Aggregator.Services;

public interface ICatalogService
{
    Task<List<CatalogModel>?> GetCatalog();

    Task<CatalogModel?> GetCatalog(string id);

    Task<List<CatalogModel>?> GetCatalogByCategory(string category);
}