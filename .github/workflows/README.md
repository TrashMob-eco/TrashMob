# GitHub Actions Workflows

This directory contains automated workflows for the TrashMob project.

## Dependency Analysis Workflow

**File**: `dependency-analysis.yml`

### Purpose
Automatically analyzes dependency updates from Renovate and Dependabot, providing safety assessments and detailed change information in PR descriptions.

### When It Runs
- **Trigger**: Pull requests to `main` or `release` branches
- **Condition**: Only runs for PRs created by `renovate[bot]` or `dependabot[bot]`
- **Events**: `opened`, `synchronize`, `reopened`
- **Manual Trigger**: Can also be triggered manually via GitHub Actions UI

### Manual Trigger Options

The workflow can be manually triggered from the GitHub Actions UI with the following optional inputs:

- **pr_number**: Pull Request number to analyze (optional)
  - If provided, analyzes that specific PR
  - If omitted, analyzes the current branch against the base branch
  
- **base_ref**: Base branch to compare against (default: `main`)
  - The branch to compare dependency changes against
  - Only used when `pr_number` is not provided

**How to manually trigger:**
1. Go to the **Actions** tab in GitHub
2. Select **Dependency Update Analysis** workflow
3. Click **Run workflow**
4. Optionally provide a PR number or specify a base branch
5. Click **Run workflow** to execute

**Use cases for manual trigger:**
- Re-run analysis on an existing PR without making changes
- Analyze dependency changes on a feature branch before creating a PR
- Debug or test the dependency analysis logic

### What It Does

1. **Detects Dependency Changes**
   - Compares `package.json` files (npm dependencies)
   - Compares `.csproj` files (NuGet dependencies)
   - Identifies new, updated, and removed packages

2. **Analyzes Version Changes**
   - Uses semantic versioning to detect breaking changes
   - **Major version changes** (e.g., 1.x.x → 2.x.x): Potential breaking changes
   - **Minor/patch changes** (e.g., 1.0.0 → 1.1.0): Likely backward compatible
   - **0.x.x versions**: Treats minor version changes as potentially breaking

3. **Fetches Package Documentation**
   - Retrieves package repository URLs from npm registry
   - Retrieves project URLs from NuGet API
   - Provides direct links to changelog and release notes
   - Generates comparison links for GitHub-hosted packages

4. **Provides Safety Assessment**
   - **⚠️ REVIEW REQUIRED**: Major version changes detected
   - **✅ LIKELY SAFE**: Only minor or patch updates

5. **Updates PR Description**
   - Adds comprehensive analysis to the top of the PR description
   - Lists all dependency changes with old and new versions
   - Includes links to package documentation, changelogs, and release notes
   - Provides specific recommendations for reviewing the changes

### Example Output

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
- ⚠️ **MAJOR UPDATE**: `react` from `^18.0.0` to `^19.0.0` (potential breaking changes)
- ✅ **UPDATED**: `axios` from `^1.6.0` to `^1.6.2`

## NuGet Package Changes

### Changes in ./TrashMob/TrashMob.csproj
- ✅ **UPDATED**: `Microsoft.EntityFrameworkCore` from `10.0.1` to `10.0.2`
```

### Permissions Required
- `contents: read` - To read repository files
- `pull-requests: write` - To update PR descriptions

### Configuration
No additional configuration needed. The workflow automatically runs when Renovate or Dependabot creates/updates PRs.

### Maintenance Notes
- Node.js version: 20 (update in workflow if needed)
- .NET version: 10.0.x (should match project requirements)
- Analysis logic is based on semantic versioning conventions

### Limitations
- Analysis is based on version numbers only, not actual code changes
- Cannot detect breaking changes in packages that don't follow semantic versioning
- Requires packages to use semantic versioning format (major.minor.patch)

### Troubleshooting

**Workflow doesn't run:**
- Verify PR is from `renovate[bot]` or `dependabot[bot]`
- Check that PR targets `main` or `release` branch
- Review workflow permissions in repository settings

**Analysis is incorrect:**
- Check that dependency files follow expected formats
- Verify version numbers follow semantic versioning
- Review workflow logs for parsing errors

**PR description not updated:**
- Verify `pull-requests: write` permission is granted
- Check for GitHub API rate limiting
- Review workflow logs for API errors
