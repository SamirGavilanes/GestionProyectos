FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY . .
WORKDIR "/src/GestionProyectos.Server"
RUN dotnet restore "GestionProyectos.Server.csproj"
RUN dotnet build "GestionProyectos.Server.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "GestionProyectos.Server.csproj" -c Release -o /app/publish

FROM base AS final
ARG ENVIRONMENT_NAME
ENV PARAM_ENVIRONMENT_NAME=$ENVIRONMENT_NAME
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT "dotnet" "GestionProyectos.Server.dll" $PARAM_ENVIRONMENT_NAME





