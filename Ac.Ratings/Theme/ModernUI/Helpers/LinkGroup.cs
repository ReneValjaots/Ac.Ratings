namespace Ac.Ratings.Theme.ModernUI.Helpers {
    public class LinkGroup : Link {
        private string _groupKey;
        private Link _selectedLink;
        private bool _isNavigable;

        public string GroupKey {
            get => _groupKey;
            set => SetField(ref _groupKey, value);
        }

        internal Link SelectedLink {
            get => _selectedLink;
            set => SetField(ref _selectedLink, value);
        }

        public bool IsNavigable {
            get => _isNavigable;
            set => SetField(ref _isNavigable, value);
        }

        public LinkCollection Links { get; } = new LinkCollection();
    }
}
