using OpenCaptive.Api.Extensions;
using OpenCaptive.Application;
using OpenCaptive.Infrastructure;
using Serilog;

namespace OpenCaptive.Api;

public partial class Program
{
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

            // TODO: Add OpenTelemetry (tracing + metrics) and immutable audit trail for all mutations (AVG/GDPR, NIS2, ISO 27001, NEN 7510 compliance for all tenants).
            builder.Services.AddOpenApi();
            builder.Services.AddProblemDetails();
            builder.Services.AddHealthChecks();
            builder.Services.AddCors(options => options.AddPolicy("Default", policy =>
                policy.WithOrigins(builder.Configuration.GetSection("AllowedOrigins").Get<string[]>() ?? [])
                      .AllowAnyHeader()
                      .AllowAnyMethod()));

            builder.Services.AddApplication();
            builder.Services.AddInfrastructure(builder.Configuration);

            var app = builder.Build();

            app.UseSerilogRequestLogging();
            app.UseExceptionHandler();

            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseHttpsRedirection();
            app.UseCors("Default");

            app.MapHealthChecks("/health");
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