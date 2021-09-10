import { Namespace } from "./Namespace";

/* Build the structure of the SDK */

//Merge the local layers into the 'atlas.layer' namespace.
import * as baseLayer from "./HtmlMarkerLayer";
const layer = Namespace.merge("atlas.layer", baseLayer);
export { layer };
