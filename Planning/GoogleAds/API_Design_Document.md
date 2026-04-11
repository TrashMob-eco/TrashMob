# TrashMob Google Ads API — Design Document

## Overview

TrashMob.eco is a 501(c)(3) nonprofit organization that connects volunteers with community litter cleanup events across the United States. TrashMob uses the Google Ad Grants program (account 458-730-8084) to promote volunteer recruitment and community engagement through Google Search advertising.

This document describes TrashMob's internal tooling for managing its Google Ad Grants account via the Google Ads API.

## Purpose

TrashMob has developed a set of internal Python scripts to automate the initial setup and ongoing management of its Google Ad Grants campaigns. The tooling is used exclusively to manage TrashMob's own advertising account — it is not a commercial product or third-party service.

## API Operations Used

The tool performs the following operations against TrashMob's own Google Ads account:

| Operation | API Service | Purpose |
|-----------|------------|---------|
| Account verification | GoogleAdsService | Verify API access and account connectivity |
| Conversion actions | ConversionActionService | Create and manage conversion tracking (sign-ups, event creation) |
| Campaign budgets | CampaignBudgetService | Create daily budgets within Ad Grants limits |
| Campaigns | CampaignService | Create and manage search campaigns with geo-targeting |
| Ad groups | AdGroupService | Organize ads and keywords by theme |
| Keywords | AdGroupCriterionService | Add keywords and negative keywords |
| Ads | AdGroupAdService | Create responsive search ads |
| Sitelinks | ExtensionFeedItemService | Add sitelink extensions to campaigns |

## Architecture

The tooling consists of 8 sequential Python scripts that use the official `google-ads` Python client library. Each script:

1. Authenticates via OAuth 2.0 (Desktop App flow)
2. Performs API operations against a single Google Ads account (458-730-8084)
3. Saves created resource IDs to a local state file for idempotency

All campaign configuration (ad copy, keywords, targeting, budgets) is defined in a local configuration file. No user data is collected or stored by the tool.

## Data Handling

- **No end-user data** is accessed or stored by the API tooling
- **OAuth credentials** are stored locally and never committed to version control
- The tool accesses only TrashMob's own Google Ads account

## Technical Details

- **Language:** Python 3.10+
- **Client Library:** google-ads (official Google Python client)
- **Authentication:** OAuth 2.0 Desktop App flow
- **Target Account:** 458-730-8084 (Google Ad Grants)
- **Manager Account:** 659-304-1445

## Contact

- **Organization:** TrashMob.eco
- **Contact:** joe@trashmob.eco
- **Website:** https://www.trashmob.eco
