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
    actualNumberOfAttendees: number = 0;
    numberOfBags: number = 0;
    durationInMinutes: number = 0;
    totalWorkHours: number = 0;
}

export default DisplayEventSummary;
