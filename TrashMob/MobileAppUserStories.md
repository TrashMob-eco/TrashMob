# TrashMob.eco Mobile App User Flows

- Entries tagged in **bold** are items that are either not yet implemented or need to be updated.

## User Stories

1. As a user, I want to be able to download and install the Mobile App for Google Play or the Apple Store so that I can participate in events.
1. As a user, when I open the Mobile App, I want to see a splash screen so that I can see the app is loading.
1. As a user, when I open the Mobile App, I want to be taken to a welcome screen so that I can see what I can do.
1. On the welcome screen, I want to be able to do the following actions:
	1. Sign up for the Mobile App using my email and a new password so that I can participate in events.
	1. Sign up for the Mobile App using a third party auth provider like Google, LinkedIn, Facebook, Microsoft, or Apple so that I can participate in events.
	1. During the sign up process, I want to set a user name so that I can be identified on the site.
	1. Sign in to the Mobile App so that I can participate in events.
	1. Review the privacy policy and terms of service so that I can understand how my data will be used.
	1. **View guides for how to use the mobile apps so that I can get the most out of them.**
	1. View the TrashMob.eco safety video hosted on YouTube so I can be safe while picking litter and alleviate hesitation to sign up.		
	1. View the statistics of the organization so that I can see the impact of the organization. The statistics should include:
		1. Number of events
		1. Number of attendees
		1. Number of bags of litter collected
		1. **Number of Litter Reports created**
		1. Total number of hours spent cleaning by all volunteers

## Signed-In User Stories

1. As a signed-in user, I want to be able to sign out of the Mobile App when I am done using it.
1. **As a signed-in user, I want to review the privacy policy and terms of service so that I can understand how my data will be used.**
1. **As a signed-in user, I want to be able to view guides for how to use the mobile apps so that I can get the most out of them.**
1. **As a signed-in user, I want to be able to view the TrashMob.eco safety video hosted on YouTube so I can be safe while picking litter.**
1. As a signed-in user, I want to be able to view my statistics so that I can see my impact of the organization. The statistics should include:
	1. Number of events I have attended
	1. Number of bags of litter collected at events I have attended
	1. **Number of Litter Reports I have created**
	1. Total number of hours I have spent cleaning
1. As a signed-in user, I want to be able to contact the site administrator so that I can ask questions or report issues.
1. As a signed-in user, I want to be able to set my preferred location so that I can see events near me and be notified of new events near me.
	1. I should be able to go to a page that allows me to specify my preferred location
	1. The page should default to my current location if I have allowed the site to access my location. If not, it should default to the center of the United States.
	1. If I have already set my preferred location, it should show my preferred location so that I can change it if needed.
	1. The map should be centered on the preferred location
	1. I should be able to zoom in and out on the map
	1. I should be able to move the map around
	1. I should be able to click on the map to drop a pin which sets my new preferred location.
	1. I should be able to search for a location in the search bar and have the map center on that location
	1. A pin should be dropped on the preferred position
	1. Once the pin has been set, use the lat-long to call the Azure Api to get the exact address
	1. Update the address below the map with the location retrieved from the api
	1. I should be able to move the pin to a new location
	1. I should be able to choose a maximum radius from the preferred location to show events (must be greater than 0)
	1. I should be able to select the preferred units of measurement for the radius (miles or kilometers)
	1. I should see the location details below the map
	1. I should be able to save the preferred location
	1. I should be able to cancel the preferred location update
1. As a signed-in user, I want to be able to see a list of upcoming events near me so that I can participate in them. The list should include a card for each event that shows:
	1. Event name
	1. Date
	1. Time
	1. A method to take me to the Event Details Page for that event
1. As a signed-in user, I want to be able to see a map of upcoming events near me so that I can participate in them. The map should include a custom pin for each event that shows:
	1. Event name
	1. Date
	1. Time
	1. A method to take me to the Event Details Page for that event
1. As a signed in user, I want to be able to view the details of an event I have selected so that I can see what is involved. The details should include:
	1. Event name
	1. Date
	1. Time
	1. Description
	1. Location (Street Address, City, State/Region, Country, Postal Code)
	1. Maximum Participants allowed
	1. Number of Participants registered
	1. Associated Litter Report(s)
	1. Hauling Partner
	1. Other services partners
	1. **Map of Litter Reports associated with the event**
	1. A method to register for the event
	1. A method to unregister for the event if I am already registered
	1. A method to view the event on a map
	1. If I am the event lead, I should see a method to edit the event
	1. If I am the event lead, I should see a method to cancel the event
	1. **If the event has started and I am registered, I should see a method to track my route and time spent cleaning**
	1. **If I am tracking my route, I should see a method to stop tracking my route**
	1. **I should be able to start and stop tracking my route multiple times during the event**
	1. **If I turn tracking on, I should received a notification that tracking has started**
	1. **If I turn tracking off, I should receive a notification that tracking has stopped**
	1. **If I turn tracking on, I should be reminded to turn it off if I forget, even if the app is not in the foreground**
	1. **A method to close the event details and return to the list of events**
1. As a signed-in user, I want to be able to sign a waiver so that I can participate in events.
	1. I should only have to sign the waiver once unless the waiver has changed.
	1. I should not be able to register for an event until I have signed the waiver.
	1. I should not be able to create an event until I have signed the waiver.
