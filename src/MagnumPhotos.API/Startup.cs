using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MagnumPhotos.API.Services.Interfaces;
using MagnumPhotos.API.Services;
using MagnumPhotos.API.Entities;
using MagnumPhotos.API.Data.Seed;
using MagnumPhotos.API.Data.Context;
using Microsoft.EntityFrameworkCore;

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

            services.AddDbContext<MagnumPhotosContext> (options =>
                options.UseNpgsql (Configuration.GetConnectionString ("MagnumPhotosApi")));

            services.AddTransient<ITypeHelperService, TypeHelperService>();

            services.AddScoped<IMagnumPhotosRepository, MagnumPhotosRepository>();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env,
            ILoggerFactory loggerFactory, MagnumPhotosContext magnumPhotosContext)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
        }
    }
}
