using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Ordering.Application.Contracts.Infrastructure;
using Ordering.Application.Contracts.Persistence;
using Ordering.Application.Models;
using Ordering.Domain.Entities;

namespace Ordering.Application.Features.Orders.Commands.CheckoutOrder;

public class CheckoutOrderCommandHandler : IRequestHandler<CheckoutOrderCommand, int>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IMapper _mapper;
    private readonly IEmailService _emailService;
    private readonly ILogger<CheckoutOrderCommandHandler> _logger;

    public CheckoutOrderCommandHandler(IOrderRepository orderRepository, IMapper mapper, IEmailService emailService,
        ILogger<CheckoutOrderCommandHandler> logger)
    {
        _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<int> Handle(CheckoutOrderCommand request, CancellationToken cancellationToken)
    {
        var order = _mapper.Map<Order>(request);
        order = await _orderRepository.AddAsync(order);

        _logger.LogInformation("Hoden");
        await SendMail(order);

        return order.Id;
    }

    private async Task SendMail(Order order)
    {
        var email = new Email { To = "ezozkme@gmail.com", Body = $"Order was created.", Subject = "Order was created" };

        try
        {
            await _emailService.SendEmail(email);
        }
        catch (Exception ex)
        {
            _logger.LogError("Order {OrderId} failed due to an error with the mail service: {ExMessage}", order.Id, ex.Message);
        }
    }
}
