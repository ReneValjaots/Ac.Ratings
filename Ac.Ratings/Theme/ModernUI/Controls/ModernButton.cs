using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Ac.Ratings.Theme.ModernUI.Controls {
    public class ModernButton : Button {
        public static readonly DependencyProperty EllipseDiameterProperty = DependencyProperty.Register(nameof(EllipseDiameter), typeof(double), typeof(ModernButton), new PropertyMetadata(22D));

        public static readonly DependencyProperty EllipseStrokeThicknessProperty =
            DependencyProperty.Register(nameof(EllipseStrokeThickness), typeof(double), typeof(ModernButton), new PropertyMetadata(1D));

        public static readonly DependencyProperty IconDataProperty = DependencyProperty.Register(nameof(IconData), typeof(Geometry), typeof(ModernButton));
        public static readonly DependencyProperty IconHeightProperty = DependencyProperty.Register(nameof(IconHeight), typeof(double), typeof(ModernButton), new PropertyMetadata(12D));
        public static readonly DependencyProperty IconWidthProperty = DependencyProperty.Register(nameof(IconWidth), typeof(double), typeof(ModernButton), new PropertyMetadata(12D));

        public ModernButton() {
            DefaultStyleKey = typeof(ModernButton);
        }

        public double EllipseDiameter {
            get => (double)GetValue(EllipseDiameterProperty);
            set => SetValue(EllipseDiameterProperty, value);
        }

        public double EllipseStrokeThickness {
            get => (double)GetValue(EllipseStrokeThicknessProperty);
            set => SetValue(EllipseStrokeThicknessProperty, value);
        }

        public Geometry IconData {
            get => (Geometry)GetValue(IconDataProperty);
            set => SetValue(IconDataProperty, value);
        }

        public double IconHeight {
            get => (double)GetValue(IconHeightProperty);
            set => SetValue(IconHeightProperty, value);
        }

        public double IconWidth {
            get => (double)GetValue(IconWidthProperty);
            set => SetValue(IconWidthProperty, value);
        }
    }
}
