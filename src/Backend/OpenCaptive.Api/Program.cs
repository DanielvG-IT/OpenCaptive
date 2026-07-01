using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using OpenCaptive.Api.Authentication;
using OpenCaptive.Api.Authorization;
using OpenCaptive.Api.Errors;
using OpenCaptive.Api.Extensions;
using OpenCaptive.Application;
using OpenCaptive.Application.Common.Contracts;
using OpenCaptive.Infrastructure;
using OpenCaptive.Infrastructure.Auth;
using Serilog;

namespace OpenCaptive.Api;

public partial class Program
{
    protected Program() { }

    public static void Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateBootstrapLogger();

        try
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Host.UseSerilog((context, services, configuration) => configuration
                .ReadFrom.Configuration(context.Configuration)
                .ReadFrom.Services(services)
                .Enrich.FromLogContext());

            builder.Services.AddProblemDetails(options =>
            {
                options.CustomizeProblemDetails = context =>
                {
                    var traceId = System.Diagnostics.Activity.Current?.Id ?? context.HttpContext.TraceIdentifier;
                    context.ProblemDetails.Extensions["traceId"] = traceId;
                };
            });
            builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

            // TODO: Add OpenTelemetry (tracing + metrics)
            builder.Services.AddOpenApi();
            builder.Services.AddHealthChecks();
            builder.Services.AddCors(options => options.AddPolicy("Default", policy =>
                policy.WithOrigins(builder.Configuration.GetSection("AllowedOrigins").Get<string[]>() ?? [])
                      .AllowAnyHeader()
                      .AllowAnyMethod()));

            builder.Services.AddApplication();
            builder.Services.AddInfrastructure(builder.Configuration);

            builder.Services.AddSingleton<IAuthorizationHandler, PermissionAuthorizationHandler>();

            var jwtOptions = builder.Configuration.GetSection("Authentication:Jwt").Get<JwtOptions>() ?? throw new InvalidOperationException("JWT configuration is missing.");

            builder.Services.AddHttpContextAccessor();
            builder.Services.AddScoped<ICurrentUser, ClaimsCurrentUser>();
            builder.Services
                .AddAuthorization()
                .AddAuthentication(x =>
                {
                    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                    x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = builder.Environment.IsProduction();
                    options.MapInboundClaims = false;  // keep "sub", "email", "jti" as written

                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Convert.FromBase64String(jwtOptions.SigningKey)),

                        ValidateIssuer = true,
                        ValidIssuer = jwtOptions.Issuer,

                        ValidateAudience = true,
                        ValidAudience = jwtOptions.Audience,

                        ValidateLifetime = true,

                        ClockSkew = jwtOptions.ClockSkew
                    };
                });

            var app = builder.Build();

            app.UseSerilogRequestLogging();
            app.UseExceptionHandler();

            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseHttpsRedirection();
            app.UseCors("Default");

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapEndpoints();

            app.Run();
        }
        catch (Exception ex) when (ex is not HostAbortedException)
        {
            Log.Fatal(ex, "Host terminated unexpectedly");
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }
}
