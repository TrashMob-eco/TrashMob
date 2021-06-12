import * as React from 'react'
import { Link } from 'react-router-dom';

export const Sponsors: React.FC = () => {
    return (
        <div className="card">
            <h1>Sponsors</h1>

            <h2>The Role of Sponsors at TrashMob.eco</h2>
            <p>There are multiple "big problems" people run into when tackling litter.</p><p> First of all, litter is everywhere, and that means that it takes far more people to clean it up than any government or corporation has available to
            get it done. We need thousands of ordinary people, boots on the ground, picking up trash around the world to even make a dent in this issue. That's where the "mobs" come in.</p>
            <p>The second "big problem" is what to do with the litter once it has been collected. There is a cost associated with disposal of huge amounts of litter, and a concern that volunteers may not know how to dispose of it correctly.</p>
            <p>That's where sponsors come in. Sponsors, whether they be local goverment, or waste disposal companies, or companies not in the waste disposal business who have hauling capacity for the garbage we collect, are needed to do the hauling and disposal.</p>
            <p>And because this is a global issue, we need sponsors all over the world to make it as easy as possible for the TrashMob.eco Mob leads to figure out who to contact to ensure the haul away is done correctly.</p>
            <p>So, as this site gets going, we're looking for companies and governments that want to help, to become sponsors on our site, and to give TrashMob.eco Mob leads someone to reach out to to make the arrangements. If this is something you are interested in
                    helping with, please <Link to="./contactus">Contact Us</Link> to be added to our directory of sponsors!</p>

        </div>
    );
}

