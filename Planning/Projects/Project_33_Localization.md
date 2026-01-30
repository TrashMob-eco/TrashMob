# Project 33 — Site Localization

| Attribute | Value |
|-----------|-------|
| **Status** | Not Started |
| **Priority** | Low |
| **Risk** | Medium |
| **Size** | Large |
| **Dependencies** | None |

---

## Business Rationale

TrashMob's mission to reduce litter is global. Localizing the web and mobile applications enables volunteers worldwide to participate in their native language, increasing adoption and engagement in non-English speaking communities. This also supports partnerships with international environmental organizations.

---

## Objectives

### Primary Goals
- Enable multi-language support for web application
- Enable multi-language support for mobile application
- Implement language detection and selection
- Establish translation workflow for ongoing content updates

### Secondary Goals (Nice-to-Have)
- Right-to-left (RTL) language support (Arabic, Hebrew)
- Localized date, time, and number formats
- Region-specific content (local partners, regulations)

---

## Scope

### Phase 1 - Infrastructure Setup
- ☐ Select and integrate i18n framework (react-i18next for web)
- ☐ Select and integrate localization for MAUI mobile app
- ☐ Extract all hardcoded strings to resource files
- ☐ Set up translation file structure
- ☐ Implement language detection (browser/device settings)
- ☐ Add language selector UI component

### Phase 2 - Initial Translation (Spanish)
- ☐ Translate all UI strings to Spanish
- ☐ Translate email templates
- ☐ Translate error messages
- ☐ QA review by native speaker
- ☐ Deploy Spanish language option

### Phase 3 - Translation Workflow
- ☐ Evaluate translation management platforms (Crowdin, Lokalise, POEditor)
- ☐ Set up automated string extraction pipeline
- ☐ Establish community translation contribution process
- ☐ Document translation guidelines and glossary

### Phase 4 - Additional Languages
- ☐ French (high volunteer population in Canada)
- ☐ German
- ☐ Portuguese
- ☐ Other languages based on demand

---

## Out-of-Scope

- ❌ CMS content translation (separate effort with Strapi)
- ❌ User-generated content translation (event descriptions, etc.)
- ❌ Real-time translation features
- ❌ Voice/audio localization

---

## Success Metrics

### Quantitative
- **String extraction:** 100% of UI strings externalized
- **Translation coverage:** 100% for each supported language
- **Non-English users:** Track adoption by language
- **Signup rate:** Measure conversion in localized regions

### Qualitative
- Native speakers confirm natural, accurate translations
- No broken UI from text expansion/contraction
- Positive feedback from international users

---

## Dependencies

### Blockers (Must be complete before this project starts)
- None (can begin infrastructure work anytime)

### Enablers for Other Projects (What this unlocks)
- International expansion
- Partnerships with non-US environmental organizations
- Community growth in Spanish-speaking regions

---

## Risks & Mitigations

| Risk | Likelihood | Impact | Mitigation |
|------|------------|--------|------------|
| **Text expansion breaks UI** | High | Medium | Design with 30-40% text expansion buffer; test all languages |
| **Poor translation quality** | Medium | High | Native speaker review; use professional translators for initial pass |
| **Ongoing maintenance burden** | High | Medium | Translation management platform; community contributions |
| **Missing context for translators** | Medium | Medium | Screenshot context; developer comments in resource files |
| **RTL layout complexity** | Medium | High | Defer RTL to future phase; use CSS logical properties |

---

## Implementation Plan

### Technology Choices

**Web (React):**
- **Framework:** react-i18next
- **Format:** JSON resource files
- **Structure:** Namespace-based (common, events, auth, etc.)

**Mobile (MAUI):**
- **Framework:** .NET localization (Microsoft.Extensions.Localization)
- **Format:** .resx resource files
- **Structure:** Per-feature resource files

### Web Implementation

```typescript
// i18n configuration
import i18n from 'i18next';
import { initReactI18next } from 'react-i18next';
import LanguageDetector from 'i18next-browser-languagedetector';

i18n
  .use(LanguageDetector)
  .use(initReactI18next)
  .init({
    fallbackLng: 'en',
    supportedLngs: ['en', 'es', 'fr', 'de', 'pt'],
    ns: ['common', 'events', 'auth', 'errors'],
    defaultNS: 'common',
    interpolation: {
      escapeValue: false,
    },
  });

// Usage in components
const { t } = useTranslation();
<button>{t('events.createEvent')}</button>
```

