"""Step 1: Verify credentials and confirm the correct Google Ads account is targeted."""

from config import CUSTOMER_ID
from state import get_client, print_error


def main():
    client = get_client()

    # 1. List accessible customers to verify OAuth credentials work
    customer_service = client.get_service("CustomerService")
    try:
        accessible = customer_service.list_accessible_customers()
        print("Accessible customer accounts:")
        for resource_name in accessible.resource_names:
            print(f"  {resource_name}")
    except Exception as ex:
        print("ERROR: Failed to list accessible customers. Check your credentials.")
        print_error(ex)
        return

    # 2. Query account details for the target customer
    ga_service = client.get_service("GoogleAdsService")
    query = """
        SELECT
            customer.id,
            customer.descriptive_name,
            customer.currency_code,
            customer.time_zone
        FROM customer
        LIMIT 1
    """

    try:
        response = ga_service.search_stream(customer_id=CUSTOMER_ID, query=query)
        for batch in response:
            for row in batch.results:
                cust = row.customer
                print(f"\nAccount verified:")
                print(f"  ID:       {cust.id}")
                print(f"  Name:     {cust.descriptive_name}")
                print(f"  Currency: {cust.currency_code}")
                print(f"  Timezone: {cust.time_zone}")

                if str(cust.id) != CUSTOMER_ID:
                    print(f"\nWARNING: Returned ID {cust.id} does not match expected {CUSTOMER_ID}!")
                else:
                    print(f"\nSUCCESS: Account {CUSTOMER_ID} confirmed. Ready to proceed.")
    except Exception as ex:
        print(f"ERROR: Failed to query account {CUSTOMER_ID}.")
        print_error(ex)


if __name__ == "__main__":
    main()
