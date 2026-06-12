FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["wachturm.sln", "./"]
COPY ["src/wachturm.api/wachturm.Api.csproj", "src/wachturm.api/"]
COPY ["src/wachturm.application/wachturm.Application.csproj", "src/wachturm.application/"]
COPY ["src/wachturm.domain/wachturm.Domain.csproj", "src/wachturm.domain/"]
COPY ["src/wachturm.infrastructure/wachturm.Infrastructure.csproj", "src/wachturm.infrastructure/"]

RUN dotnet restore "src/wachturm.api/wachturm.Api.csproj"

COPY . .
RUN dotnet publish "src/wachturm.api/wachturm.Api.csproj" -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "wachturm.Api.dll"]
