FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 5125
ENV ASPNETCORE_URLS=http://+:5125

FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG configuration=Release
WORKDIR /src

COPY UserService/UserService.csproj UserService/
COPY Common/Common.csproj Common/
COPY Common.Web/Common.Web.csproj Common.Web/

RUN dotnet restore "UserService/UserService.csproj"

COPY . .

RUN dotnet build "UserService/UserService.csproj" -c $configuration -o /app/build

FROM build AS publish
ARG configuration=Release
RUN dotnet publish "UserService/UserService.csproj" -c $configuration -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "UserService.dll"]
