<#
.SYNOPSIS
    Configures Entra External ID app registrations for TrashMob.

.DESCRIPTION
    Sets optional claims (email, given_name, family_name, preferred_username)
    on all three app registrations (Web SPA, API, Mobile) and enables
    acceptMappedClaims and isFallbackPublicClient where needed.

    App registrations are Microsoft Graph resources and cannot be managed
    via Bicep/ARM templates. This script is the equivalent IaC approach.

    Must be run while logged into the Entra External ID tenant:
      az login --tenant <entra-tenant-id> --allow-no-subscriptions

.PARAMETER Environment
    Target environment: 'dev' or 'pr' (production)

.EXAMPLE
    # Login to the external tenant first
    az login --tenant 8577fa31-4b86-4e4b-8b02-93fba708cb19 --allow-no-subscriptions

    # Run the script
    .\configure-entra-apps.ps1 -Environment dev
#>

param(
    [Parameter(Mandatory = $true)]
    [ValidateSet('dev', 'pr')]
    [string]$Environment
)

$ErrorActionPreference = 'Stop'

# App registration configuration per environment
$config = @{
    'dev' = @{
        TenantId        = '8577fa31-4b86-4e4b-8b02-93fba708cb19'
        WebSpaAppId     = '1e6ae74d-0160-4a01-9d75-04048e03b17e'
        ApiAppId        = '84df543d-6535-45f5-afab-4d38528b721a'
        MobileAppId     = '33bfdd2c-80a4-4e6e-b211-337b0467226d'
        AuthExtAppId    = '219a6eac-9f9e-4ade-96b8-881928bd6d6d'
    }
    'pr' = @{
        TenantId        = ''  # TODO: Fill in when prod tenant is created
        WebSpaAppId     = ''  # TODO: Fill in when prod apps are registered
        ApiAppId        = ''  # TODO: Fill in when prod apps are registered
        MobileAppId     = ''  # TODO: Fill in when prod apps are registered
        AuthExtAppId    = ''  # TODO: Fill in when prod apps are registered
    }
}

$env_config = $config[$Environment]

if (-not $env_config.TenantId) {
    Write-Error "No tenant configuration found for environment '$Environment'. Update the script with prod values."
    exit 1
}

# Verify we're logged into the correct tenant
$currentTenant = (az account show --query tenantId -o tsv 2>$null)
if ($currentTenant -ne $env_config.TenantId) {
    Write-Host "ERROR: Current tenant is $currentTenant, expected $($env_config.TenantId)" -ForegroundColor Red
    Write-Host "Run: az login --tenant $($env_config.TenantId) --allow-no-subscriptions" -ForegroundColor Yellow
    exit 1
}

Write-Host "Configuring Entra External ID app registrations for '$Environment' environment" -ForegroundColor Cyan
Write-Host "Tenant: $($env_config.TenantId)" -ForegroundColor Cyan
Write-Host ""

# Standard optional claims for all apps
$optionalClaimsBody = @{
    optionalClaims = @{
        accessToken = @(
            @{ name = "email"; essential = $false }
            @{ name = "given_name"; essential = $false }
            @{ name = "family_name"; essential = $false }
            @{ name = "preferred_username"; essential = $false }
        )
        idToken = @(
            @{ name = "email"; essential = $false }
            @{ name = "given_name"; essential = $false }
            @{ name = "family_name"; essential = $false }
            @{ name = "preferred_username"; essential = $false }
        )
    }
} | ConvertTo-Json -Depth 5

# --- 1. Configure Web SPA (Frontend) ---
Write-Host "1. Configuring TrashMob Web ($($env_config.WebSpaAppId))..." -ForegroundColor White

# Get object ID for the Web SPA app
$webApp = az ad app show --id $env_config.WebSpaAppId --query "{objectId:id,displayName:displayName,acceptMappedClaims:api.acceptMappedClaims,hasOptionalClaims:optionalClaims}" -o json | ConvertFrom-Json

Write-Host "   Display Name: $($webApp.displayName)"

# Set optional claims + acceptMappedClaims
$webBody = @{
    api = @{ acceptMappedClaims = $true }
    optionalClaims = @{
        accessToken = @(
            @{ name = "email"; essential = $false }
            @{ name = "given_name"; essential = $false }
            @{ name = "family_name"; essential = $false }
            @{ name = "preferred_username"; essential = $false }
        )
        idToken = @(
            @{ name = "email"; essential = $false }
            @{ name = "given_name"; essential = $false }
            @{ name = "family_name"; essential = $false }
            @{ name = "preferred_username"; essential = $false }
        )
    }
} | ConvertTo-Json -Depth 5

