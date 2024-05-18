FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /App

# Copy everything
COPY . ./
# Restore as distinct layers
RUN dotnet restore ./ReportService.Api/ReportService.Api.csproj
# Build and publish a release
RUN dotnet publish ./ReportService.Api/ReportService.Api.csproj -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0

EXPOSE 5001

WORKDIR /App
COPY --from=build-env /App/out .
ENTRYPOINT ["dotnet", "ReportService.Api.dll"]