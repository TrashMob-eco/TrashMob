# Example: Dependency Analysis Workflow Output

This document shows what the dependency analysis workflow will add to PR descriptions.

## Example 1: Major Version Update (Breaking Changes Possible)

When Renovate or Dependabot creates a PR with major version updates, the workflow will add this to the PR description:

```markdown
# 🔍 Dependency Update Analysis

## ⚠️ Safety Assessment: REVIEW REQUIRED

This update contains **major version changes** that may include breaking changes.
Please review the changelog and test thoroughly before merging.

**Recommended Actions:**
- Review the package changelog for breaking changes
- Run the full test suite
- Test critical functionality manually
- Check for API changes in updated packages

---

## NPM Package Changes

### Changes in ./TrashMob/client-app
- ⚠️ **MAJOR UPDATE**: `@tanstack/react-query` from `^5.0.0` to `^6.0.0` (potential breaking changes)
  - 📦 Repository: https://github.com/TanStack/query
  - 📋 Changelog: https://github.com/TanStack/query/releases
  - 📊 Compare: https://github.com/TanStack/query/compare/v5.0.0...v6.0.0
- ✅ **UPDATED**: `axios` from `^1.6.0` to `^1.6.5`
  - 📦 Repository: https://github.com/axios/axios
  - 📋 Changelog: https://github.com/axios/axios/releases
  - 📊 Compare: https://github.com/axios/axios/compare/v1.6.0...v1.6.5

### Changes in ./Strapi
- ✅ **UPDATED**: `@strapi/strapi` from `5.6.0` to `5.6.1`
  - 📦 Repository: https://github.com/strapi/strapi
  - 📋 Changelog: https://github.com/strapi/strapi/releases
  - 📊 Compare: https://github.com/strapi/strapi/compare/v5.6.0...v5.6.1

## NuGet Package Changes

### Changes in ./TrashMob/TrashMob.csproj
- ✅ **UPDATED**: `Microsoft.EntityFrameworkCore` from `10.0.1` to `10.0.2`
  - 📦 NuGet: https://www.nuget.org/packages/Microsoft.EntityFrameworkCore/10.0.2
  - 🏠 Project: https://github.com/dotnet/efcore
  - 📋 Releases: https://github.com/dotnet/efcore/releases
  - 📊 Compare: https://github.com/dotnet/efcore/compare/v10.0.1...v10.0.2

### Changes in ./TrashMob.Shared/TrashMob.Shared.csproj
- ✅ **UPDATED**: `Azure.Identity` from `1.13.0` to `1.13.1`
  - 📦 NuGet: https://www.nuget.org/packages/Azure.Identity/1.13.1
  - 🏠 Project: https://github.com/Azure/azure-sdk-for-net
  - 📋 Releases: https://github.com/Azure/azure-sdk-for-net/releases
  - 📊 Compare: https://github.com/Azure/azure-sdk-for-net/compare/v1.13.0...v1.13.1

---

*Analysis performed by [dependency-analysis.yml](https://github.com/TrashMob-eco/TrashMob/blob/main/.github/workflows/dependency-analysis.yml)*
```

## Example 2: Minor/Patch Updates (Likely Safe)

When Renovate or Dependabot creates a PR with only minor or patch updates:

```markdown
# 🔍 Dependency Update Analysis

## ✅ Safety Assessment: LIKELY SAFE

This update contains **minor or patch version changes**.
According to semantic versioning, these changes should be backward compatible.

**Recommended Actions:**
- Run the test suite to confirm compatibility
- Monitor for any unexpected behavior after deployment

---

## NPM Package Changes

### Changes in ./TrashMob/client-app
- ✅ **UPDATED**: `axios` from `^1.6.0` to `^1.6.5`
  - 📦 Repository: https://github.com/axios/axios
  - 📋 Changelog: https://github.com/axios/axios/releases
  - 📊 Compare: https://github.com/axios/axios/compare/v1.6.0...v1.6.5
- ✅ **UPDATED**: `typescript` from `5.8.0` to `5.8.2`
  - 📦 Repository: https://github.com/microsoft/TypeScript
  - 🏠 Homepage: https://www.typescriptlang.org
  - 📋 Changelog: https://github.com/microsoft/TypeScript/releases
  - 📊 Compare: https://github.com/microsoft/TypeScript/compare/v5.8.0...v5.8.2

### Changes in ./Strapi
- ✅ **UPDATED**: `@strapi/strapi` from `5.6.0` to `5.6.1`
  - 📦 Repository: https://github.com/strapi/strapi
  - 📋 Changelog: https://github.com/strapi/strapi/releases
  - 📊 Compare: https://github.com/strapi/strapi/compare/v5.6.0...v5.6.1

## NuGet Package Changes

### Changes in ./TrashMob/TrashMob.csproj
- ✅ **UPDATED**: `Microsoft.EntityFrameworkCore` from `10.0.1` to `10.0.2`
  - 📦 NuGet: https://www.nuget.org/packages/Microsoft.EntityFrameworkCore/10.0.2
  - 🏠 Project: https://github.com/dotnet/efcore
  - 📋 Releases: https://github.com/dotnet/efcore/releases
  - 📊 Compare: https://github.com/dotnet/efcore/compare/v10.0.1...v10.0.2
- ✅ **UPDATED**: `Swashbuckle.AspNetCore` from `7.2.0` to `7.2.1`
  - 📦 NuGet: https://www.nuget.org/packages/Swashbuckle.AspNetCore/7.2.1
  - 🏠 Project: https://github.com/domaindrivendev/Swashbuckle.AspNetCore
  - 📋 Releases: https://github.com/domaindrivendev/Swashbuckle.AspNetCore/releases
  - 📊 Compare: https://github.com/domaindrivendev/Swashbuckle.AspNetCore/compare/v7.2.0...v7.2.1

---

*Analysis performed by [dependency-analysis.yml](https://github.com/TrashMob-eco/TrashMob/blob/main/.github/workflows/dependency-analysis.yml)*
```

