FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY Directory.Build.props ./
COPY Directory.Packages.props ./
COPY src/VcsDevelop.Application/VcsDevelop.Application.csproj src/VcsDevelop.Application/
COPY src/VcsDevelop.Core/VcsDevelop.Core.csproj src/VcsDevelop.Core/
COPY src/VcsDevelop.Domain/VcsDevelop.Domain.csproj src/VcsDevelop.Domain/
COPY src/VcsDevelop.Infrastructure/VcsDevelop.Infrastructure.csproj src/VcsDevelop.Infrastructure/
COPY src/VcsDevelop.WebApi/VcsDevelop.WebApi.csproj src/VcsDevelop.WebApi/

RUN dotnet restore src/VcsDevelop.WebApi/VcsDevelop.WebApi.csproj

COPY . .

RUN dotnet publish src/VcsDevelop.WebApi/VcsDevelop.WebApi.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "VcsDevelop.WebApi.dll"]
