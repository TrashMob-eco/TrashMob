namespace TrashMobMobile.ViewModels
{
    using System;
    using TrashMobMobile.Models;
    using TrashMobMobile.Services;
    using TrashMobMobile.Views;
    using Xamarin.Forms;
    using Xamarin.Forms.Maps;

    public class UserProfileViewModel : BaseViewModel
    {
        private string nameIdentifier;
        private string userName;
        private string sourceSystemUserName;
        private string givenName;
        private string surname;
        private string email;
        private string city;
        private string region;
        private string country;
        private string postalCode;
        private DateTimeOffset dateAgreedToPrivacyPolicy;
        private DateTimeOffset dateAgreedToTermsOfService;
        private string privacyPolicyVersion;
        private string termsOfServiceVersion;
        private DateTimeOffset memberSince;
        private double latitude;
        private double longitude;
        private bool prefersMetric;
        private bool isOptedOutOfAllEmails;
        private int travelLimitForLocalEvents;
        private bool isSiteAdmin = false;
        private readonly IUserManager userManager;
        private Position center;

        public UserProfileViewModel(IUserManager userManager)
        {
            Title = "User Profile";
            SaveCommand = new Command(OnSave, ValidateSave);
            CancelCommand = new Command(OnCancel);
            PropertyChanged +=
                (_, __) => SaveCommand.ChangeCanExecute();
            this.userManager = userManager;
            LoadUser();
        }

        private void LoadUser()
        {
            var user = App.CurrentUser;

            UserName = user.UserName;
            GivenName = user.GivenName;
            Surname = user.Surname;
            City = user.City;
            Region = user.Region;
            Country = user.Country;
            PostalCode = user.PostalCode;

            if (user.Latitude.HasValue)
            {
                Latitude = user.Latitude.Value;
            }

            if (user.Longitude.HasValue)
            {
                Longitude = user.Longitude.Value;
            }

            NameIdentifier = user.NameIdentifier;
            SourceSystemUserName = user.SourceSystemUserName;
            Email = user.Email;
            DateAgreedToPrivacyPolicy = user.DateAgreedToPrivacyPolicy ?? DateTimeOffset.MinValue;
            PrivacyPolicyVersion = user.PrivacyPolicyVersion;
            DateAgreedToTermsOfService = user.DateAgreedToTermsOfService ?? DateTimeOffset.MinValue;
            TermsOfServiceVersion = user.TermsOfServiceVersion;
            MemberSince = user.MemberSince ?? DateTimeOffset.MinValue;
            PrefersMetric = user.PrefersMetric;
            IsOptedOutOfAllEmails = user.IsOptedOutOfAllEmails;
            TravelLimitForLocalEvents = user.TravelLimitForLocalEvents;
            IsSiteAdmin = user.IsSiteAdmin;

            Map = new Map();
            Map.MapClicked += Map_MapClicked;

            if (user.Latitude.HasValue && user.Longitude.HasValue)
            {
                SetUserPin(user.Region, user.City, user.Latitude.Value, user.Longitude.Value);
            }
        }

        private bool ValidateSave()
        {
            return !string.IsNullOrWhiteSpace(userName);
        }

        public Map Map { get; private set; }

        public string NameIdentifier
        {
            get => nameIdentifier;
            set => SetProperty(ref nameIdentifier, value);
        }

        public string UserName
        {
            get => userName;
            set => SetProperty(ref userName, value);
        }

        public string SourceSystemUserName
        {
            get => sourceSystemUserName;
            set => SetProperty(ref sourceSystemUserName, value);
        }

        public string Email
        {
            get => email;
            set => SetProperty(ref email, value);
        }

        public string GivenName
        {
            get => givenName;
            set => SetProperty(ref givenName, value);
        }

        public string Surname
        {
            get => surname;
            set => SetProperty(ref surname, value);
        }

        public string City
        {
            get => city;
            set => SetProperty(ref city, value);
        }

        public string Region
        {
            get => region;
            set => SetProperty(ref region, value);
        }

        public string Country
        {
            get => country;
            set => SetProperty(ref country, value);
        }

        public string PostalCode
        {
            get => postalCode;
            set => SetProperty(ref postalCode, value);
        }

        public double Latitude
        {
            get => latitude;
            set => SetProperty(ref latitude, value);
        }

        public double Longitude
        {
            get => longitude;
            set => SetProperty(ref longitude, value);
        }

        public DateTimeOffset DateAgreedToPrivacyPolicy
        {
            get => dateAgreedToPrivacyPolicy;
            set => SetProperty(ref dateAgreedToPrivacyPolicy, value);
        }

        public DateTimeOffset DateAgreedToTermsOfService
        {
            get => dateAgreedToTermsOfService;
            set => SetProperty(ref dateAgreedToTermsOfService, value);
        }

        public string PrivacyPolicyVersion
        {
            get => privacyPolicyVersion;
            set => SetProperty(ref privacyPolicyVersion, value);
        }

        public string TermsOfServiceVersion
        {
            get => termsOfServiceVersion;
            set => SetProperty(ref termsOfServiceVersion, value);
        }

        public DateTimeOffset MemberSince
        {
            get => memberSince;
            set => SetProperty(ref memberSince, value);
        }

        public int TravelLimitForLocalEvents
        {
            get => travelLimitForLocalEvents;
            set => SetProperty(ref travelLimitForLocalEvents, value);
        }

        public bool PrefersMetric
        {
            get => prefersMetric;
            set => SetProperty(ref prefersMetric, value);
        }

        public bool IsOptedOutOfAllEmails
        {
            get => isOptedOutOfAllEmails;
            set => SetProperty(ref isOptedOutOfAllEmails, value);
        }

        public bool IsSiteAdmin
        {
            get => isSiteAdmin;
            set => SetProperty(ref isSiteAdmin, value);
        }

        public Position Center
        {
            get => center;
            set => SetProperty(ref center, value);
        }

        public Command SaveCommand { get; }

        public Command CancelCommand { get; }

        private void SetUserPin(string region, string city, double latitude, double longitude)
        {
            var pin = new Pin
            {
                Address = city + ", " + region,
                Label = "User's Base Location",
                Type = PinType.Place,
                Position = new Position(latitude, longitude)
            };

            var mapSpan = new MapSpan(pin.Position, 0.01, 0.01);

            Map.MoveToRegion(mapSpan);

            Map.Pins.Add(pin);
        }

        private void Map_MapClicked(object sender, MapClickedEventArgs e)
        {
            if (e != null)
            {
                var position = e.Position;
                Latitude = position.Latitude;
                Longitude = position.Longitude;

                if (Map.Pins.Count > 0)
                {
                    Map.Pins[0].Position = new Position(position.Latitude, position.Longitude);
                }
                else
                {
                    SetUserPin(Region, City, position.Latitude, position.Longitude);
                }
            }
        }

        private async void OnCancel()
        {
            // This will pop the current page off the navigation stack
            await Shell.Current.GoToAsync("..");
        }

        private async void OnSave()
        {
            User user = new User()
            {
                UserName = UserName,
                GivenName = GivenName,
                Surname = Surname,
                City = City,
                Region = Region,
                Country = Country,
                PostalCode = PostalCode,
                Latitude = Latitude,
                Longitude = Longitude,
                NameIdentifier = NameIdentifier,
                SourceSystemUserName = SourceSystemUserName,
                Email = Email,
                DateAgreedToPrivacyPolicy = DateAgreedToPrivacyPolicy,
                PrivacyPolicyVersion = PrivacyPolicyVersion,
                DateAgreedToTermsOfService = DateAgreedToTermsOfService,
                TermsOfServiceVersion = TermsOfServiceVersion,
                MemberSince = MemberSince,
                PrefersMetric = PrefersMetric,
                IsOptedOutOfAllEmails = IsOptedOutOfAllEmails,
                TravelLimitForLocalEvents = TravelLimitForLocalEvents,
                IsSiteAdmin = IsSiteAdmin,
            };

            await userManager.UpdateUserAsync(user);

            // This will pop the current page off the navigation stack
            await Shell.Current.GoToAsync($"//{nameof(MobEventsPage)}");
        }
    }
}