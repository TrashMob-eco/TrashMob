# TrashMob Feature Usage Metrics Guide

This guide documents the feature usage metrics tracked in the TrashMob web application and how to use the dashboards for data-driven decision making.

## Overview

TrashMob tracks feature usage through Application Insights custom events. These metrics help:
- Measure feature adoption and engagement
- Identify trends and user behavior patterns
- Support grant applications with usage data
- Prioritize development efforts based on actual usage

## Accessing the Dashboard

1. Navigate to the [Azure Portal](https://portal.azure.com)
2. Go to the TrashMob Application Insights resource (`as-tm-{env}-{region}`)
3. Select **Workbooks** from the left menu
4. Open **TrashMob Feature Usage Metrics** workbook

## Tracked Events

### Authentication Events
| Event Name | Description | Properties |
|------------|-------------|------------|
| `Auth_Login_Success` | User successfully logged in | timestamp |
| `Auth_Logout_Success` | User logged out | timestamp |

### Event Lifecycle Events
| Event Name | Description | Properties |
|------------|-------------|------------|
| `Event_Create_Submit` | Cleanup event created | eventTypeId, fromLitterReport |

### Attendance Events
| Event Name | Description | Properties |
|------------|-------------|------------|
| `Attendance_Register_Submit` | User registered for event | eventId |

### Litter Report Events
| Event Name | Description | Properties |
|------------|-------------|------------|
| `LitterReport_Create_Submit` | Litter report submitted | imageCount |

## Key Performance Indicators (KPIs)

### Primary KPIs

| KPI | Target | Measurement |
|-----|--------|-------------|
| **Daily Active Users** | Growing trend | Unique users logging in per day |
| **Weekly Events Created** | 10+ per week | Count of `Event_Create_Submit` events |
| **Registration Rate** | 50%+ | Registrations / Event page views |
| **Weekly Litter Reports** | 20+ per week | Count of `LitterReport_Create_Submit` events |

### Secondary KPIs

| KPI | Target | Measurement |
|-----|--------|-------------|
| **Event Completion Rate** | 80%+ | Events with summaries / Total events |
| **Avg Images per Report** | 2+ | Average `imageCount` property value |
| **Events from Litter Reports** | 20%+ | Events with `fromLitterReport=true` |

## Dashboard Sections

### 1. Overview
- Total feature events in selected time period
- Trend charts showing usage over time
- Quick comparison across all feature categories

### 2. Authentication Metrics
- Login/logout activity patterns
- Peak usage hours
- Session patterns

### 3. Event Creation Metrics
- Events created per day/week
- Breakdown by event type
- Events created from litter reports vs direct creation

### 4. Attendance Metrics
- Registration trends over time
- Top events by registration count
- Registration conversion insights

### 5. Litter Report Metrics
- Submission trends
- Average images per report
- Geographic distribution (when available)

### 6. User Journey Funnel
- Visualization of user progression:
  1. Login → 2. Event View → 3. Registration → 4. Event Creation
- Identifies drop-off points for optimization

## Automated Reports

### Weekly Summary (if configured)
- Sent every Monday morning
- Contains key metrics for the previous 7 days
- Highlights trends and notable changes

### Low Activity Alert
- Triggered if no events created in 7 days
- Indicates potential issues or seasonal patterns
- Helps maintain awareness of platform activity

## Privacy Considerations

All metrics are collected with privacy in mind:
- **No PII**: User IDs are hashed before tracking
- **Aggregate focus**: Dashboards show aggregate trends, not individual users
- **Minimal properties**: Only essential metadata is captured
- **Retention**: Data retained per Application Insights settings (90 days default)

## Using Metrics for Grant Applications

When preparing grant applications, use these metrics to demonstrate impact:

1. **User Growth**: Show trend of new signups and active users
2. **Event Activity**: Highlight total events created and attendees
3. **Community Engagement**: Show litter reports submitted and addressed
4. **Feature Adoption**: Demonstrate usage of key platform features

Export data from the workbook using the export button (Excel, CSV) for inclusion in reports.

## Querying Raw Data

For custom analysis, use Azure Log Analytics:

```kusto
// Example: Feature usage by category last 30 days
customEvents
| where timestamp >= ago(30d)
| where name startswith "Auth_" or name startswith "Event_"
    or name startswith "Attendance_" or name startswith "LitterReport_"
| summarize Count = count() by Category = extract("^([^_]+)", 1, name)
| order by Count desc
```

## Future Enhancements

Planned additions to metrics tracking:
- Partner/community interaction events
- Search and discovery events
- Team creation and membership events
- Enhanced user journey analytics

---

**Document Version:** 1.0
**Last Updated:** February 2026
**Owner:** Engineering Team
