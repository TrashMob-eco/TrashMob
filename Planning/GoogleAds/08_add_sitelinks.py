"""Step 8: Create sitelink assets and link them to campaigns."""

from config import CAMPAIGNS, CUSTOMER_ID, SITELINKS
from state import get_client, load_state, print_error, save_state


def main():
    client = get_client()
    state = load_state()
    asset_service = client.get_service("AssetService")
    campaign_asset_service = client.get_service("CampaignAssetService")

    # --- Step 1: Create sitelink assets (deduplicated) ---
    needed_sitelink_keys = set()
    for campaign_def in CAMPAIGNS:
        for key in campaign_def.get("sitelinks", []):
            if key not in state["sitelink_assets"]:
                needed_sitelink_keys.add(key)

    if needed_sitelink_keys:
        operations = []
        op_keys = []

        for key in needed_sitelink_keys:
            sl_def = SITELINKS[key]

            op = client.get_type("AssetOperation")
            asset = op.create

            asset.final_urls.append(sl_def["final_url"])
            asset.sitelink_asset.link_text = sl_def["link_text"]
            asset.sitelink_asset.description1 = sl_def["description1"]
            asset.sitelink_asset.description2 = sl_def["description2"]

            operations.append(op)
            op_keys.append(key)

        print(f"Creating {len(operations)} sitelink asset(s)...")
        try:
            response = asset_service.mutate_assets(
                customer_id=CUSTOMER_ID,
                operations=operations,
            )
            for i, result in enumerate(response.results):
                key = op_keys[i]
                state["sitelink_assets"][key] = result.resource_name
                print(f"  Created: {SITELINKS[key]['link_text']} -> {result.resource_name}")

            save_state(state)
        except Exception as ex:
            save_state(state)
            print("ERROR: Failed to create sitelink assets.")
            print_error(ex)
            return
    else:
        print("All sitelink assets already exist in state.")

    # --- Step 2: Link sitelink assets to campaigns ---
    for campaign_def in CAMPAIGNS:
        campaign_name = campaign_def["name"]
        campaign_resource = state["campaigns"].get(campaign_name)
        if not campaign_resource:
            print(f"  ERROR: Campaign '{campaign_name}' not found in state.")
            continue

        # Track which sitelinks are already linked
        linked_key = f"{campaign_name}"
        already_linked = state["campaign_sitelinks"].get(linked_key, [])

        operations = []
        link_keys = []

        for key in campaign_def.get("sitelinks", []):
            if key in already_linked:
                continue

            asset_resource = state["sitelink_assets"].get(key)
            if not asset_resource:
                print(f"  WARNING: Sitelink asset '{key}' not found in state. Skipping.")
                continue

            op = client.get_type("CampaignAssetOperation")
            ca = op.create

            ca.campaign = campaign_resource
            ca.asset = asset_resource
            ca.field_type = client.enums.AssetFieldTypeEnum.SITELINK

            operations.append(op)
            link_keys.append(key)

        if not operations:
            print(f"  All sitelinks already linked to '{campaign_name}'.")
            continue

        print(f"Linking {len(operations)} sitelink(s) to '{campaign_name}'...")
        try:
            campaign_asset_service.mutate_campaign_assets(
                customer_id=CUSTOMER_ID,
                operations=operations,
            )
            if linked_key not in state["campaign_sitelinks"]:
                state["campaign_sitelinks"][linked_key] = []
            state["campaign_sitelinks"][linked_key].extend(link_keys)
            print(f"  SUCCESS: {len(operations)} sitelink(s) linked.")
            save_state(state)
        except Exception as ex:
            save_state(state)
            print(f"  ERROR linking sitelinks to '{campaign_name}':")
            print_error(ex)

    print("\nDone. Review sitelinks in the Google Ads UI.")


if __name__ == "__main__":
    main()
