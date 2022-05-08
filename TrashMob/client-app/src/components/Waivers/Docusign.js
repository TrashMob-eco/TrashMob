/**
 * DocuSign and related operations.
 */
import { toast } from 'react-toastify';
import anchorfields_pdf from './assets/anchorfields.pdf';

const docName = 'anchorfields.pdf';
const sdkString = 'codeEg_react';
const urlFrag = '/restapi/v2.1'; // DocuSign specific

/**
 * Asset files
 * Add assets (eg PDF files) to the top level public directory.
 * (NOT under /src.) They will be included with the packaged app.
 *
 */

class DocuSign {
    //
    // constructor for the class
    //
    constructor(app) {
        this.app = app;
        this.sendEnvelope = this.sendEnvelope.bind(this);
    }

    //
    // Instance methods
    //

    /**
     * Send an envelope, return results or error
     */
    async sendEnvelope() {
        // get the document
        let response = await fetch(anchorfields_pdf)
        if (!response || response.status !== 200) {
            const msg = `Could not fetch file ${anchorfields_pdf}`;
            console.log(msg);
            toast.error(msg, { autoClose: 10000 });
            return;
        }
        // See https://stackoverflow.com/a/39951543/64904
        const fileBlob = await response.blob();
        const reader = new FileReader();
        await new Promise(resolve => {
            reader.onloadend = resolve;
            reader.readAsDataURL(fileBlob);
        });
        const base64File = reader.result.split(',')[1];

        const envelopeRequest = {
            emailSubject: 'Please sign the attached document',
            status: 'sent',
            recipients: {
                signers: [
                    {
                        email: this.app.state.formEmail,
                        name: this.app.state.formName,
                        recipientId: '1',
                        tabs: {
                            signHereTabs: [
                                {
                                    anchorString: '/sn1/',
                                    anchorXOffset: '20',
                                    anchorUnits: 'pixels',
                                },
                            ],
                        },
                    },
                ],
            },
            documents: [
                {
                    name: docName,
                    fileExtension: 'pdf',
                    documentId: '1',
                    documentBase64: base64File,
                },
            ],
        };

        try {
            const url =
                `${this.app.state.baseUri}${urlFrag}` +
                `/accounts/${this.app.state.accountId}` +
                `/envelopes`;
            const response = await fetch(url, {
                method: 'POST',
                body: JSON.stringify(envelopeRequest),
                headers: new Headers({
                    Authorization: `Bearer ${this.app.state.accessToken}`,
                    Accept: `application/json`,
                    'Content-Type': 'application/json',
                    'X-DocuSign-SDK': sdkString,
                }),
            });
            const data = response && response.ok && (await response.json());
            let result;
            const headers = response.headers;
            const availableApiReqHeader = headers.get('X-RateLimit-Remaining');
            const availableApiRequests = availableApiReqHeader
                ? parseInt(availableApiReqHeader, 10)
                : undefined;
            const apiResetHeader = headers.get('X-RateLimit-Reset');
            const apiRequestsReset = apiResetHeader
                ? new Date(parseInt(apiResetHeader, 10) * 1000)
                : undefined;
            const traceId = headers.get('X-DocuSign-TraceToken') || undefined;
            if (response.ok) {
                result = {
                    success: true,
                    errorMsg: undefined,
                    envelopeId: data.envelopeId,
                    availableApiRequests,
                    apiRequestsReset,
                    traceId,
                };
            } else {
                result = {
                    success: false,
                    errorMsg: response && (await response.text()),
                    envelopeId: undefined,
                    availableApiRequests,
                    apiRequestsReset,
                    traceId,
                };
            }
            return result;
        } catch (e) {
            // Unfortunately we don't have access to the real
            // networking problem!
            // See https://medium.com/to-err-is-aaron/detect-network-failures-when-using-fetch-40a53d56e36
            const errorMsg =
                e.message === 'Failed to fetch'
                    ? 'Networking error—check your Internet and DNS connections'
                    : e.message;
            return {
                success: false,
                errorMsg,
                envelopeId: undefined,
                availableApiRequests: undefined,
                apiRequestsReset: undefined,
                traceId: undefined,
            };
        }
    }

    /**
     * Get envelope's status, return results or error
     */
    async getEnvelope() {
        try {
            const url =
                `${this.app.state.baseUri}${urlFrag}` +
                `/accounts/${this.app.state.accountId}` +
                `/envelopes/${this.app.state.responseEnvelopeId}`;
            const response = await fetch(url, {
                method: 'GET',
                headers: new Headers({
                    Authorization: `Bearer ${this.app.state.accessToken}`,
                    Accept: `application/json`,
                    'Content-Type': 'application/json',
                    'X-DocuSign-SDK': sdkString,
                }),
            });
            const data = response && response.ok && (await response.json());
            let result;
            const headers = response.headers;
            const availableApiReqHeader = headers.get('X-RateLimit-Remaining');
            const availableApiRequests = availableApiReqHeader
                ? parseInt(availableApiReqHeader, 10)
                : undefined;
            const apiResetHeader = headers.get('X-RateLimit-Reset');
            const apiRequestsReset = apiResetHeader
                ? new Date(parseInt(apiResetHeader, 10) * 1000)
                : undefined;
            const traceId = headers.get('X-DocuSign-TraceToken') || undefined;
            if (response.ok) {
                result = {
                    success: true,
                    errorMsg: undefined,
                    resultsEnvelopeJson: data,
                    availableApiRequests,
                    apiRequestsReset,
                    traceId,
                };
            } else {
                result = {
                    success: false,
                    errorMsg: response && (await response.text()),
                    resultsEnvelopeJson: undefined,
                    availableApiRequests,
                    apiRequestsReset,
                    traceId,
                };
            }
            return result;
        } catch (e) {
            // Unfortunately we don't have access to the real
            // networking problem!
            // See https://medium.com/to-err-is-aaron/detect-network-failures-when-using-fetch-40a53d56e36
            const errorMsg =
                e.message === 'Failed to fetch'
                    ? 'Networking error—check your Internet and DNS connections'
                    : e.message;
            return {
                success: false,
                errorMsg,
                resultsEnvelopeJson: undefined,
                availableApiRequests: undefined,
                apiRequestsReset: undefined,
                traceId: undefined,
            };
        }
    }

}

export default DocuSign;
