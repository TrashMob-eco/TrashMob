import { useState, useRef, useEffect } from 'react';
import { useQuery } from '@tanstack/react-query';
import { Search } from 'lucide-react';
import { Input } from '@/components/ui/input';
import { useDebounce } from '@/hooks/useDebounce';
import { SearchAddress } from '@/services/maps';

interface AddressSearchInputProps {
    onLocationSelected: (lat: number, lng: number) => void;
}

export const AddressSearchInput = ({ onLocationSelected }: AddressSearchInputProps) => {
    const [query, setQuery] = useState('');
    const [open, setOpen] = useState(false);
    const debouncedQuery = useDebounce(query, 300);
    const containerRef = useRef<HTMLDivElement>(null);

    const { data: results } = useQuery({
        queryKey: SearchAddress().key(debouncedQuery),
        queryFn: () => SearchAddress().service({ query: debouncedQuery }),
        select: (res) => res.data.results,
        enabled: debouncedQuery.length >= 3,
    });

    // Close dropdown on outside click
    useEffect(() => {
        const handler = (e: MouseEvent) => {
            if (containerRef.current && !containerRef.current.contains(e.target as Node)) {
                setOpen(false);
            }
        };
        document.addEventListener('mousedown', handler);
        return () => document.removeEventListener('mousedown', handler);
    }, []);

    const handleSelect = (lat: number, lon: number) => {
        onLocationSelected(lat, lon);
        setQuery('');
        setOpen(false);
    };

    return (
        <div ref={containerRef} className='relative ml-auto w-64'>
            <div className='relative'>
                <Search className='absolute left-2 top-1/2 -translate-y-1/2 h-3.5 w-3.5 text-muted-foreground' />
                <Input
                    type='text'
                    placeholder='Search address...'
                    value={query}
                    onChange={(e) => {
                        setQuery(e.target.value);
                        setOpen(true);
                    }}
                    onFocus={() => {
                        if (results && results.length > 0) setOpen(true);
                    }}
                    className='h-8 pl-7 text-sm'
                />
            </div>
            {open && results && results.length > 0 ? (
                <div className='absolute z-50 mt-1 w-full rounded-md border bg-popover shadow-md max-h-48 overflow-y-auto'>
                    {results.slice(0, 5).map((result) => (
                        <button
                            key={result.id}
                            type='button'
                            className='w-full text-left px-3 py-2 text-sm hover:bg-accent truncate'
                            onClick={() => handleSelect(result.position.lat, result.position.lon)}
                        >
                            {result.address.freeformAddress}
                        </button>
                    ))}
                </div>
            ) : null}
        </div>
    );
};
