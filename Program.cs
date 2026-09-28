using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ServiceAggregator.Api.Data;
using ServiceAggregator.Api.Models;
using ServiceAggregator.Api.Services;
using Polly;
using Polly.Extensions.Http;
using System.Net;
using System.Text;

var d = DateTimeOffset.Now.ToUnixTimeMilliseconds();
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<ServiceAggregatorContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("Default")));
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
builder.Services.AddSingleton<LoginService>();
builder.Services.AddScoped<ExecutionLogService>();
builder.Services.AddSingleton<JwtService>();
builder.Services.AddMemoryCache();
builder.Services.AddSingleton<SkiaSharpCaptchaService>();
builder.Services.AddControllers();
//var proxy = new WebProxy("http://127.0.0.1:8888");
//builder.Services.AddHttpClient();
    //.ConfigurePrimaryHttpMessageHandler(() =>
    //    new HttpClientHandler
    //    {
    //        Proxy = proxy,
    //        UseProxy = true,
    //        ServerCertificateCustomValidationCallback =
    //            HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
    //    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHealthChecks();
var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key)),
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy =>
        {
            policy
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader();
        });
});
static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
{
    return HttpPolicyExtensions
        .HandleTransientHttpError()
        .OrResult(msg => msg.StatusCode == HttpStatusCode.TooManyRequests)
        .WaitAndRetryAsync(
            retryCount: 3,
            sleepDurationProvider: retryAttempt =>
                TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
            onRetry: (outcome, timespan, retryAttempt, context) =>
            {
            }
        );
}
static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
{
    return HttpPolicyExtensions
        .HandleTransientHttpError()
        .CircuitBreakerAsync(
            handledEventsAllowedBeforeBreaking: 5,
            durationOfBreak: TimeSpan.FromSeconds(30),
            onBreak: (outcome, breakDelay) =>
            {
            },
            onReset: () =>
            {
            },
            onHalfOpen: () =>
            {
            }
        );
}
builder.Services.AddHttpClient("AggregateClient", client =>
    {
        client.Timeout = TimeSpan.FromSeconds(30);
    })
    .AddPolicyHandler(GetRetryPolicy())
    .AddPolicyHandler(GetCircuitBreakerPolicy()); 
var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.UseCors("AllowAll");

app.MapControllers();

app.UseHealthChecks("/health");

app.Run();
