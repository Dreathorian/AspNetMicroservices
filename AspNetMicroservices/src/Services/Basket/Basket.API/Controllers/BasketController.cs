using System.Net;
using AutoMapper;
using Basket.API.Entities;
using Basket.API.GrpcServices;
using Basket.API.Repositories;
using EventBus.Messages.Events;
using MassTransit;
using Microsoft.AspNetCore.Mvc;

namespace Basket.API.Controllers;

[Controller]
[Route("/api/v1/basket")]
public class BasketController : ControllerBase
{
    private readonly IBasketRepository repo;
    private readonly DiscountGrpcService discountGrpcService;
    private readonly IMapper _mapper;
    private readonly IPublishEndpoint _publishEndpoint;

    public BasketController(IBasketRepository repo, DiscountGrpcService discountGrpcService, IMapper mapper,
        IPublishEndpoint _publishEndpoint)
    {
        this.repo = repo;
        this.discountGrpcService = discountGrpcService;
        _mapper = mapper;
        this._publishEndpoint = _publishEndpoint;
    }

    [HttpGet("{userName}", Name = nameof(GetBasket))]
    [ProducesResponseType(typeof(ShoppingCart), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<ShoppingCart>> GetBasket(string userName)
    {
        var basket = await repo.GetBasket(userName);
        return Ok(basket ?? new ShoppingCart(userName));
    }

    [HttpPost]
    [ProducesResponseType(typeof(ShoppingCart), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<ShoppingCart>> UpdateBasket([FromBody] ShoppingCart basket)
    {
        foreach (var item in basket.Items)
        {
            var coupon = await discountGrpcService.GetDiscount(item.ProductName);
            item.Price -= coupon.Amount;
        }
        return Ok(await repo.UpdateBasket(basket));
    }

    [HttpDelete("{userName}", Name = "DeleteBasket")]
    [ProducesResponseType(typeof(void), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> DeleteBasket(string userName)
    {
        await repo.DeleteBasket(userName);
        return Ok();
    }

    [HttpPost("[action]")]
    [ProducesResponseType((int)HttpStatusCode.Accepted)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> Checkout([FromBody] BasketCheckout basketCheckout)
    {
        var basket = await repo.GetBasket(basketCheckout.UserName);
        if (basket == null)
        {
            return BadRequest();
        }

        var eventMessage = _mapper.Map<BasketCheckoutEvent>(basketCheckout);
        eventMessage.TotalPrice = basket.TotalPrice;
        await _publishEndpoint.Publish(eventMessage);

        await repo.DeleteBasket(basket.UserName);

        return Accepted();
    }
}
