import { Component } from 'react';
import * as React from 'react'
import { Link } from 'react-router-dom';

export class AboutUs extends Component {
    render() {
        return (
            <div>
                <h1>About TrashMob</h1>

                <h2>What is a TrashMob?</h2>
                <p>
                    A TrashMob is a group of citizens who are willing to take a hour or two out of their busy lives to get together and clean up a mess. Friends, family, total strangers, it doesn't matter. If you've ever 
                    said that you want to leave the world a better place than what you found, this is your chance to follow through and actually do something. All it takes is the willingness to get your hands a 
                    little dirty, but that's what gloves are for! It also doesn't matter where you help to clean up. Whether it's your neighborhood, a park, a stream, a road, or even a parking lot of a big box store,
                    all litter needs to be cleaned up, and every bit we pick up, no matter where it is, helps make this planet of ours a little better for the next generation.
                </p> 
                    <h2>Why not do this on my own?</h2>
                <p>                    
                    There's absolutely no reason not to do this on your own. In fact, the inspiration I got for this project came from other doing this on their own.
                </p>
                <p>
                    But doing this with a group brings some sizable benefits with it:
                    <ol>
                        <li>A TrashMob can tackle really bad areas in a very short time. An individual may take days or weeks, and become demoralized</li>
                        <li>A TrashMob can grab the attention of other citizens much more quickly than an individual and can spur even more cleanups</li>
                        <li>A TrashMob can get resources like hauling of the gathered trash, local merchant support, and municipal support that an individual just can't. If you tell your city works department
                        that you are cleaning up a park, they'll be nice and say thanks. But if you say you've got <b>30</b> people showing up at 10AM to clean up a notorious area, they will help you to get
                            the supplies you need, and may even help with the haulaway. Communities want to see active citizens and will do whatever they can to encourage it.</li>
                    </ol>
                </p>

                <h2>Where did the idea come from?</h2>
                <p>
                    Years ago, Scott Hanselman at Microsoft built out the NerdDinner.com site as a demo of the capabilities of ASP.NET MVC. I (<a target="_blank" rel="noopener noreferrer" href="https://twitter.com/joebeernink">Joe Beernink</a>) actually went to a bunch of the nerd dinners in Redmond, WA which were fantastic
                    and had a huge roll in my career, including eventually leading me to join Microsoft. This site is based on both that code and the idea that getting people together to do small good things
                    results in larger good things in the long term.
                </p>

                <p>
                    Fast forward to today, and my passion is fixing problems we have on the planet with pollution and climate change. I've been thinking about what technology can do to help in these areas,
                    without creating more problems. And I keep coming back to the thought that a lot of this is a human problem. People want to help and they want to fix things, but they don't know where to start.
                    Other people have ideas on where to start, but not enough help to get started.
                </p>

                <p>
                    In January 2021, I read about <a target="_blank" rel="noopener noreferrer" href="https://twitter.com/edgarmcgregor">Edgar McGregor</a> in California, who had, at that time, spent over 580 days cleaning up a park in his community, two pails of litter at a time, and I thought,
                    that was a great idea. His actions inspired me to get out and clean up a local park one Saturday. It was fun and rewarding and other people saw what I was doing on my own and thanks me for doing it. It felt great.
                    And then I passed by an area of town that was completely covered in trash and I thought "this is too much for me alone. It would be great to have a group of people descend on this area like a mob and clean
                    it up in an hour or two". And the idea for TrashMob.eco was born.
                </p>

                <p>
                    Basically, TrashMob is the NerdDinner.com site re-purposed to allow people to start TrashMobs of their own to tackle cleanup or <i>whatever</i> needs doing. And we keep coming up with more and more ideas for the site that will
                    allow it to grow organically because of the good that we can do we get together with a common goal!
                </p>

            </div>
        );
    }
}
