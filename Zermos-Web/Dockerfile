#FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
#WORKDIR /source
#
#
#FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
#WORKDIR /src
#COPY ["Zermos-Web/Zermos-Web.csproj", "Zermos-Web/"]
#COPY ["Infrastructure/Infrastructure.csproj", "Infrastructure/"]
#
#
#RUN dotnet restore "Zermos-Web/Zermos-Web.csproj" --disable-parallel
#COPY . .
#WORKDIR "/src/Zermos-Web"
#RUN dotnet publish "Zermos-Web/Zermos-Web.csproj" -c Release -o /app --no-restore --runtime linux-x64 #for x64
#
#WORKDIR /app
#COPY --from=build /app ./
#
#ENV ASPNETCORE_URLS=http://+:5000
#
#EXPOSE 5000
#
#ENTRYPOINT ["dotnet", "Zermos-Web.dll"]

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

COPY Zermos-Web/Zermos-Web.csproj Zermos-Web/
COPY Infrastructure/Infrastructure.csproj Infrastructure/

RUN dotnet restore "Zermos-Web/Zermos-Web.csproj" --disable-parallel

COPY . .
RUN dotnet publish "Zermos-Web/Zermos-Web.csproj" -c Release -o /publish

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS final
WORKDIR /app
COPY --from=build /publish .

ENTRYPOINT ["dotnet", "Zermos-Web.dll"]