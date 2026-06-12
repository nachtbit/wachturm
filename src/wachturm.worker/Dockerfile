FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["wachturm.sln", "./"]
COPY ["src/wachturm.worker/wachturm.Worker.csproj", "src/wachturm.worker/"]
COPY ["src/wachturm.application/wachturm.Application.csproj", "src/wachturm.application/"]
COPY ["src/wachturm.domain/wachturm.Domain.csproj", "src/wachturm.domain/"]
COPY ["src/wachturm.infrastructure/wachturm.Infrastructure.csproj", "src/wachturm.infrastructure/"]

RUN dotnet restore "src/wachturm.worker/wachturm.Worker.csproj"

COPY . .
RUN dotnet publish "src/wachturm.worker/wachturm.Worker.csproj" -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/runtime:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "wachturm.Worker.dll"]
