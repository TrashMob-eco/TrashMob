// @ts-nocheck

import * as azmaps from 'azure-maps-control';
import { PieChartMarkerOptions } from './PieChartMarkerOptions';
import { ExtendedHtmlMarker } from './extensions/ExtendedHtmlMarker';

/**
 * A class for creating Pie Charts as HTML Markers on a map.
 */
export class PieChartMarker extends azmaps.HtmlMarker implements ExtendedHtmlMarker {
    /** ******************
     * Private Properties
     ******************* */

    private _options = <PieChartMarkerOptions>{
        values: [],
        radius: 40,
        colors: ['#d7191c', '#fdae61', '#ffffbf', '#abdda4', '#2b83ba'],
        fillColor: 'transparent',
        strokeWidth: 0,
        strokeColor: '#666666',
        innerRadius: 0,
    };

    /** The total of all values. */
    private _total: number = 0;

    /** Additional colors to use when enough haven't been specified. */
    public static _moreColors = [];

    /** ******************
     * Constructor
     ******************* */

    /**
     * Creates an HTML Marker in the shape of a pie chart.
     * @param options Options for rendering the Pie Chart marker.
     */
    constructor(options: PieChartMarkerOptions) {
        super(options);
        super.setOptions({
            htmlContent: document.createElement('div'),
            pixelOffset: [0, 0],
            anchor: 'center',
        });

        this.setOptions(options);
    }

    /** ******************
     * Public Methods
     ******************* */

    /** ID of the marker. */
    public id: string;

    /** Any additional properties that you want to store with the marker. */
    public properties: any = {};

    /**
     * Gets the total value of all slices summed togehter.
     * @returns The total value of all slices summed togehter.
     */
    public getTotalValue(): number {
        return this._total;
    }

    /**
     * Gets the value of a slice of the pie based on it's index.
     * @param idx The index of the slice.
     * @returns The value of a slice of the pie based on it's index.
     */
    public getSliceValue(idx: number): number {
        const vals = this._options.values;
        return idx >= 0 && idx < vals.length ? vals[idx] : 0;
    }

    /**
     * Gets the percentage value of a slice of the pie based on it's index.
     * @param idx The index of the slice.
     * @returns The percentage value of a slice of the pie based on it's index.
     */
    public getSlicePercentage(idx: number): number {
        const self = this;
        return self._total > 0 ? Math.round((self.getSliceValue(idx) / self._total) * 10000) / 100 : 0;
    }

    /**
     * Gets the options of the pie chart marker.
     * @returns The options of the pie chart marker.
     */
    public getOptions(): PieChartMarkerOptions {
        return { ...super.getOptions(), ...this._options };
    }

    /**
     * Sets the options of the pie chart marker.
     * @param options The options to set on the marker.
     */
    public setOptions(options: PieChartMarkerOptions): void {
        const self = this;
        const opt = self._options;
        const { stringify } = JSON;
        let rerender = false;

        if (options.radius && options.radius > 0 && options.radius != opt.radius) {
            opt.radius = options.radius;
            rerender = true;
        }

        if (options.innerRadius >= 0 && options.innerRadius != opt.innerRadius) {
            opt.innerRadius = options.innerRadius;
            rerender = true;
        }
        if (options.colors && stringify(options.colors) !== stringify(opt.colors)) {
            opt.colors = options.colors;
            rerender = true;
        }

        if (options.fillColor && stringify(options.fillColor) !== stringify(opt.fillColor)) {
            opt.fillColor = options.fillColor;
            rerender = true;
        }

        if (options.strokeColor && options.strokeColor !== opt.strokeColor) {
            opt.strokeColor = options.strokeColor;
            rerender = true;
        }

        if (options.strokeWidth >= 0 && options.strokeWidth != opt.strokeWidth) {
            opt.strokeWidth = options.strokeWidth;
            rerender = true;
        }

        if (options.tooltipCallback !== undefined && opt.tooltipCallback != options.tooltipCallback) {
            opt.tooltipCallback = options.tooltipCallback;
            rerender = true;
        }

        if (options.values && stringify(options.values) !== stringify(opt.values)) {
            opt.values = options.values;
            rerender = true;
        }

        if (options.text !== undefined && options.text !== opt.text) {
            // opt.text = options.text;
            super.setOptions({ text: options.text });
            rerender = true;
        }

        if (options.textClassName !== undefined && options.textClassName !== opt.textClassName) {
            opt.textClassName = options.textClassName;
            rerender = true;
        }

        if (rerender) {
            self._render();
        }

        super.setOptions(options);
    }

    /** ******************
     * Private Methods
     ******************* */

