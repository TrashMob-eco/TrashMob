import * as React from 'react';
import workgloves from './assets/lance-grandahl-Z2JPWzWGih4-unsplash.jpg';
import highways from './assets/jared-murray-NSuufgf-BME-unsplash.jpg';
import joinmob from './assets/paige-cody-G8VOA-BrnHo-unsplash.jpg';

import { Link } from 'react-router-dom';

export const GettingStarted: React.FC = () => {
    return (
        <div>
            <h1>Getting Started</h1>

            <h2>The Basics</h2>
            <p>There are really only three things you absolutely need to have to get started working with a TrashMob:</p>
            <ol>
                <li>Work gloves</li>
                <li>A bucket</li>
                <li>A good attitude</li>
            </ol>
            <img src={workgloves} alt="Work gloves" />
            <p>
                Really, that's all you need. A single trip to the hardware store, and you're ready to go. For the gloves, we recommend
                    <Link to="https://www.amazon.com/3100L-DZ-Gloves-Textured-Construction-12-Pairs/dp/B001YJHEDW/ref=sr_1_9?dchild=1&keywords=work+gloves&qid=1617415033&sr=8-9">Rubber Latex Double coated work gloves</Link>. There's a good chance you will find glass or something sharp out there, and these gloves will offer some protection from that. It's also likely that either the outside of the trash will be wet, or there will be something inside
                    the trash that is wet or icky. Get the gloves.
                </p>
            <p>
                As for the bucket, any 5 gallon pail will do. If you don't want to buy one, there are lots out there in the restaurant and construction industries that you can up-cycle. And while you can use a plastic bag, we've found that we spend a
                lot of time in the bushes, and a plastic bag gets caught on branches and will tear. Use the bucket for gathering, then transfer to the garbage bags for loading/hauling.
                </p>
            <p>
                Your attitude is just as important as your tools. A positive attitude increases the chances that other people will join you and stay longer, and decreases the likelyhood of people littering in the first place.
                </p>
            <h2>What to Wear</h2>
            <p>What you wear in a TrashMob isn't just about fashion, though I can understand why you may think it is. Wear clothes you won't mind getting dirty or possibly torn. This is also not the time for shorts and a t-shirt, unless you're strictly doing an urban pickup.
            A long sleeve shirt keeps the branches and thorns off. Jeans work best to shed prickers and mud. Most importantly, good footwear is key: hiking boots with thick soles a little water resistance are best; old running shoes you aren't afraid to get dirty are next up.
            If you come out in sandals, you'll probably regret it.
                </p>
            <p>
                If you are working along a roadside, we <b>strongly urge you</b> to get a <Link to="https://www.amazon.com/gp/product/B01G5WNO2M/ref=ppx_yo_dt_b_asin_title_o02_s01?ie=UTF8&psc=1">reflective vest</Link>. They can save your life.
                    </p>
            <h2>What About Those Grabber Things?</h2>
            <p>You'll see a lot of people using grabbers to pick litter. They save a ton of bending over when picking, and really facilitate getting small pieces of plastic off the ground, or trash out of a bush. They aren't essential, but we always carry one when we're gathering, and it
                allows us to last a lot longer, as the constant bending-over can be exhausting. That said, there are few things to keep in mind when buying and using one:</p>
            <ol>
                <li>You get what you pay for. We've tried the $10 ones and they work, but they don't last long. You'll want to treat them very gently. They're not meant to be used as a cane, or as a prybar, or to pick up bricks with.</li>
                <li>Get one with a pistol grip. You may squeeze that trigger two hundred times, and ergonomics are your friend.</li>
                <li>We're constantly on the lookout for new and better ones, but so far, the one we've had the most success with is the <Link to="https://www.amazon.com/gp/product/B01G5WNO2M/ref=ppx_yo_dt_b_asin_title_o02_s01?ie=UTF8&psc=1">Unger Grabber Plus Reacher</Link>. It's a little heavier than the
                            cheap ones, but so far so good with it. If you find a better one, please let us know.</li>
            </ol>
            <h2>What about the Highways?</h2>
            <p>In America, the highways are nortorious for litter. People tend to throw trash out their windows in areas they don't live near, and unsecured loads tend to fly out of trucks at highway speeds. Our highways can look like thousand mile long
                    trash dumps.</p>
            <p>But they are also, undoubtedly, the most dangerous places to pick up trash. With vehicles racing by at 75 MPH, driven by someone reading a text or making a phone call or eating lunch, the difference between life an death is a fraction of
            a second. Because of this danger, most states have formed Adopt-a-Highway programs which provide guidance, training, and safety equipment for those who want to work these tough environments. Please check with your Department of Transportation
                    before creating a highway cleanup event, and follow all of their guidance. <b>Safety is the number one priority</b>. If you aren't able to follow their guidelines, please choose a different type of cleanup to do.
                    </p>
            <img src={highways} alt="Highways" />
            <h2>Joining a TrashMob</h2>
            <p>A couple of tips to make your first TrashMob a success:</p>
            <ol>
                <li>Stay local. Take a walk to your nearest park, even if it is near pristine, and pick up what you can. Everything matters. If you are driving an hour to an event, you're likely burning more energy to get there than is needed, and passing by lots of opportunities closer to home. That's
                        not to say that destination events aren't worthwhile -- they are -- but start local and work with your own community first.</li>
                <li>Start with a park-based event. Starting at a park takes some of the biggest challenges out of your way right at the start:
                        <ol>
                        <li>Parks are pretty serene places, so you don't have to worry about traffic zooming by.</li>
                        <li>Parks usually have garbage cans nearby to put the litter in so you don't have to haul it away.</li>
                        <li>Other people will see you picking up trash. This is <i>so</i> important. When someone sees another person picking up trash, they are much less likely to litter in the future, especially kids! People will also thank you for doing it, and that feels awesome.
                                And some people will, because they saw you doing it, start doing it themselves. This is so crucial to our mission. The more people we can have picking up litter, the more this effort goes viral.</li>
                        <li>Parks usually have parking nearby. Parking for other types of events can be a real challenge.</li>
                    </ol>
                </li>
                <li>Recruit one friend or family member to join you. Doing this with builds a greater feeling of enjoyment and achievement, and you're more likely to want to do it again.</li>
                <li>Set a goal for how much trash you want to pick up. We aim for 2 buckets per person. Depending on the density of the area you are working, that may take an hour, or may take 5 minutes. But don't set out to clean an entire state park with 1-2 people in one day. Start small.</li>
                <li>Most of all, <b>Be safe!</b> No piece of litter is worth risking your health or well-being.</li>
            </ol>
            <img src={joinmob} alt="Woman picking trash" />
        </div>
    );
}

