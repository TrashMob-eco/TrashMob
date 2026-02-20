<#
.SYNOPSIS
    Generates a Google Play Feature Graphic (1024x500) from the TrashMob logo.

.DESCRIPTION
    Composites the horizontal logo onto a branded background with the TrashMob
    brand green (#96ba00). The logo is centered with padding. Uses .NET
    System.Drawing (no external tools required on Windows).

.EXAMPLE
    .\generate-feature-graphic.ps1
    .\generate-feature-graphic.ps1 -LogoImage ".\MyLogo.png" -Tagline "Clean Up Your Community"
#>

param(
    [string]$LogoImage = "$PSScriptRoot\HorizontalLogo_2259x588.png",
    [string]$OutputDir = "$PSScriptRoot\Generated",
    [string]$BackgroundColor = "#96ba00",
    [string]$Tagline = "Clean Up Your Community",
    [string]$TaglineColor = "#FFFFFF",
    [int]$Width = 1024,
    [int]$Height = 500
)

Add-Type -AssemblyName System.Drawing

# Validate source
if (-not (Test-Path $LogoImage)) {
    Write-Error "Logo image not found: $LogoImage"
    exit 1
}

# Create output directory
if (-not (Test-Path $OutputDir)) {
    New-Item -ItemType Directory -Path $OutputDir -Force | Out-Null
}

$outputPath = Join-Path $OutputDir "GooglePlay_FeatureGraphic_1024x500.png"

# Create canvas
$bitmap = New-Object System.Drawing.Bitmap($Width, $Height)
$graphics = [System.Drawing.Graphics]::FromImage($bitmap)
$graphics.InterpolationMode = [System.Drawing.Drawing2D.InterpolationMode]::HighQualityBicubic
$graphics.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::HighQuality
$graphics.TextRenderingHint = [System.Drawing.Text.TextRenderingHint]::AntiAlias

# Fill background
$bgColor = [System.Drawing.ColorTranslator]::FromHtml($BackgroundColor)
$brush = New-Object System.Drawing.SolidBrush($bgColor)
$graphics.FillRectangle($brush, 0, 0, $Width, $Height)

# Load and draw logo (centered, taking ~60% width, upper portion)
$logo = [System.Drawing.Image]::FromFile((Resolve-Path $LogoImage))

$maxLogoWidth = [int]($Width * 0.60)
$scale = $maxLogoWidth / $logo.Width
$logoWidth = [int]($logo.Width * $scale)
$logoHeight = [int]($logo.Height * $scale)

# Position logo in upper-center area (leave room for tagline below)
$logoX = [int](($Width - $logoWidth) / 2)
$logoY = [int](($Height - $logoHeight) / 2 - 40)

$graphics.DrawImage($logo, $logoX, $logoY, $logoWidth, $logoHeight)

# Draw tagline below logo
if ($Tagline) {
    $font = New-Object System.Drawing.Font("Segoe UI", 22, [System.Drawing.FontStyle]::Regular)
    $textBrush = New-Object System.Drawing.SolidBrush([System.Drawing.ColorTranslator]::FromHtml($TaglineColor))
    $format = New-Object System.Drawing.StringFormat
    $format.Alignment = [System.Drawing.StringAlignment]::Center

    $textY = $logoY + $logoHeight + 20
    $textRect = New-Object System.Drawing.RectangleF(0, $textY, $Width, 60)
    $graphics.DrawString($Tagline, $font, $textBrush, $textRect, $format)

    $font.Dispose()
    $textBrush.Dispose()
    $format.Dispose()
}

$bitmap.Save($outputPath, [System.Drawing.Imaging.ImageFormat]::Png)

# Cleanup
$brush.Dispose()
$logo.Dispose()
$graphics.Dispose()
$bitmap.Dispose()

Write-Host "Generated: $outputPath ($Width x $Height)"
Write-Host ""
Write-Host "Preview the image and adjust parameters if needed:"
Write-Host "  -BackgroundColor '#96ba00'  (TrashMob brand green)"
Write-Host "  -Tagline 'Your tagline here'"
Write-Host "  -TaglineColor '#FFFFFF'"
Write-Host ""
Write-Host "Upload to Google Play Console > Store listing > Feature graphic"
