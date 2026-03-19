using Terminal.Gui.App;
using Terminal.Gui.ViewBase;
using Terminal.Gui.Views;

namespace TerminalGui.Extensions.Extensions.ViewExtensions;

/// <summary>
///     Extension methods for <see cref="ScrollBar" />.
/// </summary>
public static class ScrollBarExtensions
{
    extension(ScrollBar scrollBar)
    {
        /// <summary>
        ///     Subscribes to the <see cref="ScrollBar.OrientationChanged" /> event.
        /// </summary>
        /// <param name="callback">The callback to invoke with the <see cref="EventArgs{T}" /> containing the new <see cref="Orientation" />.</param>
        /// <returns>The <see cref="ScrollBar" /> instance.</returns>
        public ScrollBar OnOrientationChanged(Action<EventArgs<Orientation>> callback)
        {
            scrollBar.OrientationChanged += (_, e) => callback(e);
            return scrollBar;
        }

        /// <summary>
        ///     Subscribes to the <see cref="ScrollBar.OrientationChanging" /> event.
        ///     Set <see cref="CancelEventArgs{T}.Cancel" /> to <see langword="true" /> to cancel the change.
        /// </summary>
        /// <param name="callback">The callback to invoke with the <see cref="CancelEventArgs{T}" /> containing the proposed <see cref="Orientation" />.</param>
        /// <returns>The <see cref="ScrollBar" /> instance.</returns>
        public ScrollBar OnOrientationChanging(Action<CancelEventArgs<Orientation>> callback)
        {
            scrollBar.OrientationChanging += (_, e) => callback(e);
            return scrollBar;
        }

        /// <summary>
        ///     Subscribes to the <see cref="ScrollBar.ScrollableContentSizeChanged" /> event.
        /// </summary>
        /// <param name="callback">The callback to invoke with the <see cref="EventArgs{T}" /> containing the new size.</param>
        /// <returns>The <see cref="ScrollBar" /> instance.</returns>
        public ScrollBar OnScrollableContentSizeChanged(Action<EventArgs<int>> callback)
        {
            scrollBar.ScrollableContentSizeChanged += (_, e) => callback(e);
            return scrollBar;
        }

        /// <summary>
        ///     Subscribes to the <see cref="ScrollBar.Scrolled" /> event.
        /// </summary>
        /// <param name="callback">The callback to invoke with the <see cref="EventArgs{T}" /> containing the scroll position.</param>
        /// <returns>The <see cref="ScrollBar" /> instance.</returns>
        public ScrollBar OnScrolled(Action<EventArgs<int>> callback)
        {
            scrollBar.Scrolled += (_, e) => callback(e);
            return scrollBar;
        }

        /// <summary>
        ///     Subscribes to the <see cref="ScrollBar.SliderPositionChanged" /> event.
        /// </summary>
        /// <param name="callback">The callback to invoke with the <see cref="EventArgs{T}" /> containing the new slider position.</param>
        /// <returns>The <see cref="ScrollBar" /> instance.</returns>
        public ScrollBar OnSliderPositionChanged(Action<EventArgs<int>> callback)
        {
            scrollBar.SliderPositionChanged += (_, e) => callback(e);
            return scrollBar;
        }

        /// <summary>
        ///     Subscribes to the <see cref="ScrollBar.ValueChanged" /> event.
        /// </summary>
        /// <param name="callback">The callback to invoke with the <see cref="ValueChangedEventArgs{T}" /> containing old and new values.</param>
        /// <returns>The <see cref="ScrollBar" /> instance.</returns>
        public ScrollBar OnValueChanged(Action<ValueChangedEventArgs<int>> callback)
        {
            scrollBar.ValueChanged += (_, e) => callback(e);
            return scrollBar;
        }

        /// <summary>
        ///     Subscribes to the <see cref="ScrollBar.ValueChanging" /> event.
        ///     Set <see cref="ValueChangingEventArgs{T}.Handled" /> to <see langword="true" /> to cancel the change.
        /// </summary>
        /// <param name="callback">The callback to invoke with the <see cref="ValueChangingEventArgs{T}" /> containing current and proposed values.</param>
        /// <returns>The <see cref="ScrollBar" /> instance.</returns>
        public ScrollBar OnValueChanging(Action<ValueChangingEventArgs<int>> callback)
        {
            scrollBar.ValueChanging += (_, e) => callback(e);
            return scrollBar;
        }
    }
}
