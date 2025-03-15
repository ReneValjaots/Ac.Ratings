using System.Collections.ObjectModel;

namespace Ac.Ratings.Theme.ModernUI.Helpers {
    public class LinkCollection : ObservableCollection<Link> {
        public LinkCollection() { }
        public LinkCollection(IEnumerable<Link> links) {
            if (links == null) {
                throw new ArgumentNullException(nameof(links));
            }

            foreach (var link in links) {
                Add(link);
            }
        }
    }
}
