using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Ordering.Infrastructure.Persistence;
using Polly;

namespace Ordering.API.Extensions;

public static class AppExtensions
{
    public static void MigrateDatabase<TContext>(this WebApplication app,
        Action<TContext, IServiceProvider> seeder) where TContext : DbContext
    {
        using var scope = app.Services.CreateScope();

        var services = scope.ServiceProvider;
        var logger = services.GetRequiredService<ILogger<TContext>>();
        var context = services.GetRequiredService<TContext>();

        try
        {
            logger.LogInformation("Migrating database associated with context {DbContextName}", typeof(TContext).Name);

            var retry = Policy.Handle<SqlException>()
                .WaitAndRetry(
                    retryCount: 5,
                    sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), // 2,4,8,16,32 sc
                    onRetry: (exception, retryCount, context) =>
                    {
                        logger.LogError("Retry {RetryCount} of {ContextPolicyKey} at {ContextOperationKey}, due to: {Exception}",
                            retryCount, context.PolicyKey, context.OperationKey, exception);
                    });

            //if the sql server container is not created on run docker compose this
            //migration can't fail for network related exception. The retry options for DbContext only 
            //apply to transient exceptions  

            retry.Execute(() =>
            {
                context.Database.Migrate();
                seeder(context, services);
            });

            logger.LogInformation("Migrated database associated with context {DbContextName}", typeof(TContext).Name);
        }
        catch (SqlException ex)
        {
            logger.LogError(ex, "An error occurred while migrating the database used on context {DbContextName}",
                typeof(TContext).Name);
        }
    }

}
