"""Step 3: Create one campaign budget per campaign ($100/day each)."""

from config import CAMPAIGNS, CUSTOMER_ID, DAILY_BUDGET_MICROS
from state import get_client, load_state, print_error, save_state


def main():
    client = get_client()
    state = load_state()
    service = client.get_service("CampaignBudgetService")

    operations = []
    for campaign_def in CAMPAIGNS:
        budget_name = f"Budget - {campaign_def['name']}"

        if campaign_def["name"] in state["budgets"]:
            print(f"  Skipping '{budget_name}' — already exists in state.")
            continue

        op = client.get_type("CampaignBudgetOperation")
        budget = op.create

        budget.name = budget_name
        budget.amount_micros = DAILY_BUDGET_MICROS
        budget.delivery_method = client.enums.BudgetDeliveryMethodEnum.STANDARD
        budget.explicitly_shared = False

        operations.append((campaign_def["name"], op))

    if not operations:
        print("All budgets already exist. Nothing to create.")
        return

    print(f"Creating {len(operations)} campaign budget(s)...")
    try:
        response = service.mutate_campaign_budgets(
            customer_id=CUSTOMER_ID,
            operations=[op for _, op in operations],
        )
        for i, result in enumerate(response.results):
            name = operations[i][0]
            state["budgets"][name] = result.resource_name
            print(f"  Created: Budget - {name} -> {result.resource_name}")

        save_state(state)
        print(f"\nSUCCESS: {len(response.results)} budget(s) created.")
    except Exception as ex:
        save_state(state)
        print("ERROR: Failed to create budgets.")
        print_error(ex)


if __name__ == "__main__":
    main()
