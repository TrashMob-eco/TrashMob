namespace TrashMobMobile.ViewModels
{
    using System.Collections.ObjectModel;
    using TrashMobMobile.Models;
    using TrashMobMobile.Services;
    using Xamarin.Forms;
    using Xamarin.Forms.Maps;

    public class UserProfileViewModel : BaseViewModel
    {
        private string id;
        private string userName;
        private string givenName;
        private string surname;
        private string city;
        private string region;
        private string country;
        private string postalCode;
        private double latitude;
        private double longitude;
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
            id = App.CurrentUser.Id;
            var user = App.CurrentUser;

            UserName = user.UserName;
            GivenName = user.GivenName;
            Surname = user.SurName;
            City = user.City;
            Region = user.Region;
            Country = user.Country;
            PostalCode = user.PostalCode;
            Latitude = user.Latitude;
            Longitude = user.Longitude;

            var pin = new Pin
            {
                Address = user.City + ", " + user.Region,
                Label = "User's Base Location",
                Type = PinType.Place,
                Position = new Position(user.Latitude, user.Longitude)
            };

            var mapSpan = new MapSpan(pin.Position, 0.01, 0.01);
            Map = new Map(mapSpan)
            {
                MinimumHeightRequest = 500
            };

            Map.Pins.Add(pin);
        }

        private bool ValidateSave()
        {
            return !string.IsNullOrWhiteSpace(userName);
        }

        public Map Map { get; private set; }

        public string UserName
        {
            get => userName;
            set => SetProperty(ref userName, value);
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

        public Position Center
        {
            get => center;
            set => SetProperty(ref center, value);
        }

        public Command SaveCommand { get; }

        public Command CancelCommand { get; }

        private async void OnCancel()
        {
            // This will pop the current page off the navigation stack
            await Shell.Current.GoToAsync("..");
        }

        private async void OnSave()
        {
            User user = new User()
            {
                Id = id,
                UserName = UserName,
                GivenName = GivenName,
                City = City,
                Region = Region,
                Country = Country,
                PostalCode = PostalCode,
                Latitude = Latitude,
                Longitude = Longitude,
            };

            await userManager.UpdateUserAsync(user);

            // This will pop the current page off the navigation stack
            await Shell.Current.GoToAsync("..");
        }
    }
}