az rest --method PATCH `
    --url "https://graph.microsoft.com/v1.0/applications/$($webApp.objectId)" `
    --headers "Content-Type=application/json" `
    --body $webBody 2>&1 | Out-Null

Write-Host "   [OK] Optional claims (email, given_name, family_name, preferred_username) on ID + access tokens" -ForegroundColor Green
Write-Host "   [OK] acceptMappedClaims = true" -ForegroundColor Green

# --- 2. Configure API (Backend) ---
Write-Host "2. Configuring TrashMob API ($($env_config.ApiAppId))..." -ForegroundColor White

$apiApp = az ad app show --id $env_config.ApiAppId --query "{objectId:id,displayName:displayName}" -o json | ConvertFrom-Json

Write-Host "   Display Name: $($apiApp.displayName)"

$apiBody = @{
    api = @{ acceptMappedClaims = $true }
    optionalClaims = @{
        accessToken = @(
            @{ name = "email"; essential = $false }
            @{ name = "given_name"; essential = $false }
            @{ name = "family_name"; essential = $false }
            @{ name = "preferred_username"; essential = $false }
        )
        idToken = @(
            @{ name = "email"; essential = $false }
            @{ name = "given_name"; essential = $false }
            @{ name = "family_name"; essential = $false }
            @{ name = "preferred_username"; essential = $false }
        )
    }
} | ConvertTo-Json -Depth 5

az rest --method PATCH `
    --url "https://graph.microsoft.com/v1.0/applications/$($apiApp.objectId)" `
    --headers "Content-Type=application/json" `
    --body $apiBody 2>&1 | Out-Null

Write-Host "   [OK] Optional claims (email, given_name, family_name, preferred_username) on ID + access tokens" -ForegroundColor Green
Write-Host "   [OK] acceptMappedClaims = true" -ForegroundColor Green

# --- 3. Configure Mobile (Public Client) ---
Write-Host "3. Configuring TrashMob Mobile ($($env_config.MobileAppId))..." -ForegroundColor White

$mobileApp = az ad app show --id $env_config.MobileAppId --query "{objectId:id,displayName:displayName}" -o json | ConvertFrom-Json

Write-Host "   Display Name: $($mobileApp.displayName)"

$mobileBody = @{
    isFallbackPublicClient = $true
    api = @{ acceptMappedClaims = $true }
    optionalClaims = @{
        accessToken = @(
            @{ name = "email"; essential = $false }
            @{ name = "given_name"; essential = $false }
            @{ name = "family_name"; essential = $false }
            @{ name = "preferred_username"; essential = $false }
        )
        idToken = @(
            @{ name = "email"; essential = $false }
            @{ name = "given_name"; essential = $false }
            @{ name = "family_name"; essential = $false }
            @{ name = "preferred_username"; essential = $false }
        )
    }
} | ConvertTo-Json -Depth 5

az rest --method PATCH `
    --url "https://graph.microsoft.com/v1.0/applications/$($mobileApp.objectId)" `
    --headers "Content-Type=application/json" `
    --body $mobileBody 2>&1 | Out-Null

Write-Host "   [OK] Optional claims (email, given_name, family_name, preferred_username) on ID + access tokens" -ForegroundColor Green
Write-Host "   [OK] acceptMappedClaims = true" -ForegroundColor Green
Write-Host "   [OK] isFallbackPublicClient = true" -ForegroundColor Green

# --- Verification ---
Write-Host ""
Write-Host "=== Verification ===" -ForegroundColor Cyan

foreach ($appId in @($env_config.WebSpaAppId, $env_config.ApiAppId, $env_config.MobileAppId)) {
    $app = az ad app show --id $appId --query "{displayName:displayName,acceptMappedClaims:api.acceptMappedClaims,isFallbackPublicClient:isFallbackPublicClient,accessTokenClaims:optionalClaims.accessToken[].name,idTokenClaims:optionalClaims.idToken[].name}" -o json | ConvertFrom-Json

    Write-Host "$($app.displayName) ($appId):" -ForegroundColor White
    Write-Host "   acceptMappedClaims: $($app.acceptMappedClaims)" -ForegroundColor $(if ($app.acceptMappedClaims) { 'Green' } else { 'Red' })
    Write-Host "   Access token claims: $($app.accessTokenClaims -join ', ')" -ForegroundColor $(if ($app.accessTokenClaims.Count -ge 4) { 'Green' } else { 'Red' })
    Write-Host "   ID token claims: $($app.idTokenClaims -join ', ')" -ForegroundColor $(if ($app.idTokenClaims.Count -ge 4) { 'Green' } else { 'Red' })
    if ($appId -eq $env_config.MobileAppId) {
        Write-Host "   isFallbackPublicClient: $($app.isFallbackPublicClient)" -ForegroundColor $(if ($app.isFallbackPublicClient) { 'Green' } else { 'Red' })
    }
}

Write-Host ""
Write-Host "Done! All app registrations configured." -ForegroundColor Green
Write-Host ""
Write-Host "Next steps:" -ForegroundColor Yellow
Write-Host "  1. Test sign-in and decode JWT at https://jwt.ms to verify claims" -ForegroundColor Yellow
Write-Host "  2. Check that user flow collects givenName, surname, dateOfBirth" -ForegroundColor Yellow
Write-Host "  3. Switch back to main subscription: az login --tenant f0062da2-b273-427c-b99e-6e85f75b23eb" -ForegroundColor Yellow
