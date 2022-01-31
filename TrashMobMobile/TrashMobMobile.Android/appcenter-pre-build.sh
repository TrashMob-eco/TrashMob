#!/usr/bin/env bash

echo "Arguments for updating:"
echo " - AppSecret: $GOOGLE_API_KEY"

# Updating ids
IdFile=$BUILD_REPOSITORY_LOCALPATH/TrashMobMobile/TrashMobMobile.Android/properties/AndroidManifest.xml

sed -i '' "s/GOOGLE_API_KEY/$GOOGLE_API_KEY/g" "$IdFile"

# Print out file for reference
cat $IdFile

echo "Updated secret key!"

ConstantsFile=$BUILD_REPOSITORY_LOCALPATH/TrashMobMobile/TrashMobMobile/Constants.cs

if [ "$APPCENTER_BRANCH" == "phcherne/multiple_api_url" ];
    then
        # sed -i '' "s/API_ENDPOINT/$DEV_ENDPOINT/g" ~"$ConstantsFile"
        awk -v repl="$PROD_ENDPOINT" '{sub(/API_ENDPOINT/, repl); print}' "$ConstantsFile"
else
    # sed -i '' "s/API_ENDPOINT/$PROD_ENDPOINT/g" ~"$ConstantsFile"  
    awk -v repl="$DEV_ENDPOINT" '{sub(/API_ENDPOINT/, repl); print}' "$ConstantsFile"

fi 

cat $ConstantsFile