<#
.SYNOPSIS
    Migrates users from Azure B2C to Entra External ID for the production environment.

.DESCRIPTION
    Phase 1: Exports all users from the B2C production tenant via Microsoft Graph API.
    Phase 2: Imports email/password users into Entra External ID with random passwords.

    Social-only users (Google, Facebook, Microsoft) are skipped — they self-migrate
    when they sign in via their social IDP on the Entra sign-in page.

    Email/password users are pre-created in Entra with random passwords. They use
    "Forgot password" on first sign-in to set a new password.

    The script is interactive and uses `az login` for authentication. No app
    registration secrets or service principals are needed.

    Requires: PowerShell 7+, Azure CLI

.PARAMETER SkipExport
    Skip Phase 1 (export) and use an existing export file. Useful when re-running
    Phase 2 after fixing issues.

.PARAMETER ExportOnly
    Only run Phase 1 (export). Useful for reviewing exported data before importing.

.PARAMETER ExportFile
    Path to the export JSON file. Defaults to Deploy/b2c-prod-users-export.json.

.PARAMETER DryRun
    Show what would be imported without actually creating users in Entra.

.EXAMPLE
    # Full migration (export + import)
    .\migrate-b2c-users-prod.ps1

    # Export only (review before importing)
    .\migrate-b2c-users-prod.ps1 -ExportOnly

    # Import only (using existing export)
    .\migrate-b2c-users-prod.ps1 -SkipExport

    # Dry run (see what would happen)
    .\migrate-b2c-users-prod.ps1 -DryRun
#>

param(
    [switch]$SkipExport,
    [switch]$ExportOnly,
    [string]$ExportFile = "$PSScriptRoot/b2c-prod-users-export.json",
    [switch]$DryRun
)

$ErrorActionPreference = 'Stop'

# Tenant configuration — PRODUCTION
$B2CTenantDomain = 'trashmob.onmicrosoft.com'
$EntraTenantId = 'b5fc8717-29eb-496e-8e09-cf90d344ce9f'
$EntraTenantDomain = 'trashmobecopr.onmicrosoft.com'

function Get-GraphToken {
    <#
    .SYNOPSIS
        Gets a Graph API access token from the current az login session.
    #>
    $token = az account get-access-token --resource https://graph.microsoft.com --query accessToken -o tsv 2>$null
    if (-not $token) {
        Write-Error "Failed to get Graph API token. Run 'az login' first."
        exit 1
    }
    return $token
}

function Invoke-GraphApi {
    <#
    .SYNOPSIS
        Calls Microsoft Graph API with automatic pagination.
    #>
    param(
        [string]$Method = 'GET',
        [string]$Url,
        [string]$Token,
        [object]$Body = $null
    )

    $headers = @{
        'Authorization' = "Bearer $Token"
        'Content-Type'  = 'application/json'
        'ConsistencyLevel' = 'eventual'
    }

    $allResults = @()

    if ($Method -eq 'GET') {
        $currentUrl = $Url
        do {
            $response = Invoke-RestMethod -Method GET -Uri $currentUrl -Headers $headers
            if ($response.value) {
                $allResults += $response.value
            }
            $currentUrl = $response.'@odata.nextLink'
        } while ($currentUrl)

        return $allResults
    }
    else {
        $bodyJson = if ($Body -is [string]) { $Body } else { $Body | ConvertTo-Json -Depth 10 }
        return Invoke-RestMethod -Method $Method -Uri $Url -Headers $headers -Body $bodyJson
    }
}

function Get-RandomPassword {
    <#
    .SYNOPSIS
        Generates a random 16-character password meeting Entra complexity requirements.
    #>
    $upper = 'ABCDEFGHJKLMNPQRSTUVWXYZ'
    $lower = 'abcdefghjkmnpqrstuvwxyz'
    $digits = '23456789'
    $special = '!@#$%&*?'

    # Ensure at least one of each category
    $password = @(
        $upper[(Get-Random -Maximum $upper.Length)]
        $lower[(Get-Random -Maximum $lower.Length)]
        $digits[(Get-Random -Maximum $digits.Length)]
        $special[(Get-Random -Maximum $special.Length)]
    )

    # Fill remaining 12 characters randomly from all categories
    $allChars = $upper + $lower + $digits + $special
    for ($i = 0; $i -lt 12; $i++) {
        $password += $allChars[(Get-Random -Maximum $allChars.Length)]
    }

    # Shuffle the array
    $password = $password | Sort-Object { Get-Random }

    return -join $password
}

