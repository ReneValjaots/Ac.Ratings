using System.Windows;
using System.Windows.Controls;

namespace Ac.Ratings.View.Controls {
    /// <summary>
    /// Interaction logic for RatingSliderControl.xaml
    /// </summary>
    public partial class RatingSliderControl : UserControl {
        public static readonly DependencyProperty LabelTextProperty =
            DependencyProperty.Register(nameof(LabelText), typeof(string), typeof(RatingSliderControl),
                new PropertyMetadata(string.Empty));

        public static readonly DependencyProperty SliderValueProperty =
            DependencyProperty.Register(nameof(SliderValue), typeof(double), typeof(RatingSliderControl),
                new PropertyMetadata(0.0));

        public static readonly DependencyProperty SliderStyleProperty =
            DependencyProperty.Register(nameof(SliderStyle), typeof(Style), typeof(RatingSliderControl),
                new PropertyMetadata(null));

        public static readonly DependencyProperty SliderWidthProperty =
            DependencyProperty.Register(nameof(SliderWidth), typeof(double), typeof(RatingSliderControl),
                new PropertyMetadata(150.0)); // Default value of 150

        public string LabelText {
            get => (string)GetValue(LabelTextProperty);
            set => SetValue(LabelTextProperty, value);
        }

        public double SliderValue {
            get => (double)GetValue(SliderValueProperty);
            set => SetValue(SliderValueProperty, value);
        }

        public Style SliderStyle {
            get => (Style)GetValue(SliderStyleProperty);
            set => SetValue(SliderStyleProperty, value);
        }

        public double SliderWidth {
            get => (double)GetValue(SliderWidthProperty);
            set => SetValue(SliderWidthProperty, value);
        }

        public RatingSliderControl() {
            InitializeComponent();
        }
    }
}
