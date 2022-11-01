using AutoMapper;
using MediatR;
using Ordering.Application.Contracts.Persistence;

namespace Ordering.Application.Features.Orders.Queries.GetOrdersList;

public class GetOrderListQueryHandler : IRequestHandler<GetOrdersListQuery, List<OrderDto>>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IMapper _mapper;

    public GetOrderListQueryHandler(IOrderRepository orderRepository, IMapper mapper)
    {
        _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

    }

    public async Task<List<OrderDto>> Handle(GetOrdersListQuery request, CancellationToken cancellationToken)
    {
        var orders = await _orderRepository.GetOrdersByUserName(request.UserName);
        return _mapper.Map<List<OrderDto>>(orders);
    }
}