function Classify-UserIdentities {
    <#
    .SYNOPSIS
        Classifies a B2C user's identities into email/password vs social.
    .OUTPUTS
        Object with Email (string or $null) and SocialProviders (string array).
    #>
    param([object]$User)

    $email = $null
    $socialProviders = @()

    foreach ($identity in $User.identities) {
        switch ($identity.signInType) {
            'emailAddress' {
                $email = $identity.issuerAssignedId
            }
            'federated' {
                $socialProviders += $identity.issuer
            }
            'userPrincipalName' {
                # Internal B2C identity — ignore
            }
        }
    }

    # Fall back to mail property if no email identity found
    if (-not $email -and $User.mail) {
        $email = $User.mail
    }

    return [PSCustomObject]@{
        Email           = $email
        SocialProviders = $socialProviders
        HasLocalAccount = ($null -ne $email -and $User.identities.signInType -contains 'emailAddress')
    }
}

# ============================================================
# Phase 1: Export users from B2C
# ============================================================

if (-not $SkipExport) {
    Write-Host ""
    Write-Host "========================================" -ForegroundColor Cyan
    Write-Host " Phase 1: Export users from B2C (PROD)" -ForegroundColor Cyan
    Write-Host "========================================" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "B2C Tenant: $B2CTenantDomain" -ForegroundColor White
    Write-Host ""

    # Prompt user to log in to B2C tenant
    Write-Host "You need to be logged into the B2C tenant." -ForegroundColor Yellow
    Write-Host "Run: az login --tenant $B2CTenantDomain --allow-no-subscriptions" -ForegroundColor Yellow
    Write-Host ""
    $response = Read-Host "Press Enter when ready (or 'q' to quit)"
    if ($response -eq 'q') { exit 0 }

    # Verify tenant
    $currentTenant = az account show --query tenantId -o tsv 2>$null
    $b2cTenantInfo = az account show --query "{tenantId:tenantId,name:name}" -o json 2>$null | ConvertFrom-Json
    Write-Host "Current tenant: $($b2cTenantInfo.name) ($($b2cTenantInfo.tenantId))" -ForegroundColor White

    # Get token and export users
    $token = Get-GraphToken

    Write-Host ""
    Write-Host "Fetching users from B2C..." -ForegroundColor White

    $selectFields = 'id,displayName,mail,givenName,surname,userPrincipalName,identities,createdDateTime,accountEnabled'
    $users = Invoke-GraphApi -Url "https://graph.microsoft.com/v1.0/users?`$select=$selectFields&`$top=100" -Token $token

    Write-Host "Exported $($users.Count) users from B2C" -ForegroundColor Green

    # Classify users
    $classified = @()
    foreach ($user in $users) {
        $identityInfo = Classify-UserIdentities -User $user
        $classified += [PSCustomObject]@{
            Id              = $user.id
            DisplayName     = $user.displayName
            Mail            = $user.mail
            GivenName       = $user.givenName
            Surname         = $user.surname
            UPN             = $user.userPrincipalName
            Identities      = $user.identities
            CreatedDateTime = $user.createdDateTime
            AccountEnabled  = $user.accountEnabled
            Email           = $identityInfo.Email
            SocialProviders = $identityInfo.SocialProviders
            HasLocalAccount = $identityInfo.HasLocalAccount
            MigrationType   = if ($identityInfo.HasLocalAccount) { 'email_password' } elseif ($identityInfo.SocialProviders.Count -gt 0) { 'social_only' } else { 'unknown' }
        }
    }

    # Save export
    $exportData = [PSCustomObject]@{
        ExportDate    = (Get-Date -Format 'o')
        SourceTenant  = $B2CTenantDomain
        TotalUsers    = $users.Count
        Users         = $classified
    }

    $exportData | ConvertTo-Json -Depth 10 | Set-Content -Path $ExportFile -Encoding utf8
    Write-Host "Export saved to: $ExportFile" -ForegroundColor Green

    # Summary
    $emailPasswordCount = ($classified | Where-Object { $_.MigrationType -eq 'email_password' }).Count
    $socialOnlyCount = ($classified | Where-Object { $_.MigrationType -eq 'social_only' }).Count
    $unknownCount = ($classified | Where-Object { $_.MigrationType -eq 'unknown' }).Count

    Write-Host ""
    Write-Host "=== Export Summary ===" -ForegroundColor Cyan
    Write-Host "  Total users:          $($users.Count)" -ForegroundColor White
    Write-Host "  Email/password:       $emailPasswordCount (will be migrated)" -ForegroundColor Green
    Write-Host "  Social-only:          $socialOnlyCount (self-migrate on sign-in)" -ForegroundColor Yellow
    Write-Host "  Unknown/no email:     $unknownCount (skipped)" -ForegroundColor $(if ($unknownCount -gt 0) { 'Red' } else { 'White' })
    Write-Host ""

    # Show details
    Write-Host "--- Email/Password Users (will be migrated) ---" -ForegroundColor Green
    $classified | Where-Object { $_.MigrationType -eq 'email_password' } | ForEach-Object {
        Write-Host "  $($_.DisplayName) <$($_.Email)>" -ForegroundColor White
    }

    if ($socialOnlyCount -gt 0) {
        Write-Host ""
        Write-Host "--- Social-Only Users (will self-migrate) ---" -ForegroundColor Yellow
        $classified | Where-Object { $_.MigrationType -eq 'social_only' } | ForEach-Object {
            $providers = $_.SocialProviders -join ', '
            Write-Host "  $($_.DisplayName) <$($_.Email)> [$providers]" -ForegroundColor White
        }
    }

    if ($unknownCount -gt 0) {
        Write-Host ""
        Write-Host "--- Unknown/No Email Users (skipped) ---" -ForegroundColor Red
        $classified | Where-Object { $_.MigrationType -eq 'unknown' } | ForEach-Object {
            Write-Host "  $($_.DisplayName) (UPN: $($_.UPN))" -ForegroundColor White
        }
    }
}

