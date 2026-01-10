# Use the official ASP.NET Core runtime as the base image
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80

# Use the .NET SDK to build the application
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

# Copy the project files for the main project and its dependencies
COPY ["src/BulkyWeb/BulkyBook.csproj", "src/BulkyWeb/"]
COPY ["src/Bulky.DataAccess/BulkyBook.DataAccess.csproj", "src/Bulky.DataAccess/"]
COPY ["src/Bulky.Models/BulkyBook.Models.csproj", "src/Bulky.Models/"]
COPY ["src/Bulky.Utility/BulkyBook.Utility.csproj", "src/Bulky.Utility/"]

# Restore dependencies
RUN dotnet restore "src/BulkyWeb/BulkyBook.csproj"

# Copy all source files
COPY . .

# Set the working directory to the main project

RUN dotnet build "src/BulkyWeb/BulkyBook.csproj" -c Release -o /app/build --verbosity detailed

# Publish the application
FROM build AS publish
RUN dotnet publish "src/BulkyWeb/BulkyBook.csproj" -c Release -o /app/publish

# Use the base image and copy the published files
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BulkyBook.dll"]
