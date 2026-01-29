# Dependency Analysis Workflow - Usage Guide

## Overview

This workflow automatically analyzes dependency updates from Renovate and Dependabot, providing comprehensive safety assessments and documentation links directly in the PR description.

## How It Works

### Automatic Activation
The workflow triggers automatically when:
- A PR is created by `renovate[bot]` or `dependabot[bot]`
- An existing bot PR is updated (synchronized or reopened)
- The PR targets `main` or `release` branches

### Analysis Process

1. **Detects Changes**
   - Scans all `package.json` files (excluding node_modules)
   - Scans all `.csproj` files (excluding obj/bin directories)

2. **Fetches Package Information**
   - npm packages: Queries npmjs.org registry API
   - NuGet packages: Queries nuget.org API
   - Retrieves repository URLs, project homepages, and descriptions

3. **Analyzes Version Changes**
   - Compares old vs new versions
   - Applies semantic versioning rules
   - Special handling for 0.x.x versions

4. **Generates Links**
   - Direct links to package pages
   - Changelog/release notes URLs
   - GitHub compare links (for GitHub-hosted packages)

5. **Updates PR Description**
   - Adds analysis at the top of the PR
   - Replaces previous analysis if PR is updated
   - Preserves original PR content

## What You'll See

### For Safe Updates (Minor/Patch)
```markdown
# 🔍 Dependency Update Analysis

## ✅ Safety Assessment: LIKELY SAFE

This update contains **minor or patch version changes**.
According to semantic versioning, these changes should be backward compatible.

**Recommended Actions:**
- Run the test suite to confirm compatibility
- Monitor for any unexpected behavior after deployment
```

### For Breaking Changes (Major)
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
```

## Interpreting the Analysis

### Version Change Indicators

- ⚠️ **MAJOR UPDATE**: Potential breaking changes
  - 1.x.x → 2.x.x (major version bump)
  - 0.5.x → 0.6.x (minor version bump in 0.x.x range)

- ✅ **UPDATED**: Likely backward compatible
  - 1.0.x → 1.1.x (minor version bump)
  - 1.1.0 → 1.1.1 (patch version bump)
  - 0.5.1 → 0.5.2 (patch in 0.x.x range)

- **NEW**: Package added to dependencies

- **REMOVED**: Package removed from dependencies

### Package Documentation Links

Each package change includes helpful links:

- 📦 **Repository/Package**: Direct link to source code or package page
- 🏠 **Homepage**: Project website (if different from repository)
- 📋 **Changelog**: Release notes and version history
- 📊 **Compare**: GitHub comparison view showing exact code changes

## Best Practices

### Reviewing Updates

1. **Check the Safety Assessment**
   - Green (✅ LIKELY SAFE): Quick review, run tests
   - Orange (⚠️ REVIEW REQUIRED): Thorough review needed

2. **Use the Provided Links**
   - Click "Changelog" to see what changed
   - Click "Compare" to review actual code changes
   - Read migration guides if major version

3. **Run Tests**
   - Always run the test suite
   - Check for deprecation warnings
   - Test critical user flows

4. **Monitor After Merge**
   - Watch for errors in Application Insights
   - Check Sentry for new issues (mobile)
   - Monitor user reports

### Handling Breaking Changes

When major updates are detected:

1. **Read the Changelog**
   - Look for "Breaking Changes" section
   - Check migration guides
   - Note deprecated APIs

2. **Update Code if Needed**
   - Fix breaking API changes
   - Update deprecated usage
   - Test thoroughly

3. **Consider Alternatives**
   - Can the update wait?
   - Is there a migration path?
   - Should we stay on current version?

## Workflow Configuration

### Permissions Required
- `contents: read` - Access repository files
- `pull-requests: write` - Update PR descriptions

### Dependencies
- Node.js 20 (for npm registry API calls)
- .NET 10.0.x (for project consistency)
- curl (for NuGet API calls)

### Timeouts
- npm registry API: 10 seconds per package
- NuGet API: 10 seconds per package
- Prevents workflow hangs on slow/unavailable services

## Troubleshooting

### Workflow Doesn't Run

**Check:**
- Is the PR from `renovate[bot]` or `dependabot[bot]`?
- Does the PR target `main` or `release` branch?
- Are workflow permissions enabled in repository settings?

### Links Are Missing

**Possible Reasons:**
- Package registry API timed out (10s limit)
- Package is not hosted on GitHub
- Package metadata doesn't include repository URL

**Note:** The workflow still provides safety assessment even if links fail to fetch.

### Analysis Is Incorrect

**Common Issues:**
- Package doesn't follow semantic versioning
- Version includes pre-release tags (alpha, beta, rc)
- Package uses non-standard versioning scheme

**Solution:** Review manually using provided package links.

## Maintenance

### Updating Node.js Version
Edit workflow line 36-38:
```yaml
- name: Setup Node.js
  uses: actions/setup-node@v4
  with:
    node-version: '20'  # Update version here
```

### Updating .NET Version
Edit workflow line 41-43:
```yaml
- name: Setup .NET
  uses: actions/setup-dotnet@v5
  with:
    dotnet-version: '10.0.x'  # Update version here
```

### Adjusting Timeout
Edit workflow lines 73 (npm) and 243 (NuGet):
```javascript
timeout: 10000  // milliseconds (10 seconds)
```
```bash
curl -s --max-time 10  # seconds
```

## Examples

See `DEPENDENCY_ANALYSIS_EXAMPLES.md` for comprehensive examples of workflow output for different scenarios.

## Support

For issues or questions:
1. Check workflow logs in GitHub Actions
2. Review this documentation
3. Create an issue in the repository

## Related Documentation

- Main workflow: `.github/workflows/dependency-analysis.yml`
- Technical details: `.github/workflows/README.md`
- Output examples: `.github/workflows/DEPENDENCY_ANALYSIS_EXAMPLES.md`
- Test script: `.github/workflows/test-dependency-analysis.sh`