if ($ExportOnly) {
    Write-Host ""
    Write-Host "Export complete. Run without -ExportOnly to proceed with import." -ForegroundColor Cyan
    exit 0
}

# ============================================================
# Phase 2: Import users into Entra External ID
# ============================================================

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host " Phase 2: Import users to Entra External ID (PROD)" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Load export file
if (-not (Test-Path $ExportFile)) {
    Write-Error "Export file not found: $ExportFile. Run without -SkipExport first."
    exit 1
}

$exportData = Get-Content -Path $ExportFile -Raw | ConvertFrom-Json
Write-Host "Loaded export from: $($exportData.ExportDate)" -ForegroundColor White
Write-Host "Source tenant: $($exportData.SourceTenant)" -ForegroundColor White
Write-Host "Total users in export: $($exportData.TotalUsers)" -ForegroundColor White

# Get email/password users to migrate
$usersToMigrate = $exportData.Users | Where-Object { $_.MigrationType -eq 'email_password' }
$socialUsers = $exportData.Users | Where-Object { $_.MigrationType -eq 'social_only' }

if ($usersToMigrate.Count -eq 0) {
    Write-Host ""
    Write-Host "No email/password users to migrate. All users are social-only or unknown." -ForegroundColor Yellow
    Write-Host "Social users will self-migrate when they sign in via their social IDP." -ForegroundColor Yellow
    exit 0
}

Write-Host ""
Write-Host "Users to import: $($usersToMigrate.Count) email/password users" -ForegroundColor Green
Write-Host "Users skipped: $($socialUsers.Count) social-only users (self-migrate)" -ForegroundColor Yellow

if ($DryRun) {
    Write-Host ""
    Write-Host "=== DRY RUN ===" -ForegroundColor Magenta
    Write-Host "The following users would be created in Entra External ID:" -ForegroundColor Magenta
    Write-Host ""
    foreach ($user in $usersToMigrate) {
        Write-Host "  CREATE: $($user.DisplayName) <$($user.Email)>" -ForegroundColor White
        Write-Host "    Given Name: $($user.GivenName)" -ForegroundColor Gray
        Write-Host "    Surname:    $($user.Surname)" -ForegroundColor Gray
        Write-Host "    Issuer:     $EntraTenantDomain" -ForegroundColor Gray
    }
    Write-Host ""
    Write-Host "Re-run without -DryRun to create these users." -ForegroundColor Magenta
    exit 0
}

# Prompt user to log in to Entra tenant
Write-Host ""
Write-Host "You need to be logged into the Entra External ID tenant." -ForegroundColor Yellow
Write-Host "Run: az login --tenant $EntraTenantId --allow-no-subscriptions" -ForegroundColor Yellow
Write-Host ""
$response = Read-Host "Press Enter when ready (or 'q' to quit)"
if ($response -eq 'q') { exit 0 }

# Verify tenant
$currentTenant = az account show --query tenantId -o tsv 2>$null
if ($currentTenant -ne $EntraTenantId) {
    Write-Host "ERROR: Current tenant is $currentTenant, expected $EntraTenantId" -ForegroundColor Red
    Write-Host "Run: az login --tenant $EntraTenantId --allow-no-subscriptions" -ForegroundColor Yellow
    exit 1
}

