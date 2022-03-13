import * as React from 'react'

export const CurrentWaiverVersion: WaiverVersion = {
    versionId: "0.1",
    versionDate: new Date(2022, 2, 21, 0, 0, 0, 0)
}

export class WaiverVersion {
    versionId: string = "0.1";
    versionDate: Date = new Date(2022, 2, 21, 0, 0, 0, 0);
}

export const Waiver: React.FC = () => {
    return (
        <div>
            <h1>TrashMob.eco Volunteer Waiver </h1>
            <p>
                BY SIGNING THIS FORM YOU ARE RELEASING TRASHMOB.ECO AND LAND MANAGERS FROM ANY AND ALL LIABILITY IN THE EVENT YOU ARE INJURED OR KILLED WHILE PARTICIPATING IN ANY PROJECT SPONSORED BY TRASHMOB.ECO, AS WELL AS FOR ANY POTENTIAL EXPOSURE TO AN ILLNESS OR DISEASE, INCLUDING, BUT NOT LIMITED TO, NOVEL CORONAVIRUS OR COVID-19, AND LYME DISEASE OR ILLNESS CAUSED BY TICKS.
            </p>
            <p>
                I wish to participate in projects sponsored by TrashMob.eco and I hereby acknowledge that said organization is doing everything they can to protect the public as well as myself as a volunteer. By signing below, I agree to comply with the written instructions included below. Failure to comply with these written instructions or verbal instructions from staff may result in my volunteer privileges being removed and I may be asked to leave the premises.
            </p>
            <p>
                I am also aware that projects sponsored by TrashMob.eco involve the maintenance of parks, trails and roadsides, and that participation in these projects poses certain dangers, including, but not limited to, the hazards of traveling in areas heavily traveled by motor vehicles, using hand tools (and of working in the proximity of such tools when used by others), injury or illness in remote places without medical aid, lifting and working in often difficult terrain and unforeseen events caused by the forces of nature.
            </p>
            <p>
                In consideration for permitting me to participate in projects sponsored by TrashMob.eco, I, for my family, my estate, heirs and assigns, and myself, hereby waive any right of recovery and any claims of liability or damages against TrashMob.eco, its officers, employees, volunteers, and agents, including, but not limited to, claims for bodily injury including death, illness, personal injury, and/or damage to property, and release TrashMob.eco, its officers, employees, volunteers, and agents from such claims and any claims made by others for personal injury or property damage allegedly caused by me. Further, I will defend, indemnify, and hold TrashMob.eco harmless from any loss or damages resulting from or arising in connection with my involvement in TrashMob.eco projects pursuant to the foregoing waiver and release. This Release and Indemnity Agreement is a contract and not a mere recital and it shall remain in effect for all projects sponsored by TrashMob.eco. The undersigned gives his/her permission to be photographed/filmed and have his/her image used by TrashMob.eco and their partner agencies, without royalty or compensation.
            </p>
            <p>
                I HAVE READ THIS RELEASE AND INDEMNITY AGREEMENT.
            </p>
            <p>
                Printed Name (participant or parent/guardian) Youth Name
            </p>
            <br />
            <br />
            <br />
            <p>
                Signature (participant or parent/guardian) Youth Initials (by initialing here, youth acknowledges they have read this agreement)
            </p>
            <br />
            <br />
            <br />
            <p>
                Date
            </p>
        </div>
    );
}
