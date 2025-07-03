using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Animation;

namespace BotQually
{
    public static class AnimationHelper
    {
        public static async Task SlideIn(this FrameworkElement element)
        {
            var sb = new Storyboard();
            var animation = new DoubleAnimation
            {
                Duration = new Duration(TimeSpan.FromMilliseconds(200)),
                From = 0,
                To = 200,
                EasingFunction = new QuadraticEase()
                {
                    EasingMode = EasingMode.EaseInOut
                }
            };
            Storyboard.SetTargetProperty(animation, new PropertyPath("Width"));
            sb.Children.Add(animation);
            sb.Begin(element);
            await Task.Delay(200);
        }

        public static async Task SlideOut(this FrameworkElement element)
        {
            var sb = new Storyboard();
            var animation = new DoubleAnimation
            {
                Duration = new Duration(TimeSpan.FromMilliseconds(200)),
                From = 200,
                To = 0,
                EasingFunction = new QuadraticEase()
                {
                    EasingMode = EasingMode.EaseInOut
                }
            };
            Storyboard.SetTargetProperty(animation, new PropertyPath("Width"));
            sb.Children.Add(animation);
            sb.Begin(element);
            await Task.Delay(200);
        }
    }

    /// <summary>
    /// A base class to run any animation method when a boolean is set to true
    /// and a reverse animation when set to false
    /// </summary>
    /// <typeparam name="Parent"></typeparam>
    public abstract class AnimateBaseProperty<Parent> : BaseAttachedProperty<Parent, bool>
        where Parent : BaseAttachedProperty<Parent, bool>, new()
    {
        #region Public Properties

        /// <summary>
        /// A flag indicating if this is the first time this property has been loaded
        /// </summary>
        public bool FirstLoad { get; set; } = true;

        #endregion

        public override void OnValueUpdated(DependencyObject sender, object value)
        {
            // Get the framework element
            if (!(sender is FrameworkElement element))
                return;

            // Don't fire if the value doesn't change
            if (sender.GetValue(ValueProperty) == value && !FirstLoad)
                return;

            // On first load...
            if (FirstLoad)
            {
                // Create a single self-unhookable event 
                // for the elements Loaded event
                RoutedEventHandler onLoaded = null;
                onLoaded = async (ss, ee) =>
                {
                    // Slight delay after load is needed for some elements to get laid out
                    // and their width/heights correctly calculated
                    await Task.Delay(5);

                    // Unhook ourselves
                    element.Loaded -= onLoaded;
                    element.Visibility = Visibility.Hidden;
                    element.Width = 0;
                    element.Visibility = Visibility.Visible;
                    // Do desired animation
                    //DoAnimation(element, (bool)value);

                    // No longer in first load
                    FirstLoad = false;
                };

                // Hook into the Loaded event of the element
                element.Loaded += onLoaded;
            }
            else
                // Do desired animation
                DoAnimation(element, (bool)value);
        }

        /// <summary>
        /// The animation method that is fired when the value changes
        /// </summary>
        /// <param name="element">The element</param>
        /// <param name="value">The new value</param>
        protected virtual void DoAnimation(FrameworkElement element, bool value) { }
    }

    /// <summary>
    /// Animates a framework element sliding it in from the left on show
    /// and sliding out to the left on hide
    /// </summary>
    public class AnimateSlideInFromLeftProperty : AnimateBaseProperty<AnimateSlideInFromLeftProperty>
    {
        protected override async void DoAnimation(FrameworkElement element, bool value)
        {
            if (value)
                // Animate in
                await element.SlideIn();
            else
                // Animate out
                await element.SlideOut();
        }
    }
}
