import React, { useState } from 'react';
////import { Link } from 'react-router-dom';
////import { updateAgreements } from '../store/accountHandler';
////import { CurrentPrivacyPolicyVersion } from './PrivacyPolicy';
////import { CurrentTermsOfServiceVersion } from './TermsOfService';


////export const AgreeToPolicies: React.FC = () => {
////    const [agree, setAgree] = useState(false);
////    const [isOpen, setIsOpen] = useState(false);

////    const checkboxHandler = () => {
////        // if agree === true, it will be set to false
////        // if agree === false, it will be set to true
////        setAgree(!agree);
////    }

////    function toggleModal() {
////        setIsOpen(!isOpen);
////    }

////    return (
////    //    <Modal isOpen={isOpen} onRequestClose={toggleModal} contentLabel="Accept Terms of Use">
////    //        <div className="container">
////    //            <div>
////    //                <input type="checkbox" id="agree" onChange={checkboxHandler} />
////    //                <label htmlFor="agree"> I agree to the <Link to="./TermsOfService">Terms of Use</Link> and the <Link to="./PrivacyPolicy">Privacy Policy</Link></label>
////    //            </div>

////    //            <div>
////    //                <button disabled={!agree} className="btn" onClick={() => {
////    //                    updateAgreements(CurrentTermsOfServiceVersion.versionId, CurrentPrivacyPolicyVersion.versionId);
////    //                    toggleModal();
////    //                    }
////    //                }>
////    //                    I Agree
////    //                </button>
////    //            </div>
////    //        </div>
////    //    </Modal>
////    );
////};
