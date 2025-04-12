using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using Ac.Ratings.Theme.ModernUI.Helpers;

namespace Ac.Ratings.Theme.ModernUI.Controls {
    public class TransitioningContentControl : ContentControl {

        private const string PresentationGroup = "PresentationStates";
        private const string NormalState = "Normal";
        public const string DefaultTransitionState = "DefaultTransition";
        internal const string PreviousContentPresentationSitePartName = "PreviousContentPresentationSite";
        internal const string CurrentContentPresentationSitePartName = "CurrentContentPresentationSite";

        private ContentPresenter CurrentContentPresentationSite { get; set; }
        private ContentPresenter PreviousContentPresentationSite { get; set; }

        public event EventHandler IsTransitioningChanged;

        private bool _allowIsTransitioningWrite;

        public bool IsTransitioning {
            get => (bool)GetValue(IsTransitioningProperty);
            private set {
                _allowIsTransitioningWrite = true;
                SetValue(IsTransitioningProperty, value);
                _allowIsTransitioningWrite = false;

                if (IsTransitioningChanged != null) {
                    IsTransitioningChanged(this, EventArgs.Empty);
                }
            }
        }

        public static readonly DependencyProperty IsTransitioningProperty =
            DependencyProperty.Register(
                "IsTransitioning",
                typeof(bool),
                typeof(TransitioningContentControl),
                new PropertyMetadata(OnIsTransitioningPropertyChanged));

        private static void OnIsTransitioningPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            TransitioningContentControl source = (TransitioningContentControl)d;

            if (!source._allowIsTransitioningWrite) {
                source.IsTransitioning = (bool)e.OldValue;
                throw new InvalidOperationException("IsTransitioning property is read-only.");
            }
        }

        private Storyboard _currentTransition;

        private Storyboard CurrentTransition {
            get => _currentTransition;
            set {
                // decouple event
                if (_currentTransition != null) {
                    _currentTransition.Completed -= OnTransitionCompleted;
                }

                _currentTransition = value;

                if (_currentTransition != null) {
                    _currentTransition.Completed += OnTransitionCompleted;
                }
            }
        }

        public string Transition {
            get => GetValue(TransitionProperty) as string;
            set => SetValue(TransitionProperty, value);
        }

        public static readonly DependencyProperty TransitionProperty =
            DependencyProperty.Register(
                "Transition",
                typeof(string),
                typeof(TransitioningContentControl),
                new PropertyMetadata(DefaultTransitionState, OnTransitionPropertyChanged));

        private static void OnTransitionPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            TransitioningContentControl source = (TransitioningContentControl)d;
            string oldTransition = e.OldValue as string;
            string newTransition = e.NewValue as string;

            if (source.IsTransitioning) {
                source.AbortTransition();
            }

            // find new transition
            Storyboard newStoryboard = source.GetStoryboard(newTransition);

            // unable to find the transition.
            if (newStoryboard == null) {
                // could be during initialization of xaml that presentationgroups was not yet defined
                if (VisualTreeHelperEx.TryGetVisualStateGroup(source, PresentationGroup) == null) {
                    // will delay check
                    source.CurrentTransition = null;
                }
                else {
                    // revert to old value
                    source.SetValue(TransitionProperty, oldTransition);

                    throw new ArgumentException(
                        string.Format(CultureInfo.CurrentCulture, "Transition '{0}' was not defined.", newTransition));
                }
            }
            else {
                source.CurrentTransition = newStoryboard;
            }
        }

        public bool RestartTransitionOnContentChange {
            get => (bool)GetValue(RestartTransitionOnContentChangeProperty);
            set => SetValue(RestartTransitionOnContentChangeProperty, value);
        }

        public static readonly DependencyProperty RestartTransitionOnContentChangeProperty =
            DependencyProperty.Register(
                "RestartTransitionOnContentChange",
                typeof(bool),
                typeof(TransitioningContentControl),
                new PropertyMetadata(false, OnRestartTransitionOnContentChangePropertyChanged));

        private static void OnRestartTransitionOnContentChangePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            ((TransitioningContentControl)d).OnRestartTransitionOnContentChangeChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        protected virtual void OnRestartTransitionOnContentChangeChanged(bool oldValue, bool newValue) { }

        public event RoutedEventHandler TransitionCompleted;

        static TransitioningContentControl() {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TransitioningContentControl), new FrameworkPropertyMetadata(typeof(TransitioningContentControl)));
        }

        public override void OnApplyTemplate() {
            if (IsTransitioning) {
                AbortTransition();
            }

            base.OnApplyTemplate();

            PreviousContentPresentationSite = GetTemplateChild(PreviousContentPresentationSitePartName) as ContentPresenter;
            CurrentContentPresentationSite = GetTemplateChild(CurrentContentPresentationSitePartName) as ContentPresenter;

            if (CurrentContentPresentationSite != null) {
                CurrentContentPresentationSite.Content = Content;
            }

            // hookup currenttransition
            Storyboard transition = GetStoryboard(Transition);
            CurrentTransition = transition;
            if (transition == null) {
                string invalidTransition = Transition;
                // revert to default
                Transition = DefaultTransitionState;

                throw new ArgumentException(
                    string.Format(CultureInfo.CurrentCulture, "Transition '{0}' was not defined.", invalidTransition));
            }

            VisualStateManager.GoToState(this, NormalState, false);
        }

        protected override void OnContentChanged(object oldContent, object newContent) {
            base.OnContentChanged(oldContent, newContent);

            StartTransition(oldContent, newContent);
        }

        private void StartTransition(object oldContent, object newContent) {
            // both presenters must be available, otherwise a transition is useless.
            if (CurrentContentPresentationSite != null && PreviousContentPresentationSite != null) {
                CurrentContentPresentationSite.Content = newContent;

                PreviousContentPresentationSite.Content = oldContent;

                // and start a new transition
                if (!IsTransitioning || RestartTransitionOnContentChange) {
                    IsTransitioning = true;
                    VisualStateManager.GoToState(this, NormalState, false);
                    VisualStateManager.GoToState(this, Transition, true);
                }
            }
        }

        private void OnTransitionCompleted(object sender, EventArgs e) {
            AbortTransition();

            RoutedEventHandler handler = TransitionCompleted;
            if (handler != null) {
                handler(this, new RoutedEventArgs());
            }
        }

        public void AbortTransition() {
            // go to normal state and release our hold on the old content.
            VisualStateManager.GoToState(this, NormalState, false);
            IsTransitioning = false;
            if (PreviousContentPresentationSite != null) {
                PreviousContentPresentationSite.Content = null;
            }
        }

        private Storyboard GetStoryboard(string newTransition) {
            VisualStateGroup presentationGroup = this.TryGetVisualStateGroup(PresentationGroup);
            Storyboard newStoryboard = null;
            if (presentationGroup != null) {
                newStoryboard = presentationGroup.States
                    .OfType<VisualState>()
                    .Where(state => state.Name == newTransition)
                    .Select(state => state.Storyboard)
                    .FirstOrDefault();
            }

            return newStoryboard;
        }
    }
}
