import React from 'react';
import { useForm, Controller, SubmitHandler } from 'react-hook-form';
import { useHistory } from 'react-router-dom';
import { useQueryClient, useMutation } from '@tanstack/react-query';
import UserData from '../Models/UserData';
import { HeroSection } from '../Customization/HeroSection';
import { GetUserById, UpdateUser } from '../../services/users';
import { Logo } from '../Logo';
import { Button } from '@/components/ui/button';
import { Checkbox } from '../ui/checkbox';

type WaiverFormInputs = {
    userId: string;
    accepted: boolean;
};

export interface WaiversProps {
    currentUser: UserData;
    onUserUpdated: () => void;
}

export const CurrentTrashMobWaiverVersion = {
    versionId: '1.0',
    versionDate: new Date(2023, 6, 1, 0, 0, 0, 0),
};

const Waivers: React.FC<WaiversProps> = ({ currentUser, onUserUpdated }) => {
    const history = useHistory();
    const queryClient = useQueryClient();
    const userId = currentUser.id;

    const {
        handleSubmit,
        control,
        getValues,
        formState: { isValid, errors },
    } = useForm<WaiverFormInputs>({
        defaultValues: {
            userId: currentUser.id,
            accepted: false,
        },
    });

    const { mutate } = useMutation({
        mutationKey: UpdateUser().key,
        mutationFn: UpdateUser().service,
        onSuccess: async () => {
            // Invalidate query
            await queryClient.invalidateQueries(GetUserById({ userId }).key);
            onUserUpdated();

            // Then redirect to home
            history.push('/');
        },
    });

    const onSubmit: SubmitHandler<WaiverFormInputs> = (data) => {
        mutate({
            ...currentUser,
            dateAgreedToTrashMobWaiver: new Date(),
            trashMobWaiverVersion: CurrentTrashMobWaiverVersion.versionId,
        });
    };

    return (
        <div className='tailwind'>
            <HeroSection Title='Waiver' Description='Safety first!' />
            <div className='container'>
                <h2 className='font-medium text-3xl'>TrashMob.eco Waiver</h2>
                <p className='p-18'>
                    You will only need to sign this waiver once unless we have to change the legalese.
                </p>

                <hr />
                <div>
                    <style type='text/css'>
                        {`
                            li, p {
                                margin-bottom: 1em;
                            }
                    `}
                    </style>
                    <h2 className='font-medium text-3xl'>TrashMob.eco</h2>
                    <h5>Volunteer Release and Waiver of Liability Form</h5>
                    <p>
                        The Volunteer Release and Waiver of Liability Form (“Release”). (“Volunteer”) hereby releases
                        TRASHMOB.ECO, ITS OFFICERS AND DIRECTORS, ANY LAND OWNERS AND MANAGERS, AND ALL RELATED SPONSORS
                        (“PARTIES”) JOINTLY AND SEVERALLY, AND INDIVIDUALLY.
                    </p>
                    <p>
                        The Volunteer desires to provide volunteer services and to participate in activities related to
                        serving as a volunteer for events facilitated by TrashMob.eco. Volunteer services (“Services”)
                        will be broadly defined as picking up trash at TrashMob.eco facilitated events (“Events”). The
                        above Volunteer hereby agrees as follows:
                    </p>
                    <ol>
                        <li>
                            WAIVER AND RELEASE: I, the Volunteer, release and forever discharge and hold harmless the
                            Parties from any and all liability, claims, and demands of whatever kind or nature, either
                            in law or in equity, which arise or may hereafter arise as result of my participation in
                            such Events. I understand and acknowledge that this Release discharges from any liability or
                            claim that I may have with respect to bodily injury, personal injury, illness, death, or
                            property damage that may result from the Services I am providing by participating in the
                            Event.
                        </li>
                        <li>
                            INSURANCE: I, the Volunteer, understand that none of the Parties assume any responsibility
                            for or obligation to provide me with financial or other assistance, including but not
                            limited to medical, health, or disability benefits or insurance of any nature in the event
                            of my injury, illness, death or damage to my property. I expressly waive any such claim for
                            compensation or liability from the Parties.
                        </li>
                        <li>
                            Medical Treatment: I, the Volunteer, do hereby release and forever discharge the Parties
                            from any claim whatsoever which arises or may hereafter arise on account of any first aid,
                            treatment, or service rendered by any person in connection with an emergency during my
                            tenure as a volunteer during the course of an Event.
                        </li>
                        <li>
                            Assumption of the Risk: I, the Volunteer, acknowledge that there are potential hazards
                            (“Hazards”), known and unknown, involved in the Event. The term “Hazards” is intended to be
                            used in its broadest sense and includes, but is not limited to natural hazards (land,
                            weather, etc.) and man-made hazards (concrete, steel, etc.). I understand and acknowledge
                            that participation may include Hazards that could harm me, and that such Hazards may or may
                            not always be obvious. I hereby expressly and specifically assume the risk of injury or harm
                            for all such Hazards.
                        </li>
                        <li>
                            PHOTOGRAPHIC RELEASE: I, the Volunteer, grant and convey to the Parties all right, title,
                            and interest in any and all photographs, images, video, and audio in connection with my
                            providing volunteer services at Events.
                        </li>
                        <li>
                            Other: I, the Volunteer, expressly agrees that this Release is intended to be as broad and
                            inclusive as permitted by law. I agree that in the event that any clause or provision of
                            this Release shall be held to be invalid by any court of competent jurisdiction, the
                            validity of the remaining provisions of this Release shall continue to be enforceable.
                        </li>
                    </ol>
                </div>
                <form onSubmit={handleSubmit(onSubmit)} className='mt-4'>
                    <Controller
                        name='accepted'
                        control={control}
                        rules={{ required: true }}
                        render={({ field }) => (
                            <div className='flex items-center space-x-2 !mb-4'>
                                <Checkbox
                                    id='i-have-read-and-accept'
                                    checked={field.value}
                                    onCheckedChange={field.onChange}
                                />
                                <label
                                    htmlFor='i-have-read-and-accept'
                                    className='text-sm font-medium !mb-0 leading-none peer-disabled:cursor-not-allowed peer-disabled:opacity-70'
                                >
                                    I HAVE READ THIS RELEASE AND WAIVER FORM
                                </label>
                            </div>
                        )}
                    />
                    <Button disabled={!isValid} type='submit' className='font-semibold'>
                        Sign Waiver
                    </Button>
                </form>
                <p className='p-18 mb-5'>Thank you!</p>
                <p className='p-18'>The team at TrashMob.eco.</p>
                <div className='mb-5'>
                    <Logo className='w-96 max-w-full' />
                </div>
            </div>
        </div>
    );
};

export default Waivers;
