#!/usr/bin/env bash

echo "Arguments for updating:"
echo " - AppSecret: $GOOGLE_API_KEY"

# Updating ids
IdFile=$BUILD_REPOSITORY_LOCALPATH/TrashMobMobile/TrashMobMobile.Android/properties/AndroidManifest.xml

sed -i '' "s/GOOGLE_API_KEY/$GOOGLE_API_KEY/g" "$IdFile"

# Print out file for reference
cat $IdFile

echo "Updated secret key!"