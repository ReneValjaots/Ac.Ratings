using System.Windows;
using System.Windows.Media;

namespace Ac.Ratings.Theme.ModernUI.Helpers {
    public static class VisualTreeHelperEx {
        public static VisualStateGroup TryGetVisualStateGroup(this DependencyObject dependencyObject, string groupName) {
            FrameworkElement root = GetImplementationRoot(dependencyObject);
            if (root == null) {
                return null;
            }

            return (from @group in VisualStateManager.GetVisualStateGroups(root).OfType<VisualStateGroup>()
                where string.CompareOrdinal(groupName, @group.Name) == 0
                select @group).FirstOrDefault<VisualStateGroup>();
        }

        public static FrameworkElement GetImplementationRoot(this DependencyObject dependencyObject) {
            if (1 != VisualTreeHelper.GetChildrenCount(dependencyObject)) {
                return null;
            }

            return (VisualTreeHelper.GetChild(dependencyObject, 0) as FrameworkElement);
        }
    }
}
