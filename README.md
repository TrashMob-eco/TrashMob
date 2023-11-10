# TrashMob.eco

**Meet up. Clean up. Feel good.**

# What is TrashMob?
TrashMob is a website dedicated to organizing groups of people to clean up the world we live in. Users create cleanup events, publicize them, and recruit people to join up, as well as ask for assistance from communities and partners to help haul away the garbage once it is gathered. The idea is to turn what can be an intimidating process for event organizers into a few mouse clicks and simple forms. And once the process is simple, events will spring up all over the world, and the cleanup of the world can begin.

# Where did this idea come from?
Years ago, Scott Hanselman (and others at Microsoft) built out the NerdDinner.com site as a demo of the capabilities of ASP.NET MVC. I actually went to a bunch of the nerd dinners. They were fantastic and had a huge roll in my career, including eventually leading me to join Microsoft. This site is based on both that code and the idea that getting people together to do small good things results in larger good things in the long term.

My passion is fixing problems we have on the planet with pollution and climate change. I've been thinking about what technology can do to help in these areas, without creating more problems. And I keep coming back to the thought that a lot of this is a human problem. People want to help and they want to fix things, but they don't know where to start. Other people have ideas on where to start, but not enough help to get started.
 
I read about a guy in California named [Edgar McGregor](https://twitter.com/edgarrmcgregor), who has spent over 1100 days cleaning up a park in his community, two pails of litter at a time, and I thought, that was a great idea. His actions inspired me to get out and clean up a local park one Saturday. It was fun and rewarding and other people saw what I was doing on my own and I know I have already inspired others to do the same. And then I passed by an area of town that is completely covered in trash and I thought "this is too much for me alone. It would be great to have a group of people descend on this area like a mob and clean it up in an hour or two". And my idea for TrashMob.eco was born.
 
Basically, TrashMob is the NerdDinner.com site re-purposed to allow people to start mobs of their own to tackle cleanup or whatever needs doing. And I keep coming up with more and more ideas for it. I'm hoping this site grows organically because of the good that we can do we get together.

## What is the website address?

To see what is currently deployed to the prod environment, go to:
https://www.trashmob.eco

To see what is currently deployed to the dev environment, go to:
https://as-tm-dev-westus2.azurewebsites.net/

# FAQ 
## What is the current state of this project?

As of 5/15/2022, we are now in full production launch. The site is up and running and people are using it ot help organize litter cleanups! TrashMob.eco is now a 501(c)(3) non-profit in the United States. We are working on new features all the time!

## Are you looking for contributors?

ABSOLUTELY! Ping [info@trashmob.eco](mailto:info@trashmob.eco) if you want to get involved. All kinds of skills needed, from reactjs to website design, to aspnet core, to .NET MAUI, to PowerBI, to deployment / github skills. If you have a couple of hours a week, and want to contribute, let us know!
 
## I have an idea for a TrashMob feature!

Fantastic! We want to build this out to be best site on the internet! But before you send us your idea, please take a look at the lists of [projects](https://github.com/orgs/TrashMob-eco/projects) and [issues](https://github.com/TrashMob-eco/TrashMob/issues) we already have going. We may already be working on your idea. If your idea is not there, feel free to reach out to us at [info@trashmob.eco](mailto:info@trashmob.eco)

# Development Notes

## Getting Started - Development

1. You must install the .NET 6 SDK
1. Install Visual Studio Code
1. Connect to github and clone the repo
1. Send your github id to info@trashmob.eco to be added as a contributor to the repository
1. Install the [Azure CLI](https://docs.microsoft.com/en-us/cli/azure/install-azure-cli-windows?tabs=azure-cli)

### To use the Shared Dev Environment
If you are not doing any database changes (i.e. design work, error handling, etc) you can save yourself time and money by doing the following and using the shared Dev environment:
1. Send the email address you use on GitHub to [info@trashmob.eco](mailto:info@trashmob.eco)
1. TrashMob will add you as a contributor to the Sandbox subscription
1. TrashMob will add you to the Dev KeyVault with Secret Get and List permissions
1. Log in to the Sandbox subscription, and go to the [Dev Azure SQL Database](https://portal.azure.com/#@jobeedevids.onmicrosoft.com/resource/subscriptions/39a254b7-c01a-45ab-bebd-4038ea4adea9/resourceGroups/rg-trashmob-dev-westus2/providers/Microsoft.Sql/servers/sql-tm-dev-westus2/overview)
1. Click on Firewalls and Virtual Networks
1. Add a new Rule with your email address as the name, with the start and end ip address set as your Client IP Address (see the line above the form for what Azure thinks your IP address is)
1. **Save** changes
1. Run the following script on your machine from the TrashMob folder in the project to set up your dev machine to run the project locally. You must be logged into Azure in your PowerShell window in the correct subscription
```
.\setupdev.ps1 -environment dev -region westus2 -subscription 39a254b7-c01a-45ab-bebd-4038ea4adea9
```

### To set up your own environment to test in:
You must use this if you are making database changes to ensure you do not break the backend for everyone else:

1. Follow the Infrastructure Deployment Steps (here)[.\Deploy\readme.md].
1. Run the following script on your machine from the TrashMob folder in the project to set up your dev machine to run the project locally. You must be logged into Azure in your PowerShell window in the correct subscription
```
.\setupdev.ps1 -environment <yourenv> -region <yourregion> -subscription <yourazuresubscription>

i.e.
.\setupdev.ps1 -environment jb -region westus2 -subscription <insert guid here>

```

## Setting up your launchsettings.json for website development

Because of RedirectUrls, life is a lot easier if you stick with the same ports as everyone else. 

cd to the TrashMob/Properties folder:
Add the following launchsettings.json file (may need to create it if you don't have it already): 

```
{
  "iisSettings": {
    "windowsAuthentication": false,
    "anonymousAuthentication": true,
    "iisExpress": {
      "applicationUrl": "http://localhost:44332/",
      "sslPort": 44332
    }
  },
  "profiles": {
    "IIS Express": {
      "commandName": "IISExpress",
      "launchBrowser": true,
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      }
    },
    "TrashMob": {
      "commandName": "Project",
      "launchBrowser": true,
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      },
      "applicationUrl": "https://localhost:44332;http://localhost:5000"
    }
  }
}

```

## Setting up your environment for Docusign Integration Testing

Docusign is integrated with the TrashMob.eco to ensure that a user has signed the appropriate liability waivers before attending a
TrashMob.eco event. If the user has not previously signed a waiver, or the waiver they signed is out of date, they will be asked to 
sign a new waiver either when they try to create a new event, or when they sign up for an existing event.

For developers, there are a number of secrets that need to be set up in your environment before you can test with an identity which has not signed the
waiver for testing. 

If you are a new developer and have no need to test the Docusign flow, simply sign into the Dev site for TrashMob.eco, and create or register for an event. 
When you try to do this, the system will ask you to sign the waivers. Follow the instructions, and then you can begin testing locally without needing the 
integration to work. This is, by far, the easiest way to get start on non-docusign features, and is highly recommended.

If you are, however, attempting to test or alter the Docusign workflow, here are the steps you need to take:

1. Create a Developer Account at https://appdemo.docusign.com/home
2. Click on **Settings** in the top action bar
3. Click on Integrations / Apps and Keys in the Left Action Bar
4. This should bring you to a page with "My Account Information" at the top.
5. Click Add App and Integration Key
6. Set the App Name
7. Set the RedirectUrl to https://localhost:44332/waivers
8. Open Visual Studio Code and a terminal window within
9. cd trashmob
10. Set the following user secrets using dotnet user-secrets set with the following names / values

| Secret Name | Where to get the value |
| --- | --- |
| DocusignAccountId | API Account Id under My Account Information |
| DocusignImpersonatedUserId | Use Id under My Account Information |
| DocusignClientId | Integration Key from the Apps and Integration Keys section |
| DocusignAuthServer | account-d.docusign.com |
| DocusignPrivateKeyEncoded | Click the Actions pull down under apps and integration keys, and click edit. Then under service integration, click generate RSA. Copy the value for PrivateKey. Then you need to base64 encode this value before setting that as a secret |
| DocusignBasePath | https://demo.docusign.net/restapi |
| DocusignRedirectHome | https://localhost:44332/waivers | 

11. Set a breakpoint in the DocusignManager.SendEnvelope method
12. Start the code in the debugger
13. Log in to TrashMob via the debugger browser
14. Click "Create an Event"
15. When the breakpoint is reached, step until you get into the catch handler after AuthenticateWithJWT call
16. Step into the if statement and get the value for the url that is created.
17. Open a browser, and paste that Url into the browser.
18. Accept the consent as instructed in the window.
19. Close the browser
20. Restart your debugger session
21. You should now be able to go through the regular Docusign Waiver form flow.

## Getting Started - Mobile Development

The mobile app is written using .NET MAUI. It requires a few prerequisites in order to get it compiling and running.

1. Ensure you have installed the latest version of Visual Studio and the .NET MAUI Framework option
2. Install Android Studio https://developer.android.com/studio 
3. Create an Android Emulator device in Android Studio
4. Load the TrashMobMobileApp.sln Project in Visual Studio.
5. Set your startup project to TrashMobMobileApp
5. In order to have the maps feature work, you will need to do the following:
    1. Create a Google Maps account: https://developers.google.com/maps/gmp-get-started
    2. Get your Google API Key from your Google Project
    3. Create a gradle.properties file in your GRADLE_USER_HOME (i.e. c:\users\\<username>\.gradle)
    4. Add the following line to your gradle properties file: 
    ```
    GOOGLE_API_KEY = "<Your api key>"
    ```
    5. Restart your emulator. Maps should work now

    <b>Never check in any file that contains your real api key.</b>

## To Build the Web App:

In the Trashmob project folder, run the following command:
```
dotnet build
```

## To Run the Web App:

In the Trashmob project folder, run the following command:
```
dotnet run
```

or if using Visual Studio, set the TrashMob project as the start up project and hit F5.

If a browser does not open, open one for yourself and go to https://localhost:44332

If the app loads, but data does not, it is likely that the firewall rule is not set up correctly. Sometimes the IP address the Web Portal displays is different from the IP address of your machine. If you run into this issue, look in the debug window of VSCode. It will report a failure, and show that your actual IP Address is not enabled to access the database.
1. Copy the IP Address from the error in VS Code
1. Log in to the Sandbox subscription, and go to the [Dev Azure SQL Database](https://portal.azure.com/#@jobeedevids.onmicrosoft.com/resource/subscriptions/39a254b7-c01a-45ab-bebd-4038ea4adea9/resourceGroups/rg-trashmob-dev-westus2/providers/Microsoft.Sql/servers/sql-tm-dev-westus2/overview)
1. Click on Firewalls and Virtual Networks
1. Add a new Rule with your email address as the name, with the start and end ip address set as your Client IP Address (see the line above the form for what Azure thinks your IP address is)
1. **Save** changes

## Testing the Web App

As the site's feature set has grown, so have the scenarios that need to be tested after large changes have been made. Please see the [Test Scenarios](./TestScenarios.md) document for a list of checks that should be run. At some point we will need to automate these tests.

## To Update the Database Model
The project uses Entity Framework Core V6 Model-First database updates.

1. Update the models / MobDbContext as needed in the repo.
2. To create the migration, do either of the following steps

In VS Code
```
dotnet ef migrations add <YourMigrationName>

```

or in Visual Studio Package Manager Console

First, set the Default Project to **TrashMob.Shared**, then run the following command:

```
  EntityFrameworkCore\Add-Migration <YourMigrationName>
```

3. In VS Code in the TrashMob Folder, run the following command

```
dotnet ef database update
```

## Allowing the App To Send Email

This is a pay-per-use feature, so, for the most part, we're going to try to limit the number of people developing with this. To <b>not</b> send email, make sure to set the following user secret on your dev box 
```
  dotnet user-secrets set "sendGridApiKey" "x"
```

To test sending email, copy the "sendGridApiKey" from the dev keyvault to your machine and repeat the above, substituting in the real key. 

## A note on Azure Maps usage
The call to find the distance between two points in Azure Maps is only available in S1 (Gen 1) or Gen2 Maps. This is significantly more expensive than the S0 maps, so for now, we default to S0 for all dev deployments, and have manually set Prod to Gen2. It is not recommended to rerun the infrastructure deployments to Prod, as this will overwrite this setting.

In the future, we may want to optimize the use of this function to reduce costs.

## How do I deploy the Azure Web App from GitHub?
The Dev site is automatically deployed with each push to the Main branch via a GitHub action. This is the preferred method of updating the Development Server. If you need to push an update from your development machine instead, please let the team know that there are changes in the environment pending checkin.

The Production site is manually deployed via a GitHub action from the release branch. This is the ONLY way production should be updated.

## How do I deploy the Azure Web App from my PC?
Use Visual Studio Publish to publish the site to the dev server.

If setting up a new environment, you will need to add the IP Address of the App Service to the list of IP Addresses accessible in the SQL Server. This needs to be automated in the future to make sure that a change to an IP address doesn't break the site.

## The site is asking me to login
If you try to access a secure page, you will need a user id on the site. When you hit a secured page, the site will redirect you to a sign in page. Click the Sign up now link at the bottom of the login box. Multiple identity providers are now available, including Facebook, Twitter, Google, and Microsoft, along with the TrashMob tenant itself if you prefer not to use an integrated signup.

## How to Change Mobile App from Test to Prod
It is currently hard-coded in the Mobile app that if you run a Debug build, you will point to the test environment, and if you run the release build, you will point to the production environment. 

## My Android Pull Request is Failing with Unable to Open jar File
In Debug mode, by default, the Android package format is set to apk, and does not build the bundle needed for signing. In order to change that
1. Open the TrashMobMobileApp.sln in Visual Studio
2. Right click on the TrashMobMobileApp project
3. Select "Properties"
4. Go to the Android Settings / Options
5. Change the Android Package Format for Debug & net7.0-android to "bundle" instead of "apk".
6. Save you changes and push to your branch.

Note: this may make deployments to your local emulator slower (more data must be copied into the emulator session). You can change this back to "apk" for local development, but failure to switch it back to "bundle" before checkin will cause the PR build to fail. There may be a way to pass this setting in on the command line for the publish step. That has not yet been investigated.

## How do I get a test distribution of the Mobile App?
We currently use Microsoft App Center for building and distributing the Mobile apps. In order to get notified of a new distribution (or to download it). 
1. Send a note to info@trashmob.eco requesting access to App Center. Please include the email address you wish to use to access App Center in the request and why you want to use a dev build.
2. A TrashMob.eco admin will review your request and if approved, will add you to the testers group.
3. You will receive an email notification when your request has been approved.
4. Click on the link to complete your onboarding.
5. You can either download the latest package at that point, or wait for the next build to be completed. You will be notified when a new version is available.

## How do I get a production distribution of the Mobile App?
The production mobile app can be downloaded here:

[Android](https://play.google.com/store/apps/details?id=eco.trashmob.trashmobmobileapp)

[iOS](https://apps.apple.com/us/app/trashmob/id1599996743) 

## IFTTT Testing

IFTTT.com testing is currently in progress. There are a couple of steps needed to get this working.

  1. Log in to the TrashMob.eco Dev site, with the User ID you wish to use for testing.
  1. Hit F-12 to open the developer tools
  1. Go to the Network tab
  1. Click on the 'My Dashboard' link in the top of the TrashMob.eco web page
  1. In the network tab, look for a call to the TrashMob.eco api.
  1. Click on the call, and look for the Authorization header in the Request Headers section.
  1. Copy the value of the Authorization header
  1. Log in to the Azure Portal, and go to the Sandbox subscription
  1. Go to the Dev KeyVault
  1. Create a new version of the 'IFTTTAuthToken' secret with the AuthHeader value as the value of the secret
  1. Go to IFTTT.com and log in with an ID which has access to the TrashMobDev_Eco service
  1. Run the Endpoint Tests

## IFTTT Test User ID

The test user id needed for submitting the IFTTT tests is stored in the Dev KeyVault as IFTTTTestUserId. 
The test user password needed for submitting the IFTTT tests is stored in the Dev KeyVault as IFTTTPasswordWebsite.
The password needed for logging in to the Email account for the test user id is stored in the Dev KeyVault as IFTTTTestPassword.
