import React, { Component } from 'react';

export class FetchData extends Component {
  static displayName = FetchData.name;

  constructor(props) {
    super(props);
    this.state = { cleanupEvents: [], loading: true };
  }

  componentDidMount() {
    this.populateCleanupEvents();
  }

  static renderCleanupEventsTable(cleanupEvents) {
    return (
      <table className='table table-striped' aria-labelledby="tabelLabel">
        <thead>
          <tr>
            <th>Date</th>
            <th>Name</th>
            <th>Address</th>
            <th>Country</th>
            <th>Description</th>
            <th>Contact Phone</th>
            <th>Latitude</th>
            <th>Longitude</th>
            <th>MaximumNumberOfParticpants</th>
            <th>EventLead</th>
           </tr>
        </thead>
        <tbody>
          {cleanupEvents.map(cleanupEvent =>
            <tr key={cleanupEvent.date}>
                  <td>{cleanupEvent.eventDate}</td>
                  <td>{cleanupEvent.name}</td>
                  <td>{cleanupEvent.address}</td>
                  <td>{cleanupEvent.country}</td>
                  <td>{cleanupEvent.description}</td>
                  <td>{cleanupEvent.contactPhone}</td>
                  <td>{cleanupEvent.latitude}</td>
                  <td>{cleanupEvent.longitude}</td>
                  <td>{cleanupEvent.maxNumberOfParticipants}</td>
                  <td>{cleanupEvent.userName}</td>
            </tr>
          )}
        </tbody>
      </table>
    );
  }

  render() {
    let contents = this.state.loading
      ? <p><em>Loading...</em></p>
      : FetchData.renderCleanupEventsTable(this.state.cleanupEvents);

    return (
      <div>
        <h1 id="tabelLabel" >Cleanup Events</h1>
        {contents}
      </div>
    );
  }

  async populateCleanupEvents() {
    const response = await fetch('cleanupEvent');
    const data = await response.json();
    this.setState({ cleanupEvents: data, loading: false });
  }
}
