# Stage 1
FROM mcr.microsoft.com/dotnet/core/sdk:2.2 AS builder
WORKDIR /source

# caches restore result by copying csproj file separately
#COPY /NuGet.config /source/
COPY /DeveRecupDirSorter/*.csproj /source/DeveRecupDirSorter/
COPY /DeveRecupDirSorter.ConsoleApp/*.csproj /source/DeveRecupDirSorter.ConsoleApp/
COPY /DeveRecupDirSorter.Tests/*.csproj /source/DeveRecupDirSorter.Tests/
COPY /DeveRecupDirSorter.sln /source/
RUN ls
RUN dotnet restore

# copies the rest of your code
COPY . .
RUN dotnet build --configuration Release
RUN dotnet test --configuration Release ./DeveRecupDirSorter.Tests/DeveRecupDirSorter.Tests.csproj
RUN dotnet publish ./DeveRecupDirSorter.ConsoleApp/DeveRecupDirSorter.ConsoleApp.csproj --output /app/ --configuration Release

# Stage 2
FROM mcr.microsoft.com/dotnet/core/runtime:2.2-alpine3.9
WORKDIR /app
COPY --from=builder /app .
ENTRYPOINT ["dotnet", "DeveRecupDirSorter.ConsoleApp.dll"]