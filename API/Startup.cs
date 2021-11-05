using API.Extenstions;
using API.Helpers;
using API.Middleware;
using Infrastructure.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

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
            services.AddAutoMapper(typeof(MappingProfiles));

            services.AddControllers();
            
            //we did that when setting up the connection string
            services.AddDbContext<StoreContext>(x => 
                x.UseSqlite(_config.GetConnectionString("Default")));

            services.AddSingleton<IConnectionMultiplexer>(c => {
                var configuration = ConfigurationOptions.Parse(_config
                    .GetConnectionString("Redis"), true);
                return ConnectionMultiplexer.Connect(configuration);
            });    

            services.AddApplicationServices();    
            services.AddSwaggerDocumentation();

            services.AddCors(opt =>
            {
                opt.AddPolicy("CorsPolicy", policy =>
                {
                    policy.AllowAnyHeader().AllowAnyMethod().WithOrigins("https://localhost:4200");
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        //configure the middleware pipeline -- on the run , the kestrel is doing this
        //in this method is important the order of these commands
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseMiddleware<ExceptionMiddleware>();

            app.UseStatusCodePagesWithReExecute("/errors/{0}");

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseStaticFiles();

            app.UseCors("CorsPolicy");

            app.UseAuthorization();
            
            app.UseSwaggerDocumentation();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
