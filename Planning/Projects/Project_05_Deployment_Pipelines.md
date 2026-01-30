# Project 5 � Deployment Pipelines & Infrastructure

| Attribute | Value |
|-----------|-------|
| **Status** | Ready for Dev Review |
| **Priority** | Critical |
| **Risk** | Moderate |
| **Size** | Medium |
| **Dependencies** | None (enables all other projects) |

---

## Business Rationale

Restore and modernize CI/CD pipelines to enable reliable, frequent deployments. Current GitHub Actions workflows have issues, mobile app store deployments are manual, and hosting costs are high due to Azure App Service usage. Modernizing to Docker containers and automating deployments will reduce downtime, lower costs, and unblock other development work.

---

## Objectives

### Primary Goals
- **Restore GitHub Actions** workflows for web, API, and function apps
- **Automate mobile app store** deployments (TestFlight, Google Play Beta)
- **Containerize applications** (web API, hourly jobs, daily jobs)
- **Set up dashboards and alerts** for deployment health

### Secondary Goals
- Reduce Azure hosting costs by 30%
- Achieve zero-downtime deployments
- Enable blue-green deployment strategy

---

## Scope

### Phase 1 - Fix Existing Pipelines
- ✅ Fix GitHub Actions workflows for web/API deployment
- ✅ Fix Azure Function App deployments (hourly/daily jobs)
- ✅ Resolve secret management issues
- ✅ Update Node.js and .NET versions in workflows
- ☐ Automate Renovate/Dependabot PR handling (see Dependency Automation section below)

### Phase 2 - Containerization
- ✅ Create Dockerfiles for web API project
- ✅ Create Dockerfiles for function apps
- ✅ Set up Azure Container Registry
- ✅ Deploy to Azure Container Apps (ACA) instead of App Service

### Phase 3 - Mobile Automation Review
- ☐ Review existing iOS build and TestFlight upload workflow
- ☐ Review existing Android build and Google Play Beta upload workflow
- ☐ Verify versioning strategy is working correctly
- ☐ Verify code signing certificate management
- ☐ Document current workflows and identify any improvements

### Phase 4 - Monitoring
- ? Deployment health dashboards (Grafana or Azure Monitor)
- ? Alerting for failed deployments
- ? Rollback automation
- ? Deployment metrics tracking

### Phase 5 - Cost Optimization
- ? Analyze current Azure spend
- ? Right-size container resources
- ? Implement auto-scaling policies
- ? Set up budget alerts

---

## Out-of-Scope

- ? Kubernetes (ACA is sufficient for now)
- ? Multi-region deployments
- ? Advanced blue-green or canary releases (simple staged rollout only)
- ? Infrastructure as Code with Terraform (using Bicep)

---

## Success Metrics

### Quantitative
- **Deployment frequency:** From 1/week to 5+/week
- **Deployment time:** Reduce from 30 min to ? 10 min
- **Deployment failure rate:** ? 5%
- **Rollback time:** ? 5 minutes
- **Azure hosting costs:** Reduce by 30%
- **Downtime during deployments:** ? 30 seconds (approaching zero)

### Qualitative
- Developers confident in deploying frequently
- Zero manual steps required for deployment
- Clear visibility into deployment status

---

## Dependencies

### Blockers
None (this is a foundational project)

### Enables
- **All projects:** Reliable deployments are required for delivering features
- **Project 4 (Mobile):** Automated builds enable faster iteration
- **Project 24 (API v2):** Containerization supports API versioning

---

## Risks & Mitigations

| Risk | Likelihood | Impact | Mitigation |
|------|------------|--------|------------|
| **Containerization breaks existing functionality** | Medium | High | Thorough testing in staging; maintain App Service as fallback |
| **Mobile code signing issues** | High | High | Early setup of certificates; backup manual process |
| **Database migration failures** | Low | Critical | Pre-deployment validation; manual rollback procedure |
| **Cost savings don't materialize** | Medium | Low | Monitor closely; adjust resources; consider reserved instances |
| **GitHub Actions outage** | Low | High | Have manual deployment procedure documented |

---

## Implementation Plan

### Infrastructure Changes

**Current Architecture:**
```
GitHub ? GitHub Actions ? Azure App Service (web/API)
                      ? Azure Function Apps (jobs)
```

**Target Architecture:**
```
GitHub ? GitHub Actions ? Docker Build ? Azure Container Registry
                                    ? Azure Container Apps (web/API/jobs)
```

**Bicep Files (Infrastructure as Code):**

