# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/RaceDay.MVC/RaceDay.MVC.csproj", "src/RaceDay.MVC/"]
RUN dotnet restore "src/RaceDay.MVC/RaceDay.MVC.csproj"
COPY . .
WORKDIR "/src/src/RaceDay.MVC"
RUN dotnet publish "RaceDay.MVC.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "RaceDay.MVC.dll"]