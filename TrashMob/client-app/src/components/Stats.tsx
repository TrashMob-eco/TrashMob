import * as React from 'react'
import { getDefaultHeaders } from '../store/AuthStore';
import StatsData from './Models/StatsData';

export const Stats: React.FC = () => {
    const [totalBags, setTotalBags] = React.useState<number>(0);
    const [totalBuckets, setTotalBuckets] = React.useState<number>(0);
    const [totalHours, setTotalHours] = React.useState<number>(0);
    const [totalEvents, setTotalEvents] = React.useState<number>(0);
    const [totalParticipants, setTotalParticipants] = React.useState<number>(0);
    const [isDataLoaded, setIsDataLoaded] = React.useState<boolean>(false);

    React.useEffect(() => {

        const headers = getDefaultHeaders('GET');

        fetch('/api/stats', {
            method: 'GET',
            headers: headers
        })
            .then(response => response.json() as Promise<StatsData>)
            .then(data => {
                setTotalBags(data.totalBags);
                setTotalBuckets(data.totalBuckets);
                setTotalHours(data.totalHours);
                setTotalEvents(data.totalEvents);
                setTotalParticipants(data.totalParticipants);
                setIsDataLoaded(true);
            });
    }, []);

    function renderStats() {
        return (
            <div className="container-fluid card">
                <h1>Here's what TrashMob.eco members have done for the planet so far!</h1>
                <table>
                    <tbody>
                        <tr>
                            <td>Total Number of Events Hosted:</td>
                            <td>{totalEvents}</td>
                        </tr>
                        <tr>
                            <td>Total Number of Bags Collected:</td>
                            <td>{totalBags}</td>
                        </tr>
                        <tr>
                            <td>Total Number of Buckets Collected:</td>
                            <td>{totalBuckets}</td>
                        </tr>
                        <tr>
                            <td>Total Number of Event Participants:</td>
                            <td>{totalParticipants}</td>
                        </tr>
                        <tr>
                            <td>Total Number of Participant Hours:</td>
                            <td>{totalHours}</td>
                        </tr>
                    </tbody>
                </table>
                <h6>1 bag = approximately 3 buckets</h6>
            </div>
        )
    }

    return (
        <>
            <div>
                {!isDataLoaded && <p><em>Loading...</em></p>}
                {isDataLoaded && renderStats()}
            </div>
        </>
    );
}

export default Stats;