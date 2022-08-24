using Basket.API.Entities;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace Basket.API.Repositories;

class BasketRepository : IBasketRepository
{
    private readonly IDistributedCache cache;

    public BasketRepository(IDistributedCache cache) =>
        this.cache = cache ?? throw new ArgumentNullException(nameof(cache));

    public async Task<ShoppingCart?> GetBasket(string userName)
    {
        var json = await cache.GetStringAsync(userName);

        return string.IsNullOrEmpty(json)
            ? null
            : JsonConvert.DeserializeObject<ShoppingCart>(json);
    }

    public async Task<ShoppingCart?> UpdateBasket(ShoppingCart basket)
    {
        await cache.SetStringAsync(basket.UserName, JsonConvert.SerializeObject(basket));
        return await GetBasket(basket.UserName);
    }

    public async Task DeleteBasket(string userName) => await cache.RemoveAsync(userName);
}
