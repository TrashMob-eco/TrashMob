#!/usr/bin/env bash
set -euo pipefail

echo "Arguments for updating:"
echo " - AppSecret: $GOOGLE_API_KEY"

# Updating ids
IdFile="$BUILD_REPOSITORY_LOCALPATH/TrashMobMobile/TrashMobMobile.Android/properties/AndroidManifest.xml"

sed -i '' -e "s|{GOOGLE_API_KEY}|$GOOGLE_API_KEY|g" "$IdFile"

# Print out file for reference
cat $IdFile

echo "Updated secret key!"

ConstantsFile="$BUILD_REPOSITORY_LOCALPATH/TrashMobMobile/TrashMobMobile/Constants.cs"

if [ "$APPCENTER_BRANCH" == "release" ]; then
    sed -i '' -e "s|{API_ENDPOINT}|$DEV_ENDPOINT|g" "$ConstantsFile"
else
    sed -i '' -e "s|{API_ENDPOINT}|$PROD_ENDPOINT|g" "$ConstantsFile"
fi 

echo "Update Constants at: ${ConstantsFile}"
cat $ConstantsFile