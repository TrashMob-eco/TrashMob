# Project 59 — Event Weather Forecast

| Attribute | Value |
|-----------|-------|
| **Status** | Complete (Phases 1-3); Phase 4 (severe weather alerts) deferred |
| **Priority** | Medium |
| **Risk** | Low |
| **Size** | Small |
| **Dependencies** | None |

---

## Business Rationale

Volunteers planning to attend a cleanup event need to know what weather conditions to expect so they can dress appropriately, bring rain gear, or decide whether outdoor conditions are safe. Showing the weather forecast directly on the event details page (web and mobile) removes the need to look it up separately and helps event leads anticipate attendance changes due to weather. This is especially important for events scheduled days or weeks in advance where conditions may change.

---

## Objectives

- Display a weather forecast summary on event details pages for both web and mobile
- Show conditions for the specific event date, time, and GPS coordinates
- Include temperature, conditions (sunny/cloudy/rain), wind speed, and precipitation chance
- Show a severe weather alert banner when applicable
- Cache weather data to avoid excessive API calls

---

## Scope

### Phase 1 — Backend Weather Service

Create a backend weather proxy endpoint that fetches and caches forecast data. Using a backend proxy avoids exposing API keys to clients and enables server-side caching.

- [x] Select weather API provider (Open-Meteo recommended — free, no API key, 10k requests/day, 7-day forecast)
- [x] Create `IWeatherService` interface in `TrashMob.Shared/Services/`
- [x] Implement `OpenMeteoWeatherService` using Open-Meteo's forecast API (`https://api.open-meteo.com/v1/forecast`)
- [x] Request parameters: `latitude`, `longitude`, `start_date`, `end_date`, hourly variables (`temperature_2m`, `precipitation_probability`, `weather_code`, `wind_speed_10m`)
- [x] Create `WeatherForecastDto` in `TrashMob.Models/Poco/V2/`: `Temperature` (°F), `ConditionCode`, `ConditionText`, `PrecipitationChance` (%), `WindSpeed` (mph), `IconUrl`, `IsAvailable`
- [x] Create `WeatherV2Controller` with `GET /v2/weather/forecast?lat={lat}&lng={lng}&date={date}&durationHours={hours}` (public, no auth)
- [x] Add in-memory cache (`IMemoryCache`) with 1-hour TTL per coordinate+date key
- [x] Return "unavailable" response for dates beyond forecast range (typically 7–16 days) — no error, just `isAvailable: false`
- [x] Register services in `ServiceBuilder.cs`
- [x] Add unit tests for service and controller

### Phase 2 — Web Event Details Integration

Add a weather card to the event details page between the event info and the description/map.

- [x] Create `WeatherForecast` service in `src/services/weather.ts` (public endpoint, factory pattern)
- [x] Create `WeatherCard` component in `src/components/events/weather-card.tsx`
  - Show: temperature, condition icon, condition text, precipitation chance, wind speed
  - Show "Forecast not yet available" for events >7 days out
  - Compact card design matching existing event detail cards
  - Use Lucide weather-related icons (Sun, Cloud, CloudRain, Wind, etc.)
- [x] Add `WeatherCard` to `eventdetails/$eventId/page.tsx` below the event info section
- [x] React Query with `staleTime: 60 * 60 * 1000` (1 hour) to match backend cache
- [x] Only render when event has latitude/longitude and event date is in the future

### Phase 3 — Mobile Event Details Integration

Add weather display to the mobile ViewEvent tab details page.

- [x] Create `IWeatherRestService` interface and `WeatherRestService` implementation
- [x] Create `WeatherForecastModel` in `TrashMobMobile/Models/`
- [x] Add weather properties to `ViewEventViewModel`: `Temperature`, `WeatherCondition`, `PrecipitationChance`, `WindSpeed`, `IsWeatherAvailable`
- [x] Add `GetWeatherForecastAsync()` call in `ViewEventViewModel.Init()` (parallel with other non-dependent calls)
- [x] Add weather section to `TabDetails.xaml` below the event info card — weather icon, temperature, condition text, precipitation, wind
- [x] Show "Forecast not yet available" placeholder for events beyond forecast range
- [x] Register `IWeatherRestService` in `MauiProgram.cs`

