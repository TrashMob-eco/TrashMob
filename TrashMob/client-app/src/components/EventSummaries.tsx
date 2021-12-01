import * as React from 'react'
import { Col, Form } from 'react-bootstrap';
import { CountryDropdown, RegionDropdown } from 'react-country-region-selector';
import { getDefaultHeaders } from '../store/AuthStore';
import { getEventType } from '../store/eventTypeHelper';
import DisplayEventSummary from './Models/DisplayEventSummary';
import EventTypeData from './Models/EventTypeData';

export const EventSummaries: React.FC = () => {
    const [displaySummaries, setDisplaySummaries] = React.useState<DisplayEventSummary[]>([]);
    const [eventTypeList, setEventTypeList] = React.useState<EventTypeData[]>([]);
    const [isEventSummaryDataLoaded, setIsEventSummaryDataLoaded] = React.useState<boolean>(false);
    const [city, setCity] = React.useState<string>("");
    const [country, setCountry] = React.useState<string>("");
    const [region, setRegion] = React.useState<string>("");
    const [postalCode, setPostalCode] = React.useState<string>("");

    React.useEffect(() => {
        const headers = getDefaultHeaders('GET');

        fetch('/api/eventtypes', {
            method: 'GET',
            headers: headers,
        })
            .then(response => response.json() as Promise<Array<any>>)
            .then(data => {
                setEventTypeList(data);
            })
    }, [])

    React.useEffect(() => {
        const headers = getDefaultHeaders('GET');
        fetch('/api/eventsummaries?country=' + country + '&region=' + region + '&city=' + city + '&postalCode=' + postalCode, {
            method: 'GET',
            headers: headers,
        })
            .then(response => response.json() as Promise<DisplayEventSummary[]>)
            .then(data => {
                setDisplaySummaries(data);
                setIsEventSummaryDataLoaded(true);
            });
    }, [country, region, city, postalCode])

    function handleCityChanged(val: string) {
        setCity(val);
    }

    function selectCountry(val: string) {
        setCountry(val);
    }

    function selectRegion(val: string) {
        setRegion(val);
    }

    function handlePostalCodeChanged(val: string) {
        setPostalCode(val);
    }

    // Returns the HTML Form to the render() method.  
    function renderSearchForm() {
        return (
            <div className="container-fluid" >
                <Form>
                    <Form.Row>
                        <Col>
                            <Form.Group>
                                <Form.Label htmlFor="Country">Country:</Form.Label>
                                <div>
                                    <CountryDropdown name="country" value={country ?? ""} onChange={(val) => selectCountry(val)} />
                                </div>
                            </Form.Group>
                        </Col>
                        <Col>
                            <Form.Group>
                                <Form.Label htmlFor="Region">Region:</Form.Label>
                                <div>
                                    <RegionDropdown
                                        country={country ?? ""}
                                        value={region ?? ""}
                                        onChange={(val) => selectRegion(val)} />
                                </div>
                            </Form.Group>
                        </Col>
                        <Col>
                            <Form.Group>
                                <Form.Label htmlFor="City">City:</Form.Label>
                                <Form.Control type="text" name="city" value={city} onChange={(val) => handleCityChanged(val.target.value)} maxLength={parseInt('256')} required />
                            </Form.Group>
                        </Col>
                        <Col>
                            <Form.Group>
                                <Form.Label htmlFor="PostalCode">Postal Code:</Form.Label>
                                <Form.Control type="text" name="postalCode" value={postalCode} onChange={(val) => handlePostalCodeChanged(val.target.value)} maxLength={parseInt('25')} />
                            </Form.Group>
                        </Col>

                    </Form.Row>
                </Form>
            </div>
        )
    }

    function renderEventSummariesTable(events: DisplayEventSummary[]) {
        return (
            <div>
                <table className='table table-striped' aria-labelledby="tableLabel" width='100%'>
                    <thead>
                        <tr>
                            <th>Name</th>
                            <th>Date</th>
                            <th>Event Type</th>
                            <th>Address</th>
                            <th>City</th>
                            <th>Region</th>
                            <th>Country</th>
                            <th>Postal Code</th>
                            <th>Attendees</th>
                            <th>Number of Bags</th>
                            <th>Duration in Minutes</th>
                            <th>Total Work Hours</th>
                        </tr>
                    </thead>
                    <tbody>
                        {events.sort((a, b) => (a.eventDate < b.eventDate) ? 1 : -1).map(mobEvent =>
                            <tr key={mobEvent.id}>
                                <td>{mobEvent.name}</td>
                                <td>{new Date(mobEvent.eventDate).toLocaleDateString("en-US", { month: "long", day: "numeric", year: 'numeric', hour: 'numeric', minute: 'numeric' })}</td>
                                <td>{getEventType(eventTypeList, mobEvent.eventTypeId)}</td>
                                <td>{mobEvent.streetAddress}</td>
                                <td>{mobEvent.city}</td>
                                <td>{mobEvent.region}</td>
                                <td>{mobEvent.country}</td>
                                <td>{mobEvent.postalCode}</td>
                                <td>{mobEvent.actualNumberOfAttendees}</td>
                                <td>{Math.round(mobEvent.numberOfBags * 100) / 100}</td>
                                <td>{mobEvent.durationInMinutes}</td>
                                <td>{Math.round(mobEvent.totalWorkHours * 100) / 100}</td>
                            </tr>
                        )}
                    </tbody>
                </table>
            </div>
        );
    }

    return (
        <>
            <div>
                <h1>Summary of Events</h1>
                {renderSearchForm()}
                {isEventSummaryDataLoaded && renderEventSummariesTable(displaySummaries)}
            </div>
        </>
    );
}