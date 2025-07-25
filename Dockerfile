﻿FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["IBS-Europe.App/IBS-Europe.App.csproj", "IBS-Europe.App/"]
COPY ["IBS-Europe.Domains/IBS-Europe.Domains.csproj", "IBS-Europe.Domains/"]
COPY ["IBS-Europe.Infrastructures/IBS-Europe.Infrastructures.csproj", "IBS-Europe.Infrastructures/"]

# Restaurer les packages NuGet
RUN dotnet restore "IBS-Europe.App/IBS-Europe.App.csproj"

# Copier le reste des fichiers sources
COPY . .

WORKDIR "/src/IBS-Europe.App"
RUN dotnet build "IBS-Europe.App.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "IBS-Europe.App.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish . 
ENTRYPOINT ["dotnet", "IBS-Europe.App.dll"]
