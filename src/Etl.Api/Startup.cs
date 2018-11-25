﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Etl.Extract.Service;
using Etl.Logger;
using Etl.Transform.Service;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Etl.Api {
    public class Startup {
        public Startup (IConfiguration configuration) {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices (IServiceCollection services) {
            services.AddCors (o => o.AddPolicy ("CorsPolicy", builder => {
                builder
                    .AllowAnyMethod ()
                    .AllowAnyHeader ()
                    .AllowCredentials ()
                    .WithOrigins ("http://localhost:4200");
            }));

            services.AddSignalR ();
            services.AddScoped<ICustomLogger, CustomLogger> ();
            services.AddScoped<IExtractor, Extractor> ();
            services.AddScoped<ITransformer, Transformer> ();
            services.AddScoped<ICarModelExtractor, CarModelExtractor> ();
            services.AddMvc ().SetCompatibilityVersion (CompatibilityVersion.Version_2_1);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure (IApplicationBuilder app, IHostingEnvironment env) {
            if (env.IsDevelopment ()) {
                app.UseDeveloperExceptionPage ();
            } else {
                app.UseHsts ();
            }
            app.UseCors ("CorsPolicy");
            app.UseSignalR (routes => {
                routes.MapHub<LoggerHub> ("/logger");
            });
            app.UseHttpsRedirection ();
            app.UseMvc ();

            // using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory> ().CreateScope ()) {
            //     var context = serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext> ();
            //     context.Database.EnsureCreated ();
            // }
        }
    }
}