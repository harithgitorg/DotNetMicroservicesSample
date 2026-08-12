FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["SharedContracts/SharedContracts.csproj", "SharedContracts/"]
COPY ["OrderingService/OrderingService.csproj", "OrderingService/"]
RUN dotnet restore "OrderingService/OrderingService.csproj"
COPY . .
WORKDIR "/src/OrderingService"
RUN dotnet build "OrderingService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "OrderingService.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OrderingService.dll"]
