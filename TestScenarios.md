# TrashMob.eco Test Scenarios

## General UI Requirements:

- Required fields should be visually tagged as required
- All fields should have tool tips on hover
- All fields which have restrictions should show errors when value outside of range

## Scenarios

1. Sign up for site
    a. UserName required & Unique
    b. Email required and Unique
    c. Agree to Privacy Policy Required
    d. Agree to Terms of Service Required
    e. User is added to the database
    f. Email sent to Admin
    g. Welcome email sent
2. Sign in to site
    a. Agree to Privacy Policy Required
    b. Agree to Terms of Service Required
3. Contact Us
    a. Request is saved to database
    b. Request is emailed to Admin
4. Update User Location Preference
    a. Future
        v. Instructions / links provided for how to update these values
    b. User can set preferred location, radius, and metric versus imperial
5. Create Event
    a. If user has not signed current waiver, taken to sign waiver screen first
    b. Name required
    c. Location required
    d. EventDate required
    e. If event is public, date must be in the future
    f. User can choose services from list of partner locations
        i. The list of partner locations should only include those where either the city or postalcode are the same
        ii. Inactive partner locations should not appear in the list
        iii. User should be able to choose multiple services per partner
    g. Email is sent to TrashMob admins on create
    h. Email is sent to all contacts for the selected partner location on create
6. Update event
    a. If location or time changes, notify all attendees
7. Sign Waiver
    a. User is taken to a page to sign a waiver
    b. If user signs waiver, they can register for events or create events
    c. If user does not sign waiver, user cannot register for or create events
8. Register for event
    a. If user has not signed current waiver, taken to sign waiver screen first
    b. User is registered for event
    c. Notification Email is sent to event lead
9. Send Invite to potential partner
    a. Email is sent to potential partner
    b. Invite shows up as sent on MyDashboard
10. Request to become a partner
    a. User fills out form
    b. Email is sent to TrashMob Admin
11. Update Partner
    a. Update properties
    b. Invite Admin
        i. Email is sent to email address invited
        ii. Invitation shows up on MyDashboard
        iii. Invitation shows up on Partner Dashboard
    c. Add Partner Contact
    d. Add Partner Social Media Account
    e. Add Partner Document
    f. Add Partner Location
        i. Update Partner Location Properties
        ii. Add Partner Location Contact
        iii. Add Partner Location Services
12. Site Administration
    a. Option appears on menu for administrators
    b. Option does not appear on menu for non-administrators
