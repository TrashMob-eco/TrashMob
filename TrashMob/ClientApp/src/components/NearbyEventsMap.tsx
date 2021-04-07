import React, { useEffect, useState } from 'react';
import { AzureMap, AzureMapsProvider, IAzureMapOptions } from 'react-azure-maps'
import { getOption } from '../store/MapStore';

const option: IAzureMapOptions = {
    authOptions: {},
}

const NearbyEventsMap: React.FC = () => {
    const [isKeyLoaded, setIsKeyLoaded] = useState(false);

    // componentDidMount()
    useEffect(() => {
        // simulate fetching subscriptionKey from Key Vault
        async function GetMap() {
            option.authOptions = await getOption()
            setIsKeyLoaded(true);            
        } 

        GetMap();
    }, []);

    // render()
    return (
        <AzureMapsProvider>
            <div style={{ height: '300px' }}>
                {isKeyLoaded && <AzureMap options={option} />}
                {!isKeyLoaded && <div>Map is loading.</div>}
            </div>
        </AzureMapsProvider>
    );
}

export default NearbyEventsMap
