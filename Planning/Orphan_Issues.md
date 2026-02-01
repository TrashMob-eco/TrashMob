# Orphan Issues - Not Tied to Existing Projects

Generated: January 31, 2026

This document lists open GitHub issues that are not currently tied to any project in the Planning/Projects folder. These issues need review to determine if they should:
1. Be added to an existing project
2. Become a new project
3. Be addressed as standalone bug fixes
4. Be closed as won't-fix or duplicates

---

## Summary

| Category | Count |
|----------|-------|
| Website Bugs | 8 |
| Website Enhancements | 10 |
| Infrastructure | 6 |
| SEO/Marketing | 5 |
| UX Improvements | 5 |
| Design/Content | 3 |

**Note:** Issues #550 and #294 moved to Project 34 (User Feedback) and Project 35 (Partner Location Map)

---

## Website Bugs (Fix Without Project)

| Issue | Title | Priority |
|-------|-------|----------|
| [#2173](https://github.com/trashmob/TrashMob/issues/2173) | Event Summary - Submit Pickup Locations button doesn't work | High |
| [#2153](https://github.com/trashmob/TrashMob/issues/2153) | Partner event requests not on dashboard | Medium |
| [#2137](https://github.com/trashmob/TrashMob/issues/2137) | Event popup shows "Created By" but no name | Low |
| [#1711](https://github.com/trashmob/TrashMob/issues/1711) | Long username resizes menu improperly | Low |
| [#1608](https://github.com/trashmob/TrashMob/issues/1608) | Daily Summary Email not being sent | High |
| [#911](https://github.com/trashmob/TrashMob/issues/911) | SPA default page error in logs | Low |

---

## Website Enhancements (Consider for Future Projects)

| Issue | Title | Suggestion |
|-------|-------|------------|
| [#2215](https://github.com/trashmob/TrashMob/issues/2215) | Job Opportunities page - Allow text to be formatted | Extend Project 16 (CMS) |
| [#2172](https://github.com/trashmob/TrashMob/issues/2172) | Enable Service dialog - Notes should be optional | Small fix, no project |
| [#2171](https://github.com/trashmob/TrashMob/issues/2171) | Separate bundle into smaller chunks | Performance project candidate |
| [#2109](https://github.com/trashmob/TrashMob/issues/2109) | Event move to Completed based on time, not day | Enhancement |
| [#1874](https://github.com/trashmob/TrashMob/issues/1874) | Forms could use Toast/Notification | UX standardization |
| [#1077](https://github.com/trashmob/TrashMob/issues/1077) | Adding public cleanup events by admin | Admin feature |
| [#822](https://github.com/trashmob/TrashMob/issues/822) | Show Event Partners on View Event | Partner integration |
| [#675](https://github.com/trashmob/TrashMob/issues/675) | Add User Report Page | Analytics feature |
| [#674](https://github.com/trashmob/TrashMob/issues/674) | Add Event Tags | Event categorization |
| [#672](https://github.com/trashmob/TrashMob/issues/672) | Bi-directional relationship between fields and map | UX improvement |

---

## Infrastructure Issues

| Issue | Title | Suggestion |
|-------|-------|------------|
| [#670](https://github.com/trashmob/TrashMob/issues/670) | Move Azure Subscriptions to new Tenant | Major infrastructure change |
| [#280](https://github.com/trashmob/TrashMob/issues/280) | KeyVault setup has hardcoded tenants | Add to Project 26 |
| [#200](https://github.com/trashmob/TrashMob/issues/200) | Assign Website a static IP | Infrastructure |
| [#989](https://github.com/trashmob/TrashMob/issues/989) | Set up periodic web scanning | Security enhancement |
| [#182](https://github.com/trashmob/TrashMob/issues/182) | Google Maps key transmitted to client | Security review needed |
| [#1476](https://github.com/trashmob/TrashMob/issues/1476) | Maps API - Better locations for Large Parks | Maps enhancement |

---

## SEO/Marketing Issues

| Issue | Title | Suggestion |
|-------|-------|------------|
| [#1075](https://github.com/trashmob/TrashMob/issues/1075) | Short URLs for events for SEO | SEO project candidate |
| [#207](https://github.com/trashmob/TrashMob/issues/207) | Allow for Shortened URLs for event details | Duplicate of #1075 |
| [#719](https://github.com/trashmob/TrashMob/issues/719) | Page Index Issues Detected by Google | SEO fixes |
| [#718](https://github.com/trashmob/TrashMob/issues/718) | Issues Detected by Google | SEO fixes |
| [#916](https://github.com/trashmob/TrashMob/issues/916) | Add Partnership section to FAQ | Content update |

---

## UX Improvements

| Issue | Title | Suggestion |
|-------|-------|------------|
| [#302](https://github.com/trashmob/TrashMob/issues/302) | Remind user to save work before switching tabs | Good first issue |
| [#215](https://github.com/trashmob/TrashMob/issues/215) | Allow users to filter events on dashboard | Dashboard enhancement |
| [#211](https://github.com/trashmob/TrashMob/issues/211) | Show number of characters remaining on text fields | Good first issue |

---

## Design/Content Issues

| Issue | Title | Suggestion |
|-------|-------|------------|
| [#1350](https://github.com/trashmob/TrashMob/issues/1350) | Create official document template with letterhead | Marketing asset |
| [#926](https://github.com/trashmob/TrashMob/issues/926) | Shorten Board Bios to fit standard card size | Content update |
| [#197](https://github.com/trashmob/TrashMob/issues/197) | Ensure site meets Accessibility Standards | WCAG compliance |

---

## Code Quality

| Issue | Title | Suggestion |
|-------|-------|------------|
| [#1627](https://github.com/trashmob/TrashMob/issues/1627) | Linting must be more strict | Add to Project 6 |
| [#1473](https://github.com/trashmob/TrashMob/issues/1473) | Event Summary API - Bad security practice | Add to Project 6 |
| [#492](https://github.com/trashmob/TrashMob/issues/492) | Look through codebase for TODOs | Tech debt cleanup |

---

## Recommendations

### Create New Projects For:
1. **SEO Improvements** - Combine #1075, #207, #719, #718 into a single SEO project
2. **Performance Optimization** - #2171 (bundle splitting) could lead a broader performance project

### Add to Existing Projects:
1. **Project 6 (Backend Standards)** - Add #1627, #1473
2. **Project 16 (CMS)** - Add #2215
3. **Project 26 (KeyVault RBAC)** - Add #280

### Close as Duplicates:
1. **#207** is duplicate of **#1075** (Short URLs)

### Triage as Bug Fixes:
- Prioritize #2173, #1608 as high-priority bugs to fix immediately

---

**Last Updated:** January 31, 2026
