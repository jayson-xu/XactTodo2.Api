#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 50666

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["XactTodo.Api/XactTodo.Api.csproj", "XactTodo.Api/"]
COPY ["XactTodo.Domain/XactTodo.Domain.csproj", "XactTodo.Domain/"]
COPY ["XactTodo.Security/XactTodo.Security.csproj", "XactTodo.Security/"]
COPY ["XactTodo.Infrastructure/XactTodo.Infrastructure.csproj", "XactTodo.Infrastructure/"]
RUN dotnet restore "./XactTodo.Api/./XactTodo.Api.csproj"
COPY . .
WORKDIR "/src/XactTodo.Api"
RUN dotnet build "./XactTodo.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./XactTodo.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "XactTodo.Api.dll"]