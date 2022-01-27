#!/usr/bin/env bash

echo "Arguments for updating:"
echo " - AppSecret: $GOOGLE_API_KEY"

# Updating ids
IdFile=$BUILD_REPOSITORY_LOCALPATH/TrashMobMobile/TrashMobMobile.Android/properties/AndroidManifest.xml

sed -i '' "s/GOOGLE_API_KEY/$GOOGLE_API_KEY/g" "$IdFile"

# Print out file for reference
cat $IdFile

echo "Updated secret key!"

ConstantsFile=$BUILD_REPOSITORY_LOCALPATH/TrashMobMobile/TrashMobMobile.Shared/Constants.cs

if [ "$APPCENTER_BRANCH" != "release" ];
    then
        sed -i '' "s/API_URL/$DevURL/g" "$ConstantsFile"
else
    sed -i '' "s/API_URL/$ProdURL/g" "$ConstantsFile"  
fi 
