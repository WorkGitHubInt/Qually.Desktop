using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Animation;

namespace QuallyFlash
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
}
