# Route Tracking: See Where You Cleaned, How Much You Collected, and Where the Hotspots Are

**Slug:** route-tracking-see-where-you-cleaned-and-where-the-hotspots-are
**Author:** Joe Beernink
**Category:** Features
**Tags:** ["route-tracking", "features", "GPS", "litter-density", "heatmaps", "privacy", "maps"]
**Featured:** true
**Estimated Read Time:** 5

---

## Excerpt

A walkthrough of TrashMob's route tracking system — GPS route recording on mobile, litter density visualization, privacy-first design with spatial trimming, heat maps, and how route data flows into event summaries and community analytics.

---

## Body

When you pick up trash, you walk. Sometimes a mile around a park. Sometimes four miles along a highway shoulder. That walk tells a story — where you went, how long it took, how much litter you found along the way. Route tracking captures that story automatically.

TrashMob's route tracking records your GPS path while you clean, calculates distance and duration, and lets you log bags collected and weight removed per route. On the web, those routes become color-coded maps showing litter density, heat maps highlighting hotspot areas, and aggregate statistics that tell program managers exactly what their volunteers accomplished.

Here's how it all works.

### Recording a route

When you attend a cleanup event through the TrashMob mobile app, you can start route tracking with a single tap. The app records your GPS position as you walk, building a path of coordinates with timestamps and altitude data.

When you're done, stop the recording. The app calculates your total distance (using the Haversine great-circle formula for accuracy) and duration, then uploads the route to the server. You can log your bags collected and weight removed right there — those numbers attach to your specific route, not just the event overall.

The tracking runs in the background so you can use your phone for other things while cleaning. And it's designed to be battery-conscious — no one wants their phone dead halfway through a three-hour cleanup.

### Privacy by design

We thought hard about privacy before building this feature. Your GPS route is a record of exactly where you walked, which means it shows where you started — probably your home or your car.

So TrashMob applies **spatial trimming** automatically. By default, the first 100 meters and last 100 meters of every route are removed before anyone else can see it. Your full route is stored securely, but what's displayed to others has the endpoints clipped. No one sees where you parked.

Beyond that, every route has a **privacy level** that you control:

- **Private** — only you can see it. It appears in your personal route history but nowhere else.
- **Event Only** — visible to other attendees of the same event. This is the default.
- **Public** — visible on event maps and community analytics. Public routes automatically expire after two years — the data doesn't live forever.

You can change the privacy level at any time from your route history.

### Time trimming: fix recording mistakes

Forgot to stop recording when you got in your car and drove home? It happens. The time trim tool lets you adjust the end time of your route after the fact.

A slider shows your route's full duration. Drag it back to the moment you actually stopped cleaning, and TrashMob recalculates your distance and duration based only on the GPS points within the trimmed timeframe. The original data is preserved — if you trim too much, you can restore the route to its original state with one click.

This matters for data quality. A route that includes a 15-minute drive home inflates your distance by miles and drags down your litter density calculation. Time trimming keeps the numbers honest.

### Litter density: the metric that changes how you see routes

Distance and duration are useful, but litter density is where route tracking gets interesting.

When you log bags and weight for a route, TrashMob calculates **grams of litter per meter walked**. That single number tells you how dirty a stretch was. A quiet residential street might be 3 g/m. A highway interchange might be 80 g/m.

On the map, routes are color-coded by density:

- **Green** — under 5 g/m (light litter)
- **Yellow-green** — 5 to 15 g/m
- **Yellow** — 15 to 30 g/m (average)
- **Orange** — 30 to 60 g/m (above average)
- **Deep orange** — 60 to 120 g/m (heavy)
- **Red** — over 120 g/m (extremely heavy)
- **Gray** — no weight data logged

One glance at a density-colored map tells you which corridors need the most attention. Program managers use this to prioritize where to schedule future events and where to advocate for better infrastructure — more trash cans, better signage, increased enforcement.

### Three ways to view routes on the web

Every event page with recorded routes shows an interactive map with three viewing modes:

**Routes view.** Each volunteer's route is displayed as a distinct polyline in a different color — blue, red, green, purple, and so on through ten colors. Hover over any route to see its distance, duration, bags, and weight. This view answers "who cleaned where?"

**Density view.** Routes are colored by their litter density (the green-to-red scale above), with a legend overlay explaining the scale. This view answers "where was the litter worst?"

**Heat map view.** A Google Maps heat layer shows point density across all routes. Bright spots indicate corridors walked by multiple volunteers or areas with concentrated GPS points. This view answers "where did the most activity happen?"

Toggle between the three views with buttons above the map.

### Event statistics that write themselves

Below the route map, an event statistics card aggregates data from all recorded routes:

- **Total routes** recorded
- **Total distance** walked (in miles)
- **Total time** spent cleaning
- **Unique contributors** (volunteers who recorded routes)
- **Coverage area** (calculated by dividing the area into 25-meter grid cells and counting unique cells touched)
- **Average litter density** across all routes
- **Maximum litter density** for the worst stretch
- **Total bags** collected
- **Total weight** removed

