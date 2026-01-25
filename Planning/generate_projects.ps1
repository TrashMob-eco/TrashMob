# Generate Standardized Project Files from 2026 Plan
# This script helps extract projects from the main 2026 plan into individual files

# Define the standardized template structure
$projectTemplate = @"
# Project {NUMBER} — {TITLE}

| Attribute | Value |
|-----------|-------|
| **Status** | {STATUS} |
| **Priority** | {PRIORITY} |
| **Risk** | {RISK} |
| **Size** | {SIZE} |
| **Timeline** | {TIMELINE} |
| **Dependencies** | {DEPENDENCIES} |

---

## Business Rationale

{RATIONALE}

---

## Objectives

{OBJECTIVES}

---

## Scope

{SCOPE}

---

## Out-of-Scope

{OUT_OF_SCOPE}

---

## Success Metrics

{SUCCESS_METRICS}

---

## Dependencies

{DEPENDENCY_DETAILS}

---

## Risks & Mitigations

| Risk | Likelihood | Impact | Mitigation |
|------|------------|--------|------------|
{RISKS_TABLE}

---

## Implementation Plan

{IMPLEMENTATION_PLAN}

---

## Rollout Plan

{ROLLOUT_PLAN}

---

## Open Questions

{OPEN_QUESTIONS}

---

## Related Documents

{RELATED_DOCS}

---

**Last Updated:** January 24, 2026  
**Owner:** {OWNER}  
**Status:** {REVIEW_STATUS}  
**Next Review:** {NEXT_REVIEW}
"@

# Project metadata mapping
$projects = @(
    @{Number=1; Title="Auth Revamp"; Status="Planning"; Priority="High"; Risk="Very High"; Size="Large"; Timeline="Q1-Q2 2026"},
    @{Number=2; Title="Home Page Improvements"; Status="Ready for Review"; Priority="High"; Risk="Medium"; Size="Medium"; Timeline="Q2 2026"},
    @{Number=3; Title="Litter Reporting Web"; Status="Ready for Review"; Priority="Moderate"; Risk="Low"; Size="Medium"; Timeline="Q2 2026"},
    @{Number=4; Title="Mobile Robustness"; Status="Developers Engaged"; Priority="Very High"; Risk="Low"; Size="Medium"; Timeline="Q1 2026"},
    @{Number=5; Title="Deployment Pipelines"; Status="Ready for Review"; Priority="Medium"; Risk="Moderate"; Size="Medium"; Timeline="Q1 2026"},
    @{Number=6; Title="Backend Standards"; Status="Ready for Review"; Priority="Low"; Risk="Low"; Size="Medium"; Timeline="Ongoing"},
    @{Number=7; Title="Event Weights"; Status="Ready for Review"; Priority="High"; Risk="Low"; Size="Very Small"; Timeline="Q2-Q3 2026"},
    @{Number=8; Title="Waivers V3"; Status="Planning"; Priority="High"; Risk="Very Large"; Size="Very Large"; Timeline="Q3 2026"},
    @{Number=9; Title="Teams MVP"; Status="Ready for Review"; Priority="High"; Risk="Large"; Size="Very Large"; Timeline="Q3 2026"},
    @{Number=10; Title="Community Pages"; Status="Planning"; Priority="High"; Risk="Very High"; Size="Very Large"; Timeline="Q3 2026"},
    @{Number=11; Title="Adopt-A-Location"; Status="Planning"; Priority="Medium"; Risk="High"; Size="Very Large"; Timeline="Q4 2026"},
    @{Number=12; Title="In-App Messaging"; Status="Not Started"; Priority="Low"; Risk="High"; Size="Medium"; Timeline="TBD"},
    @{Number=13; Title="Bulk Email Invites"; Status="Not Started"; Priority="High"; Risk="Low"; Size="Medium"; Timeline="TBD"},
    @{Number=14; Title="Social Media Integration"; Status="Not Started"; Priority="Medium"; Risk="Low"; Size="Small"; Timeline="TBD"},
    @{Number=15; Title="Route Tracing"; Status="Planning"; Priority="Medium"; Risk="High"; Size="Large"; Timeline="Q4 2026"},
    @{Number=16; Title="Content Management (CMS)"; Status="In Progress"; Priority="Medium"; Risk="Low"; Size="Medium"; Timeline="Q2 2026"},
    @{Number=17; Title="MCP Server (AI)"; Status="Not Started"; Priority="Low"; Risk="Moderate"; Size="Medium"; Timeline="Optional"},
    @{Number=18; Title="Before/After Photos"; Status="Planning"; Priority="Low"; Risk="Moderate"; Size="Medium"; Timeline="Q4 2026"},
    @{Number=19; Title="Newsletter Support"; Status="Not Started"; Priority="Medium"; Risk="Low"; Size="Medium"; Timeline="TBD"},
    @{Number=20; Title="Gamification"; Status="Not Started"; Priority="Medium"; Risk="High"; Size="Medium"; Timeline="Q4 2026"},
    @{Number=21; Title="Event Co-Leads"; Status="Planning"; Priority="Medium"; Risk="High"; Size="Medium"; Timeline="TBD"},
    @{Number=22; Title="Attendee Metrics"; Status="Not Started"; Priority="Medium"; Risk="Medium"; Size="Medium"; Timeline="Q4 2026"},
    @{Number=23; Title="Parental Consent"; Status="Planning"; Priority="High"; Risk="High"; Size="Large"; Timeline="Q1-Q3 2026"},
    @{Number=24; Title="API v2 Modernization"; Status="Planning"; Priority="High"; Risk="Medium"; Size="Very Large"; Timeline="Q2-Q4 2026"},
    @{Number=25; Title="Automated Testing"; Status="Not Started"; Priority="Medium"; Risk="Low"; Size="Medium"; Timeline="TBD"}
)

# Instructions for manual completion:
Write-Host "=" * 80
Write-Host "Project File Generation Helper"
Write-Host "=" * 80
Write-Host ""
Write-Host "This script provides the structure for creating standardized project files."
Write-Host "Project 1 (Auth Revamp) and Project 24 (API v2) have already been created as examples."
Write-Host ""
Write-Host "To complete the remaining projects:"
Write-Host "1. Extract each project section from TrashMob_2026_Product_Engineering_Plan.md"
Write-Host "2. Use the template structure above (see `$projectTemplate`)"
Write-Host "3. Fill in the placeholders with content from the original document"
Write-Host "4. Save as: Planning/Projects/Project_{NUMBER}_{TITLE}.md"
Write-Host ""
Write-Host "Project List:"
Write-Host "-------------"

foreach ($project in $projects) {
    $filename = "Project_{0:D2}_{1}.md" -f $project.Number, ($project.Title -replace ' ', '_')
    $status = if ($project.Number -in @(1, 2, 3, 4, 5, 24)) { "? Created" }
              else { "?? Pending" }
    
    Write-Host ("{0} - Project {1}: {2} ({3})" -f $status, $project.Number, $project.Title, $filename)
}

Write-Host ""
Write-Host "=" * 80
Write-Host "Next Steps:"
Write-Host "1. Review Planning/Projects/Project_01_Auth_Revamp.md for template example"
Write-Host "2. Review Planning/Projects/Project_24_API_v2_Modernization.md for API example"
Write-Host "3. Extract remaining projects from the main 2026 plan"
Write-Host "4. Create individual files following the standardized format"
Write-Host "=" * 80
