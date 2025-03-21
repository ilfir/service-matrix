FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["service-matrix.csproj", "./"]
RUN dotnet restore "service-matrix.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "service-matrix.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "service-matrix.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "service-matrix.dll"]