```bicep
// containerAppWebApi.bicep
resource webApi 'Microsoft.App/containerApps@2023-05-01' = {
  name: 'trashmob-web-api'
  location: location
  properties: {
    configuration: {
      ingress: {
        external: true
        targetPort: 8080
      }
      secrets: [
        {
          name: 'connection-string'
          value: connectionString
        }
      ]
    }
    template: {
      containers: [
        {
          name: 'web-api'
          image: '${acrName}.azurecr.io/trashmob-web-api:${imageTag}'
          resources: {
            cpu: 0.5
            memory: '1.0Gi'
          }
        }
      ]
      scale: {
        minReplicas: 1
        maxReplicas: 10
      }
    }
  }
}
```

### Dockerfile Examples

**Web API Dockerfile:**

```dockerfile
# TrashMob/Dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["TrashMob/TrashMob.csproj", "TrashMob/"]
COPY ["TrashMob.Models/TrashMob.Models.csproj", "TrashMob.Models/"]
COPY ["TrashMob.Shared/TrashMob.Shared.csproj", "TrashMob.Shared/"]
RUN dotnet restore "TrashMob/TrashMob.csproj"
COPY . .
WORKDIR "/src/TrashMob"
RUN dotnet build "TrashMob.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TrashMob.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TrashMob.dll"]
```

### GitHub Actions Workflows

**Web API Deployment:**

```yaml
name: Deploy Web API

on:
  push:
    branches: [main]
    paths:
      - 'TrashMob/**'
      - 'TrashMob.Models/**'
      - 'TrashMob.Shared/**'

jobs:
  build-and-deploy:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      
      - name: Set up Docker Buildx
        uses: docker/setup-buildx-action@v3
      
      - name: Login to Azure Container Registry
        uses: docker/login-action@v3
        with:
          registry: ${{ secrets.ACR_NAME }}.azurecr.io
          username: ${{ secrets.ACR_USERNAME }}
          password: ${{ secrets.ACR_PASSWORD }}
      
      - name: Build and push
        uses: docker/build-push-action@v5
        with:
          context: .
          file: ./TrashMob/Dockerfile
          push: true
          tags: |
            ${{ secrets.ACR_NAME }}.azurecr.io/trashmob-web-api:latest
            ${{ secrets.ACR_NAME }}.azurecr.io/trashmob-web-api:${{ github.sha }}
      
      - name: Deploy to Azure Container Apps
        uses: azure/container-apps-deploy-action@v1
        with:
          resource-group: ${{ secrets.RESOURCE_GROUP }}
          container-app-name: trashmob-web-api
          image: ${{ secrets.ACR_NAME }}.azurecr.io/trashmob-web-api:${{ github.sha }}
```

**Mobile App (iOS):**

```yaml
name: iOS Build and Deploy

on:
  push:
    branches: [main]
    paths:
      - 'TrashMobMobile/**'

jobs:
  build-ios:
    runs-on: macos-latest
    steps:
      - uses: actions/checkout@v4
      
      - name: Setup .NET
        uses: actions/setup-dotnet@v4
        with:
          dotnet-version: '10.0.x'
      
      - name: Install MAUI workload
        run: dotnet workload install maui
      
      - name: Restore dependencies
        run: dotnet restore TrashMobMobile/TrashMobMobile.csproj
      
      - name: Build iOS
        run: dotnet build TrashMobMobile/TrashMobMobile.csproj -c Release -f net10.0-ios
      
      - name: Upload to TestFlight
        env:
          APPLE_ID: ${{ secrets.APPLE_ID }}
          APPLE_PASSWORD: ${{ secrets.APPLE_PASSWORD }}
        run: |
          xcrun altool --upload-app \
            --file TrashMobMobile/bin/Release/net10.0-ios/TrashMob.ipa \
            --type ios \
            --username "$APPLE_ID" \
            --password "$APPLE_PASSWORD"
```

### Dependency Automation Workflow

**Goal:** Automatically handle Renovate and Dependabot PRs with zero manual intervention for non-breaking updates.

**Workflow Steps:**

1. **Renovate/Dependabot creates PR** with dependency update
2. **CI runs full build and test suite** on the PR
3. **If build/tests pass:**
   - Auto-approve and merge (for patch/minor updates)
   - For major updates: create summary comment, await manual review
4. **If build/tests fail:**
   - Use Claude agent to analyze the failure
   - Agent attempts to fix breaking changes automatically
   - Agent commits fixes to the PR branch
   - Re-run CI to verify fixes
   - If fixed: auto-merge; if not: flag for manual review

**GitHub Actions Workflow:**

