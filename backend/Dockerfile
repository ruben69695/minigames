ARG sdkTag=6.0.401-bullseye-slim
ARG runtimeTag=6.0.9-bullseye-slim
ARG runtimeImage=mcr.microsoft.com/dotnet/aspnet
ARG sdkImage=mcr.microsoft.com/dotnet/sdk
FROM ${runtimeImage}:${runtimeTag} as base
FROM ${sdkImage}:${sdkTag} AS build

ARG DEPLOYMENT_ENVIRONMENT=Release
FROM build as publish
WORKDIR /src
COPY . .
RUN dotnet publish minigames-api/minigames-api.csproj -c $DEPLOYMENT_ENVIRONMENT -o /app

ARG DEPLOYMENT_ENVIRONMENT=Release
FROM base as final
WORKDIR /app
EXPOSE 80
EXPOSE 443
COPY --from=publish /app ./
ENV ASPNETCORE_ENVIRONMENT $DEPLOYMENT_ENVIRONMENT
ENTRYPOINT [ "dotnet", "minigames-api.dll"]
