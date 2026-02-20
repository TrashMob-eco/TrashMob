# Privacy Manifest — App Store & Play Store Declarations

| Attribute | Value |
|-----------|-------|
| **Last Updated** | 2026-02-20 |
| **Owner** | Product Lead |
| **Review Frequency** | Every release that adds new data collection or permissions |

---

## Purpose

This document tracks what data TrashMob collects, how it's used, and what must be declared in the Apple App Store privacy nutrition label, Google Play Data Safety form, and the iOS `PrivacyInfo.xcprivacy` manifest. When this file changes, a CI workflow creates a GitHub issue to remind you to update the store forms.

---

## 1. iOS Privacy Manifest (`PrivacyInfo.xcprivacy`)

**File:** `TrashMobMobile/Platforms/iOS/PrivacyInfo.xcprivacy`

| API Category | Reason Code | Justification |
|-------------|-------------|---------------|
| `NSPrivacyAccessedAPICategoryFileTimestamp` | `C617.1` | Access file timestamps for cache management |
| `NSPrivacyAccessedAPICategorySystemBootTime` | `35F9.1` | Used by Sentry SDK for crash diagnostics |
| `NSPrivacyAccessedAPICategoryDiskSpace` | `E174.1` | Used by Sentry SDK for device context |

**When to update:** Add entries when introducing new SDKs or using new Apple-required-reason APIs. Apple rejects apps that use these APIs without declaring reasons.

---

## 2. Data Collected by TrashMob

### Personal Information

| Data Type | Collected | Purpose | Shared With Third Parties | Store Declaration |
|-----------|-----------|---------|---------------------------|-------------------|
| **Name** (first, last) | Yes | User profile, event attendance | No | Apple: Name; Google: Name |
| **Email address** | Yes | Account authentication, notifications | SendGrid (email delivery) | Apple: Email Address; Google: Email address |
| **Date of birth** | Yes | Minor detection (13+ age gate) | No | Apple: N/A (not in nutrition label); Google: Date of birth |
| **Profile photo** | Optional | User avatar | Azure Blob Storage (hosting) | Apple: Photos or Videos; Google: Photos |
| **City/region** | Yes | Matching to local events | No | Apple: Coarse Location; Google: Approximate location |

### Location Data

| Data Type | Collected | Purpose | Shared With Third Parties | Store Declaration |
|-----------|-----------|---------|---------------------------|-------------------|
| **Precise location** | Yes (with permission) | Map display, event proximity, litter reports | Azure Maps (geocoding) | Apple: Precise Location; Google: Precise location |
| **Event location** | Yes | Event address for attendees | No (public to all users) | Apple: Precise Location; Google: Precise location |
| **Litter report location** | Yes | Report location on map | No (public to all users) | Apple: Precise Location; Google: Precise location |

### Usage Data

| Data Type | Collected | Purpose | Shared With Third Parties | Store Declaration |
|-----------|-----------|---------|---------------------------|-------------------|
| **Crash logs** | Yes | App stability monitoring | Sentry.io | Apple: Crash Data; Google: Crash logs |
| **Performance data** | Yes | App performance monitoring | Sentry.io | Apple: Performance Data; Google: Diagnostics |
| **App interactions** | Yes | Feature usage analytics | Azure Application Insights | Apple: Product Interaction; Google: App interactions |

### User-Generated Content

| Data Type | Collected | Purpose | Shared With Third Parties | Store Declaration |
|-----------|-----------|---------|---------------------------|-------------------|
| **Event details** | Yes | Event creation/management | No (public to all users) | Apple: Other User Content; Google: Other actions |
| **Event photos** | Optional | Event documentation | Azure Blob Storage (hosting) | Apple: Photos or Videos; Google: Photos |
| **Litter report photos** | Optional | Litter documentation | Azure Blob Storage (hosting) | Apple: Photos or Videos; Google: Photos |
| **Waiver signatures** | Yes | Legal compliance | No | Apple: N/A; Google: N/A |

---

## 3. iOS App Permissions (`Info.plist`)

| Permission | Key | Usage Description |
|-----------|-----|-------------------|
| Location (When In Use) | `NSLocationWhenInUseUsageDescription` | To show nearby events and allow creating events and litter reports at your location |
| Camera | `NSCameraUsageDescription` | To take photos of litter reports and events |
| Photo Library | `NSPhotoLibraryUsageDescription` | To select photos for litter reports and events |

---

## 4. Android Permissions (`AndroidManifest.xml`)

| Permission | Usage |
|-----------|-------|
| `ACCESS_FINE_LOCATION` | Map display, event proximity, litter reports |
| `ACCESS_COARSE_LOCATION` | Approximate location for event discovery |
| `CAMERA` | Taking photos for litter reports and events |
| `READ_EXTERNAL_STORAGE` | Selecting photos for upload |
| `INTERNET` | API communication |

---

## 5. Third-Party SDKs

| SDK | Data Sent | Privacy Policy |
|-----|-----------|----------------|
| **Sentry.io** | Crash logs, device info, OS version | https://sentry.io/privacy/ |
| **Azure Maps** | Location coordinates (for geocoding) | https://privacy.microsoft.com/ |
| **SendGrid** | Email addresses (for delivery) | https://www.twilio.com/legal/privacy |
| **MSAL (Microsoft)** | Auth tokens, user identifiers | https://privacy.microsoft.com/ |
| **Google Maps (Android)** | Location coordinates (for map display) | https://policies.google.com/privacy |

---

## 6. Store Form Quick Reference

### Apple App Store Privacy Nutrition Label

**URL:** https://appstoreconnect.apple.com → App → App Privacy

Categories to declare:
- Contact Info: Name, Email Address
- Location: Precise Location
- User Content: Photos or Videos, Other User Content
- Diagnostics: Crash Data, Performance Data
- Usage Data: Product Interaction

### Google Play Data Safety

**URL:** https://play.google.com/console → App → Data Safety

Categories to declare:
- Personal info: Name, Email address, Date of birth
- Location: Approximate location, Precise location
- Photos and videos: Photos
- App activity: App interactions
- App info and performance: Crash logs, Diagnostics

---

## 7. Change Log

| Date | Change | Store Update Required |
|------|--------|----------------------|
| 2026-02-20 | Initial privacy manifest created | Verify current store declarations match |
| | | |

**Instructions:** When you change data collection, add a row here AND update the relevant store forms. The CI workflow will create a GitHub issue to remind you.
