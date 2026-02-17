# -----------------------------------------------
# Stage 1: Base image
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

# -----------------------------------------------
# Stage 2: Build image
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
ARG BUILD_CONFIGURATION=Release

# Copy csproj and restore
# Copy ALL .csproj files first
COPY ["API/API.csproj", "API/"]
COPY ["Application/Application.csproj", "Application/"]
COPY ["Contracts/Contracts.csproj", "Contracts/"]
COPY ["Infrastructure/Infrastructure.csproj", "Infrastructure/"]

# restore
RUN dotnet restore "API/API.csproj"

# Copy ALL source code
COPY . .

# build
RUN dotnet build "API/API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# -----------------------------------------------
# Stage 3: Publish image
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "API/API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# -----------------------------------------------
# Stage 4: Final image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "API.dll"]


# docker run command from root sln folder
# docker build -f API/Dockerfile -t my-api .

# image run command:
# docker run -d -p 8080:8080 --name my-api-container my-api

# get request
# http://localhost:8080/api/Home/dashboard
# return response