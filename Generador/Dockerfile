#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Generador/Generador.csproj", "Generador/"]
RUN dotnet restore "Generador/Generador.csproj"
COPY . .
WORKDIR "/src/Generador"
RUN dotnet build "Generador.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Generador.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
EXPOSE 5098
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Generador.dll"]