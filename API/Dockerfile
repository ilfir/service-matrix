FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
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
COPY resources/definitions.txt ./resources/definitions.txt
COPY resources/merged.txt ./resources/merged.txt
COPY data/include.txt ./data/include.txt
COPY data/exclude.txt ./data/exclude.txt
ENTRYPOINT ["dotnet", "service-matrix.dll"]