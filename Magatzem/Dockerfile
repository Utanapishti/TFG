#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app



FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Magatzem/Magatzem.csproj", "Magatzem/"]
RUN dotnet restore "Magatzem/Magatzem.csproj"
COPY . .
WORKDIR "/src/Magatzem"
RUN dotnet build "Magatzem.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Magatzem.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
EXPOSE 5098
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Magatzem.dll"]