```yaml
name: Dependency Update Automation

on:
  pull_request:
    types: [opened, synchronize]

jobs:
  check-dependency-pr:
    if: github.actor == 'renovate[bot]' || github.actor == 'dependabot[bot]'
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
        with:
          ref: ${{ github.head_ref }}
          fetch-depth: 0

      - name: Setup .NET
        uses: actions/setup-dotnet@v4
        with:
          dotnet-version: '10.0.x'

      - name: Build and Test
        id: build
        continue-on-error: true
        run: |
          dotnet build --configuration Release
          dotnet test --configuration Release --no-build

      - name: Auto-fix Breaking Changes
        if: steps.build.outcome == 'failure'
        uses: anthropics/claude-code-action@v1
        with:
          prompt: |
            The dependency update PR has build/test failures.
            Analyze the errors and fix any breaking changes.
            Common fixes include:
            - Updating API calls for new library versions
            - Fixing type signature changes
            - Updating deprecated method calls
            - Adjusting configuration for new package versions
            Do not change functionality, only fix compatibility issues.
          allowed_tools: "Edit,Read,Bash"

      - name: Commit Fixes
        if: steps.build.outcome == 'failure'
        run: |
          git config user.name "github-actions[bot]"
          git config user.email "github-actions[bot]@users.noreply.github.com"
          git add -A
          git diff --staged --quiet || git commit -m "fix: Auto-fix breaking changes from dependency update"
          git push

      - name: Re-run Build After Fixes
        if: steps.build.outcome == 'failure'
        run: |
          dotnet build --configuration Release
          dotnet test --configuration Release --no-build

      - name: Auto-merge on Success
        if: success()
        uses: pascalgn/automerge-action@v0.16.3
        env:
          GITHUB_TOKEN: ${{ secrets.GITHUB_TOKEN }}
          MERGE_METHOD: squash
          MERGE_COMMIT_MESSAGE: "chore: Update dependencies"
```

**Configuration (renovate.json):**

```json
{
  "$schema": "https://docs.renovatebot.com/renovate-schema.json",
  "extends": ["config:recommended"],
  "automerge": true,
  "automergeType": "pr",
  "packageRules": [
    {
      "matchUpdateTypes": ["patch", "minor"],
      "automerge": true
    },
    {
      "matchUpdateTypes": ["major"],
      "automerge": false,
      "labels": ["major-update", "needs-review"]
    }
  ],
  "schedule": ["before 6am on Monday"]
}
```

**Safety Checks:**
- All tests must pass before auto-merge
- Major version updates always require human review
- Security updates prioritized and processed immediately
- Failed auto-fix attempts are flagged for manual intervention
- Maintain audit log of all automated merges

---

## Implementation Phases

### Phase 1: Pipeline Fixes
- Audit existing workflows
- Fix failing builds
- Update dependencies
- Test deployments to staging
- Implement Renovate/Dependabot automation with auto-fix

### Phase 2: Containerization
- Create Dockerfiles
- Set up ACR
- Deploy to ACA (staging)
- Performance testing

### Phase 3: Mobile Automation
- Review existing iOS/Android build workflows
- Verify TestFlight upload process
- Verify Google Play Beta upload process
- Document current workflow and any improvements needed

### Phase 4: Monitoring
- Set up dashboards
- Configure alerts
- Test rollback procedures
- Document runbooks

### Phase 5: Cost Optimization
- Analyze spend
- Right-size resources
- Implement auto-scaling
- Set budget alerts

**Note:** Pipeline fixes (Phase 1) are prerequisite for all other work.

---

## Open Questions

1. **Should we migrate to Azure Container Apps or Azure Kubernetes Service (AKS)?**
   **Decision:** Azure Container Apps (ACA) for simplicity. AKS would only be needed for advanced orchestration which is not required.
   **Status:** Decided

2. **What's our rollback strategy?**
   **Decision:** Keep 3 previous container images in ACR; redeploy previous revision using Azure CLI. Documentation added to root CLAUDE.md.
   **Status:** Decided

3. **Do we need staging and production environments?**
   **Decision:** Yes. We have dev environment (dev.trashmob.eco) for staging and production (www.trashmob.eco). Deploy to dev first, then production after validation.
   **Status:** Decided

4. **What's the versioning strategy for mobile apps?**
   **Decision:** Semantic versioning with auto-increment build number.
   **Status:** Decided

---

## Related Documents

- **[Project 4 - Mobile Robustness](./Project_04_Mobile_Robustness.md)** - Benefits from automated builds
- **[Project 24 - API v2](./Project_24_API_v2_Modernization.md)** - Containerization supports API versioning
- **Infrastructure:** `/Deploy` folder with Bicep files

---

**Last Updated:** January 29, 2026
**Owner:** DevOps/Build Engineer
**Status:** In Progress
**Next Review:** Regular standups during development
