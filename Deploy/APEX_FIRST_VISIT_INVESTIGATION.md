# Apex First-Visit "Site Not Found" — Investigation Log

**Status:** ✅ Root cause identified and mitigated 2026-07-21. See change log entry for that date; short version: Azure DNS alias-A resolution for the AFD Standard endpoint was returning the ACA env's static IP (20.69.75.244) from a subset of nameservers, poisoning the caches of every major public resolver (Cloudflare, Google, Quad9) for months. Fixed by replacing the apex alias with explicit AFD anycast A records + binding apex to the Container App as a temporary safety net for stale-cache clients.
**First observed:** ongoing for months as of 2026-07-05
**Reproduction rate:** intermittent; multiple users report seeing it on first visit to `trashmob.eco`
**Owner:** Joe

**⚠ Theories 1-4 below are preserved for historical accuracy but were all wrong.** The actual root cause turned out to be an Azure DNS bug, not a TLS/HSTS/POP-config issue as originally hypothesized. Read the 2026-07-21 change log entry first.

## Symptom

A user types `trashmob.eco` into their browser (no protocol, no `www.`) and sees a "site not found" style error page. Refreshing the same URL then loads the site normally. Reported by the owner and by multiple other users. Been happening for months; the DNS / Front Door / Container App config has not changed in that window, so this is **not** a residual artifact of the App Service → Container App migration.

## What is verified working (2026-07-05)

| Path | Result |
|---|---|
| `Resolve-DnsName trashmob.eco -Type A` | 13.107.253.70 + 13.107.226.70 (Microsoft Front Door anycast) ✓ |
| `Resolve-DnsName www.trashmob.eco -Type A` | CNAME → `fde-tm-pr.azurefd.net` → `mr-azurefd.tm-azurefd.net` → 150.171.110.146 ✓ |
| `HEAD http://trashmob.eco/` | 307 → `https://trashmob.eco/` ✓ |
| `HEAD https://trashmob.eco/` | 308 → `https://www.trashmob.eco/` ✓ |
| `HEAD http://www.trashmob.eco/` | 307 → `https://www.trashmob.eco/` ✓ |
| `HEAD https://www.trashmob.eco/` | 200 with index.html (4,442 bytes) ✓ |

All responses carry `x-azure-ref` proving Front Door is the responder. TLS handshakes complete cleanly. Container App has `minReplicas: 1` in `Deploy/containerApp.bicep`, so scale-to-zero cold start is not the cause either.

**Conclusion:** the routing and cert chain both work from a warm probe. The bug is either intermittent at the Front Door / CDN edge, browser-side, or specific to the "cold" state of a fresh browser session.

## Working theories

### Theory 1 (most compelling): Two-hop TLS handshake on cold browser sessions

**Why the double redirect happens** — reading [`Deploy/frontDoor.bicep`](frontDoor.bicep):

- The **route** (`route-default`) has `httpsRedirect: 'Enabled'` on both protocols. Any HTTP request gets an unconditional 307 → HTTPS *same host*, applied at the route level *before* rule sets run.
- The **rule set** (`ApexRedirect`) only fires when the hostname equals `trashmob.eco` AND after the route-level httpsRedirect has already run. So the rule set only ever sees HTTPS traffic; it never gets a chance to catch HTTP-apex and redirect straight to HTTPS-www.

So the chain a cold browser walks when the user types `trashmob.eco`:

1. Browser tries `http://trashmob.eco/` (no HSTS → uses HTTP)
2. Front Door route-level `httpsRedirect` → 307 → `https://trashmob.eco/` → **first TLS handshake** for the apex hostname
3. Front Door rule set fires → 308 → `https://www.trashmob.eco/` → **second TLS handshake** for the www hostname (SNI is different, so cannot reuse the apex session)
4. Origin serves 200

Two consecutive TLS handshakes on a slow / high-latency connection can push total time-to-first-byte past the browser's "loading forever" threshold, or trip an intermediary (corporate proxy, mobile carrier, captive portal) that gives up on the second handshake. Chrome / Edge / Safari all have distinct heuristics here; **on refresh, HSTS for `www.trashmob.eco` is now cached** (from the first attempt reaching the www target), so the second request goes straight to `https://www.trashmob.eco/` in one hop — no double handshake — and it succeeds instantly. That matches the observed pattern.

