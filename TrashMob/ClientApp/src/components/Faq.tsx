import { Component } from 'react';
import * as React from 'react'

export class Faq extends Component {
    render() {
        return (
            <div>
                <h1>Frequently Asked Questions</h1>

                <h2>Questions about the site</h2>
                <h3>Does this site cost anything to use?</h3>
                <p>No. Setting up Trashmobs or joining a trashmob is free.</p>

                <h3>Can I donate to this site?</h3>
                <p>Not yet. But we hope to add that feature soon.</p>

                <h3>What would donations be used for?</h3>
                <p>The site has hosting costs that currently come out of the pocket of the devs. While these aren't huge, things do add up over time.</p>

                <h3>Is there a 503c Charity behind this site?</h3>
                <p>Not yet. But it is in the plans</p>

                <h3>Are you still looking for developers/designers to help with the site?</h3>
                <p>We have more ideas than time to implement them. We would love more help!</p>

                <h3>Is this an open-source project?</h3>
                <p>Yes. The hope is that this project can be used as a basis for other crowd-based sites which aim to improve life on planet Earth. If you can find a way to use this to help with these types of efforts, in a
                        way that continues the spirit of Trashmob, we encourage you to fork the repo.</p>

                <h3>Why do I have to be a registered user to Create an Event?</h3>
                <p>We want to make sure all events are legitimate and properly led. Leaving the site open to anonymous users to create events is a recipe for trouble</p>

                <h3>What do you do with my email address?</h3>
                <p>We will only use your email address to contact you regarding events on this site. We will never give your email address to outside agencies (except as required by law) or other users. Period.</p>

                <h2>Questions about Trashmob Events</h2>
                <h3>Can I go to an event if I am not signed up in advance?</h3>
                <p>Sometime yes, sometimes no. If the event has a maximum number of people set, and the maximum number has been reached, then just showing up could be an issue, especially when it comes to safety. Some events are size limited because
                    they are along busy roads or in areas which require special gear. We suggest that if you are unable to sign up for the event you want in advance, and still want to do something, that you go to a local park or a nearby area and do some picking
                    of your own. Every little bit matters!</p>
            </div>
        );
    }
}
