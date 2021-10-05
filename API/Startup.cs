using Core.Interfaces;
using Infrastructure.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace API
{
    public class Startup
    {
        private readonly IConfiguration _config ;
        public Startup(IConfiguration configuration)
        {
            _config = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        //do the dependency injections -- on the build is called, the kestrel is doing this
        //in this method is not important the order of these commands
        public void ConfigureServices(IServiceCollection services)
        {
            //lifetime of an http request, whn it creates a controller it creates this repository,
            //when the controller is destroyed, it destroys this too
            services.AddScoped<IProductRepository, ProductRepository>(); 
            services.AddControllers();
            //we did that when setting up the connection string
            services.AddDbContext<StoreContext>(x => 
                x.UseSqlite(_config.GetConnectionString("Default")));
                
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "API", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        //configure the middleware pipeline -- on the run , the kestrel is doing this
        //in this method is important the order of these commands
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
