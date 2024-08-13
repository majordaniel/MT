#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["MTMiddleware.BackgroundService/MTMiddleware.BackgroundService.csproj", "MTMiddleware.BackgroundService/"]
COPY ["MTMiddleware.Core/MTMiddleware.Core.csproj", "MTMiddleware.Core/"]
COPY ["MTMiddleware.Data/MTMiddleware.Data.csproj", "MTMiddleware.Data/"]
COPY ["MTMiddleware.Shared/MTMiddleware.Shared.csproj", "MTMiddleware.Shared/"]
COPY ["UtilityLibrary/UtilityLibrary.csproj", "UtilityLibrary/"]
RUN dotnet restore "MTMiddleware.BackgroundService/MTMiddleware.BackgroundService.csproj"
COPY . .
WORKDIR "/src/MTMiddleware.BackgroundService"
RUN dotnet build "MTMiddleware.BackgroundService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MTMiddleware.BackgroundService.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MTMiddleware.BackgroundService.dll"]