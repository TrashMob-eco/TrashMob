import { FC, useEffect, useState } from "react";
import { Link, RouteComponentProps, withRouter } from "react-router-dom";
import UserData from "../Models/UserData";
import { Col, Container, Row } from "react-bootstrap";
import Globe2 from "../assets/globe2.png";
import Logo from "../assets/logo.svg";
import Calendar from "../assets/home/Calendar.svg";
import Trashbag from "../assets/home/Trashbag.svg";
import Person from "../assets/home/Person.svg";
import Clock from "../assets/home/Clock.svg";
import { GettingStartedSection } from "../GettingStartedSection";
import { Share } from "react-bootstrap-icons";
import { SocialsModal } from "../EventManagement/ShareToSocialsModal";
import * as SharingMessages from "../../store/SharingMessages";
import { EventsSection } from "../EventsSection";
import { Button } from "reactstrap";
import { GetStats } from "../../services/stats";
import { useQuery } from "@tanstack/react-query";
import { Services } from "../../config/services.config";
import StatsData from "../Models/StatsData";

export interface HomeProps extends RouteComponentProps<any> {
  isUserLoaded: boolean;
  currentUser: UserData;
  onUserUpdated: any;
}

const Home: FC<HomeProps> = ({
  isUserLoaded,
  currentUser,
  history,
  location,
  match,
}) => {
  const [showModal, setShowSocialsModal] = useState<boolean>(false);
  const [stats, setStats] = useState<StatsData | null>(null);

  const handleShowModal = (showModal: boolean) =>
    setShowSocialsModal(showModal);
  const getStats = useQuery({
    queryKey: GetStats().key,
    queryFn: GetStats().service,
    staleTime: Services.CACHE.DISABLE,
    enabled: false,
  });

  useEffect(() => {
    window.scrollTo(0, 0);
    getStats.refetch().then((res) => {
      setStats(res.data?.data || null);
    });
  }, [isUserLoaded, currentUser]);

  return (
    <>
      <Container fluid>
        <SocialsModal
          show={showModal}
          handleShow={handleShowModal}
          modalTitle="Invite a friend to join TrashMob.eco"
          eventLink="https://www.trashmob.eco"
          emailSubject="Join TrashMob.eco to help clean up the planet!"
          message={SharingMessages.InvitationMessage}
        />
        <Row className="shadow position-relative">
          <Col
            className="d-flex flex-column px-0 py-4 pl-lg-5"
            sm={6}
            style={{ zIndex: 1 }}
          >
            <div className="ml-sm-2 ml-lg-5 pl-sm-3 pl-md-5 mt-md-5 mb-md-2">
              <img
                src={Logo}
                alt="TrashMob.eco logo"
                className="banner-logo"
              ></img>
              <h3 className="ml-md-4 mt-4 mb-4 mb-md-5 font-weight-bold font-size-xl banner-heading pl-3">
                Meet up. Clean up. Feel good.
              </h3>
              <Link
                className="btn btn-primary ml-5 py-md-3 banner-button"
                to="/gettingstarted"
              >
                Join us today
              </Link>
              <Button
                className="btn btn-primary ml-5 py-md-3 banner-button"
                onClick={() => handleShowModal(true)}
              >
                <Share className="mr-2" />
                Invite a friend
              </Button>
            </div>
          </Col>
          <img
            src={Globe2}
            className="position-absolute p-0 m-0 h-100 banner-globe"
            alt="Globe"
          ></img>
        </Row>
      </Container>
      <div className="bg-white">
        <Container>
          <Row className="py-4 px-2">
            <Col
              lg={6}
              className="order-2 order-lg-1 d-flex flex-column text-center"
            >
              <h1 className="d-md-none d-lg-block text-md-left mt-4 font-weight-bold">
                What is a TrashMob?
              </h1>
              <h4 className="text-md-left">
                A TrashMob is a group of citizens who are willing to take an
                hour or two out of their lives to get together and clean up
                their communities. Start your impact today.
              </h4>

              <div className="d-flex flex-column flex-md-row">
                <Link
                  className="mx-auto mr-md-2 ml-md-0 my-2 btn btn-primary btn-128 h-auto w-auto"
                  to="/aboutus"
                  role="button"
                >
                  Learn more
                </Link>
                <Link
                  className="mx-auto ml-md-0 my-2 btn btn-primary btn-128 w-auto"
                  to="/events"
                  role="button"
                >
                  View Upcoming Events
                </Link>
              </div>
            </Col>
            <Col lg={6} className="order-1 order-lg-2 align-content-center">
              <h1 className="d-none d-md-block d-lg-none my-4 font-weight-bold">
                What is a TrashMob?
              </h1>
              <div className="embed-responsive embed-responsive-16by9">
                <iframe
                  className="embed-responsive-item"
                  width="560"
                  height="315"
                  src="https://www.youtube.com/embed/ylOBeVHRtuM"
                  title="YouTube video player"
                  allow="accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture; web-share"
                  allowFullScreen
                ></iframe>
              </div>
            </Col>
          </Row>
        </Container>
      </div>
      <div className="d-flex justify-content-center m-4">
        <div className="row row-cols-2 row-cols-sm-4 justify-content-center p-2">
          {[
            {
              id: 0,
              title: "Events Hosted",
              value: stats?.totalEvents || 0,
              icon: Calendar,
              alt: "Calendar icon",
            },
            {
              id: 1,
              title: "Bags Collected",
              value: stats?.totalBags || 0,
              icon: Trashbag,
              alt: "Trashbag icon",
            },
            {
              id: 2,
              title: "Participants",
              value: stats?.totalParticipants || 0,
              icon: Person,
              alt: "Person icon",
            },
            {
              id: 3,
              title: "Hours Spent",
              value: stats?.totalHours || 0,
              icon: Clock,
              alt: "Clock icon",
            },
          ].map((item) => (
            <div
              key={item.id}
              className="col d-flex flex-column bg-light rounded-lg p-4 shadow"
            >
              <img src={item.icon} alt={item.alt} className="w-25 mb-3" />
              <span className="font-weight-bold font-size-lg">
                {item.value}
              </span>
              <span className="font-weight-bold">{item.title}</span>
            </div>
          ))}
        </div>
      </div>

      <EventsSection
        currentUser={currentUser}
        isUserLoaded={isUserLoaded}
        history={history}
        location={location}
        match={match}
      ></EventsSection>
      <GettingStartedSection />
    </>
  );
};

export default withRouter(Home);
