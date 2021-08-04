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

            <p>If you are not already registered for the site, please register, and then come back here to become a TrashMob.eco Partner!</p>
            <p><Link to="./becomeapartner">Become a TrashMob.eco Partner</Link>!</p>
        </div>
    );
}
