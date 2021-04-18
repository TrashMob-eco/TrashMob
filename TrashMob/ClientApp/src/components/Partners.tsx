import { Component } from 'react';
import * as React from 'react'
import { Link } from 'react-router-dom';

export class Partners extends Component {
    render() {
        return (
            <div>
                <h1>Partners</h1>

                <h2>What are Partners?</h2>
                <p>Partners are other organizations around the world that we are working with towards a common goal: a better planet for all of us.</p> 
                <p>There isn't just one way to make the planet better.</p> 
                <p>There isn't just one way to clean up litter.</p>
                <p>It's entirely possible that our way isn't even going to work.</p>
                <p>But by forming partnerships, we can share information about what is working, and what is not. TrashMob is attempting to fix the very final stages of the waste management problem... fixing the damage already done. Other 
                    groups are working to prevent waste from being created in the first place, or making sure that what is created, is disposed of in the best possible way for the planet. Others are educating consumers on the choices they
                    make on a daily basis that affect the quality of the world we all live in. TrashMob is just a small piece of the puzzle, and in a lot of cases, we don't even know what the finished puzzle looks like yet.</p>

                <p>We're just starting out, so this page is otherwise empty. But if you've found us, and you want to become a partner, in whatever way that means to you, please <Link to="./contactus">contact us</Link>.</p>
            </div>
        );
    }
}
