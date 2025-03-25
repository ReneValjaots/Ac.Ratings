using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using Ac.Ratings.Theme.ModernUI.Helpers;

namespace Ac.Ratings.Theme.ModernUI.Controls {
    public class ModernMenu : Control {

        public static readonly DependencyProperty LinkGroupsProperty =
            DependencyProperty.Register(nameof(LinkGroups), typeof(LinkGroupCollection), typeof(ModernMenu), new PropertyMetadata(OnLinkGroupsChanged));

        public static readonly DependencyProperty SelectedLinkGroupProperty =
            DependencyProperty.Register("SelectedLinkGroup", typeof(LinkGroup), typeof(ModernMenu), new PropertyMetadata(OnSelectedLinkGroupChanged));

        public static readonly DependencyProperty SelectedLinkProperty = DependencyProperty.Register(nameof(SelectedLink), typeof(Link), typeof(ModernMenu), new PropertyMetadata(OnSelectedLinkChanged));

        public static readonly DependencyProperty SelectedSourceProperty =
            DependencyProperty.Register(nameof(SelectedSource), typeof(Uri), typeof(ModernMenu), new PropertyMetadata(OnSelectedSourceChanged));

        private static readonly DependencyPropertyKey VisibleLinkGroupsPropertyKey =
            DependencyProperty.RegisterReadOnly("VisibleLinkGroups", typeof(ReadOnlyLinkGroupCollection), typeof(ModernMenu), null);

        public static readonly DependencyProperty VisibleLinkGroupsProperty = VisibleLinkGroupsPropertyKey.DependencyProperty;

        public event EventHandler<SourceEventArgs> SelectedSourceChanged;

        private Dictionary<string, ReadOnlyLinkGroupCollection> groupMap = new Dictionary<string, ReadOnlyLinkGroupCollection>(); // stores LinkGroupCollections by GroupKey
        private bool isSelecting;

        public ModernMenu() {
            DefaultStyleKey = typeof(ModernMenu);

            // create a default link groups collection
            SetCurrentValue(LinkGroupsProperty, new LinkGroupCollection());
        }

        private static void OnLinkGroupsChanged(DependencyObject o, DependencyPropertyChangedEventArgs e) {
            ((ModernMenu)o).OnLinkGroupsChanged((LinkGroupCollection)e.OldValue, (LinkGroupCollection)e.NewValue);
        }

        private void OnLinkGroupsChanged(LinkGroupCollection oldValue, LinkGroupCollection newValue) {
            if (oldValue != null) {
                // detach old event handler
                oldValue.CollectionChanged -= OnLinkGroupsCollectionChanged;
            }

            if (newValue != null) {
                // ensures the menu is rebuild when changes in the LinkGroups occur
                newValue.CollectionChanged += OnLinkGroupsCollectionChanged;
            }

            RebuildMenu(newValue);
        }

        private static void OnSelectedLinkGroupChanged(DependencyObject o, DependencyPropertyChangedEventArgs e) {
            var menu = (ModernMenu)o;
            var group = (LinkGroup)e.NewValue;

            if (group != null && group.IsNavigable && group.Source != null) {
                menu.SetCurrentValue(SelectedSourceProperty, group.Source);
            }
            else {
                Link selectedLink = null;
                if (group != null) {
                    selectedLink = group.SelectedLink;

                    if (group.Links != null) {
                        if (selectedLink != null && !group.Links.Any(l => l == selectedLink)) {
                            selectedLink = null;
                        }

                        if (selectedLink == null) {
                            selectedLink = group.Links.FirstOrDefault();
                        }
                    }
                }

                menu.SetCurrentValue(SelectedLinkProperty, selectedLink);
            }
        }

        private static void OnSelectedLinkChanged(DependencyObject o, DependencyPropertyChangedEventArgs e) {
            // update selected source
            var newValue = (Link)e.NewValue;
            Uri selectedSource = null;
            if (newValue != null) {
                selectedSource = newValue.Source;
            }

            ((ModernMenu)o).SetCurrentValue(SelectedSourceProperty, selectedSource);
        }

        private void OnLinkGroupsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
            RebuildMenu((LinkGroupCollection)sender);
        }

