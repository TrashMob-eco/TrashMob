export function formatDistance(meters: number): string {
    if (meters >= 1609) {
        return `${(meters / 1609.34).toFixed(1)} mi`;
    }
    return `${meters.toLocaleString()} m`;
}

export function formatDuration(minutes: number): string {
    if (minutes >= 60) {
        const hours = Math.floor(minutes / 60);
        const mins = minutes % 60;
        return mins > 0 ? `${hours}h ${mins}m` : `${hours}h`;
    }
    return `${minutes}m`;
}

export function formatArea(sqMeters: number): string {
    const sqMetersPerAcre = 4046.86;
    if (sqMeters >= sqMetersPerAcre) {
        return `${(sqMeters / sqMetersPerAcre).toFixed(1)} acres`;
    }
    const sqFeet = sqMeters * 10.7639;
    return `${Math.round(sqFeet).toLocaleString()} sq ft`;
}

export function formatWeight(weight: number, weightUnitId: number | null): string {
    return `${weight.toFixed(1)} ${weightUnitId === 2 ? 'kg' : 'lbs'}`;
}
