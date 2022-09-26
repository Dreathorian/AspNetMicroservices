using Dapper;
using Discount.Grpc.Entities;
using Npgsql;

namespace Discount.Grpc.Repositories;

class DiscountRepository : IDiscountRepository
{

    private const string ConnectionStringName = "DatabaseSettings:ConnectionString";
    private readonly string connectionString;

    public DiscountRepository(IConfiguration configuration)
    {
        if (configuration == null) throw new ArgumentNullException(nameof(configuration));
        connectionString = configuration[ConnectionStringName];
    }
    public async Task<Coupon> GetDiscount(string productName)
    {
        await using var connection = new NpgsqlConnection(connectionString);
        var coupon = await connection.QueryFirstOrDefaultAsync<Coupon>("Select * from Coupon where ProductName = @ProductName",
            new { ProductName = productName });

        if (coupon == null)
            return new Coupon
                { ProductName = "No Discount", Amount = 0, Description = "No Discount Desc" };

        return coupon;
    }

    public async Task<bool> CreateDiscount(Coupon coupon)
    {
        await using var connection = new NpgsqlConnection(connectionString);

        var affected = await connection.ExecuteAsync
        ("INSERT INTO Coupon (ProductName, Description, Amount) VALUES (@ProductName, @Description, @Amount)",
            new
            {
                coupon.ProductName,
                coupon.Description,
                coupon.Amount,
            });

        return affected != 0;
    }

    public async Task<bool> UpdateDiscount(Coupon coupon)
    {
        await using var connection = new NpgsqlConnection(connectionString);

        var affected = await connection.ExecuteAsync
        ("UPDATE Coupon SET ProductName=@ProductName, Description = @Description, Amount = @Amount WHERE Id = @Id",
            new
            {
                coupon.ProductName,
                coupon.Description,
                coupon.Amount,
                coupon.Id
            });

        return affected != 0;

    }

    public async Task<bool> DeleteDiscount(string productName)
    {
        await using var connection = new NpgsqlConnection(connectionString);

        var affected = await connection.ExecuteAsync("DELETE FROM Coupon WHERE ProductName = @ProductName",
            new { ProductName = productName });

        return affected != 0;
    }
}
