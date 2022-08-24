using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace Catalog.API.Entities;

public interface ICatalogContext
{
    IMongoCollection<Product> Products { get; }  
}