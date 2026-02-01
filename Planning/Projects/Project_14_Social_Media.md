# Project 14 — Improve Social Media Integration

| Attribute | Value |
|-----------|-------|
| **Status** | Not Started |
| **Priority** | Medium |
| **Risk** | Low |
| **Size** | Small |
| **Dependencies** | None |

---

## Business Rationale

Modernize social integrations across key touchpoints with thoughtful UX. Easy sharing increases event visibility and volunteer recruitment. Current sharing options are limited and outdated.

---

## Objectives

### Primary Goals
- **Audit current integrations** and identify gaps
- **Add current platforms** (Instagram, TikTok, LinkedIn, Threads)
- **Define sharing by context** with design/social input
- **Consistent sharing experience** across web and mobile

### Secondary Goals
- Open Graph meta tags optimization
- Twitter/X card previews
- Social proof widgets (follower counts, shares)
- Social login expansion

---

## Scope

### Phase 1 - Audit & Design
- ✅ Audit existing social integrations
- ✅ Design sharing UX patterns
- ✅ Define platform priorities
- ✅ Open Graph meta tag review

### Phase 2 - Share Improvements
- ✅ Add share buttons to events
- ✅ Add share buttons to teams/communities
- ✅ Add share buttons to impact stats
- ✅ Copy link functionality
- ✅ QR code generation

### Phase 3 - Platform Expansion
- ✅ Instagram sharing (story, post)
- ✅ LinkedIn sharing
- ✅ Bluesky sharing
- ✅ TikTok integration (if applicable)
- ✅ WhatsApp/Messenger sharing
- ✅ Reddit sharing
- ✅ Discord integration (TrashMob Discord server)

---

## Out-of-Scope

- ❌ Social media management tools
- ❌ Auto-posting to social accounts
- ❌ Social listening/monitoring
- ❌ Influencer management
- ❌ Paid social advertising integration
- ❌ Social login expansion (current auth options sufficient)

---

## Success Metrics

### Quantitative
- **Share button clicks:** Track per platform
- **Referral traffic from social:** ≥ 20% increase
- **Event signups from social shares:** Track attribution

### Qualitative
- Users find sharing intuitive
- Event leads actively promote events
- Increased social media mentions

---

## Dependencies

### Blockers
None

### Enables
- Organic growth through social sharing
- Better event promotion
- Community building

---

## Risks & Mitigations

| Risk | Likelihood | Impact | Mitigation |
|------|------------|--------|------------|
| **Platform API changes** | Medium | Low | Use standard share URLs; minimal API dependency |
| **Poor share previews** | Medium | Medium | Proper Open Graph tags; test across platforms |
| **Privacy concerns with tracking** | Low | Medium | Clear privacy policy; opt-out options |

---

## Implementation Plan

### API Changes

```csharp
// Generate shareable links
[HttpGet("api/events/{eventId}/share")]
public async Task<ActionResult<ShareLinksDto>> GetEventShareLinks(Guid eventId)
{
    // Return pre-formatted share URLs for each platform
}

[HttpGet("api/teams/{teamId}/share")]
public async Task<ActionResult<ShareLinksDto>> GetTeamShareLinks(Guid teamId)
{
    // Return pre-formatted share URLs for each platform
}

// Track share events (analytics)
[HttpPost("api/analytics/share")]
public async Task<ActionResult> TrackShare([FromBody] ShareTrackRequest request)
{
    // Log share for analytics (platform, content type, user)
}
```

### Web UX Changes

**Open Graph Meta Tags:**
```html
<!-- Event pages -->
<meta property="og:title" content="{Event Name} - TrashMob.eco">
<meta property="og:description" content="Join us for a cleanup event on {Date}!">
<meta property="og:image" content="{Event image or default}">
<meta property="og:url" content="https://trashmob.eco/events/{id}">
<meta property="og:type" content="website">

<!-- Twitter Cards -->
<meta name="twitter:card" content="summary_large_image">
<meta name="twitter:site" content="@trashmobeco">
```

**Share Component:**
```tsx
<ShareButtons
  url={eventUrl}
  title={eventTitle}
  description={eventDescription}
  platforms={['facebook', 'twitter', 'bluesky', 'linkedin', 'instagram', 'whatsapp', 'reddit', 'discord', 'copy']}
  onShare={(platform) => trackShare(platform)}
/>
```

**Share Locations:**
- Event detail page (prominent placement)
- Event cards (on hover/menu)
- Team pages
- Community pages
- User dashboard (share stats)
- Post-event summary

### Mobile App Changes

- Native share sheet integration
- Platform-specific sharing (Instagram stories, etc.)
- Deep link support for shared content

---

## Implementation Phases

### Phase 1: Audit & Foundation
- Audit current state
- Open Graph improvements
- Basic share buttons (web)

### Phase 2: Enhanced Sharing
- All platforms
- Mobile integration
- Analytics tracking

### Phase 3: Advanced Features
- QR codes
- Pre-filled templates
- Social proof widgets

**Note:** Small project; can be completed by one developer.

---

## Platform-Specific Notes

### Facebook
- Standard share dialog
- Open Graph required

### Twitter/X
- Tweet intent URL
- Twitter cards meta tags

### Bluesky
- Post intent URL (similar to Twitter)
- Growing platform with environmental community
- AT Protocol-based sharing

### Instagram
- Stories via mobile app only
- Link in bio tracking

### LinkedIn
- Professional audience
- Share URL with title

### WhatsApp/Messenger
- Direct share links
- Mobile-focused

### TikTok
- Limited share API
- QR code focus

### Reddit
- Share to subreddits (r/DeTrashed, local community subreddits)
- Cross-post to relevant environmental/volunteer communities
- Submit link with pre-filled title

### Discord
- TrashMob Discord server for community engagement
- Share event links to Discord channels
- Webhook integration for event notifications
- Invite link on website/mobile app

---

## Open Questions

1. ~~**Priority platforms?**~~
   **Decision:** All platforms equal priority: Facebook, Twitter/X, Bluesky, LinkedIn, WhatsApp, Instagram, Reddit, Discord, TikTok, Copy Link
   **Status:** ✅ Resolved

2. ~~**Instagram story templates?**~~
   **Decision:** Design branded story templates; implement if resources allow (nice-to-have, not blocking)
   **Status:** ✅ Resolved

3. ~~**Social login expansion?**~~
   **Decision:** Out of scope; current auth options are sufficient
   **Status:** ✅ Resolved

---

## Related Documents

- **[Project 2 - Home Page](./Project_02_Home_Page.md)** - Social proof on home page
- **[Project 9 - Teams](./Project_09_Teams.md)** - Team sharing
- **[Project 10 - Community Pages](./Project_10_Community_Pages.md)** - Community sharing

---

**Last Updated:** January 31, 2026
**Owner:** Web Team
**Status:** Not Started
**Next Review:** When prioritized