1. As a signed-in user, I want to be able to create a litter report so that others know where litter is accumulating.
	1. I click the Create Litter Report button
	1. I am taken to a page where I can enter the following information:
		1. Name of the litter report
		1. Description
		1. I can click a camera icon to take a photo of the litter
		1. The photo is geotagged with the location of the litter report
		1. Up to 5 photos can be added to the litter report, each geotagged with the location of the photo.
		1. The litter report can then be saved to the site
		1. I am taken back to the home screen
1. As a signed-in user, I want to be able to see a list of nearby open litter reports so that I can see where litter is accumulating. I should see the following information for each litter report:
	1. Name of the litter report
	1. Description
	1. Location (Street Address, City, State/Region, Country, Postal Code) of the first photo in the litter report
	1. A method to view the litter report details
1. As a signed-in user, I want to be able to see a map of nearby open litter reports so that I can see where litter is accumulating. A map pin should appear on the map for every photo in all litter reports in the area. The pin should show the following information:
	1. Name of the litter report
	1. Description
	1. Location of the photo (Street Address, City, State/Region, Country, Postal Code)
	1. A method to view the litter report details
1. As a signed-in user, I want to be able to view the details of a selected litter report so that I can see what is being reported. I should be able to see the following information:
	1. Name
	1. Description
	1. Date Created
	1. A map of all photos in the litter report with their associated location (Street Address, City, State/Region, Country, Postal Code)
	1. A view of all photos in the litter report with their associated location (Street Address, City, State/Region, Country, Postal Code)
1. **As a signed-in user, I want to see anonymized tracking reports of other events so I can know which areas have been cleaned recently and which areas need attention.**
1. **As a signed-in user, I want to be able to delete my account so that my data is removed from the site.**
1. As a signed-in user, I want to be able to view a dashboard of all my activities so that I can track my participation.
1. As a signed-in user, on my dashboard I want to be able to see a list of events that I have signed up for so that I can plan my schedule.
1. As a signed-in user, on my dashboard I want to be able to see a map of events that I have signed up for so that I can plan my schedule.
1. As a signed-in user, on my dashboard I want to be able to see a list of events that I have created so that I can manage them.
1. As a signed-in user, on my dashboard I want to be able to see a map of events that I have created so that I can manage them.
1. As a signed-in user, on my dashboard I want to be able to see a list of events that I have attended so that I can track my participation.
1. As a signed-in user, on my dashboard I want to be able to see a map of events that I have attended so that I can track my participation.
1. As a signed-in user, on my dashboard I want to be able to unregister for an event so that I can change my plans.
1. **As a signed-in user, on my dashboard I want to be able to edit a litter report that I created.**
1. **As a signed-in user, on my dashboard I want to be able to close a litter report that I created.**
1. As a signed-in user, on my dashboard I want to be able to view my statistics so that I can see my impact on my community.

## Event Lead Stories
1. As an event lead, I want to be able to create an event so that I can organize a litter pickup.
	1. An event has the following required fields: 		
		1. Name
		1. Type
		1. Description
		1. Location
			1. Street Address
			1. City
			1. State/Region
			1. Country
			1. Postal Code
			1. Latitude / Longitude
		1. Date
		1. Start Time
		1. Expected Duration in Hours + Minutes
			1. Duration cannot be longer than 10 hours
		1. Whether or not the event is Public/Private
			1. If an event is public, the date must be in the future
			1. If an event is private, the date can be in the past
	1. An event has the following optional fields:
		1. Services
		1. Maximum Participants
		1. Minimum Age
		1. Associated Litter Report(s)
		1. Hauling Partner
			1. There can be only 1 hauling partner per event
		1. Other services partners
		1. **Map of Litter Reports associated with the event**
1. **As an event lead, I want to be able to create and event based on a selected litter report with the details pre-populated.**
	1. Details should include:
		1. Event Name = Litter Report Name
		1. Street Address = Litter Report Street Address
		1. City = Litter Report City
		1. State/Region = Litter Report State/Region
		1. Country = Litter Report Country
		1. Postal Code = Litter Report Postal Code
1. **As an event lead, I want to be able to associate a litter report with an event so that the two are linked.**
1. As an event lead, I want to be able to edit an event I created so that I can update the details.	
1. As an event lead, I want to be able to cancel an event I created and notify all attendees.
1. **As an event lead, I want changes to an event I created to be sent to all attendees.**
1. As an event lead, I want to be able to see a list of attendees for my event so that I can plan accordingly.
1. As an event lead, I want to be able to enter a summary of the event after it is completed so that I can track participation and impact.
	1. A summary includes:
		1. Number of attendees
		1. Number of bags of litter collected
		1. Total actual duration of the event
1. As an event lead, I want to be able to record where the bags of litter were left for a hauling partner to pick up.
	1. 
1. As an event lead, I want to be able to notify the hauling partner that the bags are ready to be picked up 
1. **As an event lead, I want to be able to see a map of where attendees picked litter for my event so that I can track what areas have been cleaned.**
1. As an event lead, I want to be able to add events from the past to the site so I can track my impact, but not have them show up in the list of upcoming events or have notifications going out.
1. **As an event lead, I want to mark associated litter reports as resolved so that they are no longer shown as needing attention.**

