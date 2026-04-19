namespace TrashMobMobile.ViewModels
{
    using TrashMob.Models;

    /// <summary>
    /// Wraps a Dependent with its active invitation (if any) for display in the My Dependents page.
    /// </summary>
    public class DependentWithInvitation
    {
        public DependentWithInvitation(Dependent dependent, DependentInvitation? activeInvitation = null, int? privoConsentStatus = null)
        {
            Dependent = dependent;
            ActiveInvitation = activeInvitation;
            PrivoConsentStatus = privoConsentStatus;
        }

        public Dependent Dependent { get; }

        public DependentInvitation? ActiveInvitation { get; set; }

        public int? PrivoConsentStatus { get; set; }

        public int Age
        {
            get
            {
                var today = DateOnly.FromDateTime(DateTime.UtcNow);
                var dob = Dependent.DateOfBirth;
                var age = today.Year - dob.Year;
                if (dob > today.AddYears(-age)) age--;
                return age;
            }
        }

        /// <summary>PRIVO consent has been approved for this dependent.</summary>
        public bool IsConsentApproved => PrivoConsentStatus == 2;

        /// <summary>PRIVO consent is pending approval.</summary>
        public bool IsConsentPending => PrivoConsentStatus == 1;

        /// <summary>Dependent is 13+ and needs PRIVO consent initiated.</summary>
        public bool NeedsConsent => Age >= 13 && !IsConsentApproved && !IsConsentPending && !HasAcceptedInvitation;

        public bool IsEligibleForInvite => Age >= 13 && IsConsentApproved && !HasActiveInvitation && !HasAcceptedInvitation;

        public bool HasActiveInvitation =>
            ActiveInvitation != null &&
            ActiveInvitation.InvitationStatusId == (int)InvitationStatusEnum.Sent;

        public bool HasAcceptedInvitation =>
            ActiveInvitation != null &&
            ActiveInvitation.InvitationStatusId == (int)InvitationStatusEnum.Accepted;

        public string InvitationStatusText
        {
            get
            {
                if (HasAcceptedInvitation) return "Account Created";
                if (HasActiveInvitation) return "Invited";
                if (IsConsentPending) return "Consent Pending";
                if (NeedsConsent) return "Needs Consent";
                if (IsEligibleForInvite) return "Ready to Invite";
                return string.Empty;
            }
        }

        public bool ShowStatusBadge => !string.IsNullOrEmpty(InvitationStatusText);
    }
}
