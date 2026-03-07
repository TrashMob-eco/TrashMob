namespace TrashMobMobile.ViewModels
{
    using TrashMob.Models;

    /// <summary>
    /// Wraps a Dependent with its active invitation (if any) for display in the My Dependents page.
    /// </summary>
    public class DependentWithInvitation
    {
        public DependentWithInvitation(Dependent dependent, DependentInvitation? activeInvitation = null)
        {
            Dependent = dependent;
            ActiveInvitation = activeInvitation;
        }

        public Dependent Dependent { get; }

        public DependentInvitation? ActiveInvitation { get; set; }

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

        public bool IsEligibleForInvite => Age >= 13 && !HasActiveInvitation && !HasAcceptedInvitation;

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
                if (IsEligibleForInvite) return "Eligible";
                return string.Empty;
            }
        }

        public bool ShowStatusBadge => !string.IsNullOrEmpty(InvitationStatusText);
    }
}
