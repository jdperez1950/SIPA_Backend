# ---------- BUILD ----------
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /Api

COPY Api/Api.csproj Api/
RUN dotnet restore Api/Api.csproj

COPY . .

RUN dotnet publish Api/Api.csproj -c Release -o /app/publish

# ---------- RUNTIME ----------
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app

EXPOSE 8080

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "Api.dll"]
