using Ac.Ratings.Core;

namespace Ac.Ratings.Theme.ModernUI.Helpers {
    public class Link : ObservableObject {
        private string _displayName;
        private Uri _source;

        public string DisplayName {
            get => _displayName;
            set => SetField(ref _displayName, value);
        }

        public Uri Source {
            get => _source;
            set => SetField(ref _source, value);
        }
    }
}
