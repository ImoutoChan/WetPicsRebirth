#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["WetPicsRebirth/WetPicsRebirth.csproj", "WetPicsRebirth/"]
COPY ["WetPicsRebirth.Infrastructure.ImageProcessing/WetPicsRebirth.Infrastructure.ImageProcessing.csproj", "WetPicsRebirth.Infrastructure.ImageProcessing/"]
RUN dotnet restore "WetPicsRebirth/WetPicsRebirth.csproj"
COPY . .
WORKDIR "/src/WetPicsRebirth"
RUN dotnet build "WetPicsRebirth.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WetPicsRebirth.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WetPicsRebirth.dll"]
