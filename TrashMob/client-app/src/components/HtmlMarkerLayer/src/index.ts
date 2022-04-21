import { Namespace } from "./helpers/Namespace";

/* Build the structure of the SDK */

//Merge the local layers into the 'atlas.layer' namespace.
import * as baseLayer from "./layer";
const layer = Namespace.merge("atlas.layer", baseLayer);
export { layer };

export * from "./PieChartMarker";