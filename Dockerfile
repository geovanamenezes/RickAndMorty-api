FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

WORKDIR /app

COPY *.sln ./
COPY **/*.csproj ./

RUN dotnet restore

COPY . .

RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime

RUN apt-get update && apt-get install -y sqlite3 && rm -rf /var/lib/apt/lists/*

WORKDIR /app

COPY --from=build /app/out .

RUN mkdir -p /app/Database
RUN chmod 777 /app/Database

EXPOSE 5164

ENTRYPOINT ["dotnet", "RickAndMorty.dll"]
