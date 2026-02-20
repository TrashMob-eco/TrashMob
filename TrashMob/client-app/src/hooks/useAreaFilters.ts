import { useMemo, useState } from 'react';
import AdoptableAreaData, {
    AdoptableAreaType,
    AdoptableAreaStatus,
} from '@/components/Models/AdoptableAreaData';

export const AREA_TYPES: AdoptableAreaType[] = [
    'Park',
    'School',
    'Trail',
    'Street',
    'Highway',
    'HighwaySection',
    'Interchange',
    'Waterway',
    'CityBlock',
    'Spot',
];

export const AREA_STATUSES: AdoptableAreaStatus[] = ['Available', 'Adopted', 'Unavailable'];

export function useAreaFilters(areas: AdoptableAreaData[]) {
    const [search, setSearch] = useState('');
    const [areaType, setAreaType] = useState('all');
    const [status, setStatus] = useState('all');

    const filteredAreas = useMemo(() => {
        let result = areas;
        if (search) {
            const lower = search.toLowerCase();
            result = result.filter((a) => a.name.toLowerCase().includes(lower));
        }
        if (areaType !== 'all') {
            result = result.filter((a) => a.areaType === areaType);
        }
        if (status !== 'all') {
            result = result.filter((a) => a.status === status);
        }
        return result;
    }, [areas, search, areaType, status]);

    const hasActiveFilters = search !== '' || areaType !== 'all' || status !== 'all';

    return {
        search,
        setSearch,
        areaType,
        setAreaType,
        status,
        setStatus,
        filteredAreas,
        totalCount: areas.length,
        hasActiveFilters,
    };
}
