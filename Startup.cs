using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WebApplication2.Data;
using Microsoft.EntityFrameworkCore;
using WebApplication2.Services;
using Microsoft.OpenApi.Models;
using System;
using System.Reflection;
using System.IO;

namespace WebApplication2
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
            services.AddControllers();

            //services.AddSwaggerGen();
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("ExchangeRates", new OpenApiInfo
                {
                    Version = "ExchangeRates",
                    Title = "API по работе с курсами валют",
                    Description = "API предоставляет возможность сохранять курсы валют за период, " +
                    "сохранять актуальные курсы с заданной периодичностью и получать статистику по курсам",
                    Contact = new OpenApiContact
                    {
                        Name = "Samoilov",
                        Url = new Uri("https://github.com/smylebifa")
                    }
                });

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                options.IncludeXmlComments(xmlPath);
            }
            );

            string mySqlConnectionStr = Configuration.GetConnectionString("Default");

            services.AddDbContext<ExchangeRatesDbContext>(options => options.UseMySql(mySqlConnectionStr,
                ServerVersion.AutoDetect(mySqlConnectionStr)));

            services.AddTransient<ExchangeRatesService>();
            services.AddTransient<ParseExchangeRatesService>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/ExchangeRates/swagger.json", "ExchangeRates");
            });

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
           
        }
    }
}
