# Apex First-Visit "Site Not Found" — Investigation Log

**Status:** Open — root cause not yet confirmed
**First observed:** ongoing for months as of 2026-07-05
**Reproduction rate:** intermittent; multiple users report seeing it on first visit to `trashmob.eco`
**Owner:** Joe

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

- **2026-07-05 (evening)** — Fix A deployed to production Front Door (`fd-tm-pr` in `rg-trashmob-pr-westus2`).
  - Bicep changes shipped via [#3487](https://github.com/TrashMob-eco/TrashMob/pull/3487) (Fix A) merged to `main`, then [#3489](https://github.com/TrashMob-eco/TrashMob/pull/3489) merged main → release.
  - First `az deployment group create` attempt failed preflight — `Microsoft.Cdn 2024-02-01` now requires `queryStringCachingBehavior` on any route with `cacheConfiguration`. The pre-existing block had it implicit. Hotfix [#3490](https://github.com/TrashMob-eco/TrashMob/pull/3490) added `queryStringCachingBehavior: 'IgnoreQueryString'` (matches effective behavior). Same fix synced back to main in [#3491](https://github.com/TrashMob-eco/TrashMob/pull/3491).
  - Second `az deployment group create` succeeded at 16:56Z, `provisioningState: Succeeded`, `duration: PT1M48.2140413S`, all 10 resources including the new `RedirectApexToWww` + `RedirectWwwHttpToHttps` rules deployed.
  - **First probe ~1 min post-deploy still showed the old two-hop chain** (`http://trashmob.eco/` → 307 → `https://trashmob.eco/`). Attributed to normal Front Door edge propagation (5–15 min per POP, anycast can serve stale POPs longer). Awaiting re-probe at 30+ min mark to confirm the new behavior lands.
  - If probes still show two hops after 30+ min, next step is `az afd route show --resource-group rg-trashmob-pr-westus2 --profile-name fd-tm-pr --endpoint-name fde-tm-pr --route-name route-default` to inspect the actually-deployed route state and confirm `httpsRedirect: 'Disabled'` persisted.
- **2026-07-05 (afternoon)** — Investigation opened. Confirmed all four HTTP/HTTPS paths return correct status codes from a warm probe. Theories 1–4 documented. Fix A queued.
