// @ts-nocheck

import * as azmaps from "azure-maps-control";
import { HtmlMarkerLayerOptions } from './HtmlMarkerLayerOptions';
import { ExtendedHtmlMarker } from '../extensions/ExtendedHtmlMarker';

/**
 * A layer that renders point data from a data source as HTML elements on the map.
 */
export class HtmlMarkerLayer extends azmaps.layer.BubbleLayer {

    /*********************
     * Private Properties
     *********************/

    private _options: HtmlMarkerLayerOptions = {
        minZoom: 0,
        maxZoom: 24,
        visible: true,
        updateWhileMoving: false,
        filter: ['==', ['geometry-type'], 'Point'],
        markerCallback: (id, position, properties) => {
            if (properties.cluster) {
                return new azmaps.HtmlMarker({
                    position: position,
                    text: properties.point_count_abbreviated
                });
            } else {
                return new azmaps.HtmlMarker({
                    position: position
                });
            }
        }
    };

    private _map: azmaps.Map;

    private _markers: ExtendedHtmlMarker[] = [];
    private _markerIds: string[] = [];
    private _markerCache: any = {};

    private _timer: number;

    /** Events supported by the HTML Marker Layer */
    private _supportedEvents = ["click", "contextmenu", "dblclick", "drag", "dragstart", "dragend", "keydown", "keypress", "keyup", "mousedown", "mouseenter", "mouseleave", "mousemove", "mouseout", "mouseover", "mouseup"];

    /*********************
     * Constructor
     *********************/

    /**
    * Constructs a new HtmlMarkerLayer.
    * @param source The id or instance of a data source which the layer will render.
    * @param id The id of the layer. If not specified a random one will be generated.
    * @param options The options of the Html marker layer.
    */
    constructor(source?: string | azmaps.source.Source, id?: string, options?: HtmlMarkerLayerOptions) {
        super(source, id);

        super.setOptions({
            color: 'transparent',
            radius: 0,
            strokeWidth: 0
        } as azmaps.BubbleLayerOptions);

        this.setOptions(options || {});
    }

    /*********************
     * Public methods
     *********************/

    /**
    * Gets the options of the Html Marker layer.
    */
    public getOptions(): HtmlMarkerLayerOptions {
        return Object.assign({}, this._options);
    }

    /**
    * Sets the options of the Html marker layer.
    * @param options The new options of the Html marker layer.
    */
    public setOptions(options: HtmlMarkerLayerOptions): void {
        const self = this;
        const opt = self._options;

        const newBaseOptions = {} as azmaps.BubbleLayerOptions;
        let cc = false;

        if (options.source && opt.source !== options.source) {
            opt.source = options.source
            newBaseOptions.source = options.source;
            cc = true;
        }

        if (options.sourceLayer && opt.sourceLayer !== options.sourceLayer) {
            opt.sourceLayer = options.sourceLayer
            newBaseOptions.sourceLayer = options.sourceLayer;
            cc = true;
        }

        if (options.filter && opt.filter !== options.filter) {
            opt.filter = options.filter
            newBaseOptions.filter = options.filter;
            cc = true;
        }

        if (typeof options.minZoom === 'number' && opt.minZoom !== options.minZoom) {
            opt.minZoom = options.minZoom
            newBaseOptions.minZoom = options.minZoom;
        }

        if (typeof options.maxZoom === 'number' && opt.maxZoom !== options.maxZoom) {
            opt.maxZoom = options.maxZoom
            newBaseOptions.maxZoom = options.maxZoom;
        }

        if (typeof options.visible !== 'undefined' && opt.visible !== options.visible) {
            opt.visible = options.visible
            newBaseOptions.visible = options.visible;
        }

        if (options.markerCallback && opt.markerCallback !== options.markerCallback) {
            opt.markerCallback = options.markerCallback;
            cc = true;
        }

        if (typeof options.updateWhileMoving === 'boolean' && opt.updateWhileMoving !== options.updateWhileMoving) {
            opt.updateWhileMoving = options.updateWhileMoving;
        }

        if (cc) {
            self._clearCache(true);
        } else {
            self._updateMarkers();
        }

        super.setOptions(newBaseOptions);
    }

    /** Force the layer to refresh and update. */
    public update(): void {
        this._clearCache(true);
        this._updateMarkers();
    }

    /***************************
     * Public override methods
     ***************************/

