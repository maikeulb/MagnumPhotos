using System;
using MagnumPhotos.API.Data.Context;
using MagnumPhotos.API.Data.Seed;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Web;

namespace MagnumPhotos.API
{
    public class Program
    {
        public static void Main (string[] args)
        {
            var host = BuildWebHost (args);
            using (var scope = host.Services.CreateScope ())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var magnumPhotosContext = services.GetRequiredService<MagnumPhotosContext> ();
                    var magnumPhotosContextInitializerLogger = services.GetRequiredService<ILogger<MagnumPhotosContextInitializer>> ();
                    MagnumPhotosContextInitializer.Initialize (magnumPhotosContext, magnumPhotosContextInitializerLogger).Wait ();
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>> ();
                    logger.LogError (ex, "An error occurred while seeding the database.");
                }
            }
            host.Run ();
        }

        public static IWebHost BuildWebHost (string[] args) =>
            WebHost.CreateDefaultBuilder (args)
            .UseStartup<Startup> ()
            .UseNLog ()
            .Build ();
    }
}
