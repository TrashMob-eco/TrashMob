# TrashMob Google Ad Grants — API Setup Scripts

Automates the configuration of TrashMob's Google Ad Grants account (458-730-8084) using the Google Ads API.

**Grant value:** Up to $10,000/month ($120,000/year) in free Google Search advertising.

## Prerequisites

### Already Complete
- [x] Goodstack (TechSoup) nonprofit verification
- [x] Google for Nonprofits account activation
- [x] Google Ads conversion tracking tag deployed (gtag.js in index.html)
- [x] Sign-up conversion event firing on account creation

### Still Needed

1. **Google Ads Manager (MCC) Account**
   - Create at: https://ads.google.com/home/tools/manager-accounts/
   - Link the Ad Grants account (458-730-8084) to the Manager account
   - The Manager account ID becomes your `login_customer_id`

2. **Developer Token**
   - In the Manager account, go to **Admin > API Center**
   - Copy the developer token (NOT available in regular accounts — only Manager accounts)
   - New tokens start as "Test Account" access — apply for **Basic Access** before running against the live account

3. **OAuth 2.0 Credentials**
   - Go to https://console.cloud.google.com/
   - Create a project (or use existing) and enable the **Google Ads API**
   - Create an **OAuth 2.0 Desktop App** credential under APIs & Services > Credentials
   - Note the `client_id` and `client_secret`

4. **Refresh Token**
   - Install the library: `pip install google-ads`
   - Run the helper script:
     ```bash
     python -c "from google_ads.google_ads.client import GoogleAdsClient; GoogleAdsClient.generate_user_credentials()"
     ```
     Or use: https://github.com/googleads/google-ads-python/blob/main/examples/authentication/generate_user_credentials.py
   - Authenticate as joe@trashmob.eco
   - Copy the `refresh_token` from the output

5. **Python 3.10+**

## Setup

```bash
cd Planning/GoogleAds

# Create virtual environment
python -m venv .venv
.venv/Scripts/activate  # Windows
# source .venv/bin/activate  # macOS/Linux

# Install dependencies
pip install -r requirements.txt

# Configure credentials
cp google-ads.yaml.template google-ads.yaml
# Edit google-ads.yaml and fill in:
#   developer_token, client_id, client_secret, refresh_token, login_customer_id
```

## Execution Order

Run scripts sequentially. Each saves resource IDs to `state.json` for subsequent scripts.

| Script | What it does |
|--------|-------------|
| `01_verify_access.py` | Verify credentials and confirm account 458-730-8084 |
| `02_create_conversions.py` | Create 4 conversion actions |
| `03_create_budgets.py` | Create campaign budgets ($100/day each) |
| `04_create_campaigns.py` | Create 6 campaigns (PAUSED) with geo targeting |
| `05_create_ad_groups.py` | Create 2 ad groups per campaign (12 total) |
| `06_add_keywords.py` | Add keywords + negative keywords |
| `07_add_rsas.py` | Add responsive search ads per ad group |
| `08_add_sitelinks.py` | Add sitelinks to each campaign |

```bash
python 01_verify_access.py
# Verify output, then continue...
python 02_create_conversions.py
python 03_create_budgets.py
python 04_create_campaigns.py
python 05_create_ad_groups.py
python 06_add_keywords.py
python 07_add_rsas.py
python 08_add_sitelinks.py
```

**After all scripts complete:**
1. Review everything in the Google Ads UI at ads.google.com
2. Enable the Brand Campaign first (highest CTR, easiest compliance)
3. Wait 48 hours, then enable remaining campaigns one at a time

## State File

`state.json` tracks all created resource IDs so scripts can be re-run safely (they skip existing resources). If you need to start fresh, delete `state.json`.

## Campaigns Created

| Campaign | Geo Target | Ad Groups |
|----------|-----------|-----------|
| TrashMob Brand Campaign | US | Brand - Core, Brand - Mission |
| Volunteer Recruitment - Pacific NW | WA, OR | Cleanup Events, Community Service |
| Volunteer Recruitment - National | US | General, Community |
| City and Municipality Outreach | US | Programs, Adopt a Location |
| Event Organizer Recruitment | US | Create Events, Volunteer Coordination |
| Community Partners and Sponsors | US | Nonprofit Cleanup, Environmental Giving |

## Ad Grants Compliance Notes

- All campaigns start **PAUSED** — enable in the UI after review
- **Maximize Conversions** bidding removes the $2 CPC cap
- All keywords are 2+ words (single-word keywords violate policy)
- Account must maintain **5% CTR** — Brand campaign helps with this
- At least **one meaningful conversion per month** is required
- Account must be **actively managed** (no 90-day inactivity)
- Monthly maintenance: review Search Terms, pause low-CTR keywords, add negative keywords

## Security

**Never commit** `google-ads.yaml` or `state.json` — both are in `.gitignore`.