### Phase 4 — Severe Weather Alerts (Deferred)

- [ ] Check for active NWS (National Weather Service) alerts for event coordinates
- [ ] Display alert banner on event details with severity level and description
- [ ] Optional: Send push notification to registered attendees when severe weather alert issued for event day

---

## Out-of-Scope

- Historical weather data for past events
- Weather-based event auto-cancellation
- Multi-day forecast view (show only event day/time)
- User temperature unit preference (use °F, matching US-centric user base; internationalize later with Project 33)

---

## Success Metrics

| Metric | Target |
|--------|--------|
| Weather forecast visible on event details | 100% of future events with GPS coordinates |
| API response time (cached) | < 50ms |
| API response time (uncached, backend→Open-Meteo) | < 500ms |
| Cache hit rate | > 80% during peak hours |

---

## Risks & Mitigations

| Risk | Impact | Mitigation |
|------|--------|------------|
| Weather API rate limiting | Medium | Open-Meteo allows 10k/day free; server-side caching reduces calls significantly |
| Forecast accuracy for events >5 days out | Low | Show "approximate" qualifier for events >3 days out; hide for >16 days |
| API provider becomes unavailable | Low | `isAvailable: false` graceful fallback; swap providers behind `IWeatherService` interface |
| Timezone handling for event date | Medium | Use event's local time (derived from coordinates) for forecast lookup |

---

## Technical Design

### Weather API Selection: Open-Meteo

Open-Meteo is recommended because:
- **Free** — no API key required, no billing
- **No authentication** — simple REST calls
- **Generous limits** — 10,000 requests/day
- **Hourly forecasts** — can match event start time precisely
- **WMO weather codes** — standard condition codes with well-defined icon mappings
- **7–16 day range** — covers most event planning windows

### API Request Example

```
GET https://api.open-meteo.com/v1/forecast
  ?latitude=47.6062&longitude=-122.3321
  &hourly=temperature_2m,precipitation_probability,weather_code,wind_speed_10m
  &temperature_unit=fahrenheit&wind_speed_unit=mph
  &start_date=2026-03-25&end_date=2026-03-25
  &timezone=auto
```

### WMO Weather Code Mapping

| Code | Condition | Icon |
|------|-----------|------|
| 0 | Clear sky | Sun |
| 1-3 | Partly cloudy | CloudSun |
| 45, 48 | Foggy | CloudFog |
| 51-55 | Drizzle | CloudDrizzle |
| 61-65 | Rain | CloudRain |
| 71-75 | Snow | Snowflake |
| 80-82 | Rain showers | CloudRainWind |
| 95, 96, 99 | Thunderstorm | CloudLightning |

### Backend Cache Strategy

```
Cache key: $"weather:{lat:F2}:{lng:F2}:{date:yyyy-MM-dd}"
TTL: 1 hour
Eviction: Sliding expiration
```

Rounding coordinates to 2 decimal places (~1.1km precision) improves cache hit rate for nearby events without meaningful accuracy loss.

---

## Implementation Notes

- **Web placement:** Below event badges, above map — matches the "at a glance" information flow
- **Mobile placement:** New card below the Event Info card, above the action buttons
- **Both platforms:** Only show for future events; hide for past/completed events
- **Graceful degradation:** If weather service is down or event has no coordinates, simply don't render the component — no error state needed

---

## Rollout Plan

1. Phase 1 (backend) → Phase 2 (web) → Phase 3 (mobile) — sequential, each phase is independently deployable
2. No feature flag needed — weather is purely additive, non-breaking
3. Monitor Open-Meteo API call volume in Application Insights after web deployment before proceeding to mobile
