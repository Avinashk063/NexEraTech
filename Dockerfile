# -----------------------------
# BUILD STAGE
# -----------------------------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy all project files (multi-project fix)
COPY ["NexEraTech.Web/NexEraTech.Web.csproj", "NexEraTech.Web/"]
COPY ["NexEraTech.Domain/NexEraTech.Domain.csproj", "NexEraTech.Domain/"]
COPY ["NexEraTech.Infrastructure/NexEraTech.Infrastructure.csproj", "NexEraTech.Infrastructure/"]
COPY ["NexEraTech.Application/NexEraTech.Application.csproj", "NexEraTech.Application/"]

# Restore dependencies
RUN dotnet restore "NexEraTech.Web/NexEraTech.Web.csproj"

# Copy full source
COPY . .

# Build & publish
WORKDIR /src/NexEraTech.Web
RUN dotnet publish "NexEraTech.Web.csproj" -c Release -o /app/publish

# -----------------------------
# RUNTIME STAGE
# -----------------------------
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_ENVIRONMENT=Production
ENV PORT=8080

EXPOSE 8080

ENTRYPOINT ["dotnet", "NexEraTech.Web.dll"]
