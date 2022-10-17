FROM mcr.microsoft.com/dotnet/aspnet:6.0-focal AS base
WORKDIR /app

EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0-focal AS build
WORKDIR /src

COPY ["./TodoApiWithControllers/TodoApiWithControllers.csproj", "./"]
RUN dotnet restore "./TodoApiWithControllers.csproj" --disable-parallel

COPY ./TodoApiWithControllers .
WORKDIR "/src/."

RUN dotnet build "TodoApiWithControllers.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TodoApiWithControllers.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TodoApiWithControllers.dll"]