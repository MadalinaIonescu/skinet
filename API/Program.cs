using StackExchange.Redis;
using Core.Entities.Identity;

//add services to the container
var builder = WebApplication.CreateBuilder(args);

//this was public void ConfigureServices(IServiceCollection services) {}
// This method gets called by the runtime. Use this method to add services to the container.
//do the dependency injections -- on the build is called, the kestrel is doing this
//in this method is not important the order of these commands
builder.Services.AddAutoMapper(typeof(MappingProfiles));

builder.Services.AddControllers();

//we did that when setting up the connection string
builder.Services.AddDbContext<StoreContext>(x =>
    x.UseNpgsql(builder.Configuration.GetConnectionString("Default")));

builder.Services.AddDbContext<AppIdentityDbContext>(x =>
{
    x.UseNpgsql(builder.Configuration.GetConnectionString("IdentityConnection"));

});

builder.Services.AddSingleton<IConnectionMultiplexer>(c =>
{
    var configuration = ConfigurationOptions.Parse(builder.Configuration
        .GetConnectionString("Redis"), true);
    return ConnectionMultiplexer.Connect(configuration);
});

builder.Services.AddApplicationServices();
builder.Services.AddIdentityServices(builder.Configuration);

builder.Services.AddSwaggerDocumentation();

builder.Services.AddCors(opt =>
{
    opt.AddPolicy("CorsPolicy", policy =>
    {
        policy.AllowAnyHeader().AllowAnyMethod().WithOrigins("https://localhost:4200");
    });
});
//end of void ConfigureServices(IServiceCollection services) {}


//configure http request pipeline
var app = builder.Build();

//this was void Configure(IApplicationBuilder app, IWebHostEnvironment env) {} from StartUp
//This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
//configure the middleware pipeline -- on the run , the kestrel is doing this
//in this method is important the order of these commands

app.UseMiddleware<ExceptionMiddleware>();

app.UseStatusCodePagesWithReExecute("/errors/{0}");

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "Content")),
    RequestPath = "/content"
});

app.UseCors("CorsPolicy");

app.UseAuthentication();

app.UseAuthorization();

app.UseSwaggerDocumentation();

app.MapControllers();
//api serves angular app so, for angular routes, 
//we don't want api to try to find routes, and we redirect here
app.MapFallbackToController("Index", "Fallback");
//end of void Configure(IApplicationBuilder app, IWebHostEnvironment env) {}

//this was void Main
using var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;
var loggerFactory = services.GetRequiredService<ILoggerFactory>();
try
{
    var context = services.GetRequiredService<StoreContext>();
    await context.Database.MigrateAsync();
    await StoreContextSeed.SeedAsync(context, loggerFactory);

    var userManager = services.GetRequiredService<UserManager<AppUser>>();
    var identityContext = services.GetRequiredService<AppIdentityDbContext>();
    await identityContext.Database.MigrateAsync();
    await AppIdentityDbContextSeed.SeedUserAsync(userManager);
}
catch (Exception ex)
{
    var logger = loggerFactory.CreateLogger<Program>();
    logger.LogError(ex, "An error occured during migration");
}

await app.RunAsync();

