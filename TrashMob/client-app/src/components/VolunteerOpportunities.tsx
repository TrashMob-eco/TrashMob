import * as React from 'react';

export const VolunteerOpportunities: React.FC = () => {
    return (
        <div className="card">
            <h2>Looking for ways to contribute to the growth of TrashMob.eco?</h2>
            <p>
                There are many ways to get involved in the growth of TrashMob.eco besides picking litter. Below
                are a few ways you can contribute from the comfort of your own home! If you are interested in any of these opportunuities,
                reach out to us at info@trashmob.eco. 
            </p>
            <h3>Web Designers</h3>
            <p>
            We're always looking for more people to help with the UI design on the website and the mobile app. If you've got some Figma skills and some spare time,
            we'd love to have you give us a hand!
            </p>
            <h3>Web Developers</h3>
            <p>
                Our site is built with ReactJS and .NETCore, and is hosted on Microsoft Azure. We've got lots of ideas for new features, and lots of existing features that need
                some TLC to make them shine. Here's a list of some of the technology we use that we can always use help with:
                <ol>
                    <li>ReactJS</li>
                    <li>CSS</li>
                    <li>Azure Maps</li>
                    <li>AzureAD B2C</li>
                    <li>GitHub</li>
                </ol>
            </p>
            <h3>Mobile Developers</h3>
            <p>
                Our mobile app is being built with Xamarin for Android and Apple. There are so many ideas we have for the mobile app, and can always use devs to help us turn these dreams into reality.
            </p>
            <h3>Accounting and Finance</h3>
            <p>
                As we launch our organization, we're looking for someone with solid experience in accounting and finance to advise us on how to set up our treasury and accounting system. This is
                a critical step for every non-profit to get right, to ensure we observe all relevant regulations, and to set up a system our donors and partners can trust.
            </p>
            <h3>City and Country Leads</h3>
            <p>
                One of the main goals of TrashMob.eco is to remove any barriers our litter-picking volunteers may face. These barriers may include finding out who to talk to in each municipality,
                which forms needs to be filled out to make sure everything we do meets local regulations, and which corporate partners may want to get involved to assist with the TrashMob.eco events. If
                organizing events and talking with local leaders is your thing, then this may be the role for you.
            </p>
            <p>
                While we aren't quite ready to start launching our efforts in too many cities yet, we are looking forward to the next round of launches. If you want to help with this, please let us know.
            </p>
        </div>
    );
}
