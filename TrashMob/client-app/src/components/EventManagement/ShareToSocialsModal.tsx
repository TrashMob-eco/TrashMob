import { useState } from "react";
import { Modal, Button, Tooltip, OverlayTrigger } from 'react-bootstrap';
import { Clipboard, GeoAltFill, Clock } from "react-bootstrap-icons";
import { FacebookShareButton, FacebookIcon, TwitterShareButton, TwitterIcon, LinkedinShareButton, LinkedinIcon, WhatsappShareButton, WhatsappIcon, EmailShareButton, EmailIcon } from 'react-share';
import Card from 'react-bootstrap/Card';
import * as ToolTips from '../../store/ToolTips';

interface ModalProps {
	eventToShare?: any;
	show: boolean;
	handleShow: (value: boolean) => void;
	modalTitle: string;
	eventLink?: string;
	message: string;
	emailSubject?: string;
}

export const SocialsModal: React.FC<ModalProps> = (props) => {
	const [copiedLink, setCopied] = useState(false);
	const eventLink = props.eventLink ?? `https://as-tm-dev-westus2.azurewebsites.net/eventdetails/${props.eventToShare.id}`
	const eventDate = props.eventToShare ? new Date(props.eventToShare.eventDate).toLocaleDateString("en-us", { year: "numeric", month: "2-digit", day: "2-digit" }) : ""
	const eventTime = props.eventToShare ? new Date(props.eventToShare.eventDate).toLocaleTimeString("en-us", { hour12: true, hour: 'numeric', minute: '2-digit' }) : ""

	const renderCopyToClipboardToolTip = (
		<Tooltip id="tooltip">
			{ToolTips.CopyToClipboard}
		</Tooltip>
	);

	const handleClose = () => {
		props.handleShow(false);
		setCopied(false);
	}

	const handleCopyLink = () => {
		if (copiedLink) {
			return
		}

		navigator.clipboard.writeText(eventLink);
		setCopied(true);
		setTimeout(() => {
			setCopied(false);
		}, 2000)
	}

	const parseShareMessage = (provider: string) => {

		var TrashMobTag = provider === 'twitter' ? "@TrashMobEco" : "TrashMob.eco"
		return props.message.replace('{{TrashMob}}', TrashMobTag)

	}

	const EventLink = () => {
		return (
			<Card className="pr-2">
				<Card.Body className= 'p-0' style={{ backgroundColor: '#f0f0f1' }}>
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
				<Modal show={props.show} onHide={handleClose} centered>

					<Modal.Header>
						<Modal.Title>{props.modalTitle ?? "Share Event"}</Modal.Title>
					</Modal.Header>
					<Modal.Body className="p-4">
						{props.eventToShare &&
							<div className="d-flex flex-column mb-4">
								<h6> {props.eventToShare.name} </h6>

								<div className="d-flex flex-row align-items-center mb-2">
									<GeoAltFill className="mr-2" />
									{`${props.eventToShare.streetAddress}, ${props.eventToShare.city}, ${props.eventToShare.region}`}
								</div>

								<div className="d-flex flex-row align-items-center">
									<Clock className="mr-2" style={{ fontSize: '14px', marginLeft: '2px' }} />
									{eventDate} at {eventTime}
								</div>

							</div>
						}

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
										title={parseShareMessage("twitter")}
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
										title={parseShareMessage("whatsapp")}
									>
										<WhatsappIcon size={32} round />
									</WhatsappShareButton>
								</div>
								<div className="iconWrapper modalIcon">
									<EmailShareButton
										className={"socials-modal-icon"}
										url={eventLink}
										subject={props.emailSubject ?? `Join me at this TrashMob.eco event in ${props.eventToShare.city}!`}
										body={parseShareMessage("email")}
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