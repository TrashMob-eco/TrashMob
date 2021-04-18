import React, { useState } from 'react';
import { Link } from 'react-router-dom';
import { Modal } from 'reactstrap';
import { updateAgreements } from '../store/accountHandler';
import { CurrentPrivacyPolicyVersion } from './PrivacyPolicy';
import { CurrentTermsOfServiceVersion } from './TermsOfService';

const AgreeToPolicies: React.FC = () => {
    const [agree, setAgree] = useState(false);
    const [isOpen, setIsOpen] = useState(false);

    const checkboxhandler = () => {
        // if agree === true, it will be set to false
        // if agree === false, it will be set to true
        setAgree(!agree);
    }

    function togglemodal() {
        setIsOpen(!isOpen);
    }

    return (
        <Modal isopen={isOpen} onrequestclose={togglemodal} contentlabel="accept terms of use">
            <div className="container">
                <div>
                    <input type="checkbox" id="agree" onChange={checkboxhandler} />
                    <label htmlFor="agree"> I agree to the TrashMob <Link to="./termsofservice">terms of use</Link> and the TrashMob <Link to="./privacypolicy">privacy policy</Link></label>
                </div>

                <div>
                    <button disabled={!agree} className="btn" onClick={() => {
                        updateAgreements(CurrentTermsOfServiceVersion.versionId, CurrentPrivacyPolicyVersion.versionId);
                        togglemodal();
                        }
                    }>
                        I Agree
                    </button>
                </div>
            </div>
        </Modal>
    );
};

export default AgreeToPolicies