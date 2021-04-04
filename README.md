# FlashTrashMob

# Credit Where Credit Is Due
Years ago, Scott Hanselman built out the NerdDinner.com site as a demo of the capabilities of ASP.NET MVC. I actually went to a bunch of the nerd dinners which were fantastic and had a huge roll in my career, including eventually leading me to join Microsoft. This site is based on both that code and the idea that getting people together to do small good things results in larger good things in the long term.
 
# What is a TrashMob?
Fast forward to today, and my passion is fixing problems we have on the planet with pollution and climate change. I've been thinking about what technology can do to help in these areas, without creating more problems. And I keep coming back to the thought that a lot of this is a human problem. People want to help and they want to fix things, but they don't know where to start. Other people have ideas on where to start, but not enough help to get started.
 
I read about a guy in California named [Edgar McGregor](https://twitter.com/edgarrmcgregor), who has spent over 600 days cleaning up a park in his community, two pails of litter at a time, and I thought, that was a great idea. His actions inspired me to get out and clean up a local park one Saturday. It was fun and rewarding and other people saw what I was doing on my own and I know I have already inspired others to do the same. And then I passed by an area of town that is completely covered in trash and I thought "this is too much for me alone. It would be great to have a group of people descend on this area like a mob and clean it up in an hour or two". And my idea for TrashMob.org was born.
 
Basically, TrashMob is the NerdDinner.com site re-purposed to allow people to start mobs of their own to tackle cleanup or whatever needs doing. And I keep coming up with more and more ideas for it. I'm hoping this site grows organically because of the good that we can do we get together.

# FAQ 
## What is the current state of this project?

This project is currently under development, with the plan to launch in the spring of 2021. There's lots of work to get the site to an MVP state. I have a plan I have been working on that I will share if people are interested

## Top Priority Issues

Just a few of the development things I haven't solved yet (besides design).
1. Fix routing for sub pages
1. When The MSALAuthenticationTemplate is added back into the App.tsx, every route on the site requires authentication, instead of just the pages inside the template
1. The Key for the Azure Maps control needs to be retrieved from the backend, but it's an async operation, and the page isn't waiting for it
1. The Date Picker on the Add Event page needs to be replaced with a DateTime Picker that works
1. Add a login / logout button
1. When a new user signs up, create the user in the database
1. Finish save event experience
1. Need content for Privacy Policy
1. Need content for Terms of Service
1. Add Contact Us functionality
1. Add My Dashboard with ability to add a new event, and view / update existing ones
1. Get proper logo
1. Figure out how to deploy to a production versus dev environment

## What is the domain name going to be?

I'm still working on procuring the domain name(s).

## Are you looking for contributors?

ABSOLUTELY! Ping [Joe Beernink](https://www.twitter.com/joebeernink) if you want to get involved. All kinds of skills needed, from logo design to reactjs, to website design, to aspnet core, to deployment / github skills.

## What needs to be done today?

So much. Best to ping me as I am coding every day on this. 

## When this project is "done" will it be open sourced?

Yes. Since this idea came from NerdDinner.com (which was open-source), this project will continue to be available for others to learn from, contribute to, and fork, under the same license. I'm hopeful that others can use this project as an example of how to organize large numbers of people for good purposes, and by adding event types to it (or spawning new types of sites from a fork), that we can fix global problems by starting locally.

## Will this site be turned into a non-profit. 

Quite possibly. We'll see how fast the site gains momentum

# Development Notes

## Database generation
The project is now using Entity Framework Core V6 Model-First database updates.

1. Update the models / MobDbContext as needed.
2. In VS Code, run the following commands from the TrashMob folder

```
dotnet ef migrations add <YourMigrationName>
dotnet ef database update
```

## Where do I get the connection for the database?
The connection to the dev database is stored in an Azure KeyVault, and can be stored as a User Secret locally. At some point, I need to figure out how to make this work for multiple devs.

## The secrets you will need to run locally

```

cd Trashmob
dotnet user-secrets init
dotnet user-secrets set "AzureMapsDev" "<insert secret here from keyvault>"
dotnet user-secrets set "TMDBServerConnectionString" "<insert secret here from keyvault>"

```
## How do I deploy to the Azure Web App?
I currently use Visual Studio Publish to publish the site to the dev server. Eventually I want this to happen via GitHub.




