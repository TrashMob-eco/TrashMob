#!/usr/bin/env bash
# Downloads all 4 SendGrid dynamic templates and saves them to Planning/SendGridTemplates/originals/
# Usage: SENDGRID_API_KEY=<key> bash scripts/sendgrid-download-templates.sh

set -euo pipefail

API_KEY="${SENDGRID_API_KEY:?Set SENDGRID_API_KEY environment variable}"
OUTPUT_DIR="Planning/SendGridTemplates/originals"
mkdir -p "$OUTPUT_DIR"

TEMPLATES=(
  "EventEmail:d-11fedbf069ae4c098a3e837fe45d3fe1"
  "GenericEmail:d-a485d1a8e98d4038b2ab34b1daed6196"
  "PickupEmail:d-50e4ea3024c7459092e96b17c2895dc3"
  "LitterReportEmail:d-f1b8a32c4eef4e1592ece19f0a024c3e"
)

for entry in "${TEMPLATES[@]}"; do
  NAME="${entry%%:*}"
  ID="${entry##*:}"

  echo "Downloading template: $NAME ($ID)..."

  # Get template metadata (includes version IDs)
  TEMPLATE_JSON=$(curl -s -X GET \
    "https://api.sendgrid.com/v3/templates/$ID" \
    -H "Authorization: Bearer $API_KEY" \
    -H "Content-Type: application/json")

  echo "$TEMPLATE_JSON" > "$OUTPUT_DIR/${NAME}_metadata.json"

  # Extract the active version ID
  VERSION_ID=$(echo "$TEMPLATE_JSON" | python3 -c "
import sys, json
data = json.load(sys.stdin)
for v in data.get('versions', []):
    if v.get('active') == 1:
        print(v['id'])
        break
")

  if [ -n "$VERSION_ID" ]; then
    # Get the full version (includes html_content)
    VERSION_JSON=$(curl -s -X GET \
      "https://api.sendgrid.com/v3/templates/$ID/versions/$VERSION_ID" \
      -H "Authorization: Bearer $API_KEY" \
      -H "Content-Type: application/json")

    echo "$VERSION_JSON" > "$OUTPUT_DIR/${NAME}_version.json"

    # Extract HTML content to a standalone file
    echo "$VERSION_JSON" | python3 -c "
import sys, json
data = json.load(sys.stdin)
print(data.get('html_content', ''))
" > "$OUTPUT_DIR/${NAME}.html"

    echo "  Saved: ${NAME}.html, ${NAME}_metadata.json, ${NAME}_version.json"
  else
    echo "  WARNING: No active version found for $NAME"
  fi
done

echo "Done. Templates saved to $OUTPUT_DIR/"
