using Microsoft.AspNetCore.HttpOverrides;

namespace RPPP_WebApp;

public static class StartupExtensions
{
  public static WebApplication ConfigureServices(this WebApplicationBuilder builder)
  {
    builder.Services.AddControllersWithViews();
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