import { FC, useEffect, useMemo } from 'react';
import { useLocation } from 'react-router-dom';
import UserData from '../Models/UserData';

export interface WaiversReturnProps {
    isUserLoaded: boolean;
    currentUser: UserData;
};

const WaiversReturn: FC<WaiversReturnProps> = ({ currentUser, isUserLoaded }) => {

    useEffect(() => {
        if (!isUserLoaded || !currentUser) {
            return;
        }
    }, [isUserLoaded, currentUser]);

    // A custom hook that builds on useLocation to parse
    // the query string for you.
    function useQuery() {
        const { search } = useLocation();

        return useMemo(() => new URLSearchParams(search), [search]);
    }

    var query = useQuery();
    var eventStatus = query.get("event");

    return (
        <div className="container-fluid card">
            <h1>Waivers Return</h1>
            <p>
                Signing Status: {eventStatus}
            </p>
        </div>
    );
}

export default WaiversReturn;
