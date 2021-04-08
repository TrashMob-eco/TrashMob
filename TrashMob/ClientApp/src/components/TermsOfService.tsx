import { Component } from 'react';
import * as React from 'react'

export const CurrentTermsOfServiceVersion: TermsOfServiceVersion = {
    versionId: "0.1",
    versionDate: new Date(2021, 4, 1, 0, 0, 0, 0)
}

export class TermsOfService extends Component {
    render() {
        return (
            <div>
                <h1>Terms of Service</h1>
            </div>
        );
    }
}

export class TermsOfServiceVersion {
    versionId: string = "0.1";
    versionDate: Date = new Date(2021, 4, 1, 0, 0, 0, 0);
}