## Example 3: New Package Added

When a new package is added to the project:

```markdown
# 🔍 Dependency Update Analysis

## ✅ Safety Assessment: LIKELY SAFE

This update contains **minor or patch version changes**.
According to semantic versioning, these changes should be backward compatible.

**Recommended Actions:**
- Run the test suite to confirm compatibility
- Monitor for any unexpected behavior after deployment

---

## NPM Package Changes

### Changes in ./TrashMob/client-app
- **NEW**: `@radix-ui/react-dialog` @ `^1.0.5`
  - 📦 Repository: https://github.com/radix-ui/primitives
  - 🏠 Homepage: https://www.radix-ui.com
- ✅ **UPDATED**: `react` from `^18.3.0` to `^18.3.1`
  - 📦 Repository: https://github.com/facebook/react
  - 📋 Changelog: https://github.com/facebook/react/releases
  - 📊 Compare: https://github.com/facebook/react/compare/v18.3.0...v18.3.1

## NuGet Package Changes

No dependency changes detected.

---

*Analysis performed by [dependency-analysis.yml](https://github.com/TrashMob-eco/TrashMob/blob/main/.github/workflows/dependency-analysis.yml)*
```

## Example 4: Multiple Projects Updated

When multiple package.json or .csproj files are updated in a single PR:

```markdown
# 🔍 Dependency Update Analysis

## ⚠️ Safety Assessment: REVIEW REQUIRED

This update contains **major version changes** that may include breaking changes.
Please review the changelog and test thoroughly before merging.

**Recommended Actions:**
- Review the package changelog for breaking changes
- Run the full test suite
- Test critical functionality manually
- Check for API changes in updated packages

---

## NPM Package Changes

### Changes in ./TrashMob/client-app
- ⚠️ **MAJOR UPDATE**: `vite` from `^7.0.0` to `^8.0.0` (potential breaking changes)
- ✅ **UPDATED**: `@types/react` from `^18.3.0` to `^18.3.12`

### Changes in ./Strapi
- ✅ **UPDATED**: `@strapi/strapi` from `5.6.0` to `5.6.1`

## NuGet Package Changes

### Changes in ./TrashMob/TrashMob.csproj
- ✅ **UPDATED**: `Microsoft.EntityFrameworkCore` from `10.0.1` to `10.0.2`
- ✅ **UPDATED**: `Azure.Storage.Blobs` from `12.22.0` to `12.22.1`

### Changes in ./TrashMob.Shared/TrashMob.Shared.csproj
- ✅ **UPDATED**: `Azure.Identity` from `1.13.0` to `1.13.1`

### Changes in ./TrashMobMobile/TrashMobMobile.csproj
- ✅ **UPDATED**: `Microsoft.Maui.Controls` from `10.0.1` to `10.0.2`

---

*Analysis performed by [dependency-analysis.yml](https://github.com/TrashMob-eco/TrashMob/blob/main/.github/workflows/dependency-analysis.yml)*
```

## Key Features

1. **Clear Visual Indicators**
   - ⚠️ for major updates (potential breaking changes)
   - ✅ for minor/patch updates (likely safe)
   - **NEW** for newly added packages

2. **Direct Links to Documentation**
   - 📦 Repository/Homepage links for all packages
   - 📋 Direct links to changelog/release notes (for GitHub-hosted packages)
   - 📊 Version comparison links showing exact changes between versions
   - 🏠 Project homepage links when different from repository

3. **Organized by Project**
   - Separate sections for each package.json or .csproj file
   - Shows the relative path to help locate the file

4. **Actionable Recommendations**
   - Different guidance based on change severity
   - Specific steps to take before merging

5. **Version Comparison**
   - Shows both old and new versions
   - Makes it easy to assess the magnitude of change
   - Provides direct GitHub compare links for detailed diffs

6. **Automatic Placement**
   - Always appears at the top of the PR description
   - Previous analysis is replaced when PR is updated
   - Original PR content is preserved below the analysis

7. **Multi-Ecosystem Support**
   - npm packages with npmjs.org integration
   - NuGet packages with nuget.org integration
   - GitHub repository detection for both ecosystems
   - Intelligent changelog link generation
