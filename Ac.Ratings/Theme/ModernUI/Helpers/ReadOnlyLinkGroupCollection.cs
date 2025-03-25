using System.Collections.ObjectModel;

namespace Ac.Ratings.Theme.ModernUI.Helpers {

    public class ReadOnlyLinkGroupCollection : ReadOnlyObservableCollection<LinkGroup> {
        public ReadOnlyLinkGroupCollection(LinkGroupCollection list) : base(list) {
            List = list;
        }

        internal LinkGroupCollection List { get; private set; }
    }
}
