"""Step 2: Create conversion actions for tracking volunteer signups, RSVPs, etc."""

from google.ads.googleads.client import GoogleAdsClient

from config import CONVERSION_ACTIONS, CUSTOMER_ID
from state import get_client, load_state, print_error, save_state

# Map friendly names to enum values
CATEGORY_MAP = {
    "SIGNUP": "SIGN_UP",
    "SUBMIT_LEAD_FORM": "SUBMIT_LEAD_FORM",
    "OTHER": "DEFAULT",
}

COUNTING_MAP = {
    "ONE_PER_CLICK": "ONE_PER_CLICK",
    "MANY_PER_CLICK": "MANY_PER_CLICK",
}


def main():
    client = get_client()
    state = load_state()
    service = client.get_service("ConversionActionService")

    operations = []
    for action_def in CONVERSION_ACTIONS:
        # Skip if already created
        if action_def["name"] in state["conversion_actions"]:
            print(f"  Skipping '{action_def['name']}' — already exists in state.")
            continue

        op = client.get_type("ConversionActionOperation")
        ca = op.create

        ca.name = action_def["name"]
        ca.type_ = client.enums.ConversionActionTypeEnum.WEBPAGE
        ca.category = getattr(
            client.enums.ConversionActionCategoryEnum,
            CATEGORY_MAP[action_def["category"]],
        )
        ca.status = client.enums.ConversionActionStatusEnum.ENABLED
        ca.counting_type = getattr(
            client.enums.ConversionActionCountingTypeEnum,
            COUNTING_MAP[action_def["counting_type"]],
        )
        ca.value_settings.default_value = 1.0
        ca.value_settings.always_use_default_value = True

        operations.append((action_def["name"], op))

    if not operations:
        print("All conversion actions already exist. Nothing to create.")
        return

    print(f"Creating {len(operations)} conversion action(s)...")
    try:
        response = service.mutate_conversion_actions(
            customer_id=CUSTOMER_ID,
            operations=[op for _, op in operations],
        )
        for i, result in enumerate(response.results):
            name = operations[i][0]
            state["conversion_actions"][name] = result.resource_name
            print(f"  Created: {name} -> {result.resource_name}")

        save_state(state)
        print(f"\nSUCCESS: {len(response.results)} conversion action(s) created.")
        print("NOTE: Actions will show UNVERIFIED until the Google tag fires a real event.")
    except Exception as ex:
        # Save partial state if some succeeded
        save_state(state)
        print("ERROR: Failed to create conversion actions.")
        print_error(ex)


if __name__ == "__main__":
    main()
