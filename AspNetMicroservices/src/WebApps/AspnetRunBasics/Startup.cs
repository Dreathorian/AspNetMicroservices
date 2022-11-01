using System;
using AspnetRunBasics.Services;
using Common.Logging;
using HealthChecks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Pollycies;

namespace AspnetRunBasics
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpClient<ICatalogService, CatalogService>(c =>
                    c.BaseAddress = new Uri(Configuration["ApiSettings:GatewayAddress"]))
                .AddDefaultPollycies();

            services.AddHttpClient<IBasketService, BasketService>(c =>
                    c.BaseAddress = new Uri(Configuration["ApiSettings:GatewayAddress"]))
                .AddDefaultPollycies();

            services.AddHttpClient<IOrderService, OrderService>(c =>
                    c.BaseAddress = new Uri(Configuration["ApiSettings:GatewayAddress"]))
                .AddDefaultPollycies();

            services.AddRazorPages();

            services.AddHealthChecks()
                .AddUrlGroup(new Uri(Configuration["ApiSettings:GatewayAddress"]), "Ocelot API Gw", HealthStatus.Degraded);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapHealthChecksDefault();
            });
        }
    }
}
