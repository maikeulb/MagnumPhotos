using FluentValidation.AspNetCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MagnumPhotos.API.Services.Interfaces;
using MagnumPhotos.API.Services;
using MagnumPhotos.API.Entities;
using MagnumPhotos.API.Helpers;
using MagnumPhotos.API.Data.Seed;
using MagnumPhotos.API.Data.Context;
using Newtonsoft.Json.Serialization;

namespace MagnumPhotos.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            .AddJsonOptions(options =>
            {
                options.SerializerSettings.ContractResolver =
                new CamelCasePropertyNamesContractResolver();
            });
            .AddFluentValidation (options => { options.RegisterValidatorsFromAssemblyContaining<Startup> (); });

            services.AddDbContext<MagnumPhotosContext> (options =>
                options.UseNpgsql (Configuration.GetConnectionString ("MagnumPhotosApi")));

            services.AddScoped<IMagnumPhotosRepository, MagnumPhotosRepository>();

            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();

            services.AddScoped<IUrlHelper>(implementationFactory =>
            {
                var actionContext = implementationFactory.GetService<IActionContextAccessor>()
                .ActionContext;
                return new UrlHelper(actionContext);
            });

            services.AddTransient<IPropertyMappingService, PropertyMappingService>();

            services.AddScoped<IUrlHelper, UrlHelper>(); */
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env,
            ILoggerFactory loggerFactory, MagnumPhotosContext magnumPhotosContext)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler(appBuilder =>
                {
                    appBuilder.Run(async context =>
                    {
                        var exceptionHandlerFeature = context.Features.Get<IExceptionHandlerFeature>();
                        if (exceptionHandlerFeature != null)
                        {
                            var logger = loggerFactory.CreateLogger("Global exception logger");
                            logger.LogError(500,
                                exceptionHandlerFeature.Error,
                                exceptionHandlerFeature.Error.Message);
                        }

                        context.Response.StatusCode = 500;
                        await context.Response.WriteAsync("An unexpected fault happened. Try again later.");

                    });                      
                });
            }

            AutoMapper.Mapper.Initialize(options =>
            {
                options.CreateMap<Entities.Photographer, Models.PhotographerDto>()
                    .ForMember(dest => dest.Name, opt => opt.MapFrom(src =>
                    $"{src.FirstName} {src.LastName}"))
                    .ForMember(dest => dest.Age, opt => opt.MapFrom(src =>
                    src.DateOfBirth.GetCurrentAge()));

                options.CreateMap<Entities.Book, Models.BookDto>();

                options.CreateMap<Models.PhotographerForCreationDto, Entities.Photographer>();

                options.CreateMap<Models.BookForCreationDto, Entities.Book>();

                options.CreateMap<Entities.Book, Models.BookForUpdateDto>();
            });

            app.UseMvc();
        }
    }
}
