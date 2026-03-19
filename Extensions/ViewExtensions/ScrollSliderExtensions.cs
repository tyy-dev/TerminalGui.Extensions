using Terminal.Gui.App;
using Terminal.Gui.ViewBase;
using Terminal.Gui.Views;

namespace TerminalGui.Extensions.Extensions.ViewExtensions;

/// <summary>
///     Extension methods for <see cref="ScrollSlider" />.
/// </summary>
public static class ScrollSliderExtensions
{
    extension(ScrollSlider scrollSlider)
    {
        /// <summary>
        ///     Subscribes to the <see cref="ScrollSlider.OrientationChanged" /> event.
        /// </summary>
        /// <param name="callback">The callback to invoke with the <see cref="EventArgs{T}" /> containing the new <see cref="Orientation" />.</param>
        /// <returns>The <see cref="ScrollSlider" /> instance.</returns>
        public ScrollSlider OnOrientationChanged(Action<EventArgs<Orientation>> callback)
        {
            scrollSlider.OrientationChanged += (_, e) => callback(e);
            return scrollSlider;
        }

        /// <summary>
        ///     Subscribes to the <see cref="ScrollSlider.OrientationChanging" /> event.
        ///     Set <see cref="CancelEventArgs{T}.Cancel" /> to <see langword="true" /> to cancel the change.
        /// </summary>
        /// <param name="callback">The callback to invoke with the <see cref="CancelEventArgs{T}" /> containing the proposed <see cref="Orientation" />.</param>
        /// <returns>The <see cref="ScrollSlider" /> instance.</returns>
        public ScrollSlider OnOrientationChanging(Action<CancelEventArgs<Orientation>> callback)
        {
            scrollSlider.OrientationChanging += (_, e) => callback(e);
            return scrollSlider;
        }

        /// <summary>
        ///     Subscribes to the <see cref="ScrollSlider.PositionChanged" /> event.
        /// </summary>
        /// <param name="callback">The callback to invoke with the <see cref="EventArgs{T}" /> containing the new position.</param>
        /// <returns>The <see cref="ScrollSlider" /> instance.</returns>
        public ScrollSlider OnPositionChanged(Action<EventArgs<int>> callback)
        {
            scrollSlider.PositionChanged += (_, e) => callback(e);
            return scrollSlider;
        }

        /// <summary>
        ///     Subscribes to the <see cref="ScrollSlider.PositionChanging" /> event.
        ///     Set <see cref="CancelEventArgs{T}.Cancel" /> to <see langword="true" /> to cancel the change.
        /// </summary>
        /// <param name="callback">The callback to invoke with the <see cref="CancelEventArgs{T}" /> containing the proposed position.</param>
        /// <returns>The <see cref="ScrollSlider" /> instance.</returns>
        public ScrollSlider OnPositionChanging(Action<CancelEventArgs<int>> callback)
        {
            scrollSlider.PositionChanging += (_, e) => callback(e);
            return scrollSlider;
        }

        /// <summary>
        ///     Subscribes to the <see cref="ScrollSlider.Scrolled" /> event.
        /// </summary>
        /// <param name="callback">The callback to invoke with the <see cref="EventArgs{T}" /> containing the scroll position.</param>
        /// <returns>The <see cref="ScrollSlider" /> instance.</returns>
        public ScrollSlider OnScrolled(Action<EventArgs<int>> callback)
        {
            scrollSlider.Scrolled += (_, e) => callback(e);
            return scrollSlider;
        }
    }
}
