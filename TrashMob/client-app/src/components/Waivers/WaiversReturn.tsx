import React from 'react';
import UserData from '../Models/UserData';

export interface WaiversReturnProps {
    isUserLoaded: boolean;
    currentUser: UserData;
};

const WaiversReturn: React.FC<WaiversReturnProps> = (props) => {
    return (
        <div className="container-fluid card">
            <h1>Waivers Return</h1>
        </div>
    );
}

export default WaiversReturn;
