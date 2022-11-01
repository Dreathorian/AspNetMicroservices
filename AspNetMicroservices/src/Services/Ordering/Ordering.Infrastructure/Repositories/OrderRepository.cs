using Microsoft.EntityFrameworkCore;
using Ordering.Application.Contracts.Persistence;
using Ordering.Domain.Entities;
using Ordering.Infrastructure.Persistence;

namespace Ordering.Infrastructure.Repositories;

public class OrderRepository : RepositoryBase<Order>, IOrderRepository
{
    private readonly OrderContext _context;

    public OrderRepository(OrderContext context) : base(context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Order>> GetOrdersByUserName(string userName)
    {
        return await _context.Orders
            .Where(o => o.UserName == userName)
            .ToListAsync();
    }
}
