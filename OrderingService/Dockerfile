FROM ://microsoft.com AS base
WORKDIR /app
EXPOSE 8080

FROM ://microsoft.com AS build
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
