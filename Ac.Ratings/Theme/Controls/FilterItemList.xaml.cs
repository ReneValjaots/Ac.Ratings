using System.Windows;
using System.Windows.Controls;

namespace Ac.Ratings.Theme.Controls {
    /// <summary>
    /// Interaction logic for FilterItemList.xaml
    /// </summary>
    public partial class FilterItemList : UserControl {
        public static readonly DependencyProperty ItemsProperty =
            DependencyProperty.Register(nameof(Items), typeof(object), typeof(FilterItemList), new PropertyMetadata(null));
        public FilterItemList() {
            InitializeComponent();
        }

        public object Items {
            get => GetValue(ItemsProperty);
            set => SetValue(ItemsProperty, value);
        }
    }
}
