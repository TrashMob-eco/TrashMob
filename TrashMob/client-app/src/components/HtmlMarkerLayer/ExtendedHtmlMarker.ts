import * as azmaps from "azure-maps-control";

/** An interface for the HtmlMarker class that extends it by adding an `id` and a `properties` property. */
export interface ExtendedHtmlMarker extends azmaps.HtmlMarker {
    /** ID of the marker. */
    id?: string;

    /** Properties attached to the marker. */
    properties?: any;

    /** Internal flag indicating if the marker has had wrapped HtmlMarkerLayer events attached. */
    _eventsAttached?: boolean;
}