namespace TrashMobMobileApp.StateContainers
{
    public class PageTitleContainer
    {
        public Action<string> OnTitleChange { get; set; }
        private string _title;
        public string Title
        {
            get => _title;
            set
            {
                _title = value;
                NotifyOnTitleChange();
            }
        }

        private void NotifyOnTitleChange() => OnTitleChange?.Invoke(Title);
    }
}
