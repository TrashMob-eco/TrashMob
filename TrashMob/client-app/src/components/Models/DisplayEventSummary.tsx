class DisplayEventSummary {
    id: string = "";
    name: string = "";
    eventDate: Date = new Date();
    eventTypeId: number = 0;
    streetAddress: string = "";
    city: string = "";
    region: string = "";
    country: string = "";
    postalCode: string = "";
    totalAttendees: number = 0;
    totalBags: number = 0;
    durationInMinutes: number = 0;
}

export default DisplayEventSummary;
