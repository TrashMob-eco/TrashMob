import * as React from "react";
import { Link } from "react-router-dom";
import trashbin from "../assets/the-blowup-t06aN6vewaQ-unsplash1.png";
import trashcans from "../assets/trashcan-artwork.png";
import { GettingStartedSection } from "../GettingStartedSection";
import { Col, Container, Image, Row } from "react-bootstrap";

export const AboutUs: React.FC = () => {
  React.useEffect(() => {
    window.scrollTo(0, 0);
  });

  return (
    <>
      <Container fluid className="mt-1 bg-white p-5">
        <Row className="mb-4 px-md-5" xs={1} lg={2}>
          <Col className="mb-5 pl-lg-5">
            <div className="px-lg-5">
              <h2>What is a TrashMob?</h2>
              <p className="font-weight-bold">
                A TrashMob is a group of citizens who are willing to take an
                hour or two out of their lives to get together and clean up
                their communities.
              </p>
              <p>
                Whether the motivation is to better your local community,
                connect with like-minded individuals, or improve your own mental
                health and wellbeing, TrashMob provides an avenue for
                accomplishing these goals. To participate, all it takes is the
                willingness to get your hands a little dirty and a desire to
                leave the world better than how you found it. Whether it's your
                neighborhood, a park, a stream, a road, or even a parking lot of
                a big box store, all litter being cleaned up contributes to our
                goal of making this planet of ours a little better for the next
                generation.
              </p>
              <Link className="mt-2 btn btn-primary" to="/" role="button">
                Find an event
              </Link>
            </div>
          </Col>
          <Col className="mt-3 d-flex justify-content-center">
            <Image src={trashcans} alt="trash cans" className="h-75" />
          </Col>
        </Row>
      </Container>
      <Container fluid className="px-0">
        <div
          className="w-100 d-flex align-items-center"
          style={{
            backgroundImage: `url(${trashbin})`,
            backgroundRepeat: "no-repeat",
            backgroundSize: "cover",
            backgroundPosition: "center",
          }}
        >
          <Col xs={11} lg={8} className="px-0 mx-auto">
            <div
              className="text-white bg-black border-rounded-lg my-5 mx-auto"
              style={{ opacity: 0.95 }}
            >
              <div className="p-4 p-sm-5">
                <h1 className="font-weight-bold text-center">
                  Benefits of joining TrashMob
                </h1>
                <ol className="list-unstyled mt-5 p-sm-5">
                  <li className="mb-4">
                    <div className="d-flex align-top">
                      <span className="mr-3 font-weight-bold font-size-lg">
                        1
                      </span>
                      <span>
                        TrashMobs allow you to connect with your local community
                        and foster relationships built on positive changes.
                      </span>
                    </div>
                  </li>
                  <li className="mb-4">
                    <div className="d-flex align-top">
                      <span className="mr-3 font-weight-bold font-size-lg">
                        2
                      </span>
                      <span>
                        TrashMobs clean up our parks, streams, and neighborhoods
                        which benefits our Earth and our communities.
                      </span>
                    </div>
                  </li>
                  <li className="mb-4">
                    <div className="d-flex align-top">
                      <span className="mr-3 font-weight-bold font-size-lg">
                        3
                      </span>
                      <span>
                        A TrashMob can tackle highly polluted areas in a shorter
                        time than individuals which improves morale and
                        satisfaction.
                      </span>
                    </div>
                  </li>
                  <li className="mb-4">
                    <div className="d-flex align-top">
                      <span className="mr-3 font-weight-bold font-size-lg">
                        4
                      </span>
                      <span>
                        TrashMobs can garner attention from neighbors, friends,
                        and local governments that can spur on more cleanups.
                      </span>
                    </div>
                  </li>
                  <li className="mb-4">
                    <div className="d-flex align-top">
                      <span className="mr-3 font-weight-bold font-size-lg">
                        5
                      </span>
                      <span>
                        TrashMobs can acquire municipal support which can help
                        with the hauling of gathered trash and providing
                        supplies.
                      </span>
                    </div>
                  </li>
                </ol>
              </div>
            </div>
          </Col>
        </div>
      </Container>
      <Container fluid className="bg-white pb-5">
        <Row className="text-center justify-content-center py-5">
          <h2 className="font-weight-bold">The Journey</h2>
        </Row>
        <Row>
          <Col>
            <div
              className="timeline-steps aos-init aos-animate"
              data-aos="fade-up"
            >
              <div className="timeline-step">
                <div
                  className="timeline-content"
                  data-toggle="popover"
                  data-trigger="hover"
                  data-placement="top"
                  title=""
                  data-content="And here's some amazing content. It's very engaging. Right?"
                  data-original-title="2003"
                >
                  <div className="inner-circle">
                    <span className="inner-circle-text">01</span>
                  </div>
                  <h6 className="mt-3 mb-1 font-weight-bold">
                    The Inspiration That Flourished
                  </h6>
                  <p className="mb-0 mb-lg-0">
                    TrashMob founder, Joe Beernink, first gained inspiration for
                    TrashMob from Microsoft colleagues. An idea formed for a
                    project that would bring people together to take small,
                    positive actions that would cascade into meaningful long
                    term effects.
                  </p>
                </div>
              </div>
              <div className="timeline-step">
                <div
                  className="timeline-content"
                  data-toggle="popover"
                  data-trigger="hover"
                  data-placement="top"
                  title=""
                  data-content="And here's some amazing content. It's very engaging. Right?"
                  data-original-title="2004"
                >
                  <div className="inner-circle">
                    <span className="inner-circle-text">02</span>
                  </div>
                  <h6 className="mt-3 mb-1 font-weight-bold">Starting Out</h6>
                  <p className="mb-0 mb-lg-0">
                    Joe, passionate about pollution and climate change, took
                    inspiration from Edgar McGregor in California who spent over
                    580 days cleaning up a park in his community. He began
                    cleaning up his own local parks, and others soon began to
                    take notice.
                  </p>
                </div>
              </div>
              <div className="timeline-step">
                <div
                  className="timeline-content"
                  data-toggle="popover"
                  data-trigger="hover"
                  data-placement="top"
                  title=""
                  data-content="And here's some amazing content. It's very engaging. Right?"
                  data-original-title="2005"
                >
                  <div className="inner-circle">
                    <span className="inner-circle-text">03</span>
                  </div>
                  <h6 className="mt-3 mb-1 font-weight-bold">
                    Connecting Others
                  </h6>
                  <p className="mb-0 mb-lg-0">
                    After realizing cleaning up would be too much to do on his
                    own, Joe saw potential in connecting others. He knew that
                    technology had potential to fix these human problems, and
                    assembled a team to help bring his vision to life.
                  </p>
                </div>
              </div>
              <div className="timeline-step">
                <div
                  className="timeline-content"
                  data-toggle="popover"
                  data-trigger="hover"
                  data-placement="top"
                  title=""
                  data-content="And here's some amazing content. It's very engaging. Right?"
                  data-original-title="2010"
                >
                  <div className="inner-circle">
                    <span className="inner-circle-text">04</span>
                  </div>
                  <h6 className="mt-3 mb-1 font-weight-bold">
                    TrashMob was Born
                  </h6>
                  <p className="mb-0 mb-lg-0">
                    Today, TrashMob has provided communities the opportunity to
                    create and participate in TrashMobs of their own. The
                    TrashMob team is continuously coming up with more ways to
                    grow the TrashMob community to achieve the common goal of
                    bettering our community!
                  </p>
                </div>
              </div>
            </div>
          </Col>
        </Row>
      </Container>
      <GettingStartedSection />
    </>
  );
};
