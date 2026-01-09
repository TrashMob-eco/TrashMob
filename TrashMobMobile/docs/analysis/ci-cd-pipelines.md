# CI/CD Pipelines (Mobile Only)

Mobile workflows live in the parent repo under `TrashMob/.github/workflows`. This doc only describes the workflows that build and release the `TrashMobMobile` app.

## Mobile app pipelines

- `PullRequest-Mobile.yml`
  - Trigger: PRs to `main` affecting mobile or shared model paths.
  - Runs build-number generation, then calls reusable Android and iOS build workflows.
  - Uses `USETEST` constants and dev bundle IDs.

- `main_trashmobmobileapp.yml`
  - Trigger: push to `main` with mobile changes (or workflow dispatch).
  - Builds Android and iOS and then calls publish workflows to deploy to Google Play internal track and TestFlight.
  - Uses dev bundle IDs and `USETEST`.

- `release_trashmobmobileapp.yml`
  - Trigger: push to `release` with mobile changes (or workflow dispatch).
  - Builds Android and iOS with production bundle IDs and `USEPROD` constants.
  - Calls publish workflows for production deployment.

- `build-android.yml` (reusable)
  - Updates `TrashMobMobile.csproj` version, bundle ID, and constants.
  - Injects Android Google Maps API key into `AndroidManifest.xml`.
  - Builds Android AAB and signs it using a keystore secret.
  - Uploads artifacts named `artifacts-android-<semVer>`.

- `build-ios.yml` (reusable)
  - Updates `TrashMobMobile.csproj` version, bundle ID, and constants.
  - Installs MAUI workloads, configures Xcode, imports signing certs and provisioning profiles.
  - Builds iOS IPA and uploads artifacts named `artifacts-ios-<build_number>`.

- `publish-android.yml` (reusable)
  - Downloads signed AAB artifact and uploads to Google Play internal track.

- `publish-ios.yml` (reusable)
  - Downloads IPA artifact and uploads to TestFlight.

## Secrets used (high level)

- Mobile signing: `ANDROID_KEYSTORE`, `ANDROID_KEYSTORE_PASSWORD`, `IOS_CERTIFICATES_P12`, `IOS_CERTIFICATES_P12_PASSWORD`.
- App Store / Play: `APPSTORE_*`, `GCP_SERVICE_ACCOUNT`.
- App config: `MOBILE_APPSETTINGS` injected in mobile build workflows.
