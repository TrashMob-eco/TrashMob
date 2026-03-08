#!/usr/bin/env bash
# Uploads redesigned SendGrid templates from the repo to SendGrid via API.
# Usage: SENDGRID_API_KEY=<key> bash scripts/sendgrid-upload-templates.sh [template-dir]

set -euo pipefail

API_KEY="${SENDGRID_API_KEY:?Set SENDGRID_API_KEY environment variable}"
TEMPLATE_DIR="${1:-Planning/SendGridTemplates/redesigned}"

TEMPLATES=(
  "EventEmail:d-11fedbf069ae4c098a3e837fe45d3fe1"
  "GenericEmail:d-a485d1a8e98d4038b2ab34b1daed6196"
  "PickupEmail:d-50e4ea3024c7459092e96b17c2895dc3"
  "LitterReportEmail:d-f1b8a32c4eef4e1592ece19f0a024c3e"
)

FAILED=0

for entry in "${TEMPLATES[@]}"; do
  NAME="${entry%%:*}"
  ID="${entry##*:}"
  HTML_FILE="$TEMPLATE_DIR/${NAME}.html"

  echo "--------------------------------------------"
  echo "Uploading template: $NAME ($ID)"

  if [ ! -f "$HTML_FILE" ]; then
    echo "  FAILED: HTML file not found: $HTML_FILE"
    FAILED=$((FAILED + 1))
    continue
  fi

  # GET the template to find the active version ID
  TEMPLATE_JSON=$(curl -s -X GET \
    "https://api.sendgrid.com/v3/templates/$ID" \
    -H "Authorization: Bearer $API_KEY" \
    -H "Content-Type: application/json")

  VERSION_ID=$(echo "$TEMPLATE_JSON" | python3 -c "
import sys, json
data = json.load(sys.stdin)
for v in data.get('versions', []):
    if v.get('active') == 1:
        print(v['id'])
        break
")

  if [ -z "$VERSION_ID" ]; then
    echo "  FAILED: No active version found for $NAME"
    FAILED=$((FAILED + 1))
    continue
  fi

  echo "  Active version ID: $VERSION_ID"

  # JSON-encode the HTML content using python3
  HTML_CONTENT_JSON=$(python3 -c "
import sys, json
with open(sys.argv[1], 'r') as f:
    html = f.read()
print(json.dumps({'html_content': html}))
" "$HTML_FILE")

  # PATCH the version with the new html_content
  HTTP_CODE=$(curl -s -o /tmp/sendgrid_response.json -w "%{http_code}" -X PATCH \
    "https://api.sendgrid.com/v3/templates/$ID/versions/$VERSION_ID" \
    -H "Authorization: Bearer $API_KEY" \
    -H "Content-Type: application/json" \
    -d "$HTML_CONTENT_JSON")

  if [ "$HTTP_CODE" -ge 200 ] && [ "$HTTP_CODE" -lt 300 ]; then
    echo "  SUCCESS: $NAME updated (HTTP $HTTP_CODE)"
  else
    echo "  FAILED: $NAME (HTTP $HTTP_CODE)"
    cat /tmp/sendgrid_response.json 2>/dev/null || true
    echo ""
    FAILED=$((FAILED + 1))
  fi
done

echo "--------------------------------------------"

if [ "$FAILED" -gt 0 ]; then
  echo "Finished with $FAILED failure(s)."
  exit 1
else
  echo "All templates uploaded successfully."
fi
