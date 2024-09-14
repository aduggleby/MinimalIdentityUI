using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MinimalIdentityUI.Data;
using System.Threading.RateLimiting;
using RedisRateLimiting.AspNetCore;
using RedisRateLimiting;
using StackExchange.Redis;
using Humanizer.Localisation;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();

#region Rate Limiting Setup

var useRedisRateLimiter = true;

if (!useRedisRateLimiter)
{
    #region Rate Limiting Configuration

    builder.Services.AddRateLimiter(options =>
    {
        // Switch to standard 429 response, default is 503
        options.RejectionStatusCode = 429;

        // Policy for login and register endpoints: rate limited to 10 requests per minute
        options.AddPolicy(RateLimiterPolicy.LoginAndRegister, httpContext =>
            RateLimitPartition.GetSlidingWindowLimiter(
                // Partition key is the IP address of the client (will be overwritten by UseForwardedHeaders below if proxied)
                partitionKey: $"{RateLimiterPolicy.LoginAndRegister}-{httpContext.Connection.RemoteIpAddress?.ToString()}",
                factory: _ => new SlidingWindowRateLimiterOptions
                {
                    PermitLimit = 10,
                    Window = TimeSpan.FromMinutes(1)
                }));

        // Policy for anonymous identity endpoints: rate limited to 5 requests per minute
        options.AddPolicy(RateLimiterPolicy.ForgotPassword, httpContext =>
            RateLimitPartition.GetSlidingWindowLimiter(
                // Partition key is the IP address of the client (will be overwritten by UseForwardedHeaders below if proxied)
                partitionKey: $"{RateLimiterPolicy.ForgotPassword}-{httpContext.Connection.RemoteIpAddress?.ToString()}",
                factory: _ => new SlidingWindowRateLimiterOptions
                {
                    PermitLimit = 5,
                    Window = TimeSpan.FromMinutes(1)
                }));

        // Policy for all other endpoints: rate limited to 100 requests per minute
        options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
            RateLimitPartition.GetSlidingWindowLimiter(
                partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? string.Empty,
                factory: partition => new SlidingWindowRateLimiterOptions
                {
                    AutoReplenishment = true,
                    PermitLimit = 100,
                    Window = TimeSpan.FromMinutes(1),
                    SegmentsPerWindow = 4
                }));
    });

    #endregion
}
else
{
    #region Rate Limiting Configuration using Redis as a backplane for distributed rate limiting

    // Replace with 
    var connectionMultiplexer = ConnectionMultiplexer.Connect(
        builder.Configuration.GetConnectionString("Redis") ??
            throw new InvalidOperationException("Connection string 'Redis' not found."));

    builder.Services.AddRateLimiter(options =>
    {
        // Switch to standard 429 response, default is 503
        options.RejectionStatusCode = 429;

        // Policy for login and register endpoints: rate limited to 10 requests per minute
        options.AddPolicy(RateLimiterPolicy.LoginAndRegister, httpContext =>
            RedisRateLimitPartition.GetSlidingWindowRateLimiter(
                // Partition key is the IP address of the client (will be overwritten by UseForwardedHeaders below if proxied)
                partitionKey: $"{RateLimiterPolicy.LoginAndRegister}-{httpContext.Connection.RemoteIpAddress?.ToString()}",
                factory: _ => new RedisSlidingWindowRateLimiterOptions
                {
                    ConnectionMultiplexerFactory = () => connectionMultiplexer,
                    PermitLimit = 10,
                    Window = TimeSpan.FromMinutes(1)
                }));

        // Policy for anonymous identity endpoints: rate limited to 5 requests per minute
        options.AddPolicy(RateLimiterPolicy.ForgotPassword, httpContext =>
            RedisRateLimitPartition.GetSlidingWindowRateLimiter(
                // Partition key is the IP address of the client (will be overwritten by UseForwardedHeaders below if proxied)
                partitionKey: $"{RateLimiterPolicy.ForgotPassword}-{httpContext.Connection.RemoteIpAddress?.ToString()}",
                factory: _ => new RedisSlidingWindowRateLimiterOptions
                {
                    ConnectionMultiplexerFactory = () => connectionMultiplexer,
                    PermitLimit = 5,
                    Window = TimeSpan.FromMinutes(1)
                }));

        // Policy for all other endpoints: rate limited to 100 requests per minute
        options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
            RedisRateLimitPartition.GetSlidingWindowRateLimiter(
                partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? string.Empty,
                factory: partition => new RedisSlidingWindowRateLimiterOptions
                {
                    ConnectionMultiplexerFactory = () => connectionMultiplexer,
                    PermitLimit = 100,
                    Window = TimeSpan.FromMinutes(1),
                }));


    });

    #endregion
}

#endregion Rate Limiting Setup

var razorPageBuild = builder.Services.AddRazorPages(options =>
{
    options.Conventions.RateLimitAreaPage("Identity", "/Account/Register", RateLimiterPolicy.LoginAndRegister);
    options.Conventions.RateLimitAreaPage("Identity", "/Account/Login", RateLimiterPolicy.LoginAndRegister);
    options.Conventions.RateLimitAreaPage("Identity", "/Account/ForgotPassword", RateLimiterPolicy.ForgotPassword);
});

if (builder.Environment.IsDevelopment())
{
    razorPageBuild.AddRazorRuntimeCompilation();
}



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();


app.UseRouting();

#region Rate Limiting Activation (after Static Files and after UseRouting)

// Enable overwriting RemoteIpAddress with X-Forwarded-For header for Rate Limiter
app.UseForwardedHeaders();

app.UseRateLimiter();

#endregion

app.UseAuthorization();

app.MapRazorPages();

app.Run();


public static class RateLimiterPolicy
{
    public const string LoginAndRegister = "login-and-register";
    public const string ForgotPassword = "forgot-password";
};
