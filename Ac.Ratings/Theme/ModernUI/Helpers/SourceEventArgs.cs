namespace Ac.Ratings.Theme.ModernUI.Helpers {
    public class SourceEventArgs : EventArgs {
        public SourceEventArgs(Uri source) {
            Source = source;
        }

        public Uri Source { get; private set; }
    }
}
