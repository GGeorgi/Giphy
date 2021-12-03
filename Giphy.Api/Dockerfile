FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Giphy.Api/Giphy.Api.csproj", "Giphy.Api/"]
COPY ["Giphy.Application/Giphy.Application.csproj", "Giphy.Application/"]
COPY ["Giphy.Domain/Giphy.Domain.csproj", "Giphy.Domain/"]
COPY ["Giphy.Infrastructure/Giphy.Infrastructure.csproj", "Giphy.Infrastructure/"]
COPY ["Giphy.Persistance/Giphy.Persistance.csproj", "Giphy.Persistance/"]
RUN dotnet restore "Giphy.Api/Giphy.Api.csproj"
COPY . .
WORKDIR "/src/Giphy.Api"
RUN dotnet build "Giphy.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Giphy.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Giphy.Api.dll"]
