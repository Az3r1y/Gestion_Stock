# �tape 1: Utilisation de l'image de base pour l'ex�cution (runtime)
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

# �tape 2: Utilisation de l'image SDK pour la compilation de l'application
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Gestion_Stock/Gestion_Stock.csproj", "Gestion_Stock/"]
RUN dotnet restore "Gestion_Stock/Gestion_Stock.csproj"
COPY . .
WORKDIR "/src/Gestion_Stock"
RUN dotnet build "Gestion_Stock.csproj" -c Release -o /app/build

# �tape 3: Publication de l'application
FROM build AS publish
RUN dotnet publish "Gestion_Stock.csproj" -c Release -o /app/publish

# �tape 4: Utilisation de l'image de runtime pour d�marrer l'application
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Gestion_Stock.dll"]
