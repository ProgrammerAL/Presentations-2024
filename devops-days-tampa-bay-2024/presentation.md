---
marp: true
title: DevOps like a Dev
paginate: true
theme: gaia
author: Al Rodriguez
---

# DevOps like a Dev

with AL Rodriguez

---

# Me (AL)

- @ProgrammerAL
- ProgrammerAL.com
- Senior Azure Cloud Engineer at Microsoft

---

# DevOps History

- First DevOps Days held in Ghent, Belgium in 2007
- DevOps becomes a cool term
- DevOps Engineer role is born

---

# What is a DevOps Engineer TODAY?

- No Single Definition
- For This Session:
  - CI/CD
  - Cloud Stuff
  - Containers!
  - Kubernetes?

---

# Who here does DevOps?

---

# DevOps - Devs And Ops working together

- Not throwing work over the wall
- Almost the same team

---

# Dev == Automation

- Automate more
  - More than that
    - And more than that

---

# YAML is Everywhere

- A DSL
- Custom to each provider
- Write-Once

---

# YAML can get big

```yaml
name: CI/CD Pipeline

on:
  push:
    branches:
      - main
  pull_request:
    branches:
      - main
  schedule:
    - cron: '0 0 * * 0' # Runs every Sunday at midnight

jobs:
  build:
    runs-on: ${{ matrix.os }}
    strategy:
      matrix:
        os: [ubuntu-latest, windows-latest, macos-latest]
        dotnet-version: [6.0.x, 7.0.x]

    steps:
      - name: Checkout code
        uses: actions/checkout@v4

      - name: Setup .NET
        uses: actions/setup-dotnet@v3
        with:
          dotnet-version: ${{ matrix.dotnet-version }}

      - name: Restore dependencies
        run: dotnet restore

      - name: Build
        run: dotnet build --no-restore --configuration Release

      - name: Test
        run: dotnet test --no-build --verbosity normal

      - name: Publish
        if: github.ref == 'refs/heads/main'
        run: dotnet publish --no-build --configuration Release --output ./publish

  lint:
    runs-on: ubuntu-latest
    steps:
      - name: Checkout code
        uses: actions/checkout@v4

      - name: Setup .NET
        uses: actions/setup-dotnet@v3
        with:
          dotnet-version: 7.0.x

      - name: Install .NET tools
        run: dotnet tool install --global dotnet-format

      - name: Run linters
        run: dotnet format --check

  deploy:
    runs-on: ubuntu-latest
    needs: build
    steps:
      - name: Checkout code
        uses: actions/checkout@v4

      - name: Setup .NET
        uses: actions/setup-dotnet@v3
        with:
          dotnet-version: 7.0.x

      - name: Deploy to Azure
        uses: azure/webapps-deploy@v2
        with:
          app-name: 'your-app-name'
          publish-profile: ${{ secrets.AZURE_WEBAPP_PUBLISH_PROFILE }}
          package: ./publish

  cache:
    runs-on: ubuntu-latest
    steps:
      - name: Checkout code
        uses: actions/checkout@v4

      - name: Cache .NET packages
        uses: actions/cache@v3
        with:
          path: ~/.nuget/packages
          key: ${{ runner.os }}-nuget-${{ hashFiles('**/packages.lock.json') }}
          restore-keys: |
            ${{ runner.os }}-nuget-

      - name: Restore dependencies
        run: dotnet restore

      - name: Build
        run: dotnet build --no-restore --configuration Release

      - name: Test
        run: dotnet test --no-build --verbosity normal

  matrix:
    runs-on: ${{ matrix.os }}
    strategy:
      matrix:
        os: [ubuntu-latest, windows-latest, macos-latest]
        node-version: [14.x, 16.x]

    steps:
      - name: Checkout code
        uses: actions/checkout@v4

      - name: Setup Node.js
        uses: actions/setup-node@v3
        with:
          node-version: ${{ matrix.node-version }}

      - name: Install dependencies
        run: npm install

```
---

# YAML Tooling isn't as Mature as Other Dev Tools

```yaml
on: [push]

jobs:
  publish:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v2
      - uses: my-org/publish-docker@v1
        with:
          registry_username: ${{secrets.REGISTRY_USERNAME}}
          registry_password: ${{secrets.REGISTRY_PASSWORD}}
```

---

# General Purpose Programming Languages to the Rescue

- Write more code!
- Specialty Frameworks
  - C# Cake, C# Nuke, PowerShell, Dagger.io
- Or whatever you want

---

# Example: GitHub Actions with C# Cake

```yaml
name: Build and Deploy

on:
  push:
    branches: [main]

jobs:
  build-and-publish:
    runs-on: ubuntu-latest

    steps:
      - uses: actions/checkout@v2
      - name: Setup .NET
        uses: actions/setup-dotnet@v1
        with:
          dotnet-version: ${{ env.DOTNET_VERSION }}

      - name: Cake - Build
        run: dotnet run --project build/build/Build.csproj -- --configuration=${{ env.CONFIGURATION }} --srcDirectoryPath=${{ env.SRC_DIRECTORY_PATH }} --BuildArtifactsPath=${{ env.BUILD_ARTIFACTS_PATH }}

      - name: Cake - Deploy
        run: dotnet run --project ${{ github.workspace }}/deploy/deploy/Deploy.csproj -- --configuration=${{ env.CONFIGURATION }} --WorkspacePath=${{ github.workspace }} --BuildArtifactsPath=${{ env.BUILD_ARTIFACTS_PATH }}
```

---

# Automation Task: Automate Manual Processes for Deployment

- Example: Cosmos DB Indexes
  - Work with Devs to know what indexes should be enabled/disabled

---

# Key Takeaways

- More automation
- Less config/DSLs

---

# PRs!

- You're Devs too
- Pull Requests

---

# TODO:

- PRs for Infra changes, like permissions
  - Code has history, gets reviews, etc

---

# Online Info

- @ProgrammerAL
- programmerAL.com

