# TrashMob.eco

**Cleaning up Planet Earth, one bucket of litter at a time. Together.**

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

As of 5/26/2021, we are now in Beta launch. We'll hold at Beta until a few more key features are complete so we can do a grand-relaunch when those features go in. Beta alsop means that if things really go wrong, we may have to delete data manually and depending on load, site availability is not guaranteed.

## Are you looking for contributors?

ABSOLUTELY! Ping [Joe Beernink](https://www.twitter.com/joebeernink) if you want to get involved. All kinds of skills needed, from logo design to reactjs, to website design, to aspnet core, to deployment / github skills.
 
# Development Notes

## Getting Started

1. You must install the .net 5 SDK
1. Install Visual Studio Code
1. Connect to github and clone the repo
1. Follow the Infrastructure Deployment Steps (here)[.\Deploy\readme.md].
1. Run the following script on your machine from the TrashMob folder in the project to set up your dev machine to run the project locally. You must be logged into Azure in your PowerShell window in the correct subscription
```
.\setupdev.ps1 -environment <yourenv> -region <yourregion>

i.e.
.\setupdev.ps1 -environment jb -region westus2

```

## Setting up your launchsettings.json

Because of RedirectUrls, life is a lot easier if you stick with the same ports as everyone else. In the TrashMob folder, under Properties, add the following launchsettings.json file: 

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

## To Build the app:

In the Trashmob project folder, run the following command:
```
dotnet build
```

## To Run the app:

In the Trashmob project folder, run the following command:
```
dotnet run
```

or if using Visual Studio, set the TrashMob project as the start up project and hit F5.

If a browser does not open, open one for yourself and go to https://localhost:44332

## To Update the Database Model
The project uses Entity Framework Core V6 Model-First database updates.

1. Update the models / MobDbContext as needed in the repo.
2. In VS Code, run the following commands from the TrashMob.Shared folder

```
dotnet ef migrations add <YourMigrationName>
dotnet ef database update
```

## Allowing the App To Send Email

This is a pay-per-use feature, so, for the most part, we're going to try to limit the number of people developing with this. To <b>not</b> send email, make sure to set the following user secret on your dev box 
```
  dotnet user-secrets set "sendGridApiKey" "x"
```

To test sending email, copy the "sendGridApiKey" fron the dev keyvault to your machine and repeat the above, sustituting in the real key. 

## How do I deploy the Azure Web App from GitHub?
The Dev site is automatically deployed with each push to the Main branch via a GitHub action. This is the preferred method of updating the Development Server. If you need to push an update from your development machine instead, please let the team know that there are changes in the environment pending checkin.

The Production site is manually deployed via a GitHub action from the release branch. This is the ONLY way production should be updated.

## How do I deploy the Azure Web App from my PC?
Use Visual Studio Publish to publish the site to the dev server.

If setting up a new environment, you will need to add the IP Address of the App Service to the list of IP Addresses accessible in the SQL Server. This needs to me automated in the future to make sure that a change to an IP address doesn't break the site.

## The site is asking me to login
If you try to access a secure page, you will need a user id on the site. When you hit a secured page, the site will redirect you to a sign in page. Click the Sign up now link on the bottom of the login box. Multiple identity providers are now available, including Facebook, Twitter, Google, and Microsoft, along with the TrashMob tenant itself if you prefer not to use an integrated signup.
