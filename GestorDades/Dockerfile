#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app



FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["GestorDades/GestorDades.csproj", "GestorDades/"]
RUN dotnet restore "GestorDades/GestorDades.csproj"
COPY . .
WORKDIR "/src/GestorDades"
RUN dotnet build "GestorDades.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "GestorDades.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GestorDades.dll"]
EXPOSE 5098