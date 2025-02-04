# Base runtime image
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80

# Build image (with SDK)
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /repo

# Copy everything from the repo into /repo
COPY . .

# Move into the Presentation folder, where your main csproj/program.cs is
WORKDIR /repo/Presentation

# 1) Restore dependencies (Presentation references may also restore sibling projects if they are in the .sln)
RUN dotnet restore

# 2) Optionally build (this is sometimes skipped because 'publish' also does a build).
RUN dotnet build -c Release --no-restore

# 3) Publish to a folder
RUN dotnet publish -c Release -o /app/publish --no-build

# Final stage: produce a runtime image
FROM base AS final
WORKDIR /app

# Copy published output from the build stage
COPY --from=build /app/publish ./

# By default, the DLL name matches your .csproj. 
# If your project is "MyProject.csproj", the output will be MyProject.dll.
# Adjust the ENTRYPOINT line to the actual dll name if different.
ENTRYPOINT ["dotnet", "Presentation.dll"]
