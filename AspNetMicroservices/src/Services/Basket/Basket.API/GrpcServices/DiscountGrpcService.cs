using Discount.Grpc.Protos;

namespace Basket.API.GrpcServices;

public class DiscountGrpcService
{
    private readonly DiscountProtoService.DiscountProtoServiceClient _discountClient;

    public DiscountGrpcService(DiscountProtoService.DiscountProtoServiceClient discountClient) => _discountClient = discountClient;
    
    public async Task<CouponModel> GetDiscount(string productName)
    {
        var discountRequest = new GetDiscountRequest { ProductName = productName };
        return await _discountClient.GetDiscountAsync(discountRequest);
    }
}
