FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["SmartHospital.Api/SmartHospital.Api.csproj", "SmartHospital.Api/"]
COPY ["SmartHospital.Application/SmartHospital.Application.csproj", "SmartHospital.Application/"]
COPY ["SmartHospital.Domain/SmartHospital.Domain.csproj", "SmartHospital.Domain/"]
COPY ["SmartHospital.Infrastructure/SmartHospital.Infrastructure.csproj", "SmartHospital.Infrastructure/"]
RUN dotnet restore "SmartHospital.Api/SmartHospital.Api.csproj"
COPY . .
WORKDIR "/src/SmartHospital.Api"
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "SmartHospital.Api.dll"]
