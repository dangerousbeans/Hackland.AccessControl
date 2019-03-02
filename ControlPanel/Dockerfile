FROM microsoft/dotnet:2.2-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 5555

FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /src
COPY ["Hackland.AcessControl.Web/Hackland.AccessControl.Web.csproj", "Hackland.AcessControl.Web/"]
COPY ["Hackland.AcessControl.Data/Hackland.AccessControl.Data.csproj", "Hackland.AcessControl.Data/"]
RUN dotnet restore "Hackland.AcessControl.Web/Hackland.AccessControl.Web.csproj"
COPY . .
WORKDIR "/src/Hackland.AcessControl.Web"
RUN dotnet build "Hackland.AccessControl.Web.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "Hackland.AccessControl.Web.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Hackland.AccessControl.Web.dll"]