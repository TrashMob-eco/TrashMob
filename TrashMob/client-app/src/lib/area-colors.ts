// Deterministic color assignment for adoptable areas.
// Each area gets a consistent color based on its ID.

const PALETTE = [
    '#E63946',
    '#457B9D',
    '#2A9D8F',
    '#E9C46A',
    '#F4A261',
    '#264653',
    '#A855F7',
    '#06B6D4',
    '#84CC16',
    '#F43F5E',
    '#8B5CF6',
    '#14B8A6',
];

/** Returns a deterministic color from PALETTE based on a string hash of the area ID. */
export function getAreaColor(areaId: string): string {
    let hash = 0;
    for (let i = 0; i < areaId.length; i++) {
        hash = (hash * 31 + areaId.charCodeAt(i)) % 1_000_000_007;
    }
    return PALETTE[Math.abs(hash) % PALETTE.length];
}
