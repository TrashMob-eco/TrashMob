import { Component } from 'react';
import * as React from 'react'

export const CurrentPrivacyPolicyVersion: PrivacyPolicyVersion = {
    versionId: "0.1",
    versionDate: new Date(2021, 4, 1, 0, 0, 0, 0)
}

export class PrivacyPolicy extends Component {
    render() {
        return (
            <div>
                <h1>Privacy Policy</h1>
            </div>
        );
    }
}

export class PrivacyPolicyVersion {
    versionId: string = "0.1";
    versionDate: Date = new Date(2021, 4, 1, 0, 0, 0, 0);
}