    //Override the layers onAdd function. 
    public onAdd(map: azmaps.Map): void {
        const self = this;
        let mapEvents = map.events;

        if (map) {
            mapEvents.remove('moveend', self._updateMarkers);
            mapEvents.remove('move', self._mapMoved);
            mapEvents.remove('sourcedata', self._sourceUpdated);
        }

        self._map = map;
        mapEvents = map.events;

        mapEvents.add('moveend', self._updateMarkers);
        mapEvents.add('move', self._mapMoved);
        mapEvents.add('sourcedata', self._sourceUpdated);

        //Call the underlying functionaly for this.
        super.onAdd(map);
    }

    //Override the layers onRemove function.
    public onRemove(): void {
        const self = this;
        const map = self._map;

        if (map) {
            const mapEvents = map.events;
            mapEvents.remove('moveend', self._updateMarkers);
            mapEvents.remove('move', self._mapMoved);
            mapEvents.remove('sourcedata', self._sourceUpdated);
        }

        self._clearCache(false);
        self._map = null;

        super.onRemove();
    }

    /*********************
     * Private methods
     *********************/

    /** Event handler for when the map moves. */
    private _mapMoved = () => {
        if (this._options.updateWhileMoving) {
            this._updateMarkers();
        }
    }

    /**
     * Gets the source class of the layer.
     */
    private _getSourceClass(): azmaps.source.Source {
        const self = this;
        const s = self.getSource();
        if (typeof s === 'string' && self._map !== null) {
            return self._map.sources.getById(s);
        } else if (s instanceof azmaps.source.Source) {
            return s;
        }

        return null;
    }

    /**
     * Event handler for when a data source in the map changes.
     */
    private _sourceUpdated = (e) => {
        const s = this._getSourceClass();

        if (s && s.getId() === e.source.id) {
            //this._clearCache(true);

            //Check to see if there is a timer already waiting, if so, remove it.
            if (this._timer) {
                clearTimeout(this._timer);
            }

            //Wait 33ms (~30 frames per second) before processing the update. This will help throttle updates.
            //@ts-ignore
            this._timer = setTimeout(this._sourceUpdater, 33);
        }
    }

    /**
     * Throttled event handler for updating the clearing cache.
     */
    private _sourceUpdater = () => {
        //Clear the timer.
        this._timer = null;

        //Clear the cache and force an update if the data source has changed.
        this._clearCache(true);
    }

    /**
     * Clears the marker cache.
     * @param update A boolean indicating if the layer should rerender/update after clearing the cache.
     */
    private _clearCache(update): void {
        const self = this;

        self._markerCache = {}; //Clear marker cache. 
        if (self._map) {
            for (let i = 0, len = self._markers.length; i < len; i++) {
                const m = self._markers[i];
                //Remove wrapped events from marker.
                self._removeEvents(m)
                m._eventsAttached = false;

                //Remove marker from map.
                self._map.markers.remove(m);
            }
        }
        self._markers = [];
        self._markerIds = [];

        if (update) {
            self._updateMarkers();
        }
    }

