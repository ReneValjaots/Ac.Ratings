using Ac.Ratings.Theme.ModernUI.Helpers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Ac.Ratings.Theme.ModernUI.Controls {
    public class ModernFrame : ContentControl {
        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register(nameof(Source), typeof(Uri), typeof(ModernFrame), new PropertyMetadata(OnSourceChanged));

        public static readonly DependencyProperty ContentLoaderProperty =
            DependencyProperty.Register(nameof(ContentLoader), typeof(IContentLoader), typeof(ModernFrame), new PropertyMetadata(new ContentLoader())); // Ensure default ContentLoader

        private Stack<Uri> history = new Stack<Uri>();
        private bool isNavigatingHistory = false;

        public Uri Source {
            get => (Uri)GetValue(SourceProperty);
            set => SetValue(SourceProperty, value);
        }

        public IContentLoader ContentLoader {
            get => (IContentLoader)GetValue(ContentLoaderProperty);
            set => SetValue(ContentLoaderProperty, value);
        }

        public ModernFrame() {
            CommandBindings.Add(new CommandBinding(NavigationCommands.BrowseBack, OnBrowseBack, OnCanBrowseBack));
        }

        private static void OnSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            var frame = (ModernFrame)d;
            frame.Navigate((Uri)e.OldValue, (Uri)e.NewValue);
        }

        private void Navigate(Uri oldSource, Uri newSource) {
            if (newSource == null || (oldSource != null && newSource.Equals(oldSource))) {
                return; // Avoid navigating to null or the same source
            }

            if (!isNavigatingHistory && oldSource != null) // Don't add to history when going back
            {
                history.Push(oldSource);
            }
            isNavigatingHistory = false; // Reset flag

            LoadContent(newSource);
        }


        private async void LoadContent(Uri source) {
            if (source == null) {
                System.Diagnostics.Debug.WriteLine("ModernFrame: Source is null");
                return;
            }

            var loader = ContentLoader ?? new ContentLoader(); // Use default if not set
            var content = await loader.LoadContentAsync(source, CancellationToken.None);
            if (content == null) {
                System.Diagnostics.Debug.WriteLine($"ModernFrame: Failed to load content for URI: {source.OriginalString}");
            }
            else {
                System.Diagnostics.Debug.WriteLine($"ModernFrame: Successfully loaded content for URI: {source.OriginalString}");
            }

            Content = content;
        }

        private void OnCanBrowseBack(object sender, CanExecuteRoutedEventArgs e) {
            e.CanExecute = history.Count > 0;
        }

        private void OnBrowseBack(object sender, ExecutedRoutedEventArgs e) {
            if (history.Count > 0) {
                Uri previousSource = history.Pop();
                if (previousSource != null) {
                    isNavigatingHistory = true; // Set flag to prevent adding to history again
                    SetCurrentValue(SourceProperty, previousSource); // Use SetCurrentValue to avoid property changed recursion if bound
                }
            }
        }
    }
}
