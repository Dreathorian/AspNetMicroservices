using System.Net;
using Microsoft.AspNetCore.Mvc;
using Shopping.Aggregator.Models;
using Shopping.Aggregator.Services;

namespace Shopping.Aggregator.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class ShoppingController : ControllerBase
{
    private readonly ICatalogService _catalogService;
    private readonly IBasketService _basketService;
    private readonly IOrderService _orderService;

    public ShoppingController(ICatalogService catalogService, IBasketService basketService, IOrderService orderService)
    {
        _catalogService = catalogService ?? throw new ArgumentNullException(nameof(catalogService));
        _basketService = basketService ?? throw new ArgumentNullException(nameof(basketService));
        _orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
    }

    [HttpGet("{userName}", Name = "GetShopping")]
    [ProducesResponseType(typeof(ShoppingModel), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<ShoppingModel>> GetShopping(string userName)
    {
        var basket = await _basketService.GetBasket(userName);
        if (basket == null) return NotFound(userName);

        foreach (var item in basket.Items)
        {
            var product = await _catalogService.GetCatalog(item.ProductId);
            if (product == null) return NotFound(nameof(product));

            item.ProductName = product.Name;
            item.Category = product.Name;
            item.Summary = product.Name;
            item.Description = product.Name;
            item.ImageFile = product.Name;
        }

        var orders = await _orderService.GetOrderByUserName(userName);
        if (orders == null) return NotFound(nameof(orders));

        var shoppingModel = new ShoppingModel
        {
            UserName = userName,
            BasketWithProducts = basket,
            Orders = orders,
        };

        return Ok(shoppingModel);
    }


}
