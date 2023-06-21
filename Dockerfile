FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /source


FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Zermos-Web/Zermos-Web.csproj", "Zermos-Web/"]
COPY ["Infrastructure/Infrastructure.csproj", "Infrastructure/"]


RUN dotnet restore "Zermos-Web/Zermos-Web.csproj" --disable-parallel
COPY . .
WORKDIR "/src/Zermos-Web"
RUN dotnet publish "Zermos-Web/Zermos-Web.csproj" -c Release -o /app --no-restore

WORKDIR /app
COPY --from=build /app ./

EXPOSE 5000

ENTRYPOINT ["dotnet", "Zermos-Web.dll"]