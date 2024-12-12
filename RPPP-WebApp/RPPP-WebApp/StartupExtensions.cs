using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RPPP_WebApp.Models;

namespace RPPP_WebApp;

public static class StartupExtensions
{
  public static WebApplication ConfigureServices(this WebApplicationBuilder builder)
  {
        IConfiguration configuration = new ConfigurationBuilder()
        .AddJsonFile("appsettings.json")
        .AddUserSecrets<Program>()
         .Build();

        IServiceCollection services = new ServiceCollection();
        var provider = services
        .AddDbContext<RPPP08Context>(options => {
            options.UseSqlServer(
            configuration.GetConnectionString("Server"));
        }, contextLifetime: ServiceLifetime.Transient)
        .BuildServiceProvider();

        builder.Services.AddControllersWithViews();


        var serviceProvider = services.AddDbContext<RPPP08Context>().BuildServiceProvider();



        return builder.Build();
  }

  public static WebApplication ConfigurePipeline(this WebApplication app)
  {
    #region Needed for nginx and Kestrel (do not remove or change this region)
    app.UseForwardedHeaders(new ForwardedHeadersOptions
    {
      ForwardedHeaders = ForwardedHeaders.XForwardedFor |
                         ForwardedHeaders.XForwardedProto
    });
    string pathBase = app.Configuration["PathBase"];
    if (!string.IsNullOrWhiteSpace(pathBase))
    {
      app.UsePathBase(pathBase);
    }
    #endregion

    if (app.Environment.IsDevelopment())
    {
      app.UseDeveloperExceptionPage();
    }

    app.UseStaticFiles()
       .UseRouting();

    app.MapDefaultControllerRoute();

    return app;
  }
}