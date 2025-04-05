using System.Windows;
using System.Windows.Controls;

namespace Ac.Ratings.View.Controls {
    /// <summary>
    /// Interaction logic for FilterItemListControl.xaml
    /// </summary>
    public partial class FilterItemListControl : UserControl {
        public static readonly DependencyProperty ItemsProperty =
            DependencyProperty.Register(nameof(Items), typeof(object), typeof(FilterItemListControl), new PropertyMetadata(null));
        public FilterItemListControl() {
            InitializeComponent();
        }

        public object Items {
            get => GetValue(ItemsProperty);
            set => SetValue(ItemsProperty, value);
        }
    }
}
