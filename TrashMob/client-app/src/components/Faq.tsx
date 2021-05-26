import * as React from 'react'
import { Link } from 'react-router-dom';
import helpwanted from './assets/tim-mossholder-vdhNO4mGQ14-unsplash.jpg';
import frustrated from './assets/jeshoots-com--2vD8lIhdnw-unsplash.jpg';

export const Faq: React.FC = () => {
    return (
        <div className="expandCollapse">
            <h1>Frequently Asked Questions</h1>

            <h2>Questions About the Site</h2>
            <div>
                <h3>Does this site cost anything to use?</h3>
                <p>No. Setting up TrashMobs or joining a TrashMob is free.</p>
            </div>
            
            <div>
                <h3>Are you still looking for developers/designers to help with the site?</h3>
                <p>As you can probably tell, this site is not finished. Besides some fit and finish, we have a lot more ideas than time to implement them. We would love more help! If you know ReactJS or web design
                        or are good with web content, ASP.NET Core, security, or have any non-technical skills that would be useful to a site / organization like ours, please <Link to="./contactus">Contact Us</Link>!
                    <img src={helpwanted} alt="Help wanted" />
                </p>
            </div>


            <div>
                <h3>Can I donate to this site?</h3> 
                <p>Not yet. But we hope to add that feature soon.</p>
            </div>

            <div>
                <h3>What would donations be used for?</h3>
                <p>The site has hosting costs that currently come out of the pocket of the developers. While these costs aren't huge, things do add up over time. With more usage, costs will climb.</p>
            </div>

            <div>
                <h3>Is there a Non-Profit Charity behind this site?</h3>
                <p>Not yet. But it is in the plans. Setting up an international not-for-profit takes a lot of time and resources away from working on the site and working on the litter problem itself. We'll need more volunteers involved in order to make this happen. If
                    you happen to have expertise in setting up non-profits and want to help in that area, please let us know.
                   </p>
            </div>

            <div>
                <h3>I found a bug!</h3>
                <p>Not possible.</p>
                <p>Ok. Fine. It's possible. Please report it to us via our <Link to="./contactus">contact us</Link> page.
                    <img src={frustrated} alt="Frustrated" />
                </p>
            </div>

            <div>
                <h3>Why do I have to be a registered user to Create an Event?</h3>
                <p>We want to make sure all events are legitimate and properly led. Leaving the site open to anonymous users to create events is a recipe for trouble.</p>
            </div>

            <div>
                <h3>What do you do with my email address?</h3>
                <p>We will only use your email address to contact you regarding events on this site. We will never give your email address to outside agencies (except as required by law) or other users. Period. See our <Link to="./PrivacyPolicy">Privacy Policy</Link></p>
            </div>

            <h2>Questions about TrashMob Events</h2>
            <div>
                <h3>Can I go to an event if I am not signed up in advance?</h3>
                <p>Sometimes yes, sometimes no. If the event has a maximum number of people set, and the maximum number has been reached, then just showing up could be an issue, especially when it comes to safety. Some events are size limited because
                they are along busy roads or in areas which require special gear or special training. We suggest that if you are unable to sign up for the event you want in advance, and still want to do something, that you go to a local park or a nearby area and do some picking of your own. Every little bit matters!</p>
            </div>
        </div>
    );
}

