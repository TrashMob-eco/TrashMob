import { useState } from "react";
import { Modal, Button, Tooltip, OverlayTrigger } from 'react-bootstrap';
import { Clipboard, GeoAltFill, Clock } from "react-bootstrap-icons";
import { FacebookShareButton, FacebookIcon, TwitterShareButton, TwitterIcon, LinkedinShareButton, LinkedinIcon, WhatsappShareButton, WhatsappIcon, EmailShareButton, EmailIcon } from 'react-share';
import Card from 'react-bootstrap/Card';
import * as ToolTips from '../../store/ToolTips';

interface ModalProps {
  createdEvent: any;
}

export const SocialsModal: React.FC<ModalProps> = (props) => {
  const [show, setShow] = useState(true);
  const [copiedLink, setCopied] = useState(false);
  const eventLink = `${window.location.origin}/eventdetails/${props.createdEvent.id}`
  const eventDate = new Date(props.createdEvent.eventDate).toLocaleDateString("en-us", { year: "numeric", month: "2-digit", day: "2-digit" })
  const eventTime = new Date(props.createdEvent.eventDate).toLocaleTimeString("en-us", { hour12: true, hour: 'numeric', minute: '2-digit' })

  const renderCopyToClipboardToolTip = (
    <Tooltip id="tooltip">
      {ToolTips.CopyToClipboard}
    </Tooltip>
  );

  const handleClose = () => {
    setShow(false);
  }

  const handleCopyLink = () => {
    navigator.clipboard.writeText(eventLink);
    setCopied(true);
    setTimeout(() => {
      setCopied(false);
    }, 2000)
  }

  const getShareMsgContent = (provider: string) => {
    return `Join my next ${provider === 'twitter' ? "@TrashMobEco" : "TrashMob.eco"} event on ${eventDate} at ${eventTime} in ${props.createdEvent.city}.\n` +
      `Sign up using the link for more details. Help me clean up ${props.createdEvent.city}!`
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
              <Card.Link className="text-truncate" href={eventLink} style={{ fontSize: '14px', width: '75%' }}>{eventLink}</Card.Link>
              <OverlayTrigger placement="top" overlay={renderCopyToClipboardToolTip} trigger={['click']} show={copiedLink}>
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
                <Clock className="mr-2" style={{ fontSize: '14px', marginLeft: '2px' }} />
                {eventDate} at {eventTime}
              </div>

            </div>
            <hr />

            Share a link

            <div className="row" id="iconsModalWrapper">
              <div className="d-flex justify-content-between">
                <div className="iconWrapper modalIcon" id="firstWrapper">
                  <FacebookShareButton
                    className={"socials-modal-icon"}
                    url={eventLink}
                    hashtag="#litter"
                  >
                    <FacebookIcon size={32} round />
                  </FacebookShareButton>
                </div>
                <div className="iconWrapper modalIcon">
                  <TwitterShareButton
                    className={"socials-modal-icon"}
                    title={getShareMsgContent("twitter")}
                    hashtags={["litter"]}
                    url={eventLink}
                    via="TrashMobEco"
                  >
                    <TwitterIcon size={32} round />
                  </TwitterShareButton>
                </div>
                {/* missing: instagram integration */}
                <div className="iconWrapper modalIcon">
                  <LinkedinShareButton
                    className={"socials-modal-icon"}
                    url={eventLink}
                  >
                    <LinkedinIcon size={32} round />
                  </LinkedinShareButton>
                </div>
                <div className="iconWrapper modalIcon">
                  <WhatsappShareButton
                    className={"socials-modal-icon"}
                    url={eventLink}
                    title={getShareMsgContent("whatsapp")}
                  >
                    <WhatsappIcon size={32} round />
                  </WhatsappShareButton>
                </div>
                <div className="iconWrapper modalIcon">
                  <EmailShareButton
                    className={"socials-modal-icon"}
                    url={eventLink}
                    subject={`Join my next TrashMob.eco event on ${eventDate} in ${props.createdEvent.city}!`}
                    body={getShareMsgContent("email")}
                  >
                    <EmailIcon size={32} round />
                  </EmailShareButton>
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