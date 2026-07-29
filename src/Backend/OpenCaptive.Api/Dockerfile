# syntax=docker/dockerfile:1.7
#
# OpenCaptive API image.
#
# Build from the REPO ROOT, not from this directory - the API's project graph reaches into
# sibling projects under src/Backend, so the build context has to contain all of them:
#
#   docker build -f src/Backend/OpenCaptive.Api/Dockerfile -t opencaptive/api .
#
# The image is framework-dependent on purpose. Do NOT add PublishTrimmed / PublishAot: the
# email templates are RazorLight templates compiled by Roslyn at runtime (hence
# PreserveCompilationContext in the .csproj), which needs the reference assemblies that
# publish drops in refs/. Trimming removes them and every outbound email fails at runtime.

# ---------------------------------------------------------
# BUILD
# ---------------------------------------------------------
FROM mcr.microsoft.com/dotnet/sdk:10.0-alpine AS build
WORKDIR /src

ENV DOTNET_NOLOGO=1 \
    DOTNET_CLI_TELEMETRY_OPTOUT=1 \
    DOTNET_SKIP_FIRST_TIME_EXPERIENCE=1

# Copy the project files alone first. They change far less often than source, so an ordinary
# code edit reuses this restore layer instead of re-resolving every NuGet package.
COPY src/Backend/OpenCaptive.Api/OpenCaptive.Api.csproj                               src/Backend/OpenCaptive.Api/
COPY src/Backend/OpenCaptive.Application/OpenCaptive.Application.csproj               src/Backend/OpenCaptive.Application/
COPY src/Backend/OpenCaptive.Domain/OpenCaptive.Domain.csproj                         src/Backend/OpenCaptive.Domain/
COPY src/Backend/OpenCaptive.Infrastructure/OpenCaptive.Infrastructure.csproj         src/Backend/OpenCaptive.Infrastructure/
COPY src/Backend/OpenCaptive.Integrations.Abstractions/OpenCaptive.Integrations.Abstractions.csproj src/Backend/OpenCaptive.Integrations.Abstractions/
COPY src/Backend/OpenCaptive.Integrations.UniFi/OpenCaptive.Integrations.UniFi.csproj src/Backend/OpenCaptive.Integrations.UniFi/

# The NuGet cache lives in a BuildKit cache mount rather than an image layer: packages stay
# warm across builds without ever shipping in the final image.
RUN --mount=type=cache,target=/root/.nuget/packages \
    dotnet restore src/Backend/OpenCaptive.Api/OpenCaptive.Api.csproj

# Only the backend tree - a frontend edit must not invalidate the backend build.
COPY src/Backend/ src/Backend/

# UseAppHost=false skips the native launcher; the entrypoint invokes `dotnet` directly, so
# it would be dead weight in the image.
RUN --mount=type=cache,target=/root/.nuget/packages \
    dotnet publish src/Backend/OpenCaptive.Api/OpenCaptive.Api.csproj \
        --configuration Release \
        --no-restore \
        --output /app/publish \
        -p:UseAppHost=false

# ---------------------------------------------------------
# RUNTIME
# ---------------------------------------------------------
FROM mcr.microsoft.com/dotnet/aspnet:10.0-alpine AS runtime
WORKDIR /app

# ICU is installed explicitly instead of relying on the base image's default: Identity,
# Npgsql and the email templates all format culture-sensitive values, and invariant mode
# silently changes casing/comparison behaviour rather than failing loudly. Drop icu-libs
# and set DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=1 to save ~30 MB if you never need
# non-invariant cultures. tzdata is needed for token lifetimes to resolve real zones.
#
# krb5-libs is not optional noise: Npgsql probes libgssapi_krb5.so.2 while negotiating auth,
# and on musl its absence prints a scary "Cannot load library" error on the first connection
# even though the connection succeeds. ~1 MB to keep the logs honest.
RUN apk add --no-cache icu-libs tzdata krb5-libs
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

# Note: Program.cs calls UseHttpsRedirection() unconditionally, so a container serving plain
# HTTP behind a TLS-terminating proxy logs "Failed to determine the https port for redirect"
# once. Harmless - no redirect is attempted - but set ASPNETCORE_HTTPS_PORTS to the public
# HTTPS port if you want the redirect to actually work.

ENV ASPNETCORE_HTTP_PORTS=8080 \
    DOTNET_NOLOGO=1 \
    DOTNET_CLI_TELEMETRY_OPTOUT=1

COPY --from=build /app/publish .

# APP_UID (1654) is the non-root user the .NET base images ship with - no adduser needed.
USER $APP_UID

EXPOSE 8080

# /health/live is anonymous and registers no checks, so it reports process liveness without
# touching Postgres. Readiness (/health/ready) is the orchestrator's job, not Docker's.
# wget is BusyBox built-in; installing curl just for this would add a package for nothing.
HEALTHCHECK --interval=30s --timeout=3s --start-period=20s --retries=3 \
    CMD wget --quiet --spider http://127.0.0.1:8080/health/live || exit 1

ENTRYPOINT ["dotnet", "OpenCaptive.Api.dll"]
