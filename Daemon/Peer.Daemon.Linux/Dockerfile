#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0-preview AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0-preview AS build
WORKDIR /src
COPY ["Peer.Daemon/Peer.Daemon.Linux.csproj", "Peer.Daemon/"]
RUN dotnet restore "Peer.Daemon/Peer.Daemon.Linux.csproj"
COPY . .
WORKDIR "/src/Peer.Daemon"
RUN dotnet build "Peer.Daemon.Linux.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Peer.Daemon.Linux.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Peer.Daemon.Linux.dll"]