These numbers update automatically as volunteers upload routes and log their metrics. No one has to compile a spreadsheet.

Even better: when an event organizer fills out the event summary after the cleanup, TrashMob **pre-fills** the summary form with aggregated route data — total bags, total weight, total duration, attendee count. The organizer can adjust the numbers if needed, but the baseline is already there. Less data entry, more accurate reporting.

### Your personal route history

Every volunteer has a **My Routes** section on their dashboard showing every route they've ever recorded. It's displayed two ways:

**Table view** — event name, date, distance, duration, density (with a color indicator), privacy level badge, and a trim button. Sort and scan your cleanup history at a glance.

**Map view** — all your routes overlaid on a single map, color-coded by density. Click any route to see the event name, date, distance, and duration in an info popup. This is your personal cleanup footprint — where you've walked, how much ground you've covered, and how the litter varied across locations.

### Why route data matters beyond individual events

Route tracking isn't just about one cleanup. Over time, the data tells larger stories:

**Seasonal patterns.** Compare density maps from spring versus fall. Are the same hotspots recurring? That's a systemic issue worth reporting to your city.

**Before and after.** Run a cleanup event, then run another on the same route a month later. If density dropped, your work had lasting impact. If it's back to the same level, the area needs structural intervention.

**Resource allocation.** When a program manager sees that Route A has 80 g/m density and Route B has 5 g/m, they know where to focus limited volunteer hours next time.

**Grant reporting.** "Our volunteers walked 1,247 miles, covered 342 acres, and removed 8,400 pounds of litter across 156 events" — that's a sentence that writes itself from route data, and it's the kind of sentence that gets grant applications funded.

### Start tracking your next cleanup

Route tracking is available in the TrashMob mobile app. Record your path, log your bags and weight, and let the data tell the story of your impact.

**Download the app or learn more at [trashmob.eco](https://www.trashmob.eco).**

---

*TrashMob.eco is a 501(c)(3) nonprofit dedicated to empowering communities to keep their neighborhoods clean.*

---

## Social Posts

### LinkedIn

We just published a walkthrough of TrashMob's route tracking system — the feature that turns every cleanup walk into actionable environmental data.

The highlights:

- GPS route recording on mobile with automatic distance and duration calculation
- Privacy by design: 100-meter spatial trimming removes start/end locations, three privacy levels, 2-year expiration on public routes
- Litter density visualization: grams per meter, color-coded from green (light) to red (heavy)
- Three map views: individual routes, density overlay, heat map
- Event statistics auto-aggregated from all volunteer routes — distance, coverage area, bags, weight, density
- Route data pre-fills event summaries, cutting data entry for organizers

One density map tells a program manager more about where to focus resources than a year of anecdotal reports.

trashmob.eco | info@trashmob.eco

#EnvironmentalData #CommunityCleanup #CivicTech #GPS #DataVisualization #NonProfit #Sustainability

### Reddit (r/DeTrashed)

We wrote up how route tracking works on TrashMob.eco — the free platform for organizing litter cleanups.

The idea: when you clean up, the mobile app records your GPS path. You log bags and weight for your route. That gives you litter density — grams per meter — which shows up as color-coded routes on event maps (green = light litter, red = heavy).

The practical value:

- See which corridors are the worst (density map)
- Compare the same route over time (is it getting better?)
- Aggregate stats across an event — total distance, coverage area, weight removed, all automatic
- Event summaries pre-filled from route data (less spreadsheet work)
- Personal route history on your dashboard so you can see everywhere you've ever cleaned

Privacy stuff: first/last 100m auto-trimmed so nobody sees where you parked. Three privacy levels (private, event-only, public). Public routes expire after 2 years.

Time trim tool fixes the "forgot to stop recording before driving home" problem without losing data.

We're a 501(c)(3) nonprofit. Free for individual volunteers.

Full post: trashmob.eco/news

### Bluesky

New post: how route tracking works on TrashMob.

GPS recording on mobile. Litter density visualization (grams per meter, color-coded). Heat maps. Privacy trimming so nobody sees where you parked. Event stats that aggregate automatically.

One density map tells you more about where to focus cleanup efforts than a stack of anecdotal reports.

trashmob.eco/news

### Newsletter

This week we published a deep dive into route tracking — the feature that turns every cleanup walk into a data point on a map. Here's the short version: when you clean up with the TrashMob mobile app, it records your GPS path, calculates your distance and duration, and lets you log bags and weight for your specific route. That data produces litter density numbers — grams of litter per meter walked — which show up as color-coded routes on event maps. Green means light litter. Red means heavy. Program managers can see at a glance which corridors need the most attention. The web interface offers three map views (individual routes, density overlay, and heat map), plus aggregate statistics that auto-populate event summaries. Privacy is built in: the first and last 100 meters of every route are automatically trimmed so nobody sees where you started, and you control whether your route is private, event-only, or public. If you forgot to stop recording before driving home, the time trim tool lets you fix it without losing data. Read the full walkthrough on our news page.