**Resource file structure:**
```
client-app/
  src/
    locales/
      en/
        common.json
        events.json
        auth.json
        errors.json
      es/
        common.json
        events.json
        auth.json
        errors.json
```

### Mobile Implementation

```csharp
// MAUI localization setup
builder.Services.AddLocalization();

// Usage in ViewModels
public class EventsViewModel
{
    private readonly IStringLocalizer<EventsViewModel> _localizer;

    public string CreateEventLabel => _localizer["CreateEvent"];
}
```

### Language Selector Component

```typescript
// LanguageSelector.tsx
const languages = [
  { code: 'en', name: 'English', flag: '🇺🇸' },
  { code: 'es', name: 'Español', flag: '🇪🇸' },
  { code: 'fr', name: 'Français', flag: '🇫🇷' },
];

const LanguageSelector = () => {
  const { i18n } = useTranslation();

  return (
    <select
      value={i18n.language}
      onChange={(e) => i18n.changeLanguage(e.target.value)}
    >
      {languages.map(lang => (
        <option key={lang.code} value={lang.code}>
          {lang.flag} {lang.name}
        </option>
      ))}
    </select>
  );
};
```

### API Considerations

- Accept-Language header support for error messages
- User preference stored in profile (optional)
- Email templates selected based on user language preference

---

## Implementation Phases

### Phase 1: Infrastructure Setup
- Install and configure i18n libraries
- Create resource file structure
- Extract strings from 2-3 key pages as proof of concept
- Implement language selector
- Set up build pipeline for resource files

### Phase 2: String Extraction
- Systematically extract all hardcoded strings
- Add translator comments for context
- Create English resource files as baseline
- Update components to use translation hooks

### Phase 3: Spanish Translation
- Engage professional translator or bilingual volunteer
- Translate all resource files
- QA review and corrections
- Deploy with language selector

### Phase 4: Translation Management
- Evaluate and select translation platform
- Set up automated sync with codebase
- Document contribution process
- Recruit community translators

### Phase 5: Additional Languages
- Prioritize based on user demand and volunteer availability
- Repeat translation and QA process
- Monitor adoption metrics

**Note:** Phases are sequential but not time-bound. Volunteers pick up work as available.

---

## Translation Guidelines

### Glossary (Key Terms)
| English | Spanish | Notes |
|---------|---------|-------|
| Event | Evento | Cleanup event |
| Litter | Basura | Trash/garbage |
| Volunteer | Voluntario/a | |
| Team | Equipo | |
| Community | Comunidad | |
| Lead | Líder | Event organizer |

### Style Guide
- Use formal "you" (usted) in Spanish for inclusivity
- Keep translations concise (UI space constraints)
- Maintain consistent terminology across the app
- Preserve placeholders exactly: `{{count}}` bags collected

---

## Open Questions

1. **Which language should be prioritized after English?**
   **Recommendation:** Spanish (large US Hispanic population, Latin America potential)
   **Owner:** Product Lead
   **Due:** Before Phase 2

2. **Should we use professional translators or community volunteers?**
   **Recommendation:** Professional for initial pass, community for ongoing maintenance
   **Owner:** Product Lead + Finance
   **Due:** Before Phase 2

3. **How do we handle CMS content localization?**
   **Recommendation:** Separate effort using Strapi's built-in i18n plugin
   **Owner:** Engineering Lead
   **Due:** Future planning

4. **Should user-generated content (event descriptions) be translatable?**
   **Recommendation:** Out of scope; users write in their preferred language
   **Owner:** Product Lead
   **Due:** N/A

---

## Related Documents

- **[Project 2 - Home Page](./Project_02_Home_Page.md)** - References multi-language as out-of-scope
- **[Project 16 - Content Management](./Project_16_Content_Management.md)** - CMS localization separate
- **[react-i18next docs](https://react.i18next.com/)** - Web framework documentation

---

**Last Updated:** January 26, 2026
**Owner:** Engineering Lead
**Status:** Not Started
**Next Review:** When prioritized
