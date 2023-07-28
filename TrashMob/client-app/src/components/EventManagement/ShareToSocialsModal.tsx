import { useState } from "react";
import { Modal, Button, Tooltip, OverlayTrigger } from 'react-bootstrap';
import { Clipboard, GeoAltFill, Clock } from "react-bootstrap-icons";
import Card from 'react-bootstrap/Card';


interface ModalProps {
  createdEvent: any;
}

export const SocialsModal: React.FC<ModalProps> = (props) => {
  const [show, setShow] = useState(true);
  const [copiedLink, setCopied] = useState(false);

  const tooltip = (
    <Tooltip id="tooltip">
      Copied to clipboard!
    </Tooltip>
  );

  const handleClose = () => {
    setShow(false);
  }

  const handleCopyLink = () => {
    navigator.clipboard.writeText(window.location.origin + '/eventdetails/' + props.createdEvent.id);
    setCopied(true);
    setTimeout(() => {
      setCopied(false);
    }, 2000)
  }

  const EventLink = () => {
    const eventLink = `${window.location.origin}/eventdetails/${props.createdEvent.id}`
    return (
      <Card className="pr-2">
        <Card.Body style={{ padding: '0px', backgroundColor: '#f0f0f1' }}>
          <div className="ml-2 px-2 py-1">
            <Card.Text className="mb-0 text-muted" style={{ fontSize: '12px' }}>
              Event link
            </Card.Text>
            <div className="d-flex align-items-center justify-content-between">
              <Card.Link className="text-truncate" href={eventLink} style={{ fontSize: '14px', width:'75%' }}>{eventLink}</Card.Link>
              <OverlayTrigger placement="top" overlay={tooltip} trigger={['click']} show={copiedLink}>
                <Button className="btn mb-2" id="helpBtn" onClick={handleCopyLink}><Clipboard /></Button>
              </OverlayTrigger>
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
              <h6> {props.createdEvent.name} </h6>

              <div className="d-flex flex-row align-items-center mb-2">
                <GeoAltFill className="mr-2" />
                {`${props.createdEvent.streetAddress}, ${props.createdEvent.city}, ${props.createdEvent.region}`}
              </div>

              <div className="d-flex flex-row align-items-center">
                <Clock className="mr-2" style={{ fontSize: '14px' }} />
                {new Date(props.createdEvent.eventDate).toLocaleDateString("en-us", {
                  year: "numeric",
                  month: "2-digit",
                  day: "2-digit"
                })} {new Date(props.createdEvent.eventDate).toLocaleTimeString("en-us", { hour12: true, hour: 'numeric', minute: '2-digit' })}
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