# TrashMob.eco

**Meet up. Clean up. Feel good.**

# What is TrashMob?
TrashMob is a website dedicated to organizing groups of people to clean up the world we live in. Users create cleanup events, publicize them, and recruit people to join up, as well as ask for assistance from sponsors to help haul away the garbage once it is gathered. The idea is to turn what can be an intimidating process for event organizers into a few mouse clicks and simple forms. And once the process is simple, events will spring up all over the world, and the cleanup of the world can begin.

# Where did this idea come from?
Years ago, Scott Hanselman (and others at Microsoft) built out the NerdDinner.com site as a demo of the capabilities of ASP.NET MVC. I actually went to a bunch of the nerd dinners which were fantastic and had a huge roll in my career, including eventually leading me to join Microsoft. This site is based on both that code and the idea that getting people together to do small good things results in larger good things in the long term.

My passion is fixing problems we have on the planet with pollution and climate change. I've been thinking about what technology can do to help in these areas, without creating more problems. And I keep coming back to the thought that a lot of this is a human problem. People want to help and they want to fix things, but they don't know where to start. Other people have ideas on where to start, but not enough help to get started.
 
I read about a guy in California named [Edgar McGregor](https://twitter.com/edgarrmcgregor), who has spent over 600 days cleaning up a park in his community, two pails of litter at a time, and I thought, that was a great idea. His actions inspired me to get out and clean up a local park one Saturday. It was fun and rewarding and other people saw what I was doing on my own and I know I have already inspired others to do the same. And then I passed by an area of town that is completely covered in trash and I thought "this is too much for me alone. It would be great to have a group of people descend on this area like a mob and clean it up in an hour or two". And my idea for TrashMob.eco was born.
 
Basically, TrashMob is the NerdDinner.com site re-purposed to allow people to start mobs of their own to tackle cleanup or whatever needs doing. And I keep coming up with more and more ideas for it. I'm hoping this site grows organically because of the good that we can do we get together.

## What is website address?

To see what is currently deployed to the prod environment, go to:
https://www.trashmob.eco

To see what is currently deployed to the dev environment, go to:
https://as-tm-dev-westus2.azurewebsites.net/

# FAQ 
## What is the current state of this project?

As of 5/26/2021, we are now in Beta launch. We'll hold at Beta until a few more key features are complete so we can do a grand-relaunch when those features go in. Beta also means that if things really go wrong, we may have to delete data manually and depending on load, site availability is not guaranteed.

## Are you looking for contributors?

ABSOLUTELY! Ping [Joe Beernink](https://www.twitter.com/joebeernink) if you want to get involved. All kinds of skills needed, from logo design to reactjs, to website design, to aspnet core, to Xamarin, to deployment / github skills.
 
# Development Notes

## Getting Started - Development

1. You must install the .net 5 SDK
1. Install Visual Studio Code
1. Connect to github and clone the repo
1. Install the [Azure CLI](https://docs.microsoft.com/en-us/cli/azure/install-azure-cli-windows?tabs=azure-cli)

### To use the Shared Dev Environment
If you are not doing any database changes (i.e. design work, error handling, etc) you can save yourself time and money by doing the following and using the shared Dev environment:
1. Send the email address you use on GitHub to Joe Beernink
1. Joe will add you as a contributor to the Sandbox subscription
1. Joe will add you to the Dev KeyVault with Secret Get and List permissions
1. Log in to the Sandbox subscription, and go to the [Dev Azure SQL Database](https://portal.azure.com/#@jobeedevids.onmicrosoft.com/resource/subscriptions/39a254b7-c01a-45ab-bebd-4038ea4adea9/resourceGroups/rg-trashmob-dev-westus2/providers/Microsoft.Sql/servers/sql-tm-dev-westus2/overview)
1. Click on Firewalls and Virtual Networks
1. Add a new Rule with your email address as the name, with the start and end ip address set as your Client IP Address (see the line above the form for what Azure thinks your IP address is)
1. **Save** changes
1. Run the following script on your machine from the TrashMob folder in the project to set up your dev machine to run the project locally. You must be logged into Azure in your PowerShell window in the correct subscription
```
.\setupdev.ps1 -environment dev -region westus2 -subscription 39a254b7-c01a-45ab-bebd-4038ea4adea9
```

### To set up your own environment to test in:
You must use this if you are making database changes to ensure you do not break the back end for everyone else:

1. Follow the Infrastructure Deployment Steps (here)[.\Deploy\readme.md].
1. Run the following script on your machine from the TrashMob folder in the project to set up your dev machine to run the project locally. You must be logged into Azure in your PowerShell window in the correct subscription
```
.\setupdev.ps1 -environment <yourenv> -region <yourregion> -subscription <yourazuresubscription>

i.e.
.\setupdev.ps1 -environment jb -region westus2 -subscription <insert guid here>

```

## Setting up your launchsettings.json

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

## Getting Started - Mobile Development

The mobile app is written using Xamarin. It requires a few prerequisites in order to get it compiling and running.

1. Ensure you have installed the Xamarin components to Visual Studio
2. Install Android Studio https://developer.android.com/studio 
3. Create an Android Emulator device in Android Studio
4. Start the TrashMobMobile Project in Visual Studio.
5. In order to have the maps feature work, you will need to do the following:
    1. Create a Google Maps account: https://developers.google.com/maps/gmp-get-started
    2. Get your Google API Key from your Google Project
    3. Create a gradle.properties file in your GRADLE_USER_HOME (i.e. c:\users\<username>\.gradle)
    4. Add the following line to your gradle properties file: 
    ```
    GOOGLE_API_KEY = "<Your api key>"
    ```
    5. Restart your emulator. Maps shoould work now

    Never check in any file that contains your real api key.

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

## To Update the Database Model
The project uses Entity Framework Core V6 Model-First database updates.

1. Update the models / MobDbContext as needed in the repo.
2. To create the migration, do either of the following steps

In VS Code
```
dotnet ef migrations add <YourMigrationName>

```

or in Visual Studio Package Manager Console
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

To test sending email, copy the "sendGridApiKey" fron the dev keyvault to your machine and repeat the above, sustituting in the real key. 

## A note on Azure Maps usage
The call to find the distance between two points in Azure Maps is only available in S1 (Gen 1) or Gen2 Maps. This is significantly more expensive than the S0 maps, so for now, we default to S0 for all dev deployments, and have manually set Prod to Gen2. It is not recommended to rerun the infrastructure deployments to Prod, as this will overwrite this setting.

In the future, we may want to optimize the use of this function to reduce costs.

## How do I deploy the Azure Web App from GitHub?
The Dev site is automatically deployed with each push to the Main branch via a GitHub action. This is the preferred method of updating the Development Server. If you need to push an update from your development machine instead, please let the team know that there are changes in the environment pending checkin.

The Production site is manually deployed via a GitHub action from the release branch. This is the ONLY way production should be updated.

## How do I deploy the Azure Web App from my PC?
Use Visual Studio Publish to publish the site to the dev server.

If setting up a new environment, you will need to add the IP Address of the App Service to the list of IP Addresses accessible in the SQL Server. This needs to me automated in the future to make sure that a change to an IP address doesn't break the site.

## The site is asking me to login
If you try to access a secure page, you will need a user id on the site. When you hit a secured page, the site will redirect you to a sign in page. Click the Sign up now link on the bottom of the login box. Multiple identity providers are now available, including Facebook, Twitter, Google, and Microsoft, along with the TrashMob tenant itself if you prefer not to use an integrated signup.
