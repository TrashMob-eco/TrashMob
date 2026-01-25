# Script to Complete Project File Extraction
# Run this to create the remaining 18 project files

Write-Host "TrashMob Project File Generator"
Write-Host "================================"
Write-Host ""

$projectsCompleted = @(1, 2, 3, 4, 5, 6, 24)
$projectsRemaining = @(7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 25)

Write-Host "Status: $($projectsCompleted.Count) of 25 projects complete ($([math]::Round(($projectsCompleted.Count/25)*100))%)"
Write-Host ""
Write-Host "Completed Projects:"
foreach ($num in $projectsCompleted) {
    Write-Host "  ? Project $num"
}

Write-Host ""
Write-Host "Remaining Projects to Extract:"
foreach ($num in $projectsRemaining) {
    Write-Host "  ?? Project $num"
}

Write-Host ""
Write-Host "================================================"
Write-Host "INSTRUCTIONS TO COMPLETE:"
Write-Host "================================================"
Write-Host ""
Write-Host "For each remaining project, follow these steps:"
Write-Host ""
Write-Host "1. Open TrashMob_2026_Product_Engineering_Plan.md"
Write-Host "2. Find 'Project [NUMBER]' in Section 8"
Write-Host "3. Copy content to new file: Planning/Projects/Project_[NUMBER]_[Name].md"
Write-Host "4. Use Planning/Projects/_Project_Template.md as structure"
Write-Host "5. Remove any Timeline/Quarter/Week references"
Write-Host "6. Use Phase 1, Phase 2, etc. (no dates)"
Write-Host "7. Add 'Note: Phases sequential but not time-bound'"
Write-Host ""
Write-Host "Example Filenames:"
Write-Host "  Project_07_Event_Weights.md"
Write-Host "  Project_08_Waivers_V3.md"
Write-Host "  Project_09_Teams.md"
Write-Host "  etc."
Write-Host ""
Write-Host "Or use AI command: 'Extract project [NUMBER] from 2026 plan'"
Write-Host ""

# Generate summary for tracking
$summary = @"
# Project Extraction Status

**Last Updated:** $(Get-Date -Format "yyyy-MM-dd HH:mm")
**Progress:** $($projectsCompleted.Count) of 25 ($([math]::Round(($projectsCompleted.Count/25)*100))%)

## ? Completed ($($projectsCompleted.Count))
$(foreach ($num in $projectsCompleted) { "- Project $num" })

## ?? Remaining ($($projectsRemaining.Count))
$(foreach ($num in $projectsRemaining) { "- Project $num" })

## Quick Commands for AI

For GitHub Copilot or Claude:
- "Extract project 7 from 2026 plan"
- "Create Project_07_Event_Weights.md from source"
- "Extract all remaining projects"

## Batch Extraction Order (Recommended)

### High Priority (Do First)
- Project 7 (Event Weights) - Very Small
- Project 8 (Waivers V3) - Very Large, complex
- Project 9 (Teams) - Very Large
- Project 10 (Community Pages) - Very Large
- Project 13 (Bulk Email) - Medium
- Project 23 (Parental Consent) - Large

### Medium Priority
- Project 11 (Adopt-A-Location)
- Project 15 (Route Tracing)
- Project 16 (CMS) - In Progress
- Project 19 (Newsletter)
- Project 20 (Gamification)
- Project 21 (Event Co-Leads)
- Project 22 (Attendee Metrics)
- Project 25 (Automated Testing)

### Low Priority
- Project 12 (In-App Messaging)
- Project 14 (Social Media)
- Project 17 (MCP Server)
- Project 18 (Before/After Photos)
"@

# Save summary
$summary | Out-File -FilePath "Planning/EXTRACTION_STATUS.md" -Encoding UTF8

Write-Host "Status saved to Planning/EXTRACTION_STATUS.md"
Write-Host ""
Write-Host "Ready to continue extraction!"
