"""Step 6: Add keywords to ad groups and negative keywords to campaigns."""

from config import CAMPAIGNS, CUSTOMER_ID, NEGATIVE_KEYWORDS
from state import get_client, load_state, print_error, save_state

MATCH_TYPE_MAP = {
    "BROAD": "BROAD",
    "PHRASE": "PHRASE",
    "EXACT": "EXACT",
}


def main():
    client = get_client()
    state = load_state()
    ag_criterion_service = client.get_service("AdGroupCriterionService")
    campaign_criterion_service = client.get_service("CampaignCriterionService")

    # --- Add keywords to ad groups ---
    operations = []
    op_labels = []

    for campaign_def in CAMPAIGNS:
        for ag_def in campaign_def["ad_groups"]:
            ag_resource = state["ad_groups"].get(ag_def["name"])
            if not ag_resource:
                print(f"  ERROR: Ad group '{ag_def['name']}' not found in state. Run 05_create_ad_groups.py first.")
                continue

            for keyword_text, match_type in ag_def["keywords"]:
                op = client.get_type("AdGroupCriterionOperation")
                criterion = op.create

                criterion.ad_group = ag_resource
                criterion.status = client.enums.AdGroupCriterionStatusEnum.ENABLED
                criterion.keyword.text = keyword_text
                criterion.keyword.match_type = getattr(
                    client.enums.KeywordMatchTypeEnum,
                    MATCH_TYPE_MAP[match_type],
                )

                operations.append(op)
                op_labels.append(f"{ag_def['name']}: {keyword_text}")

    if operations:
        print(f"Adding {len(operations)} keyword(s) across ad groups...")
        try:
            response = ag_criterion_service.mutate_ad_group_criteria(
                customer_id=CUSTOMER_ID,
                operations=operations,
            )
            print(f"  SUCCESS: {len(response.results)} keyword(s) added.")
        except Exception as ex:
            print("ERROR: Failed to add keywords.")
            print_error(ex)
    else:
        print("No keywords to add.")

    # --- Add negative keywords to each campaign ---
    for campaign_def in CAMPAIGNS:
        campaign_resource = state["campaigns"].get(campaign_def["name"])
        if not campaign_resource:
            continue

        neg_ops = []
        for neg_kw in NEGATIVE_KEYWORDS:
            op = client.get_type("CampaignCriterionOperation")
            criterion = op.create

            criterion.campaign = campaign_resource
            criterion.negative = True
            criterion.keyword.text = neg_kw
            criterion.keyword.match_type = client.enums.KeywordMatchTypeEnum.BROAD

            neg_ops.append(op)

        if neg_ops:
            print(f"Adding {len(neg_ops)} negative keyword(s) to '{campaign_def['name']}'...")
            try:
                campaign_criterion_service.mutate_campaign_criteria(
                    customer_id=CUSTOMER_ID,
                    operations=neg_ops,
                )
                print(f"  SUCCESS: {len(neg_ops)} negative keyword(s) added.")
            except Exception as ex:
                print(f"  ERROR adding negative keywords to '{campaign_def['name']}':")
                print_error(ex)

    print("\nDone adding keywords.")


if __name__ == "__main__":
    main()