Write-Host "Entra tenant: $EntraTenantDomain ($EntraTenantId)" -ForegroundColor Green

$token = Get-GraphToken

# Import users
$created = 0
$skipped = 0
$failed = 0
$errors = @()

foreach ($user in $usersToMigrate) {
    $email = $user.Email
    Write-Host ""
    Write-Host "Processing: $($user.DisplayName) <$email>..." -ForegroundColor White

    # Check if user already exists in Entra
    $existingUsers = Invoke-GraphApi `
        -Url "https://graph.microsoft.com/v1.0/users?`$filter=identities/any(i:i/issuer eq '$EntraTenantDomain' and i/issuerAssignedId eq '$email')&`$select=id,displayName,mail" `
        -Token $token

    if ($existingUsers -and $existingUsers.Count -gt 0) {
        Write-Host "  SKIP: User already exists in Entra (ID: $($existingUsers[0].id))" -ForegroundColor Yellow
        $skipped++
        continue
    }

    # Also check by mail property
    $existingByMail = Invoke-GraphApi `
        -Url "https://graph.microsoft.com/v1.0/users?`$filter=mail eq '$email'&`$select=id,displayName,mail" `
        -Token $token

    if ($existingByMail -and $existingByMail.Count -gt 0) {
        Write-Host "  SKIP: User already exists in Entra by mail (ID: $($existingByMail[0].id))" -ForegroundColor Yellow
        $skipped++
        continue
    }

    # Create user in Entra
    $password = Get-RandomPassword
    $displayName = if ($user.DisplayName) { $user.DisplayName } else { $email.Split('@')[0] }
    $givenName = if ($user.GivenName) { $user.GivenName } else { $null }
    $surname = if ($user.Surname) { $user.Surname } else { $null }

    $newUser = @{
        displayName     = $displayName
        identities      = @(
            @{
                signInType       = 'emailAddress'
                issuer           = $EntraTenantDomain
                issuerAssignedId = $email
            }
        )
        passwordProfile = @{
            password                      = $password
            forceChangePasswordNextSignIn = $false
        }
        passwordPolicies = 'DisablePasswordExpiration'
    }

    # Add optional fields if present
    if ($givenName) { $newUser.givenName = $givenName }
    if ($surname) { $newUser.surname = $surname }
    if ($email) { $newUser.mail = $email }

    try {
        $result = Invoke-GraphApi -Method POST `
            -Url "https://graph.microsoft.com/v1.0/users" `
            -Token $token `
            -Body $newUser

        Write-Host "  CREATED: ID $($result.id)" -ForegroundColor Green
        $created++
    }
    catch {
        $errorMsg = $_.Exception.Message
        Write-Host "  FAILED: $errorMsg" -ForegroundColor Red
        $errors += [PSCustomObject]@{
            Email = $email
            DisplayName = $displayName
            Error = $errorMsg
        }
        $failed++
    }
}

# ============================================================
# Summary
# ============================================================

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host " Migration Summary (PROD)" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "  Created in Entra:     $created" -ForegroundColor Green
Write-Host "  Skipped (exists):     $skipped" -ForegroundColor Yellow
Write-Host "  Skipped (social):     $($socialUsers.Count)" -ForegroundColor Yellow
Write-Host "  Failed:               $failed" -ForegroundColor $(if ($failed -gt 0) { 'Red' } else { 'White' })
Write-Host ""

if ($errors.Count -gt 0) {
    Write-Host "--- Errors ---" -ForegroundColor Red
    foreach ($err in $errors) {
        Write-Host "  $($err.DisplayName) <$($err.Email)>: $($err.Error)" -ForegroundColor Red
    }
    Write-Host ""
}

Write-Host "Next steps:" -ForegroundColor Yellow
Write-Host "  1. Verify imported users in Entra admin center:" -ForegroundColor Yellow
Write-Host "     https://entra.microsoft.com (switch to prod tenant)" -ForegroundColor Yellow
Write-Host "  2. After deploying to release, visit https://www.trashmob.eco/api/config" -ForegroundColor Yellow
Write-Host "     to verify authProvider is 'entra'" -ForegroundColor Yellow
Write-Host "  3. Test email sign-in: user clicks 'Forgot password' to set new password" -ForegroundColor Yellow
Write-Host "  4. Test social sign-in: user clicks Google/Facebook/Microsoft button" -ForegroundColor Yellow
Write-Host "  5. Switch back to main subscription:" -ForegroundColor Yellow
Write-Host "     az login --tenant f0062da2-b273-427c-b99e-6e85f75b23eb" -ForegroundColor Yellow
Write-Host ""
