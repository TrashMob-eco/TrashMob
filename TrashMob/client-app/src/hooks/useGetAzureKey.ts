import { useEffect, useState } from 'react';
import * as MapStore from '@/store/MapStore';

export const useGetAzureKey = () => {
    const [azureKey, setAzureKey] = useState<string>('');

    useEffect(() => {
        MapStore.getOption().then((opts) => {
            setAzureKey(opts.subscriptionKey);
        });
    }, []);

    return azureKey;
};
