FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY Directory.Build.props ./
COPY Directory.Packages.props ./
COPY src/GitDevelop.Application/GitDevelop.Application.csproj src/GitDevelop.Application/
COPY src/GitDevelop.Core/GitDevelop.Core.csproj src/GitDevelop.Core/
COPY src/GitDevelop.Domain/GitDevelop.Domain.csproj src/GitDevelop.Domain/
COPY src/GitDevelop.Infrastructure/GitDevelop.Infrastructure.csproj src/GitDevelop.Infrastructure/
COPY src/GitDevelop.WebApi/GitDevelop.WebApi.csproj src/GitDevelop.WebApi/

RUN dotnet restore src/GitDevelop.WebApi/GitDevelop.WebApi.csproj

COPY . .

RUN dotnet publish src/GitDevelop.WebApi/GitDevelop.WebApi.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "GitDevelop.WebApi.dll"]
