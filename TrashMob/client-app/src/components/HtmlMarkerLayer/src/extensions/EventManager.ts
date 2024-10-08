import * as azmaps from 'azure-maps-control';
import { HtmlMarkerLayer } from '../layer';

/**
 * This module partially defines the map control.
 * This definition only includes the features added by using the drawing tools.
 * For the base definition see:
 * https://docs.microsoft.com/javascript/api/azure-maps-control/?view=azure-maps-typescript-latest
 */
declare module 'azure-maps-control' {
    /**
     * This interface partially defines the map control's `EventManager`.
     * This definition only includes the method added by using the drawing tools.
     * For the base definition see:
     * https://docs.microsoft.com/javascript/api/azure-maps-control/atlas.eventmanager?view=azure-maps-typescript-latest
     */
    export interface EventManager {
        /**
         * Adds an event to the `HtmlMarkerLayer`.
         * @param eventType The event name.
         * @param target The `fullscreenchanged` to add the event for.
         * @param callback The event handler callback.
         */
        add(
            eventType:
                | 'click'
                | 'contextmenu'
                | 'dblclick'
                | 'drag'
                | 'dragstart'
                | 'dragend'
                | 'keydown'
                | 'keypress'
                | 'keyup'
                | 'mousedown'
                | 'mouseenter'
                | 'mouseleave'
                | 'mousemove'
                | 'mouseout'
                | 'mouseover'
                | 'mouseup',
            target: HtmlMarkerLayer,
            callback: (e: azmaps.TargetedEvent) => void,
        ): void;

        /**
         * Adds an event to the `HtmlMarkerLayer` once.
         * @param eventType The event name.
         * @param target The `fullscreenchanged` to add the event for.
         * @param callback The event handler callback.
         */
        addOnce(
            eventType:
                | 'click'
                | 'contextmenu'
                | 'dblclick'
                | 'drag'
                | 'dragstart'
                | 'dragend'
                | 'keydown'
                | 'keypress'
                | 'keyup'
                | 'mousedown'
                | 'mouseenter'
                | 'mouseleave'
                | 'mousemove'
                | 'mouseout'
                | 'mouseover'
                | 'mouseup',
            target: HtmlMarkerLayer,
            callback: (e: azmaps.TargetedEvent) => void,
        ): void;

        /**
         * Removes an event listener from the `HtmlMarkerLayer`.
         * @param eventType The event name.
         * @param target The `fullscreenchanged` to remove the event for.
         * @param callback The event handler callback.
         */
        remove(eventType: string, target: HtmlMarkerLayer, callback: (e: azmaps.TargetedEvent) => void): void;
    }
}
