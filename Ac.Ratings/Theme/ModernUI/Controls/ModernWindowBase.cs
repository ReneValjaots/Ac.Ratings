using Ac.Ratings.Theme.ModernUI.Helpers;
using System.Windows;
using System.Windows.Input;

namespace Ac.Ratings.Theme.ModernUI.Controls
{
    public class ModernWindowBase : Window {
        public static readonly DependencyProperty BackgroundContentProperty =
            DependencyProperty.Register(nameof(BackgroundContent), typeof(object), typeof(ModernWindowBase));

        public static readonly DependencyProperty IsTitleVisibleProperty =
            DependencyProperty.Register(nameof(IsTitleVisible), typeof(bool), typeof(ModernWindowBase), new PropertyMetadata(false));

        public static readonly DependencyProperty MenuLinkGroupsProperty =
            DependencyProperty.Register(nameof(MenuLinkGroups), typeof(LinkGroupCollection), typeof(ModernWindowBase));

        public static readonly DependencyProperty ContentSourceProperty =
            DependencyProperty.Register(nameof(ContentSource), typeof(Uri), typeof(ModernWindowBase));

        public static readonly DependencyProperty ContentLoaderProperty =
            DependencyProperty.Register(nameof(ContentLoader), typeof(IContentLoader), typeof(ModernWindowBase), new PropertyMetadata(new ContentLoader()));

        public ModernWindowBase() {
            Style = (Style)FindResource("ModernWindow");
            SetCurrentValue(MenuLinkGroupsProperty, new LinkGroupCollection());
            CommandBindings.Add(new CommandBinding(SystemCommands.CloseWindowCommand, OnCloseWindow));
            CommandBindings.Add(new CommandBinding(SystemCommands.MaximizeWindowCommand, OnMaximizeWindow, OnCanResizeWindow));
            CommandBindings.Add(new CommandBinding(SystemCommands.MinimizeWindowCommand, OnMinimizeWindow, OnCanMinimizeWindow));
            CommandBindings.Add(new CommandBinding(SystemCommands.RestoreWindowCommand, OnRestoreWindow, OnCanResizeWindow));
        }

        private void OnCanResizeWindow(object sender, CanExecuteRoutedEventArgs e) {
            e.CanExecute = ResizeMode == ResizeMode.CanResize || ResizeMode == ResizeMode.CanResizeWithGrip;
        }   

        private void OnCanMinimizeWindow(object sender, CanExecuteRoutedEventArgs e) {
            e.CanExecute = ResizeMode != ResizeMode.NoResize;
        }

        private void OnCloseWindow(object sender, ExecutedRoutedEventArgs e) {
            SystemCommands.CloseWindow(this);
        }

        private void OnMaximizeWindow(object sender, ExecutedRoutedEventArgs e) {
            SystemCommands.MaximizeWindow(this);
        }

        private void OnMinimizeWindow(object sender, ExecutedRoutedEventArgs e) {
            SystemCommands.MinimizeWindow(this);
        }

        private void OnRestoreWindow(object sender, ExecutedRoutedEventArgs e) {
            SystemCommands.RestoreWindow(this);
        }

        public object BackgroundContent {
            get => GetValue(BackgroundContentProperty);
            set => SetValue(BackgroundContentProperty, value);
        }

        public bool IsTitleVisible {
            get => (bool)GetValue(IsTitleVisibleProperty);
            set => SetValue(IsTitleVisibleProperty, value);
        }

        public LinkGroupCollection MenuLinkGroups {
            get => (LinkGroupCollection)GetValue(MenuLinkGroupsProperty);
            set => SetValue(MenuLinkGroupsProperty, value);
        }

        public Uri ContentSource {
            get => (Uri)GetValue(ContentSourceProperty);
            set => SetValue(ContentSourceProperty, value);
        }

        public IContentLoader ContentLoader {
            get => (IContentLoader)GetValue(ContentLoaderProperty);
            set => SetValue(ContentLoaderProperty, value);
        }
    }
}