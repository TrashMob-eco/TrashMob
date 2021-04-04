import * as React from 'react';

import { AzureMap, AzureMapsProvider } from 'react-azure-maps';
import { getOption } from '../store/MapStore';

export const NearbyEventsMap: React.FC = () => (
        <AzureMapsProvider>
            <div style={{ height: '300px' }}>
            <AzureMap options={getOption()} />
            </div>
        </AzureMapsProvider>
    );
