import { useEffect, useState } from 'react';
import * as MapStore from '@/store/MapStore';

interface UseMapStoreReturn {
    azureSubscriptionKey: string;
}

export const useMapStore = (): UseMapStoreReturn => {
    const [azureSubscriptionKey, setAzureSubscriptionKey] = useState<string>('');

    useEffect(() => {
        MapStore.getOption().then((opts) => {
            setAzureSubscriptionKey(opts.subscriptionKey);
        });
    }, []);

    return {
        azureSubscriptionKey,
    };
};
