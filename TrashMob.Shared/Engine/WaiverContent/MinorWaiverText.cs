namespace TrashMob.Shared.Engine.WaiverContent
{
    /// <summary>
    /// Draft minor waiver text for dependents brought to cleanup events by adults.
    /// DRAFT — Requires legal review and approval before use in production.
    /// </summary>
    public static class MinorWaiverText
    {
        /// <summary>
        /// Full waiver text for a parent or legal guardian signing on behalf of a dependent minor.
        /// Placeholders are NOT used — the waiver is generic and the dependent's details
        /// are captured separately in the DependentWaiver and Dependent records.
        /// </summary>
        public const string ParentGuardianWaiver = """
            # TrashMob.eco Minor Participant Waiver and Release of Liability

            ## 1. Statement of Guardian Authority

            By signing this waiver, I certify that I am the parent, legal guardian, or person with legal authority over the minor named in this waiver. I have the legal right to consent to the minor's participation in TrashMob.eco cleanup events and to execute this waiver on their behalf.

            ## 2. Assumption of Risk

            I understand that participation in outdoor litter cleanup events involves inherent risks, including but not limited to:

            - Contact with sharp objects, broken glass, or hazardous debris
            - Walking on uneven terrain, slopes, or near waterways
            - Proximity to vehicular traffic
            - Exposure to weather conditions (heat, cold, rain, wind)
            - Potential contact with hazardous materials, chemicals, or biological waste
            - Insect bites, stings, or encounters with wildlife
            - Physical exertion, fatigue, or dehydration

            I acknowledge these risks and voluntarily choose to allow the minor to participate despite them.

            ## 3. Liability Release and Indemnification

            I, on behalf of myself, the minor, and our heirs, executors, and assigns, hereby release, waive, and forever discharge TrashMob.eco, its officers, directors, employees, volunteers, event organizers, community partners, sponsors, and affiliated organizations (collectively, the "Released Parties") from any and all liability, claims, demands, or causes of action arising out of or related to the minor's participation in any TrashMob.eco cleanup event, including but not limited to claims for personal injury, illness, death, or property damage.

            I agree to indemnify and hold harmless the Released Parties from any claims, damages, or expenses (including reasonable attorney's fees) arising from the minor's participation or from any breach of this waiver.

            ## 4. Medical Authorization

            In the event of injury, illness, or medical emergency involving the minor during a TrashMob.eco event, I authorize TrashMob.eco, event organizers, and their designated representatives to arrange emergency medical treatment for the minor at my expense. I understand that TrashMob.eco does not provide medical insurance for participants and that I am responsible for any medical costs incurred.

            I have disclosed any known medical conditions, allergies, or medications relevant to the minor's participation in the medical notes section of the minor's profile.

            ## 5. Photo and Media Release

            I grant TrashMob.eco and its partners permission to photograph, video record, and use the minor's likeness in connection with TrashMob.eco events for promotional, educational, and social media purposes. I waive any right to inspect or approve the finished product or the copy that may be used in connection with these images.

            ## 6. Supervision Acknowledgment

            I understand that I am responsible for the direct supervision of the minor during all TrashMob.eco events. I will ensure that the minor follows all safety instructions provided by event organizers. I understand that TrashMob.eco event organizers are not responsible for supervising individual participants, including minors.

            ## 7. Activity Acknowledgment

            I understand that TrashMob.eco cleanup events involve outdoor litter and debris collection, which may include walking on uneven surfaces, handling litter and debris with provided equipment, exposure to weather conditions, and proximity to traffic or other environmental hazards. I confirm that the minor is physically capable of participating in these activities.

            ---

            **By signing below, I acknowledge that I have read this waiver in its entirety, understand its terms, and agree to be bound by them on behalf of myself and the minor.**
            """;

        /// <summary>
        /// Full waiver text for an authorized supervisor (non-parent/guardian) signing on behalf of a dependent minor.
        /// Includes additional certification that the supervisor has been authorized by the parent/legal guardian.
        /// </summary>
        public const string AuthorizedSupervisorWaiver = """
            # TrashMob.eco Minor Participant Waiver and Release of Liability
            ## Authorized Supervisor Form

            ## 1. Statement of Authorized Supervision

            By signing this waiver, I certify that I have been expressly authorized by the parent or legal guardian of the minor named in this waiver to supervise the minor during TrashMob.eco cleanup events. I understand that I am assuming responsibility for the minor's safety and well-being during the event.

            I confirm that the parent or legal guardian has provided me with written or verbal authorization to consent to the minor's participation and to execute this waiver on their behalf.

            ## 2. Assumption of Risk

            I understand that participation in outdoor litter cleanup events involves inherent risks, including but not limited to:

            - Contact with sharp objects, broken glass, or hazardous debris
            - Walking on uneven terrain, slopes, or near waterways
            - Proximity to vehicular traffic
            - Exposure to weather conditions (heat, cold, rain, wind)
            - Potential contact with hazardous materials, chemicals, or biological waste
            - Insect bites, stings, or encounters with wildlife
            - Physical exertion, fatigue, or dehydration

            I acknowledge these risks and voluntarily choose to allow the minor to participate despite them.

            ## 3. Liability Release and Indemnification

            I, on behalf of myself and to the extent authorized by the parent or legal guardian, hereby release, waive, and forever discharge TrashMob.eco, its officers, directors, employees, volunteers, event organizers, community partners, sponsors, and affiliated organizations (collectively, the "Released Parties") from any and all liability, claims, demands, or causes of action arising out of or related to the minor's participation in any TrashMob.eco cleanup event, including but not limited to claims for personal injury, illness, death, or property damage.

            ## 4. Medical Authorization

            In the event of injury, illness, or medical emergency involving the minor during a TrashMob.eco event, I authorize TrashMob.eco, event organizers, and their designated representatives to arrange emergency medical treatment for the minor at my expense or the expense of the parent/legal guardian. I have been authorized by the parent or legal guardian to consent to emergency medical treatment on behalf of the minor.

            I have reviewed any known medical conditions, allergies, or medications relevant to the minor's participation as disclosed in the minor's profile.

            ## 5. Photo and Media Release

            On behalf of the parent or legal guardian, I grant TrashMob.eco and its partners permission to photograph, video record, and use the minor's likeness in connection with TrashMob.eco events for promotional, educational, and social media purposes.

            ## 6. Supervision Acknowledgment

            I confirm that I have been authorized by the parent or legal guardian of the minor to supervise this child during TrashMob.eco events. I understand that I am personally responsible for the direct supervision of the minor during all event activities. I will ensure that the minor follows all safety instructions provided by event organizers.

            ## 7. Activity Acknowledgment

            I understand that TrashMob.eco cleanup events involve outdoor litter and debris collection, which may include walking on uneven surfaces, handling litter and debris with provided equipment, exposure to weather conditions, and proximity to traffic or other environmental hazards. I confirm that, to the best of my knowledge, the minor is physically capable of participating in these activities.

            ---

            **By signing below, I acknowledge that I have read this waiver in its entirety, understand its terms, and agree to be bound by them. I further certify that I have been authorized by the minor's parent or legal guardian to sign this waiver and supervise the minor during this event.**
            """;
    }
}
