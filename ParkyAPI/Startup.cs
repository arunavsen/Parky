using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using ParkyAPI.Data;
using ParkyAPI.ParkyMapper;
using ParkyAPI.Repository;
using ParkyAPI.Repository.IRepository;

namespace ParkyAPI
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
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            //We can now access the NationalParkRepository in any controllers
            services.AddScoped<INationalParkRepository, NationalParkRepository>();
            services.AddScoped<ITrailRepository, TrailRepository>();

            // All out mappings are in the ParkyMappings
            services.AddAutoMapper(typeof(ParkyMappings));
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("ParkyOpenAPISpecNP", new OpenApiInfo()
                {
                    Title = "Parky API (National Park)",
                    Version = "1",
                    Description = "Parky API of National Park",
                    Contact = new OpenApiContact()
                    {
                        Email = "arunavsen96@gmail.com",
                        Name = "Arunav Sen"
                    },
                    License = new OpenApiLicense()
                    {
                        Name = "ASP License"
                    }
                });

                options.SwaggerDoc("ParkyOpenAPISpecTrails", new OpenApiInfo()
                {
                    Title = "Parky API (Trails)",
                    Version = "1",
                    Description = "Parky API of Trails",
                    Contact = new OpenApiContact()
                    {
                        Email = "arunavsen96@gmail.com",
                        Name = "Arunav Sen"
                    },
                    License = new OpenApiLicense()
                    {
                        Name = "ASP License"
                    }
                });
                var xmlCommentFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var cmlCommentsFullPath = Path.Combine(AppContext.BaseDirectory, xmlCommentFile);
                options.IncludeXmlComments(cmlCommentsFullPath);
            });
            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseSwagger();

            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/ParkyOpenAPISpecNP/swagger.json", "Parky API National Park");
                options.SwaggerEndpoint("/swagger/ParkyOpenAPISpecTrails/swagger.json", "Parky API Trails");
                options.RoutePrefix = "";
            });

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
