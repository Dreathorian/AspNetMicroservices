using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Routing;

namespace HealthChecks;

public static class HealthCheckExtensions
{
    public static IEndpointConventionBuilder MapHealthChecksDefault(this IEndpointRouteBuilder builder) =>
        builder.MapHealthChecks("/hc", new HealthCheckOptions
        {
            Predicate = _ => true,
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse,
        });


}
