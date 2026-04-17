"""Step 5: Create ad groups (2+ per campaign)."""

from config import CAMPAIGNS, CUSTOMER_ID
from state import get_client, load_state, print_error, save_state


def main():
    client = get_client()
    state = load_state()
    service = client.get_service("AdGroupService")

    operations = []
    op_names = []

    for campaign_def in CAMPAIGNS:
        campaign_resource = state["campaigns"].get(campaign_def["name"])
        if not campaign_resource:
            print(f"  ERROR: Campaign '{campaign_def['name']}' not found in state. Run 04_create_campaigns.py first.")
            continue

        for ag_def in campaign_def["ad_groups"]:
            if ag_def["name"] in state["ad_groups"]:
                print(f"  Skipping '{ag_def['name']}' — already exists in state.")
                continue

            op = client.get_type("AdGroupOperation")
            ag = op.create

            ag.name = ag_def["name"]
            ag.campaign = campaign_resource
            ag.type_ = client.enums.AdGroupTypeEnum.SEARCH_STANDARD
            ag.status = client.enums.AdGroupStatusEnum.ENABLED
            # CPC bid required even with Maximize Conversions; campaign bidding overrides
            ag.cpc_bid_micros = 2_000_000  # $2.00

            operations.append(op)
            op_names.append(ag_def["name"])

    if not operations:
        print("All ad groups already exist. Nothing to create.")
        return

    print(f"Creating {len(operations)} ad group(s)...")
    try:
        response = service.mutate_ad_groups(
            customer_id=CUSTOMER_ID,
            operations=operations,
        )
        for i, result in enumerate(response.results):
            name = op_names[i]
            state["ad_groups"][name] = result.resource_name
            print(f"  Created: {name} -> {result.resource_name}")

        save_state(state)
        print(f"\nSUCCESS: {len(response.results)} ad group(s) created.")
    except Exception as ex:
        save_state(state)
        print("ERROR: Failed to create ad groups.")
        print_error(ex)


if __name__ == "__main__":
    main()
