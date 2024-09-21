import * as azmaps from 'azure-maps-control';
import { PieChartMarker } from './PieChartMarker';

/**
 * Options for styling a PieChartMarker.
 */
export interface PieChartMarkerOptions {
    /** The value of each slice of the pie. */
    values: number[];

    /** The radius of a pie chart in pixels. Default: `40` */
    radius?: number;

    /** The inner radius of the pie chart in pixels. Default: `0` */
    innerRadius: number;

    /** The colors of each category in the pie chart. Should have a length >= to largest values array in data set. Default: `['#d7191c','#fdae61','#ffffbf','#abdda4','#2b83ba']` */
    colors?: string[];

    /** The color to fill the center of a pie chart when inner radius is greated than 0. Default: `transparent` */
    fillColor?: string;

    /** The stroke width in pixels. Default: `0` */
    strokeWidth?: number;

    /** The color of the stroke line. Default: `#666666` */
    strokeColor?: string;

    /** A CSS class name to append to the `text` tag of the SVG pie chart. */
    textClassName?: string;

    /** A callback handler which defines the value of a tooltip for a slice of the pie. */
    tooltipCallback?: (marker: PieChartMarker, sliceIdx: number) => string;

    /**
     * Indicates the marker's location relative to its position on the map.
     * Optional values: `"center"`, `"top"`, `"bottom"`, `"left"`, `"right"`,
     * `"top-left"`, `"top-right"`, `"bottom-left"`, `"bottom-right"`.
     * Default `"bottom"`
     * @default "bottom"
     */
    anchor?: string;

    /**
     * Indicates if the user can drag the position of the marker using the mouse or touch controls.
     * default `false`
     * @default false
     */
    draggable?: boolean;

    /**
     * An offset in pixels to move the popup relative to the markers center.
     * Negatives indicate left and up.
     * default `[0, -18]`
     * @default [0, -18]
     */
    pixelOffset?: azmaps.Pixel;

    /**
     * The position of the marker.
     * default `[0, 0]`
     * @default [0, 0]
     */
    position?: azmaps.data.Position;

    /**
     * A popup that is attached to the marker.
     */
    popup?: azmaps.Popup;

    /**
     * Specifies if the marker is visible or not.
     * default `true`
     * @default true
     */
    visible?: boolean;

    /**
     * Text to display at the center of the pie chart.
     */
    text?: string;
}
