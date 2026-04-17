"""Step 4: Create campaigns (all PAUSED) with Maximize Conversions bidding and geo targeting."""

from datetime import date

from config import CAMPAIGNS, CUSTOMER_ID
from state import get_client, load_state, print_error, save_state


def main():
    client = get_client()
    state = load_state()
    campaign_service = client.get_service("CampaignService")
    criterion_service = client.get_service("CampaignCriterionService")

    today = date.today().strftime("%Y%m%d")
    campaigns_to_create = []

    for campaign_def in CAMPAIGNS:
        if campaign_def["name"] in state["campaigns"]:
            print(f"  Skipping '{campaign_def['name']}' — already exists in state.")
            continue

        budget_resource = state["budgets"].get(campaign_def["name"])
        if not budget_resource:
            print(f"  ERROR: No budget found for '{campaign_def['name']}'. Run 03_create_budgets.py first.")
            continue

        campaigns_to_create.append(campaign_def)

    if not campaigns_to_create:
        print("All campaigns already exist. Nothing to create.")
        return

    # Create campaigns one at a time so we can apply geo targeting immediately
    for campaign_def in campaigns_to_create:
        budget_resource = state["budgets"][campaign_def["name"]]

        op = client.get_type("CampaignOperation")
        campaign = op.create

        campaign.name = campaign_def["name"]
        campaign.status = client.enums.CampaignStatusEnum.PAUSED
        campaign.advertising_channel_type = client.enums.AdvertisingChannelTypeEnum.SEARCH
        campaign.campaign_budget = budget_resource

        # Maximize Conversions bidding (removes $2 CPC cap for Ad Grants)
        # Setting the field triggers the oneof; no target_cpa needed
        campaign.maximize_conversions.cpc_bid_ceiling_micros = 0

        # Network settings — Ad Grants accounts cannot explicitly target search network
        campaign.network_settings.target_google_search = True
        campaign.network_settings.target_search_network = False
        campaign.network_settings.target_content_network = False
        campaign.network_settings.target_partner_search_network = False

        campaign.contains_eu_political_advertising = 3  # DOES_NOT_CONTAIN_EU_POLITICAL_ADVERTISING
        campaign.start_date = today

        print(f"Creating campaign: {campaign_def['name']}...")
        try:
            response = campaign_service.mutate_campaigns(
                customer_id=CUSTOMER_ID,
                operations=[op],
            )
            campaign_resource = response.results[0].resource_name
            state["campaigns"][campaign_def["name"]] = campaign_resource
            print(f"  Created: {campaign_resource}")

            # Apply geo targeting
            _add_geo_targets(client, criterion_service, campaign_resource, campaign_def["geo_targets"])

            save_state(state)
        except Exception as ex:
            save_state(state)
            print(f"  ERROR creating campaign '{campaign_def['name']}':")
            print_error(ex)

    print(f"\nDone. {len(state['campaigns'])} campaign(s) in state. All start PAUSED.")


def _add_geo_targets(client, criterion_service, campaign_resource, geo_target_ids):
    """Add location targeting criteria to a campaign."""
    operations = []
    for geo_id in geo_target_ids:
        op = client.get_type("CampaignCriterionOperation")
        criterion = op.create

        criterion.campaign = campaign_resource
        criterion.location.geo_target_constant = f"geoTargetConstants/{geo_id}"

        operations.append(op)

    try:
        criterion_service.mutate_campaign_criteria(
            customer_id=CUSTOMER_ID,
            operations=operations,
        )
        print(f"  Geo targets applied: {geo_target_ids}")
    except Exception as ex:
        print(f"  WARNING: Failed to apply geo targets {geo_target_ids}:")
        print_error(ex)


if __name__ == "__main__":
    main()
