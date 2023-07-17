import { useState } from "react";
import { Modal, Button } from 'react-bootstrap';
import { Clipboard, GeoAltFill, Clock } from "react-bootstrap-icons";
import Card from 'react-bootstrap/Card';


interface ModalProps {
  createdEventId: string;
  history: any;
}

export const SocialsModal: React.FC<ModalProps> = (props) => {
  const [show, setShow] = useState(true);

  const handleClose = () => {

    setShow(false);
    props.history.push("/mydashboard");
  }

  const EventLink = () => {
    return (
      <Card className="pr-2">
        <Card.Body style={{ padding: '0px', backgroundColor: '#f0f0f1' }}>
          <div className="ml-2 px-2 py-1">
            <Card.Text className="mb-0 text-muted" style={{ fontSize: '12px' }}>
              Event link
            </Card.Text>
            <div className="d-flex align-items-center justify-content-between">
              <Card.Link href="#" style={{ fontSize: '14px' }}>Card Link</Card.Link>
              <Button className="btn mb-2" href="/" id="helpBtn"><Clipboard /></Button>
            </div>
          </div>
        </Card.Body>
      </Card>
    );
  }

  return (
    <>
      <div className="modal-header border-0">
        <Modal show={show} onHide={handleClose} centered>

          <Modal.Header>
            <Modal.Title>Event created</Modal.Title>
          </Modal.Header>
          <Modal.Body className="p-4">
            <div className="d-flex flex-column mb-4">
              <h6> Event Title </h6>

              <div className="d-flex flex-row align-items-center">
                <GeoAltFill className="mr-2" />
                location
              </div>

              <div className="d-flex flex-row align-items-center">
                <Clock className="mr-2" style={{ fontSize: '14px' }} />
                date / time
              </div>

            </div>
            <hr />

            Share a link

            <div className="row" id="iconsModalWrapper">
              <div className="d-flex justify-content-between">
                <div className="iconWrapper" id="firstWrapper">
                  <a href={"https://www.facebook.com/trashmob.eco/"} target="_blank" rel="noreferrer noopener">
                    <i className="fab fa-facebook socials-modal-icon"></i>
                  </a>
                </div>
                <div className="iconWrapper">
                  <a href={"https://twitter.com/TrashMobEco"} target="_blank" rel="noreferrer noopener">
                    <i className="fab fa-twitter socials-modal-icon"></i>
                  </a>
                </div>
                <div className="iconWrapper">
                  <a href={"https://www.instagram.com/trashmobinfo"} target="_blank" rel="noreferrer noopener">
                    <i className="fa-brands fa-instagram socials-modal-icon"></i>
                  </a>
                </div>
                <div className="iconWrapper">
                  <a href={"https://www.linkedin.com/company/76188984"} target="_blank" rel="noreferrer noopener">
                    <i className="fa-brands fa-linkedin socials-modal-icon"></i>
                  </a>
                </div>
              </div>
            </div>

            <EventLink />
          </Modal.Body>
          <Modal.Footer>
            <Button variant="secondary" onClick={handleClose}>
              Close
            </Button>
          </Modal.Footer>
        </Modal>
      </div>
    </>
  );
}