This would also explain "months, still happening, across users": every new user, every new device, every incognito session starts cold and has the same double-handshake vulnerability.

### Theory 2: HSTS is not preloaded on apex

Corollary of Theory 1. If we sent `Strict-Transport-Security` on the apex (`trashmob.eco`) response with `includeSubDomains; preload`, cold browsers would skip the HTTP hop on subsequent visits — but the *first ever* visit would still hit HTTP → HTTPS apex → HTTPS www. HSTS preloading via the Chromium list would fix the cold case for the vast majority of browsers, but requires a submission (see [hstspreload.org](https://hstspreload.org)) and cannot be undone.

### Theory 3: Front Door edge POP returning a stale routing config

Front Door has many POPs. If a subset are serving a stale route configuration (e.g., missing the apex → www redirect rule after a config publish), users routed to those POPs see a 404 or connection reset on first visit. Retry from the same device may hit a healthier POP (anycast route can shift under load), which would look like "refresh worked." Would explain intermittency but not the "months, unchanged config" longevity — unless a config republish is silently dropping partial state each time.

### Theory 4: Two-hop redirect is triggering browser retry logic that races the actual response

Some browsers, on cold sessions, initiate a URL-bar autocomplete DNS prefetch in parallel with the actual navigation. If the prefetch for `trashmob.eco` returns first and gets a Front Door redirect response for HTTPS while the actual navigation is still on HTTP, some races have been observed to surface as "site not found" errors. This is speculative and hard to prove without in-browser tracing.

## Not-yet-checked

- What error message do affected users actually see? The wording ("This site can't be reached" vs `DNS_PROBE_FINISHED_NXDOMAIN` vs `ERR_CONNECTION_TIMED_OUT` vs an HTML error page from Front Door / origin) is diagnostic:
  - `DNS_PROBE_FINISHED_NXDOMAIN` → intermittent DNS negative-cache from an intermediate resolver
  - `ERR_CONNECTION_TIMED_OUT` → Front Door POP not responding on first visit
  - `ERR_SSL_PROTOCOL_ERROR` → cert mismatch or TLS handshake failure (Theory 1 candidate)
  - Front Door HTML page → route config miss (Theory 3 candidate)

  **Ask affected users for a screenshot next time it happens.** Different error messages point at different theories.
- Whether HSTS response headers are currently emitted at all. `Deploy/frontDoor.bicep` doesn't add a response-header rule for `Strict-Transport-Security`, and none appeared in the 2026-07-05 HEAD probes above, so most browsers are not caching HSTS today — they walk the HTTP → HTTPS-apex → HTTPS-www chain on every cold session. See Fix B.
- Whether the apex TLS cert covers `trashmob.eco` specifically (managed certs from Container Apps only cover the exact bound hostname; Front Door managed certs can cover both apex and www but require separate hostname bindings).
- Front Door access logs filtered by `X-Cache: CONFIG_NOCACHE` and status ≠ 200 — would show whether the failed first-visits are actually reaching Front Door at all (Theory 3 evidence) or are dying at the resolver / TLS layer before that (Theory 1 evidence).

## Candidate fixes (do not run without confirming a theory first)

Ordered from lowest-risk to highest.

### Fix A: Collapse the two-hop redirect chain (Theory 1 candidate)

In `Deploy/frontDoor.bicep`:
- Set the `route-default` route's `httpsRedirect: 'Disabled'`.
- Broaden the `RedirectApexToWww` rule set so it fires for **both HTTP and HTTPS** apex traffic and outputs `redirectType: 'PermanentRedirect'` with `destinationProtocol: 'Https'` and `customHostname: primaryDomain`. That already produces a full-URL `https://www.trashmob.eco/` Location header from either `http://trashmob.eco/` or `https://trashmob.eco/` in one hop.
- Add a second rule for `www.trashmob.eco` that fires only on HTTP → `Https` same-host redirect. Or leave that path as-is if you accept a small 307 for `http://www.trashmob.eco/*` visitors.

Net effect: cold browsers do exactly one TLS handshake instead of two. Reversible; low risk; standard "canonical www" redirect setup.

### Fix B: Enable HSTS with `includeSubDomains` (Theory 1 / 2)

Add `Strict-Transport-Security: max-age=31536000; includeSubDomains` to Front Door responses. After a user's first successful visit, subsequent visits will use HTTPS directly. **Does not fix the cold case** for a user who has never visited before; only helps repeat visitors. Cannot be trivially unsettled once shipped (HSTS is a promise the browser will honor for `max-age` seconds). Prerequisite: every subdomain of `trashmob.eco` must be HTTPS-capable or `includeSubDomains` will break them.

### Fix C: Submit to HSTS preload list

After Fix B has been stable for a few months, submit the domain to [hstspreload.org](https://hstspreload.org). Chromium ships a hard-coded HSTS list, so preloaded domains skip HTTP even on the very first visit from a brand-new install. Fixes the cold case for browsers that ship the preload list (which is most). **Very hard to reverse** — takes 6-8 weeks minimum to remove a domain from the list once accepted. Do not submit until you're fully confident every hostname under `trashmob.eco` will always speak HTTPS.

### Fix D: Investigate and re-publish the Front Door route config (Theory 3)

If Theory 3 evidence appears (Front Door access logs show a nonzero rate of 404s on first-touch requests), review the current route rules in the Azure portal against [`Deploy/frontDoor.bicep`](frontDoor.bicep) to check for drift, then re-apply the Bicep. `az deployment group create` is idempotent — see the "Re-deploy Front Door" section in [`Deploy/OPERATIONS_RUNBOOK.md`](OPERATIONS_RUNBOOK.md).

## Next actions when this comes up again

1. **Get a screenshot of the failure** from any affected user — the exact error text is the biggest signal.
2. If time permits, open Chrome DevTools → Network → Preserve Log, reproduce with cache disabled, capture the full request/response chain. Attach the HAR to this file.
3. If Fix A ("collapse redirect chain") is still on the table, start there — it's cheap, reversible, and independently the right thing to do regardless of whether it turns out to be the root cause.
4. Update this file with what you find. Add a section under `Working theories` with a date, or an entry under `## Not-yet-checked`.

## Change log

- **2026-07-21 (Theory 5 — actual root cause) — Azure DNS alias resolution for AFD Standard endpoints returns the origin's ACA env IP; caches at Cloudflare/Google/Quad9 were poisoned for months.** Reddit crowd-source ([post](https://www.reddit.com/r/azure/)) surfaced two decisive signals: (1) one respondent saw an "Error 404 - This Container App is stopped or does not exist" blue page — the Azure Container Apps environment default 404, only served when a request reaches the ACA env with a Host header not bound to any custom domain; (2) a second respondent claimed "DNS shows apex and www pointing directly to Container Apps." The second observation was correct; ours was wrong.

  **Investigation** — inspection of the four Azure DNS authoritative nameservers returned three different answers to the same query for the apex alias A record:
  - `ns1-07`, `ns2-07`, `ns4-07`: `13.107.226.70` + `13.107.253.70` (classic AFD anycast — correct)
  - `ns3-07`: `150.171.110.146` (AFD Std/Prem frontend — also correct)
  - Public resolvers (`1.1.1.1`, `8.8.8.8`, `9.9.9.9`): `20.69.75.244` — **the ACA env's static IP** (`az containerapp env show -n cae-tm-pr-westus2 --query "properties.staticIp"` returns the same value)

  None of the four Azure NSes were currently returning `20.69.75.244`, but the poisoned answer was pervasively cached in the three most-used public resolvers. The alias A record's `targetResource.id` pointed at the AFD endpoint resource, which should resolve to AFD frontend IPs — but Azure DNS was apparently "walking" the AFD endpoint's origin group → origin's `hostName` (the ACA default FQDN) → A record and returning that. That's an alias-resolver bug — Azure DNS should return only the target's own frontend IPs, not traverse into origins.

  **Why this looked intermittent for months** — users whose recursive resolver happened to be pointed at Cloudflare/Google/Quad9 (huge fraction of internet DNS) got `20.69.75.244` and connected directly to the ACA env with `Host: trashmob.eco`, which was never bound as a custom domain on ACA → TLS handshake fails / connection reset / blue "Container App not found" 404 (depending on ACA env behavior at that moment). Users whose resolver was pointed elsewhere got a valid AFD anycast IP and everything worked. Refresh sometimes worked because a re-attempt might race a different resolver path.

  **Fix (live, applied 2026-07-21 ~14:20 UTC):**
  1. Deleted the apex alias A record, replaced with explicit A records for `13.107.226.70` + `13.107.253.70` (classic AFD anycast, routes to any AFD tenant via SNI). TTL 300 for future agility.
  2. Bicep change to [`Deploy/dnsZone.bicep`](dnsZone.bicep) removes the `frontDoorEndpointId` alias-target param and declares the explicit `ARecords` array with the AFD anycast pair.

  **Follow-on outage (fixed same day)** — first attempt to remove the orphan `www.trashmob.eco` custom-domain binding on the Container App (redditor's advice, sound in principle) took the site down for every user whose resolver still had the poisoned `www → ACA FQDN → 20.69.75.244` cache — with the CA binding gone, ACA no longer had a cert for `www.trashmob.eco`, so those users got TLS reset. Reversed immediately with `az containerapp hostname bind`. Lesson: **cannot remove a binding that has been publicly-served for a long time until stale caches at all major resolvers have provably aged out.**

  **Bridge state (for at least 24-48h from 2026-07-21):** ACA has BOTH `www.trashmob.eco` and `trashmob.eco` bound as custom domains, each with its own managed cert (`trashmob-eco-cert` and `trashmob-eco-apex-cert`). Front Door is the canonical path for both. The ACA bindings exist purely to TLS-terminate stale-cache clients. Once telemetry (or manual probes at 1.1.1.1/8.8.8.8/9.9.9.9) shows apex A record has fully re-resolved to `13.107.226.70`/`13.107.253.70`, the ACA bindings can be removed. Bicep changes in this PR declare both bindings so the next release does not strip them.

  **Not fixed by this change** — Azure DNS's underlying alias-resolution bug is still there and will presumably keep affecting anyone else on the internet who follows Microsoft's own "use an alias A record for apex" AFD documentation. Worth a Microsoft support ticket / GitHub issue against `MicrosoftDocs/azure-docs` documenting the reproduction.

- **2026-07-05 (late evening, apex restored)** — Apex domain fully restored; Fix A confirmed live on all four paths.
  - After the customDomains hotfix in #3492 landed, `az afd route show` confirmed both custom domains were bound to the route, but apex probes kept returning 404 / connection reset. Root cause: `az afd custom-domain show trashmob-eco` reported `domainValidationState: PendingRevalidation` while `www-trashmob-eco` was `Approved`. The stripped-then-re-added apex binding forced a fresh managed-cert validation, and the `_dnsauth.trashmob.eco` TXT record still held the *original* validation token from initial cutover.
  - Recovery: `az afd custom-domain regenerate-validation-token` on the apex custom-domain issued a new token (`_iuufe275dy62wlzy8vs9ico8lk80prs`, expires 2026-07-13). Then `az network dns record-set txt remove-record` (old token) + `add-record` (new token) on the `_dnsauth.trashmob.eco` TXT record. AFD detected the match within ~10 min, transitioned state `Pending` → `Approved` with `deploymentStatus: InProgress`, and edge POPs picked up the binding shortly after.
  - **Final verification** (probes ~15 min after revalidation, from a cold PowerShell session):
    - `http://trashmob.eco/` → **308 → `https://www.trashmob.eco/` in a single hop** ← Fix A goal achieved
    - `https://trashmob.eco/` → 308 → `https://www.trashmob.eco/`
    - `http://www.trashmob.eco/` → 308 → `https://www.trashmob.eco/`
    - `https://www.trashmob.eco/` → 200
    - All four responses carry `x-azure-ref` proving Front Door is responding; single TLS handshake for cold visitors as intended.
  - **Follow-up hygiene**: `Deploy/dnsZone.bicep` still contains `<ADD_VALIDATION_TOKEN_FROM_AZURE_PORTAL>` placeholders for the `_dnsauth` TXT records. If that Bicep is ever re-applied without patching, it will overwrite the real token again and trigger the same outage-then-revalidate loop. Options: commit the real token (tokens are not secrets), externalize as a Bicep parameter with a default, or upgrade the comment to a hard "DO NOT RE-APPLY WITHOUT UPDATING" warning. Tracked as a to-do.
  - **Watch for**: does the "first-visit site not found" symptom stop being reported? If Theory 1 was the actual root cause, we should stop seeing user reports within a couple of weeks. If it keeps happening, revisit Theory 2 (HSTS) or Theory 3 (edge POP staleness).
- **2026-07-05 (evening, +90 min after Fix A)** — Prod incident: apex outage caused by the Fix A re-deploy stripping the route → custom-domain associations.
  - **Symptom** (verified via 3 consecutive probes ~30 min post-Fix-A-deploy): `http://trashmob.eco/` returned `404 Not Found`, `https://trashmob.eco/` returned "connection closed" during TLS handshake, `www.trashmob.eco/` continued to serve normally.
  - **Root cause**: `Deploy/frontDoor.bicep` declared both custom domain resources (`customDomainWww`, `customDomainApex`) but the `route-default` resource had **no `customDomains` array binding them**. The historic associations were added manually via the Azure portal at initial cutover and never captured in Bicep. ARM Incremental deploys still reset properties declared on a resource; "not mentioned" collapsed to "empty" during the Fix A apply, unbinding both domains from the route. `www.trashmob.eco/` kept working via `linkToDefaultDomain: 'Enabled'` (the default `.azurefd.net` endpoint acted as an implicit route target).
  - **Fix** ([#3492](https://github.com/TrashMob-eco/TrashMob/pull/3492), synced to main in [#3493](https://github.com/TrashMob-eco/TrashMob/pull/3493)): added `customDomains: concat([{ id: customDomainWww.id }], [{ id: customDomainApex.id }])` to the route, plus a WARNING comment above the resource so future editors don't drop the association again. Bicep infers deploy ordering from the `.id` references — no explicit `dependsOn` needed.
  - **Verification via `az afd route show`** post-hotfix-deploy: both custom domains bound with `isActive: true`, `httpsRedirect: 'Disabled'` (Fix A intact), `ruleSets` still pointing at `ApexRedirect`. Server-side config exactly as intended.
  - **Post-hotfix probes**: apex still returning 404 / connection reset ~5 min after deploy. Attributed to slower propagation of custom-domain-to-route bindings than route/rule changes alone (Front Door apex domain bindings are documented to take up to 30 min). Not resolved at time of this entry — check again at the 30–60 min mark.
  - **Lesson for next time**: any Bicep resource whose properties can be set out-of-band (Azure portal, Azure CLI, another template) must have those properties captured in the Bicep. An "empty" or "absent" property in a subsequent apply is a *reset* to empty, not a "leave alone." The rule-of-thumb "Bicep incremental preserves what you don't declare" is only true at the *resource* level, not the *property* level.
- **2026-07-05 (evening)** — Fix A deployed to production Front Door (`fd-tm-pr` in `rg-trashmob-pr-westus2`).
  - Bicep changes shipped via [#3487](https://github.com/TrashMob-eco/TrashMob/pull/3487) (Fix A) merged to `main`, then [#3489](https://github.com/TrashMob-eco/TrashMob/pull/3489) merged main → release.
  - First `az deployment group create` attempt failed preflight — `Microsoft.Cdn 2024-02-01` now requires `queryStringCachingBehavior` on any route with `cacheConfiguration`. The pre-existing block had it implicit. Hotfix [#3490](https://github.com/TrashMob-eco/TrashMob/pull/3490) added `queryStringCachingBehavior: 'IgnoreQueryString'` (matches effective behavior). Same fix synced back to main in [#3491](https://github.com/TrashMob-eco/TrashMob/pull/3491).
  - Second `az deployment group create` succeeded at 16:56Z, `provisioningState: Succeeded`, `duration: PT1M48.2140413S`, all 10 resources including the new `RedirectApexToWww` + `RedirectWwwHttpToHttps` rules deployed.
  - **First probe ~1 min post-deploy still showed the old two-hop chain** (`http://trashmob.eco/` → 307 → `https://trashmob.eco/`). Attributed to normal Front Door edge propagation (5–15 min per POP, anycast can serve stale POPs longer). Awaiting re-probe at 30+ min mark to confirm the new behavior lands.
  - If probes still show two hops after 30+ min, next step is `az afd route show --resource-group rg-trashmob-pr-westus2 --profile-name fd-tm-pr --endpoint-name fde-tm-pr --route-name route-default` to inspect the actually-deployed route state and confirm `httpsRedirect: 'Disabled'` persisted.
- **2026-07-05 (afternoon)** — Investigation opened. Confirmed all four HTTP/HTTPS paths return correct status codes from a warm probe. Theories 1–4 documented. Fix A queued.
