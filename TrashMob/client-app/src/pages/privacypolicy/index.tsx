import { Link } from 'react-router';
import { HeroSection } from '@/components/Customization/HeroSection';
import { Logo } from '@/components/Logo';

export const CurrentPrivacyPolicyVersion: PrivacyPolicyVersion = {
    versionId: '1.0',
    versionDate: new Date(2026, 1, 23, 0, 0, 0, 0),
};

export class PrivacyPolicyVersion {
    versionId: string = '1.0';

    versionDate: Date = new Date(2026, 1, 23, 0, 0, 0, 0);
}

export const PrivacyPolicy: React.FC = () => {
    return (
        <div>
            <HeroSection Title='Privacy Policy' Description='Making your privacy a priority.' />
            <div className='container my-5 prose space-y-4'>
                <p className='text-sm text-muted-foreground'>
                    Version {CurrentPrivacyPolicyVersion.versionId} — Effective{' '}
                    {CurrentPrivacyPolicyVersion.versionDate.toLocaleDateString('en-US', {
                        year: 'numeric',
                        month: 'long',
                        day: 'numeric',
                    })}
                </p>

                <h2 className='font-medium text-3xl'>Introduction</h2>

                <p className='text-lg'>
                    TrashMob.eco is a 501(c)(3) non-profit organization dedicated to community environmental cleanups.
                    This Privacy Policy describes how we collect, use, and protect your personal information when you
                    use our website at www.trashmob.eco and the TrashMob mobile apps.
                </p>

                <p className='text-lg'>
                    We do not sell your personal data. We do not use your data for advertising or behavioral profiling.
                    Your data is used solely to operate the TrashMob platform and coordinate community cleanups.
                </p>

                <p className='text-lg'>
                    If you have questions about this policy, contact us at{' '}
                    <a href='mailto:info@trashmob.eco'>info@trashmob.eco</a>.
                </p>

                <h2 className='font-medium text-3xl'>Information We Collect</h2>

                <h3 className='font-medium text-2xl'>Account Information</h3>
                <p className='text-lg'>
                    When you register, we collect your name, email address, and a display username. You may optionally
                    provide your date of birth (used for age verification) and a profile photo.
                </p>

                <h3 className='font-medium text-2xl'>Location Information</h3>
                <p className='text-lg'>
                    You may provide your city, state/region, country, and postal code to receive notifications about
                    nearby cleanup events. If you use route tracking in the mobile app during an event, we collect GPS
                    coordinates of your cleanup route. Route tracking is opt-in and only active while you choose to
                    record.
                </p>

                <h3 className='font-medium text-2xl'>Event Activity</h3>
                <p className='text-lg'>
                    We record your event registrations, attendance, and any cleanup metrics you submit (such as bags
                    collected, weight picked up, and duration). You may also upload photos documenting cleanup events
                    and submit litter reports.
                </p>

                <h3 className='font-medium text-2xl'>Waiver Information</h3>
                <p className='text-lg'>
                    Before participating in events, you sign a liability waiver. We record your typed legal name, the
                    date of signing, and technical details (IP address, browser) for legal verification. For minor
                    participants (ages 13-17), we also collect guardian name and relationship.
                </p>

                <h3 className='font-medium text-2xl'>Technical Information</h3>
                <p className='text-lg'>
                    We collect crash logs and device information from the mobile app for stability improvements, and
                    server logs (IP addresses, timestamps) for security purposes. This data is retained for 90 days.
                </p>

                <h2 className='font-medium text-3xl'>How We Use Your Information</h2>

                <ul>
                    <li>Coordinate cleanup events and send you relevant notifications</li>
                    <li>Display community impact metrics (total bags, weight, events — in aggregate)</li>
                    <li>Verify your identity and age for event participation</li>
                    <li>Maintain signed waivers for legal compliance</li>
                    <li>Improve app stability and performance</li>
                    <li>Communicate with you about your account, events, and TrashMob updates</li>
                </ul>

                <p className='text-lg'>
                    We do <strong>not</strong> use your information for advertising, behavioral profiling, or data
                    sales.
                </p>

                <h2 className='font-medium text-3xl'>Third-Party Services</h2>

                <p className='text-lg'>
                    We use the following third-party services to operate TrashMob. Each processes data as described
                    below:
                </p>

                <ul>
                    <li>
                        <strong>Microsoft Azure</strong> — Hosting, database storage, file storage, and map services.{' '}
                        <a href='https://privacy.microsoft.com' target='_blank' rel='noopener noreferrer'>
                            Privacy Policy
                        </a>
                    </li>
                    <li>
                        <strong>Microsoft Entra ID</strong> — User authentication (sign-in).{' '}
                        <a href='https://privacy.microsoft.com' target='_blank' rel='noopener noreferrer'>
                            Privacy Policy
                        </a>
                    </li>
                    <li>
                        <strong>SendGrid (Twilio)</strong> — Email delivery for event notifications and account
                        communications.{' '}
                        <a href='https://www.twilio.com/legal/privacy' target='_blank' rel='noopener noreferrer'>
                            Privacy Policy
                        </a>
                    </li>
                    <li>
                        <strong>Sentry.io</strong> — Mobile app crash reporting and error tracking.{' '}
                        <a href='https://sentry.io/privacy/' target='_blank' rel='noopener noreferrer'>
                            Privacy Policy
                        </a>
                    </li>
                    <li>
                        <strong>Google Maps</strong> — Map display in the Android mobile app.{' '}
                        <a href='https://policies.google.com/privacy' target='_blank' rel='noopener noreferrer'>
                            Privacy Policy
                        </a>
                    </li>
                </ul>

                <h2 className='font-medium text-3xl'>Data Sharing</h2>

                <p className='text-lg'>
                    We do not sell, rent, or share your personal data with advertisers or data brokers.
                </p>

                <p className='text-lg'>Your data may be visible to other TrashMob users in these limited ways:</p>

                <ul>
                    <li>
                        Your username is displayed publicly on events you attend and on leaderboards (if you opt in)
                    </li>
                    <li>Event leads can see the names of registered attendees for their events</li>
                    <li>Site administrators can access user data for platform management and support</li>
                    <li>Your email address is never displayed publicly</li>
                </ul>

                <h2 className='font-medium text-3xl'>Data Retention</h2>

                <ul>
                    <li>
                        <strong>Active accounts:</strong> Your data is retained while your account is active.
                    </li>
                    <li>
                        <strong>Deleted accounts:</strong> When you delete your account, all personal information is
                        removed immediately. Anonymized records (with no link to your identity) are preserved for
                        community impact metrics such as total bags collected and events held.
                    </li>
                    <li>
                        <strong>Waivers:</strong> Signed waiver records are retained for up to 7 years from the event
                        date for legal compliance. Your name is anonymized immediately upon account deletion, but the
                        waiver record is preserved.
                    </li>
                    <li>
                        <strong>Technical logs:</strong> Crash reports and server logs are automatically deleted after
                        90 days.
                    </li>
                </ul>

                <h2 className='font-medium text-3xl'>Your Data Rights</h2>

                <p className='text-lg'>You have the following rights regarding your personal data:</p>

                <ul>
                    <li>
                        <strong>Right to access and portability:</strong> You can download all your personal data as a
                        JSON file from your{' '}
                        <Link to='/myprofile' className='underline'>
                            profile page
                        </Link>{' '}
                        using the "Download My Data" button. Exports are available once per 24 hours.
                    </li>
                    <li>
                        <strong>Right to correction:</strong> You can edit your profile information at any time from
                        your{' '}
                        <Link to='/myprofile' className='underline'>
                            profile page
                        </Link>
                        .
                    </li>
                    <li>
                        <strong>Right to deletion:</strong> You can permanently delete your account and all associated
                        personal data from the{' '}
                        <Link to='/deletemydata' className='underline'>
                            Delete My Data page
                        </Link>
                        . This action is immediate and irreversible.
                    </li>
                    <li>
                        <strong>Right to object:</strong> You can unsubscribe from email notifications through your
                        account settings or opt out of public leaderboards.
                    </li>
                    <li>
                        <strong>Other requests:</strong> For any other data rights requests, contact us at{' '}
                        <a href='mailto:info@trashmob.eco'>info@trashmob.eco</a>. We will respond within 30 days.
                    </li>
                </ul>

                <h2 className='font-medium text-3xl'>CCPA Privacy Rights</h2>

                <p className='text-lg'>
                    Under the California Consumer Privacy Act (CCPA), California residents have the right to:
                </p>

                <ul>
                    <li>Request disclosure of the categories and specific pieces of personal data we have collected</li>
                    <li>Request deletion of personal data</li>
                    <li>Opt out of the sale of personal data — TrashMob does not sell personal data</li>
                </ul>

                <p className='text-lg'>
                    You can exercise your access and deletion rights using the self-service tools described above, or
                    contact us at <a href='mailto:info@trashmob.eco'>info@trashmob.eco</a>.
                </p>

                <h2 className='font-medium text-3xl'>Children's Privacy</h2>

                <p className='text-lg'>
                    Users must be at least 13 years old to create a TrashMob account. We do not knowingly collect
                    personal information from children under 13. If you believe a child under 13 has created an account,
                    please contact us immediately at <a href='mailto:info@trashmob.eco'>info@trashmob.eco</a>.
                </p>

                <p className='text-lg'>
                    Users aged 13-17 have enhanced privacy protections by default. Minor usernames may be hidden from
                    certain public displays, and parental or guardian consent is required for event participation
                    through our waiver system.
                </p>

                <h2 className='font-medium text-3xl'>Cookies and Tracking</h2>

                <p className='text-lg'>
                    TrashMob uses minimal cookies, limited to authentication session management. We do not use
                    third-party advertising cookies, cross-site tracking pixels, or behavioral analytics that follow you
                    across other websites. Application performance monitoring is handled through Azure Application
                    Insights, which does not perform cross-site tracking.
                </p>

                <h2 className='font-medium text-3xl'>Data Security</h2>

                <p className='text-lg'>We protect your data through:</p>

                <ul>
                    <li>Encryption at rest for all stored data (Azure SQL and Blob Storage)</li>
                    <li>Encryption in transit via TLS 1.2+ for all connections</li>
                    <li>Role-based access controls limiting who can access user data</li>
                    <li>Azure Key Vault for managing application secrets</li>
                    <li>Photo moderation pipeline to review user-uploaded images before public display</li>
                    <li>Transaction-wrapped account deletion ensuring complete and atomic data removal</li>
                </ul>

                <h2 className='font-medium text-3xl'>Changes to This Policy</h2>

                <p className='text-lg'>
                    We may update this Privacy Policy when our data practices change. The version number and effective
                    date are shown at the top of this page. We encourage you to review this page periodically.
                </p>

                <h2 className='font-medium text-3xl'>Contact Us</h2>

                <p className='text-lg'>
                    If you have any questions about this Privacy Policy or your personal data, contact us at:
                </p>

                <p className='text-lg'>
                    Email: <a href='mailto:info@trashmob.eco'>info@trashmob.eco</a>
                    <br />
                    TrashMob.eco
                    <br />
                    Washington State, USA
                </p>

                <p className='text-lg mt-8'>The team at TrashMob.eco.</p>
                <div className='mb-5'>
                    <Logo className='w-96' />
                </div>
            </div>
        </div>
    );
};
