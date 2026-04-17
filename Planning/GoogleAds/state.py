"""Shared utilities for Google Ads API client initialization and state persistence."""

import json
import os
import sys

from google.ads.googleads.client import GoogleAdsClient

from config import CUSTOMER_ID, STATE_FILE

_SCRIPT_DIR = os.path.dirname(os.path.abspath(__file__))
_STATE_PATH = os.path.join(_SCRIPT_DIR, STATE_FILE)
_YAML_PATH = os.path.join(_SCRIPT_DIR, "google-ads.yaml")

_VAULT_NAME = "kv-tm-pr-westus2"
_SECRET_NAMES = {
    "developer_token": "GoogleAds--DeveloperToken",
    "client_id": "GoogleAds--ClientId",
    "client_secret": "GoogleAds--ClientSecret",
    "refresh_token": "GoogleAds--RefreshToken",
}
_LOGIN_CUSTOMER_ID = "6593041445"


def _load_from_keyvault() -> GoogleAdsClient | None:
    """Try to load credentials from Azure Key Vault. Returns None if unavailable."""
    try:
        from azure.identity import DefaultAzureCredential
        from azure.keyvault.secrets import SecretClient

        credential = DefaultAzureCredential()
        client = SecretClient(vault_url=f"https://{_VAULT_NAME}.vault.azure.net", credential=credential)

        config = {"use_proto_plus": True, "login_customer_id": _LOGIN_CUSTOMER_ID}
        for key, secret_name in _SECRET_NAMES.items():
            try:
                config[key] = client.get_secret(secret_name).value
            except Exception:
                if key == "refresh_token":
                    # Not stored yet — will be added after OAuth flow
                    return None
                raise

        print("Loaded credentials from Azure Key Vault.")
        return GoogleAdsClient.load_from_dict(config, version="v22")
    except ImportError:
        return None
    except Exception as ex:
        print(f"Key Vault unavailable ({ex}), falling back to local yaml.")
        return None


def get_client() -> GoogleAdsClient:
    """Load credentials from Key Vault or google-ads.yaml and return an initialized client."""
    kv_client = _load_from_keyvault()
    if kv_client:
        return kv_client

    if not os.path.exists(_YAML_PATH):
        print(f"ERROR: {_YAML_PATH} not found and Key Vault unavailable.")
        print("Copy google-ads.yaml.template to google-ads.yaml and fill in your credentials,")
        print("or install azure-identity and azure-keyvault-secrets packages.")
        sys.exit(1)
    return GoogleAdsClient.load_from_storage(_YAML_PATH, version="v22")


def load_state() -> dict:
    """Read the persisted state file. Returns empty sections if not found."""
    default = {
        "conversion_actions": {},
        "budgets": {},
        "campaigns": {},
        "ad_groups": {},
        "sitelink_assets": {},
        "campaign_sitelinks": {},
    }
    if not os.path.exists(_STATE_PATH):
        return default
    with open(_STATE_PATH, "r") as f:
        data = json.load(f)
    # Merge with defaults so new keys are always present
    for key in default:
        if key not in data:
            data[key] = default[key]
    return data


def save_state(state: dict) -> None:
    """Write state to the JSON file."""
    with open(_STATE_PATH, "w") as f:
        json.dump(state, f, indent=2)
    print(f"State saved to {_STATE_PATH}")


def print_error(ex: Exception) -> None:
    """Print Google Ads API error details."""
    # google-ads library raises GoogleAdsException with structured errors
    if hasattr(ex, "failure"):
        for error in ex.failure.errors:
            print(f"  Error: {error.message}")
            print(f"  Error code: {error.error_code}")
    else:
        print(f"  {ex}")
