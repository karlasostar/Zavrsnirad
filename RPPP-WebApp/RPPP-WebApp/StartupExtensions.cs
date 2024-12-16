using FluentValidation.AspNetCore;
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
        builder.Services.AddDbContext<RPPP08Context>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("Server")));

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

        //unos podataka u bazu test

        //using (var scope = app.Services.CreateScope()) 
        //{
        //    var context = scope.ServiceProvider.GetRequiredService<RPPP08Context>();
        //    Dvorana dvorana = new Dvorana
        //    {
        //        OznDvorana = "D2",
        //        Kapacitet = 260
        //    };
        //    context.Dvoranas.Add(dvorana);
        //    context.SaveChanges();
        //}

        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseStaticFiles()
           .UseRouting();

        app.MapDefaultControllerRoute();

        app.MapControllerRoute(
            name: "dvorana",
            pattern: "{controller=Dvorana}/{action=Index}/{id?}");

        return app;
    }
}