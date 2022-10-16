using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using  Catalog.Api.Repositories;
using MongoDB.Driver;
using  Catalog.Api.Settings;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
namespace Catalog.Api
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
        {   BsonSerializer.RegisterSerializer(new GuidSerializer(BsonType.String));
            BsonSerializer.RegisterSerializer(new DateTimeOffsetSerializer(BsonType.String));
        var mongoDbSettings = Configuration.GetSection(nameof(MongoDBSettings)).Get<MongoDBSettings>();
            services.AddSingleton<IMongoClient>(ServiceProvider => {
            
            return new MongoClient(mongoDbSettings.ConnectionString);
        });

            services.AddSingleton<IInMemItemRepository,MongoDbRepo>();
            services.AddControllers(services =>{
                services.SuppressAsyncSuffixInActionNames = false;
            });
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Catalog", Version = "v1" });
            });
            services.AddHealthChecks()
            .AddMongoDb(mongoDbSettings.ConnectionString,name:"mongodb",timeout:TimeSpan.FromSeconds(3),
             tags:new[]{"ready"});
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Catalog v1"));
            }



            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHealthChecks("/health/ready",new HealthCheckOptions{
                    Predicate = (check) => check.Tags.Contains("ready")
                });
                endpoints.MapHealthChecks("/health/live",new HealthCheckOptions{
                    Predicate = (_) => false
                });
            });
        }
    }
}