    /**
     * Method that generates the SVG pie chart for the marker.
     */
    private _render() {
        const self = this;
        const opt = self._options;
        const data = opt.values;
        const { radius } = opt;

        let startAngle = 0;
        let angle = 0;

        if (data) {
            self._total = data.reduce((a, b) => a + b, 0);

            // Ensure that there are enough colors defined.
            const moreColors = PieChartMarker._moreColors;
            const { random } = Math;
            const { round } = Math;
            let mIdx = 0;

            while (data.length > opt.colors.length) {
                // Generate additional random colors, but try and stagger them such that there is a good variation between agenct colors.
                if (moreColors.length < data.length) {
                    moreColors.push(
                        `hsl(${round(random() * 360)},${round(random() * 20) + 70}%,${round(random() * 40) + 30}%)`,
                    );
                }

                // Grab the next additional color from the global pallet.
                opt.colors.push(moreColors[mIdx]);
                mIdx++;
            }

            // Origin for cx/cy
            const o = radius + opt.strokeWidth;
            const svg = [`<svg xmlns="http://www.w3.org/2000/svg" width="${2 * o}px" height="${2 * o}px">`];

            let tooltip = '';
            let maskId: string;

            if (opt.innerRadius > 0 && opt.innerRadius <= opt.radius) {
                maskId = `piechart-innercircle-${round(random() * 10000000)}`;

                svg.push(`<defs><mask id="${maskId}"><rect width="100%" height="100%" fill="white"/><circle r="${opt.innerRadius}" cx="${o}" cy="${o}" fill="black"/></mask></defs>
                    <circle r="${opt.innerRadius}" cx="${o}" cy="${o}" style="fill:${opt.fillColor};stroke:${opt.strokeColor};stroke-width:${opt.strokeWidth * 2}px;"/>`);
            }

            if (self._total > 0) {
                const ttc = opt.tooltipCallback;
                const ratio = (Math.PI * 2) / self._total;
                for (let i = 0; i < data.length; i++) {
                    angle = ratio * data[i];

                    if (ttc) {
                        tooltip = ttc(self, i);
                    }

                    const c = i < opt.colors.length ? opt.colors[i] : moreColors[i];

                    svg.push(self._createSlice(o, o, radius, startAngle, angle, c, tooltip, maskId));
                    startAngle += angle;
                }
            }

            const { text } = self.getOptions();
            if (text) {
                svg.push(
                    `<text x="${o}" y="${o + 7}" style="font-size:16px;font-family:arial;fill:#000;font-weight:bold;" class="${opt.textClassName || ''}" text-anchor="middle">${text}</text>`,
                );
            }

            svg.push('</svg>');

            (<HTMLDivElement>super.getOptions().htmlContent).innerHTML = svg.join('');
        }
    }

    /**
     * Generates the SVG path for an arc slice of a pie.
     * @param cx Center x-origin of the arc.
     * @param cy Center y-origin of the arc.
     * @param r Radius of arc.
     * @param startAngle The start angle of the arc (0 = up, PI/2 = right, PI = down, 3/2 PI = left)
     * @param angle The angle width of the arc.
     * @param fillColor The fill color of the path.
     * @param tooltip The tooltip text to display when hovered.
     */
    private _createSlice(
        cx: number,
        cy: number,
        r: number,
        startAngle: number,
        angle: number,
        fillColor: string,
        tooltip: string,
        maskId: string,
    ): string {
        const opt = this._options;
        const pi = Math.PI;
        let mask = '';

        if (maskId) {
            mask = ` mask="url(#${maskId}"`;
        }

        if (angle > 2 * pi * 0.99) {
            // If the shape is nearly a complete circle, create a circle instead of an arc.
            return `<circle r="${r}" cx="${cx}" cy="${cy}" style="fill:${fillColor};stroke:${opt.strokeColor};stroke-width:${opt.strokeWidth}px;"${mask}><title>${tooltip}</title></circle>`;
        }

        const { sin } = Math;
        const { cos } = Math;

        const x1 = cx + r * sin(startAngle);
        const y1 = cy - r * cos(startAngle);
        const x2 = cx + r * sin(startAngle + angle);
        const y2 = cy - r * cos(startAngle + angle);

        const x21 = cx + opt.innerRadius * sin(startAngle);
        const y21 = cy - opt.innerRadius * cos(startAngle);
        const x22 = cx + opt.innerRadius * sin(startAngle + angle);
        const y22 = cy - opt.innerRadius * cos(startAngle + angle);

        // Flag for when arcs are larger than 180 degrees in radians.
        let big = 0;
        if (angle > pi) {
            big = 1;
        }

        return `<path d="M${cx} ${cy} L ${x1} ${y1} A ${r},${r} 0 ${big} 1 ${x2} ${y2}z" style="fill:${fillColor};stroke:${opt.strokeColor};stroke-width:${opt.strokeWidth}px;"${mask}><title>${tooltip}</title></path>`;
    }
}
