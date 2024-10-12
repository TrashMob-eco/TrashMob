import { useGetEventType } from "../../../hooks/useGetEventType"
import EventData from "../../Models/EventData"

type EventDetailInfoWindowContentProps = EventData & {
	hideTitle?: boolean
}

export const EventDetailInfoWindowHeader = (props: EventDetailInfoWindowContentProps) => 
	<h5 className='mt-1 font-weight-bold' style={{ fontFamily: 'Poppins' }}>{props.name}</h5>

export const EventDetailInfoWindowContent = (props: EventDetailInfoWindowContentProps) => {
	const { id, name, eventTypeId, eventDate, city, region, country, postalCode, createdByUserName, hideTitle = false } = props

	const { data: eventType } = useGetEventType(eventTypeId)

	const date = new Date(eventDate).toLocaleDateString([], {
			month: 'long',
			day: '2-digit',
			year: 'numeric',
	});
	const time = new Date(eventDate).toLocaleTimeString([], {
			timeZoneName: 'short',
	});

	return (
		<div style={{ width: 500, overflowX: 'auto' }}>
			<div>
				{!hideTitle && <h5 className='mt-1 font-weight-bold'>{name}</h5>}
				<p className='my-3 event-list-event-type p-2 rounded'>{eventType?.name}</p>
				<p className='m-0'>
					{date},{time}
				</p>
				<p className='m-0'>
					{city ? `${city},` : ''} {region ? `${region},` : ''} {country ? `${country},` : ''}{' '}
					{postalCode || ''}
				</p>
			</div>
			<div className='d-flex justify-content-between mt-2'>
				<span className='align-self-end'>
					Created by
					{createdByUserName}
				</span>
				<button className='btn btn-outline mr-0'>
					<a id='viewDetails' type='button' href={`/eventdetails/${id}`}>
						View Details
					</a>
				</button>
				{/* Todo: RegisterBtn */}
			</div>
		</div>
  	)
}