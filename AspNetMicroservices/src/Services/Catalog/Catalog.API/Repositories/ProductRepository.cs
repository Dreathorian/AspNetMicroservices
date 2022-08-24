using Catalog.API.Entities;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace Catalog.API.Repositories;

class ProductRepository : IProductRepository
{
    private readonly ICatalogContext context;

    public ProductRepository(ICatalogContext context) => this.context = context;

    public IMongoQueryable<Product> GetAll() => context.Products.AsQueryable();

    public Task<Product> GetById(string id) =>
        GetAll().FirstOrDefaultAsync(p => p.Id == id);

    public IMongoQueryable<Product> GetByName(string name) =>
        GetAll().Where(p => p.Name == name);

    public IMongoQueryable<Product> GetByCategory(string cateogryName) =>
        GetAll().Where(p => p.Category == cateogryName);

    public Task Create(Product product) =>
        context.Products.InsertOneAsync(product);

    public async Task<bool> Update(Product product)
    {
        var result = await context.Products.ReplaceOneAsync(p => p.Id == product.Id, product);
        return result.IsAcknowledged && result.ModifiedCount > 0;
    }

    public async Task<bool> Delete(string id)
    {
        var result = await context.Products.DeleteOneAsync(p => p.Id == id);
        return result.IsAcknowledged && result.DeletedCount > 0;
    }
}
