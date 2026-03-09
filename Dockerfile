FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

COPY FlightStatus.Domain/FlightStatus.Domain.csproj FlightStatus.Domain/
COPY FlightStatus.Application/FlightStatus.Application.csproj FlightStatus.Application/
COPY FlightStatus.Infrastructure/FlightStatus.Infrastructure.csproj FlightStatus.Infrastructure/
COPY FlightStatus.Api/FlightStatus.Api.csproj FlightStatus.Api/

RUN dotnet restore FlightStatus.Api/FlightStatus.Api.csproj
COPY . .
RUN dotnet publish FlightStatus.Api/FlightStatus.Api.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "FlightStatus.Api.dll"]
