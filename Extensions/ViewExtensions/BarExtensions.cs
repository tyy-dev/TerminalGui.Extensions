using Terminal.Gui.App;
using Terminal.Gui.ViewBase;
using Terminal.Gui.Views;

namespace TerminalGui.Extensions.Extensions.ViewExtensions;

/// <summary>
///     Extension methods for <see cref="Bar" />.
/// </summary>
public static class BarExtensions
{
    extension(Bar bar)
    {
        /// <summary>
        ///     Subscribes to the <see cref="Bar.OrientationChanged" /> event.
        /// </summary>
        /// <param name="callback">The callback to invoke with the <see cref="EventArgs{T}" /> containing the new <see cref="Orientation" />.</param>
        /// <returns>The <see cref="Bar" /> instance.</returns>
        public Bar OnOrientationChanged(Action<EventArgs<Orientation>> callback)
        {
            bar.OrientationChanged += (_, e) => callback(e);
            return bar;
        }

        /// <summary>
        ///     Subscribes to the <see cref="Bar.OrientationChanging" /> event.
        ///     Set <see cref="CancelEventArgs{T}.Cancel" /> to <see langword="true" /> to cancel the change.
        /// </summary>
        /// <param name="callback">The callback to invoke with the <see cref="CancelEventArgs{T}" /> containing the proposed <see cref="Orientation" />.</param>
        /// <returns>The <see cref="Bar" /> instance.</returns>
        public Bar OnOrientationChanging(Action<CancelEventArgs<Orientation>> callback)
        {
            bar.OrientationChanging += (_, e) => callback(e);
            return bar;
        }
    }
}
