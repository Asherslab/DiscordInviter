FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["AshersLab.Discord.Inviter/AshersLab.Discord.Inviter.csproj", "AshersLab.Discord.Inviter/"]
RUN dotnet restore "AshersLab.Discord.Inviter/AshersLab.Discord.Inviter.csproj"
COPY . .
WORKDIR "/src/AshersLab.Discord.Inviter"
RUN dotnet build "AshersLab.Discord.Inviter.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AshersLab.Discord.Inviter.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AshersLab.Discord.Inviter.dll"]
