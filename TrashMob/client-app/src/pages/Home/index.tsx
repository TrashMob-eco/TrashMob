import { Link } from "react-router-dom";
import { Button } from '@/components/ui/button';
import { StatsSection } from "./StatsSection";
import EventSection from "./EventSection";
import UserData from "@/components/Models/UserData";

interface HomeProps {
  isUserLoaded: boolean
  currentUser: UserData
}

export const Home = (props: HomeProps) => {
  return (
    <div className="tailwind">
      <section id="hero-section" className={`bg-[url('/img/globe2-minified.png')] bg-[position:105%_0%] bg-no-repeat bg-contain`}>
        <div className="container py-32 lg:min-h-[50vh]">
          <div className="w-[600px] max-w-full">
            <div className="w-full my-16">
              <img src="/img/logo-with-tagline.svg" alt="Trashmob Logo with tagline" />
            </div>
            <Button>Join us today</Button>
            <div className="flex flex-row gap-1 items-center mt-16">
              <a href="#" className="">
                <img 
                  className="android mt-0 -ml-2 h-14" 
                  alt="Get it on Google Play" 
                  src="https://play.google.com/intl/en_us/badges/images/generic/en_badge_web_generic.png" />
              </a>
              
              <a href="https://apps.apple.com/us/app/trashmob/id1599996743?itscg=30200&itsct=apps_box_badge&mttnsubad=1599996743" style={{ display: "inline-block" }}>
                <img
                  src="https://toolbox.marketingtools.apple.com/api/v2/badges/download-on-the-app-store/black/en-us?releaseDate=1682899200"
                  alt="Download on the App Store"
                  className="m-0 h-10"
                />
              </a>
            </div>
          </div>
        </div>
      </section>
      <section id="introduction" className="bg-card">
        <div className="mx-auto max-w-screen-lg py-8">
          <div className="px-8 flex flex-col items-center md:grid md:auto-rows-auto md:grid-cols-5 md:gap-x-4">
            <div className="col-span-3">
              <h3 className="!mt-0 !mb-8 md:!mb-0">
                What is a TrashMob?
              </h3>
            </div>
            <div className="row-span-4 col-start-4 col-span-2">
              <img src="/img/jeremy-bezanger-u5mCQ-c5oSI-unsplash.jpg" alt="What is Trashmob" className="!mt-0" />
            </div>
            <div className="row-span-3 col-start-1 col-span-3">
              <p className="!mt-8 md:!mt-0 !mb-16 text-center md:!text-left">
                A TrashMob is a group of citizens who are willing to take an hour or two out of their
                lives to get together and clean up their communities. Start your impact today.
              </p>
              <div className="flex flex-row justify-center md:justify-start gap-4">
                <Button asChild>
                  <Link to="/aboutus">Learn more</Link>
                </Button>
                <Button asChild>
                  <a href='/#events'>View Upcoming Events</a>
                </Button>
              </div>
            </div>
          </div>
        </div>
      </section>
      <StatsSection />
      <EventSection currentUser={props.currentUser} isUserLoaded={props.isUserLoaded} />
    </div>
  )
}