<#
.SYNOPSIS
    Generates all required app store icons from the source 2500x2500 PNG.

.DESCRIPTION
    Resizes AppIcon_2500x2500.png to all sizes needed for Apple App Store,
    Google Play Store, and other platforms. Uses .NET System.Drawing (no
    external tools required on Windows).

.EXAMPLE
    .\generate-icons.ps1
    .\generate-icons.ps1 -SourceImage ".\MyIcon.png"
    .\generate-icons.ps1 -OutputDir ".\output"
#>

param(
    [string]$SourceImage = "$PSScriptRoot\AppIcon_2500x2500.png",
    [string]$OutputDir = "$PSScriptRoot\Generated"
)

Add-Type -AssemblyName System.Drawing

# All required icon sizes
$sizes = @(
    @{ Name = "AppStore_1024x1024";    Width = 1024; Height = 1024; Description = "Apple App Store icon" }
    @{ Name = "GooglePlay_512x512";    Width = 512;  Height = 512;  Description = "Google Play hi-res icon" }
    @{ Name = "PWA_512x512";           Width = 512;  Height = 512;  Description = "PWA manifest icon (large)" }
    @{ Name = "PWA_192x192";           Width = 192;  Height = 192;  Description = "PWA manifest icon (small)" }
    @{ Name = "Favicon_32x32";         Width = 32;   Height = 32;   Description = "Browser favicon" }
    @{ Name = "EntraProfile_240x240";  Width = 240;  Height = 240;  Description = "Entra External ID profile" }
)

# Validate source
if (-not (Test-Path $SourceImage)) {
    Write-Error "Source image not found: $SourceImage"
    exit 1
}

# Create output directory
if (-not (Test-Path $OutputDir)) {
    New-Item -ItemType Directory -Path $OutputDir -Force | Out-Null
}

Write-Host "Source: $SourceImage"
Write-Host "Output: $OutputDir"
Write-Host ""

$source = [System.Drawing.Image]::FromFile((Resolve-Path $SourceImage))
Write-Host "Source dimensions: $($source.Width)x$($source.Height)"
Write-Host ""

foreach ($size in $sizes) {
    $outputPath = Join-Path $OutputDir "$($size.Name).png"

    $bitmap = New-Object System.Drawing.Bitmap($size.Width, $size.Height)
    $graphics = [System.Drawing.Graphics]::FromImage($bitmap)

    # High-quality resize
    $graphics.InterpolationMode = [System.Drawing.Drawing2D.InterpolationMode]::HighQualityBicubic
    $graphics.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::HighQuality
    $graphics.PixelOffsetMode = [System.Drawing.Drawing2D.PixelOffsetMode]::HighQuality
    $graphics.CompositingQuality = [System.Drawing.Drawing2D.CompositingQuality]::HighQuality

    $graphics.DrawImage($source, 0, 0, $size.Width, $size.Height)
    $bitmap.Save($outputPath, [System.Drawing.Imaging.ImageFormat]::Png)

    $graphics.Dispose()
    $bitmap.Dispose()

    Write-Host "  Generated: $($size.Name).png ($($size.Width)x$($size.Height)) - $($size.Description)"
}

$source.Dispose()

Write-Host ""
Write-Host "Done! Generated $($sizes.Count) icons in $OutputDir"
Write-Host ""
Write-Host "Next steps:"
Write-Host "  1. Upload AppStore_1024x1024.png to Apple App Store Connect"
Write-Host "  2. Upload GooglePlay_512x512.png to Google Play Console"
Write-Host "  3. Review icons visually before uploading"
