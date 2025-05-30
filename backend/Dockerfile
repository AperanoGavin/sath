FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY PRS.sln .

COPY PRS.Domain/PRS.Domain.csproj         PRS.Domain/
COPY PRS.Application/PRS.Application.csproj PRS.Application/
COPY PRS.Infrastructure/PRS.Infrastructure.csproj PRS.Infrastructure/
COPY PRS.Presentation/PRS.Presentation.csproj   PRS.Presentation/
COPY PRS.Domain.Tests/PRS.Domain.Tests.csproj   PRS.Domain.Tests/

RUN dotnet restore

# 4) Copy the rest of your code
COPY . .

RUN dotnet publish \
    PRS.Presentation/PRS.Presentation.csproj \
    -c Release \
    -o /app/publish \
    --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish ./
ENTRYPOINT ["dotnet", "PRS.Presentation.dll"]
