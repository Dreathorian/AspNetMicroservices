using Shopping.Aggregator.Models;

namespace Shopping.Aggregator.Services;

public interface IOrderService
{
    Task<List<OrderResponseModel>?> GetOrderByUserName(string userName);
}