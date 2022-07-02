import { FC } from 'react'
import { Col, Container, Image, Row } from 'react-bootstrap';
import globes from '../assets/gettingStarted/globes.png';
import logo from '../assets/logo.svg';

export const CurrentTermsOfServiceVersion: TermsOfServiceVersion = {
    versionId: "0.3",
    versionDate: new Date(2021, 5, 14, 0, 0, 0, 0)
}

export class TermsOfServiceVersion {
    versionId: string = "0.1";
    versionDate: Date = new Date(2021, 4, 1, 0, 0, 0, 0);
}

export const TermsOfService: FC = () => {
    return (
        <>
            <Container fluid className='bg-grass mb-5'>
                <Row className="text-center pt-0">
                    <Col md={7} className="d-flex flex-column justify-content-center pr-5">
                        <h1 className="font-weight-bold">Terms of Service</h1>
                        <p className="font-weight-bold">Transparency matters to us.</p>
                    </Col>
                    <Col md={5}>
                        <Image src={globes} alt="globes" className="h-100 mt-0" />
                    </Col>
                </Row>
            </Container>
            <Container className="py-5">
                <h5 className="font-weight-bold">1. Terms</h5>

                <p>By accessing this Website, accessible from www.trashmob.eco, you are agreeing to be bound by these Website Terms and Conditions of Use and agree that you are responsible for the agreement with any applicable local laws. If you disagree with any of these terms, you are prohibited from accessing this site. The materials contained in this Website are protected by copyright and trade mark law.</p>

                <h5 className="font-weight-bold">2. Use License</h5>

                <p>Permission is granted to temporarily download one copy of the materials on TrashMob's Website for personal, non-commercial transitory viewing only. This is the grant of a license, not a transfer of title, and under this license you may not:</p>

                <ul>
                    <li>Modify or copy the materials;</li>
                    <li>Use the materials for any commercial purpose or for any public display;</li>
                    <li>Attempt to reverse engineer any software contained on TrashMob's Website;</li>
                    <li>Remove any copyright or other proprietary notations from the materials; or</li>
                    <li>Transferring the materials to another person or "mirror" the materials on any other server.</li>
                </ul>

                <p>This will let TrashMob to terminate upon violations of any of these restrictions. Upon termination, your viewing right will also be terminated and you should destroy any downloaded materials in your possession whether it is printed or electronic format. These Terms of Service has been created with the help of the <a href="https://www.termsofservicegenerator.net">Terms Of Service Generator</a> and the <a href="https://www.generateprivacypolicy.com">Privacy Policy Generator</a>.</p>

                <h5 className="font-weight-bold">3. Disclaimer</h5>

                <p>All the materials on TrashMob’s Website are provided "as is". TrashMob makes no warranties, may it be expressed or implied, therefore negates all other warranties. Furthermore, TrashMob does not make any representations concerning the accuracy or reliability of the use of the materials on its Website or otherwise relating to such materials or any sites linked to this Website.</p>

                <h5 className="font-weight-bold">4. Limitations</h5>

                <p>TrashMob or its suppliers will not be hold accountable for any damages that will arise with the use or inability to use the materials on TrashMob’s Website, even if TrashMob or an authorize representative of this Website has been notified, orally or written, of the possibility of such damage. Some jurisdiction does not allow limitations on implied warranties or limitations of liability for incidental damages, these limitations may not apply to you.</p>

                <h5 className="font-weight-bold">5. Revisions and Errata</h5>

                <p>The materials appearing on TrashMob’s Website may include technical, typographical, or photographic errors. TrashMob will not promise that any of the materials in this Website are accurate, complete, or current. TrashMob may change the materials contained on its Website at any time without notice. TrashMob does not make any commitment to update the materials.</p>

                <h5 className="font-weight-bold">6. Links</h5>

                <p>TrashMob has not reviewed all of the sites linked to its Website and is not responsible for the contents of any such linked site. The presence of any link does not imply endorsement by TrashMob of the site. The use of any linked website is at the user’s own risk.</p>

                <h5 className="font-weight-bold">7. Site Terms of Use Modifications</h5>

                <p>TrashMob may revise these Terms of Use for its Website at any time without prior notice. By using this Website, you are agreeing to be bound by the current version of these Terms and Conditions of Use.</p>

                <h5 className="font-weight-bold">8. Your Privacy</h5>

                <p>Please read our Privacy Policy.</p>

                <h5 className="font-weight-bold">9. Governing Law</h5>

                <p>Any claim related to TrashMob's Website shall be governed by the laws of us without regards to its conflict of law provisions.</p>

                <p className="mt-5">The team at TrashMob.eco</p>
                <img src={logo} className="logo mb-5" alt="TrashMob Logo" />
            </Container>
        </>
    );
}