    /**
     * Main function that updates all displayed markers on the map.
     */
    private _updateMarkers = async (): Promise<void> => {
        const self = this;
        const map = self._map;
        const markers = self._markers;
        const opt = self._options;
        const zoom = (map) ? map.getCamera().zoom : undefined;

        if (opt.visible && zoom !== undefined && zoom >= opt.minZoom && zoom <= opt.maxZoom) {
            //TODO: Bug: this doesn't currently return clusters. Using underlying code to work around this.
            //const shapes = map.layers.getRenderedShapes(null, self, opt.filter);

            const source = self.getSource();
            const sourceId = (typeof source === 'string') ? source : source.getId();

            //@ts-ignore
            const shapes = map.map.querySourceFeatures(sourceId, {
                sourceLayer: self.getOptions().sourceLayer,
                filter: opt.filter
            });

            const newMarkers = [];
            const newMarkerIds = [];

            let id: string;
            let properties: any;
            let position: azmaps.data.Position;
            let shape: azmaps.Shape;
            let feature: azmaps.data.Feature<azmaps.data.Geometry, any>;
            let marker: ExtendedHtmlMarker;

            for (let i = 0, len = shapes.length; i < len; i++) {
                marker = null;
                id = null;

                if (shapes[i] instanceof azmaps.Shape) {
                    shape = shapes[i] as azmaps.Shape;

                    if (shape.getType() === 'Point') {
                        position = shape.getCoordinates() as azmaps.data.Position;
                        properties = shape.getProperties();
                        id = shape.getId() as string;
                    }
                } else {
                    feature = shapes[i] as azmaps.data.Feature<azmaps.data.Geometry, any>;

                    if (feature.geometry.type === 'Point') {
                        position = feature.geometry.coordinates as azmaps.data.Position;
                        properties = feature.properties;

                        //Check to see if the point represents a clustered point from a GeoJSON data source. Vector tile sources may have cluster data, but may not align with the same property schema.
                        if (properties && properties.cluster) {
                            id = 'cluster_' + feature.properties.cluster_id;
                        } else if (feature.id) {
                            id = feature.id as string;
                        }
                    }
                }

                if (position) {
                    marker = await self._getMarker(id, position, properties);
                    //Add marker events to wrap layer events.
                    if (!marker._eventsAttached) {
                        self._addEvents(marker);
                        marker._eventsAttached = true;
                    }

                    if (marker) {
                        if (marker.id) {
                            newMarkerIds.push(marker.id);
                        }

                        if (!marker.id || self._markerIds.indexOf(marker.id) === -1) {
                            newMarkers.push(marker);
                            map.markers.add(marker);
                        }
                    }
                }
            }

            //Remove all markers that are no longer in view. 
            for (let i = markers.length - 1; i >= 0; i--) {
                if (!markers[i].id || newMarkerIds.indexOf(markers[i].id) === -1) {
                    map.markers.remove(markers[i]);
                    markers.splice(i, 1);
                }
            }

            self._markers = markers.concat(newMarkers);
            self._markerIds = newMarkerIds;
        } else if (self._markers.length > 0) {
            map.markers.remove(self._markers);
            self._markers = [];
        }
    }

    /**
     * Gets a marker either from cache or from the rendering callback.
     * @param id The id of the marker.
     * @param position The position of the marker.
     * @param properties The properties of the marker.
     */
    private _getMarker(id: string, position: azmaps.data.Position, properties: any): Promise<ExtendedHtmlMarker> {
        const self = this;
        const markerCache = self._markerCache;
        const opt = self._options;

        if (!id) {
            //If no id, create an ID based on the position and properties.
            id = position.join(',') + JSON.stringify(properties || {});
        }

        //Check cache for existing marker.
        if (markerCache[id]) {
            return markerCache[id];
        } else {
            const callbackResult = opt.markerCallback(id, position, properties);
            if (callbackResult instanceof azmaps.HtmlMarker) {
                const m = self._getExtendedMarker(callbackResult, id, position, properties);
                if (m) {
                    markerCache[id] = m;
                    return Promise.resolve(m);
                }
            } else {
                return new Promise<ExtendedHtmlMarker>(resolve => {
                    callbackResult.then(marker => {
                        const m = self._getExtendedMarker(marker, id, position, properties);
                        if (m) {
                            markerCache[id] = m;
                            resolve(m);
                        }
                    });
                });
            }

            return null;
        }
    }

    private _getExtendedMarker(marker: azmaps.HtmlMarker, id: string, position: azmaps.data.Position, properties: any): ExtendedHtmlMarker {
        const result: ExtendedHtmlMarker = marker;
        if (result) {
            result.properties = properties;
            result.id = id;

            //Make sure position is set.
            result.setOptions({
                position: position
            });

            return result;
        }
        return null;
    }

    /**
     * Wraps all events on a marker.
     * @param marker Marker to wrap events on.
     */
    private _addEvents(marker: azmaps.HtmlMarker): void {
        const self = this;
        self._supportedEvents.forEach(e => {
            //@ts-ignore
            self.map.events.add(e, marker, self._wrappedEvent);
        });
    }

    /**
     * Removes all wrapped events on a marker.
     * @param marker Marker to remove events from.
     */
    private _removeEvents(marker: azmaps.HtmlMarker): void {
        const self = this;
        self._supportedEvents.forEach(e => {
            //@ts-ignore
            self.map.events.remove(e, marker, self._wrappedEvent);
        });
    }

    /**
     * A simple event handler wrapper.
     * @param e Event arg. Will be a TargetedEvent from an HTML Marker.
     */
    private _wrappedEvent = (e) => {
        this.map.events.invoke(e.type, this, e);
    }
}