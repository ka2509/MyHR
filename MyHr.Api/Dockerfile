# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy project files first for layer caching
COPY MyHr.Api/MyHr.Api.csproj MyHr.Api/
COPY MyHr.Service/MyHr.Service.csproj MyHr.Service/
COPY MyHr.Data/MyHr.Data.csproj MyHr.Data/

# Restore dependencies
RUN dotnet restore MyHr.Api/MyHr.Api.csproj

# Copy everything else
COPY . .

# Publish
RUN dotnet publish MyHr.Api/MyHr.Api.csproj -c Release -o /app/out

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/out .

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "MyHr.Api.dll"]
