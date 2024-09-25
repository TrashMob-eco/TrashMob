import * as azmaps from 'azure-maps-control';

/**
 * Options for the HTML Marker Layer class.
 */
export interface HtmlMarkerLayerOptions {
    /**
     * The id or instance of a data source which the layer will render.
     */
    source?: string | azmaps.source.Source;

    /**
     * Required when the source of the layer is a VectorTileSource.
     * A vector source can have multiple layers within it, this identifies which one to render in this layer.
     * Prohibited for all other types of sources.
     */
    sourceLayer?: string;

    /**
     * Specifies if the layer should update while the map is moving. When set to false, rendering in the map view will
     * occur after the map has finished moving. New data is not rendered until the moveend event fires. When set to true,
     * the layer will constantly re-render as the map is moving which ensures new data is always rendered right away,
     * but may reduce overall performance. Default: `false`
     */
    updateWhileMoving?: boolean;

    /**
     * A callback function that generates a HtmlMarker for a given data point.
     * The `id` and `properties` values will be added to the marker as properties within the layer after being created by this callback function.
     */
    markerCallback?: (
        id: string,
        position: azmaps.data.Position,
        properties: any,
    ) => azmaps.HtmlMarker | Promise<azmaps.HtmlMarker>;

    /**
     * An expression specifying conditions on source features.
     * Only features that match the filter are displayed.
     * Default: `['==', ['geometry-type'], 'Point']`
     */
    filter?: azmaps.Expression;
    /**
     * An integer specifying the minimum zoom level to render the layer at.
     * This value is inclusive, i.e. the layer will be visible at `maxZoom > zoom >= minZoom`.
     * Default `0`.
     * @default 0
     */
    minZoom?: number;
    /**
     * An integer specifying the maximum zoom level to render the layer at.
     * This value is exclusive, i.e. the layer will be visible at `maxZoom > zoom >= minZoom`.
     * Default `24`.
     * @default 24
     */
    maxZoom?: number;
    /**
     * Specifies if the layer is visible or not.
     * Default `true`.
     * @default true
     */
    visible?: boolean;
}
