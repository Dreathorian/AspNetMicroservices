using Catalog.API.Entities;
using MongoDB.Driver.Linq;

namespace Catalog.API.Repositories;

public interface IProductRepository
{
    IMongoQueryable<Product> GetAll();

    Task<Product> GetById(string id);

    IMongoQueryable<Product> GetByName(string name);

    IMongoQueryable<Product> GetByCategory(string cateogryName);

    Task CreateProduct(Product product);

    Task<bool> UpdateProduct(Product product);

    Task<bool> DeleteProduct(string id);
}