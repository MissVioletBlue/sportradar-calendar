FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Sportradar.Calendar.Presentation.Web/Sportradar.Calendar.Presentation.Web.csproj", "Sportradar.Calendar.Presentation.Web/"]
RUN dotnet restore "Sportradar.Calendar.Presentation.Web/Sportradar.Calendar.Presentation.Web.csproj"
COPY . .
WORKDIR "/src/Sportradar.Calendar.Presentation.Web"
RUN dotnet build "./Sportradar.Calendar.Presentation.Web.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Sportradar.Calendar.Presentation.Web.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Sportradar.Calendar.Presentation.Web.dll"]
