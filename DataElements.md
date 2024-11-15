# Data Elements and Concepts

## Events

An event is an organized effort to clean up litter at a specific location at a specific date and time.

An Event has:

- Name
	- The event creator (lead) chooses a name for the event that makes it easy for it to recognize
- Description
	- The lead adds any notes that attendees might need in order to have a great experience, including directions to the event meet up location, what to wear, what to bring, what will be supplied, etc.
- Event Type
	- A large set of options is available to classify the event so that users get a general idea of what the event might entail and who the event is suitable for. A Park Cleanup is probably appropriate for kids to attend. A highway cleanup is probably for adults only.
- Start Time
	- The time the lead has chosen to meet up with everyone to get the cleanup started
- Duration
	- The expected length of time where the lead will be cleaning. Attendees may not stay the whole time, or may stay later.
- Location
	- The address (including latitude and longitude) of the event meetup location so the attendee can find events near them
- Maximum Number of Attendees
	- For most events, there is no maximum. Defaults to 0. But in some cases, for safety reasons, or because limited supplies are available, the lead may choose to set a maximum number of attendees.
- Status
	- This is inferred from todays date relative the start time of the event. Events which have not yet started, are considered upcoming. Events which have started may be considered completed because the event lead may not be at the meetup location any longer. An event may also be canceled. Canceled is an internal marker only. The event is deleted from the list of available sites and is no longer visible to the perspective attendees or the lead. A completed event cannot be deleted (for legal reasons we need to track who organized and attended the event)
- Is Public Event
	- Most events are public, but a lead can create a private event as well if they want to add to their stats but not have others join them. Private events can also be set up in the past, so that if a user has just joined TrashMob, they can record some of their individual efforts from the recent past. A private event does not send out notifications to other users.
- Lead
	- The person who creates the event is considered the event lead. (We may allow multiple leads in the future.)
- Attendees
	- A person who registers for the event is considered an attended. An attendee must sign the TrashMob.eco Liability Waiver prior to signing up to attend their first event.
- Partner
	- A partner is a person or organization who provides support, supplies, or services to the event.
- Litter Reports
	- 0 to many litter reports can be assigned to an event so that attendees can find and target nearby areas that need cleaning.
- Event Summary
	- After the event is completed, the lead records a summary of the event, including:
		- The number of bags picked
		- The number of attendees
		- The actual duration of the event
		- Any notes about the event
	- Pickup Locations
		- If the event has a hauling partner, the event lead can take pictures of the collected litter. These images are geo-tagged and sent to the hauling partner who will dispatch a team to pick up the bags.

## Litter Reports

A litter report is filed by a TrashMob.eco user to report areas where a litter cleanup is needed.

A litter report has:
- Name
	- A short description of the litter which will be visible on the TrashMob.eco maps.
- Description
	- A longer description of the litter
- Litter Images
	- 1 to 5 photos of the litter. Each litter image is geo-tagged with a latitude and longitude which can be plotted on a map.
	- While a litter report can have images that are in radically different locations, the Location the Litter Report is associated to will be the location of the first image. On maps however, the location of all images may be rendered.
- Status
	- A litter report can have one of the following statuses:
		- New
			- The default status when the Litter Report is created
		- Assigned
			- The Litter Report has be assigned to an event to be cleaned up during that event
		- Cleaned
			- The Litter Report has been cleaned
		- Canceled 
			- The Litter Report has been deleted by the user.

## Users

A user registers on the TrashMob.eco website via one of the available identity providers such as Microsoft, Google, Apple, Linked-In, or Email (among others)

A user has:
- An email address
	- All users must supply a valid email address to allow TrashMob.eco to contact them.
- A user name
	- A friendly user name must be chosen by the user during registration as we never display the user's email to the public.
- A preferred location
	- The user can set their preferred location to 

## Partners

A partner provides services to event leads and attendees to help with the logistics of the event. A partner may be an individual, a government agency, a private business, or another organization.

A partner has:

- Name
	- The name of the partner
- Description
	- The description of the partner
- Locations
	- The partner may have 1 to many locations from which it can provide services. For example, a grocery store chain may have multiple locations which are willing to let volunteers user the location's dumpsters after an event to dispose of trash picked from nearby locations.
	- The location has a latitude and longitude which can be plotted on a map
- Services Provided
	- A partner may provide 1 or more of the following services:
		- A disposal location (i.e. a dumpster)
		- Hauling - the partner will come to the location of the event after the event and pick up the bagged garbage and haul it away for proper disposal
		- Supplies - i.e. food, drinks, garbage bags
		- Equipment - i.e. buckets, reflective vests, litter pickers, gloves, etc.
- Social Media contact info
	- They provide this to help amplify the event via various social media providers