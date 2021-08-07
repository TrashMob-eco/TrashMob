import * as React from 'react'
import { Link } from 'react-router-dom';

export const Partners: React.FC = () => {
    return (
        <div className="card">
            <h1>Partners</h1>

            <h2>What are Partners?</h2>
            <p>Partners are other organizations around the world that we are working with towards a common goal: a better planet for all of us.</p>
            <p>There isn't just one way to make the planet better.</p>
            <p>There isn't just one way to clean up litter.</p>
            <p>It's entirely possible that our way isn't even going to work.</p>
            <p>But by forming partnerships, we can share information about what is working, and what is not. <b>TrashMob.eco</b> is attempting to fix the very final stages of the waste management problem... fixing the damage already done. Other
            groups are working to prevent waste from being created in the first place, or making sure that what is created, is disposed of in the best possible way for the planet. Others are educating consumers on the choices they
                    make on a daily basis that affect the quality of the world we all live in. TrashMob.eco is just a small piece of the puzzle, and in a lot of cases, we don't even know what the finished puzzle looks like yet.</p>
            <p>There are multiple "big problems" people run into when tackling litter.</p><p> First of all, litter is everywhere, and that means that it takes far more people to clean it up than any government or corporation has available to
            get it done. We need thousands of ordinary people, boots on the ground, picking up trash around the world to even make a dent in this issue. That's where the "mobs" come in.</p>
            <p>The second "big problem" is what to do with the litter once it has been collected. There is a cost associated with disposal of huge amounts of litter, and a concern that volunteers may not know how to dispose of it correctly.</p>
            <p>That's where Partners come in. Partners, whether they be local goverment, or waste disposal companies, or companies not in the waste disposal business who have hauling capacity for the garbage we collect, are needed to do the hauling and disposal.</p>
            <p>And because this is a global issue, we need Partners all over the world to make it as easy as possible for the TrashMob.eco Mob leads to figure out who to contact to ensure the haul away is done correctly.</p>

            <h2>How to Become a Partner?</h2>
            <ol>
                <li>If you (personally) are not already registered for the site, please register by signing in to the site and filling out your user profile.</li>
                <li>Click on the menu option in the header to <Link to="./becomeapartner">Become a Partner</Link>. This will get some basic information from you so TrashMob.eco can ensure you are who you say you are. We should be able to approve or deny your request within 1 business day.</li>
                <li>Once you are approved as a Partner, please go to your Partner Dashboard on the header menu. This page allows you to fill in some more details about your organization, and also allows you to set up Partner Locations. Often, organizations have more than one location in a city or region, and we want to make sure that the location matching algorithm with events is as close to your location as possible. Currently, the matches are suggest by either by City or Postal Code, but that will be optiomized further in the future.</li>
                <li>Set up as many locations as you like. Each location can have separate contact information to allow them to coordinate their own events.</li>
                <li>Ensure you set your Partner Status and Partner Location Status to Active. If you ever need to suspend your partnership, or a location's partnership, simply change this to Inactive and come back later to reactivate it.</li>
            </ol>

            <p>Once you have activated your partnership and partner locations, your organization's locations will begin appearing on a list when TrashMob.eco users create / edit events in your area.</p>
            <p>If they choose to request help from you, you will receive an email letting you know that an event has requested your help. You can then come back to your Partner Dashboard, and accept or decline the request.</p>
            <p>If you accept the request, the Event Lead will be notified and will reach out to you to coordinate the event.</p>

            <h2>Adding Users to a Partner</h2>
            You should always have more than one user set up in TrashMob.eco to administer your organization and respond to requests.

            To add a new user:
            <ol>
                <li>Go to your Partner Dashboard</li>
                <li>Select Manage Partner Users</li>
                <li>Enter the username of the person you wish to add as a Partner Administrator. They must have previously registered on the site.</li>
                <li>Click Add User.</li>
            </ol>
        </div>
    );
}
