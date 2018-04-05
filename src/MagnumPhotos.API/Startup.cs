using AspNetCoreRateLimit;
using FluentValidation.AspNetCore;
using MagnumPhotos.API.Data.Context;
using MagnumPhotos.API.Helpers;
using MagnumPhotos.API.Services;
using MagnumPhotos.API.Services.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Serialization;
using Swashbuckle.AspNetCore.Swagger;

namespace MagnumPhotos.API
{
    public class Startup
    {
        public Startup (IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices (IServiceCollection services)
        {
            services.AddMvc (setup =>
                {
                    setup.ReturnHttpNotAcceptable = true;
                })
                .AddJsonOptions (options =>
                {
                    options.SerializerSettings.ContractResolver =
                        new CamelCasePropertyNamesContractResolver ();
                })
                .AddFluentValidation (options => { options.RegisterValidatorsFromAssemblyContaining<Startup> (); });

            services.AddResponseCaching ();

            services.AddSwaggerGen (options =>
            {
                options.SwaggerDoc ("v1", new Info { Title = "Magnum Photos API", Version = "v1" });
            });

            services.AddDbContext<MagnumPhotosContext> (options =>
                options.UseNpgsql (Configuration.GetConnectionString ("MagnumPhotosApi")));

            services.AddScoped<IMagnumPhotosRepository, MagnumPhotosRepository> ();

            services.AddSingleton<IActionContextAccessor, ActionContextAccessor> ();

            services.AddScoped<IUrlHelper> (implementationFactory =>
            {
                var actionContext = implementationFactory.GetService<IActionContextAccessor> ()
                    .ActionContext;
                return new UrlHelper (actionContext);
            });

            services.AddTransient<IPropertyMappingService, PropertyMappingService> ();

            services.AddHttpCacheHeaders (
                (expirationModelOptions) =>
                {
                    expirationModelOptions.MaxAge = 600;
                },
                (validationModelOptions) =>
                {
                    validationModelOptions.AddMustRevalidate = true;
                });

            services.AddMemoryCache ();

            services.Configure<IpRateLimitOptions> ((options) =>
            {
                options.GeneralRules = new System.Collections.Generic.List<RateLimitRule> ()
                {
                new RateLimitRule ()
                {
                Endpoint = "*",
                Limit = 1000,
                Period = "5m"
                },
                new RateLimitRule ()
                {
                Endpoint = "*",
                Limit = 200,
                Period = "10s"
                }
                };
            });

            services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore> ();

            services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore> ();
        }

        public void Configure (IApplicationBuilder app, IHostingEnvironment env,
            ILoggerFactory loggerFactory, MagnumPhotosContext magnumPhotosContext)
        {
            if (env.IsDevelopment ())
            {
                app.UseDeveloperExceptionPage ();
            }
            else
            {
                app.UseExceptionHandler (appBuilder =>
                {
                    appBuilder.Run (async context =>
                    {
                        var exceptionHandlerFeature = context.Features.Get<IExceptionHandlerFeature> ();
                        if (exceptionHandlerFeature != null)
                        {
                            var logger = loggerFactory.CreateLogger ("Global exception logger");
                            logger.LogError (500,
                                exceptionHandlerFeature.Error,
                                exceptionHandlerFeature.Error.Message);
                        }
                        context.Response.StatusCode = 500;
                        await context.Response.WriteAsync ("An unexpected fault happened. Try again later.");
                    });
                });
            }

            AutoMapper.Mapper.Initialize (options =>
            {
                options.CreateMap<Entities.Photographer, Models.PhotographerDto> ()
                    .ForMember (dest => dest.Name, opt => opt.MapFrom (src =>
                        $"{src.FirstName} {src.LastName}"))
                    .ForMember (dest => dest.Age, opt => opt.MapFrom (src =>
                        src.DateOfBirth.GetCurrentAge ()));
                options.CreateMap<Entities.Book, Models.BookDto> ();
                options.CreateMap<Entities.Book, Models.BookForUpdateDto> ();
                options.CreateMap<Entities.Photographer, Models.PhotographerForUpdateDto> ();
                options.CreateMap<Models.PhotographerForCreationDto, Entities.Photographer> ();
                options.CreateMap<Models.PhotographerForUpdateDto, Entities.Photographer> ();
                options.CreateMap<Models.BookForCreationDto, Entities.Book> ();
                options.CreateMap<Models.BookForUpdateDto, Entities.Book> ();
            });

            app.UseIpRateLimiting ();

            app.UseResponseCaching ();

            app.UseHttpCacheHeaders ();

            app.UseMvc ();

            app.UseSwagger (c =>
            {
                c.RouteTemplate = "swagger/{documentName}/swagger.json";
            });

            app.UseSwaggerUI (c =>
            {
                c.SwaggerEndpoint ("/swagger/v1/swagger.json", "Magnum Photos API V1");
            });
        }
    }
}