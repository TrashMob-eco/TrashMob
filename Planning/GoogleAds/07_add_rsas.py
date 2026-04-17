"""Step 7: Add responsive search ads (RSAs) to each ad group."""

from config import CAMPAIGNS, CUSTOMER_ID
from state import get_client, load_state, print_error, save_state


def main():
    client = get_client()
    state = load_state()
    service = client.get_service("AdGroupAdService")

    operations = []
    op_labels = []

    for campaign_def in CAMPAIGNS:
        for ag_def in campaign_def["ad_groups"]:
            ag_resource = state["ad_groups"].get(ag_def["name"])
            if not ag_resource:
                print(f"  ERROR: Ad group '{ag_def['name']}' not found in state. Run 05_create_ad_groups.py first.")
                continue

            rsa_def = ag_def.get("rsa")
            if not rsa_def:
                print(f"  WARNING: No RSA definition for '{ag_def['name']}'. Skipping.")
                continue

            op = client.get_type("AdGroupAdOperation")
            ad_group_ad = op.create

            ad_group_ad.ad_group = ag_resource
            ad_group_ad.status = client.enums.AdGroupAdStatusEnum.ENABLED

            ad = ad_group_ad.ad
            ad.final_urls.append(rsa_def["final_url"])

            if rsa_def.get("path1"):
                ad.responsive_search_ad.path1 = rsa_def["path1"]
            if rsa_def.get("path2"):
                ad.responsive_search_ad.path2 = rsa_def["path2"]

            # Add headlines (max 15, min 3, each max 30 chars)
            for i, headline_text in enumerate(rsa_def["headlines"][:15]):
                headline = client.get_type("AdTextAsset")
                headline.text = headline_text[:30]  # Enforce character limit
                ad.responsive_search_ad.headlines.append(headline)

                # Pin first headline on brand campaigns for brand consistency
                if campaign_def["name"] == "TrashMob Brand Campaign" and i == 0:
                    ad.responsive_search_ad.headlines[-1].pinned_field = (
                        client.enums.ServedAssetFieldTypeEnum.HEADLINE_1
                    )

            # Add descriptions (max 4, min 2, each max 90 chars)
            for desc_text in rsa_def["descriptions"][:4]:
                desc = client.get_type("AdTextAsset")
                desc.text = desc_text[:90]  # Enforce character limit
                ad.responsive_search_ad.descriptions.append(desc)

            operations.append(op)
            op_labels.append(ag_def["name"])

    if not operations:
        print("No RSAs to create.")
        return

    print(f"Creating {len(operations)} responsive search ad(s)...")
    try:
        response = service.mutate_ad_group_ads(
            customer_id=CUSTOMER_ID,
            operations=operations,
        )
        for i, result in enumerate(response.results):
            print(f"  Created RSA for '{op_labels[i]}': {result.resource_name}")

        print(f"\nSUCCESS: {len(response.results)} RSA(s) created.")
    except Exception as ex:
        print("ERROR: Failed to create RSAs.")
        print_error(ex)


if __name__ == "__main__":
    main()
