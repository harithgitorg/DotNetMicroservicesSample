FROM ://microsoft.com AS base
WORKDIR /app
EXPOSE 8080

FROM ://microsoft.com AS build
WORKDIR /src
COPY ["SharedContracts/SharedContracts.csproj", "SharedContracts/"]
COPY ["CatalogService/CatalogService.csproj", "CatalogService/"]
RUN dotnet restore "CatalogService/CatalogService.csproj"
COPY . .
WORKDIR "/src/CatalogService"
RUN dotnet build "CatalogService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CatalogService.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CatalogService.dll"]