        private static void OnSelectedSourceChanged(DependencyObject o, DependencyPropertyChangedEventArgs e) {
            ((ModernMenu)o).OnSelectedSourceChanged((Uri)e.OldValue, (Uri)e.NewValue);
        }

        private void OnSelectedSourceChanged(Uri oldValue, Uri newValue) {
            // Uri "Page1.xaml#111" and "Page1#222" points to the same page, but with a different fragment
            // Must remove the fragment to avoid believing we are on different pages.
            Uri oldValueNoFragment = NavigationHelper.RemoveFragment(oldValue);
            Uri newValueNoFragment = NavigationHelper.RemoveFragment(newValue);

            if (!isSelecting) {
                // if old and new are equal, don't do anything
                if (newValueNoFragment != null && newValueNoFragment.Equals(oldValueNoFragment)) {
                    return;
                }

                UpdateSelection();
            }

            // raise SelectedSourceChanged event
            var handler = SelectedSourceChanged;
            if (handler != null) {
                handler(this, new SourceEventArgs(newValue));
            }
        }

        public LinkGroupCollection LinkGroups {
            get => (LinkGroupCollection)GetValue(LinkGroupsProperty);
            set => SetValue(LinkGroupsProperty, value);
        }

        public Link SelectedLink {
            get => (Link)GetValue(SelectedLinkProperty);
            set => SetValue(SelectedLinkProperty, value);
        }

        public Uri SelectedSource {
            get => (Uri)GetValue(SelectedSourceProperty);
            set => SetValue(SelectedSourceProperty, value);
        }

        public LinkGroup SelectedLinkGroup => (LinkGroup)GetValue(SelectedLinkGroupProperty);

        public ReadOnlyLinkGroupCollection VisibleLinkGroups => (ReadOnlyLinkGroupCollection)GetValue(VisibleLinkGroupsProperty);

        private static string GetGroupKey(LinkGroup group) {
            // use special key for GroupKey <null>
            return group.GroupKey ?? "<null>";
        }

        private void RebuildMenu(LinkGroupCollection groups) {
            groupMap.Clear();
            if (groups != null) {
                // fill the group map based on group key
                foreach (var group in groups) {
                    var groupKey = GetGroupKey(group);

                    ReadOnlyLinkGroupCollection groupCollection;
                    if (!groupMap.TryGetValue(groupKey, out groupCollection)) {
                        // create a new collection for this group key
                        groupCollection = new ReadOnlyLinkGroupCollection(new LinkGroupCollection());
                        groupMap.Add(groupKey, groupCollection);
                    }

                    // add the group
                    groupCollection.List.Add(group);
                }
            }

            // update current selection
            UpdateSelection();
        }

        private void UpdateSelection() {
            LinkGroup selectedGroup = null;
            Link selectedLink = null;

            Uri sourceNoFragment = NavigationHelper.RemoveFragment(SelectedSource);

            if (LinkGroups != null) {
                // Check for a navigable group first
                var navigableGroup = LinkGroups.FirstOrDefault(g => g.IsNavigable && g.Source == sourceNoFragment);
                if (navigableGroup != null) {
                    selectedGroup = navigableGroup;
                }
                else {
                    // Original logic for groups with sublinks
                    var linkInfo = (from g in LinkGroups
                                    from l in g.Links
                                    where l.Source == sourceNoFragment
                                    select new { Group = g, Link = l }).FirstOrDefault();

                    if (linkInfo != null) {
                        selectedGroup = linkInfo.Group;
                        selectedLink = linkInfo.Link;
                    }
                    else {
                        selectedGroup = SelectedLinkGroup;

                        if (!LinkGroups.Any(g => g == selectedGroup)) {
                            selectedGroup = LinkGroups.FirstOrDefault();
                        }
                    }
                }
            }

            ReadOnlyLinkGroupCollection groups = null;
            if (selectedGroup != null) {
                if (!selectedGroup.IsNavigable) {
                    selectedGroup.SelectedLink = selectedLink;
                }

                var groupKey = GetGroupKey(selectedGroup);
                groupMap.TryGetValue(groupKey, out groups);
            }

            isSelecting = true;
            SetValue(VisibleLinkGroupsPropertyKey, groups);
            SetCurrentValue(SelectedLinkGroupProperty, selectedGroup);
            SetCurrentValue(SelectedLinkProperty, selectedLink);
            isSelecting = false;
        }
    }
}
