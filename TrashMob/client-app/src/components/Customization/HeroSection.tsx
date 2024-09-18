import { FC } from "react";
import { Col, Container, Image, Row } from "react-bootstrap";
// import TrashGlobe from "../assets/gettingStarted/globes.png";
import TrashGlobe from "../assets/trashglobe.png";

interface HeroSectionProps {
  Title: string;
  Description: string;
}

export const HeroSection: FC<HeroSectionProps> = ({ Title, Description }) => {
  return (
    <Container fluid className="bg-grass">
      <div
        className="d-flex flex-column px-0 py-4 pl-lg-5 m-auto"
        style={{ zIndex: 1 }}
      >
        <Container className="d-flex flex-column flex-sm-row-reverse align-content-center">
          <img
            src={TrashGlobe}
            className="h-50 w-50 m-auto"
            alt="Globe"
            style={{ maxHeight: "250px", maxWidth: "250px" }}
          ></img>
          <div
            className="d-flex flex-column justify-content-center w-100 m-auto"
            style={{ maxWidth: "400px" }}
          >
            <h1 className="text-center text-sm-left text-md-left mt-4 ml-md-4 font-weight-bold">
              {Title}
            </h1>
            <p className="text-center text-sm-left mb-4 mb-md-5 ml-md-4 font-weight-bold banner-heading ml-sm-">
              {Description}
            </p>
          </div>
        </Container>
      </div>
    </Container>
  );
};
