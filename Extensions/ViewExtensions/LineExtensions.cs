using Terminal.Gui.App;
using Terminal.Gui.ViewBase;
using Terminal.Gui.Views;

namespace TerminalGui.Extensions.Extensions.ViewExtensions;

/// <summary>
///     Extension methods for <see cref="Line" />.
/// </summary>
public static class LineExtensions
{
    extension(Line line)
    {
        /// <summary>
        ///     Subscribes to the <see cref="Line.OrientationChanged" /> event.
        /// </summary>
        /// <param name="callback">The callback to invoke with the <see cref="EventArgs{T}" /> containing the new <see cref="Orientation" />.</param>
        /// <returns>The <see cref="Line" /> instance.</returns>
        public Line OnOrientationChanged(Action<EventArgs<Orientation>> callback)
        {
            line.OrientationChanged += (_, e) => callback(e);
            return line;
        }

        /// <summary>
        ///     Subscribes to the <see cref="Line.OrientationChanging" /> event.
        ///     Set <see cref="CancelEventArgs{T}.Cancel" /> to <see langword="true" /> to cancel the change.
        /// </summary>
        /// <param name="callback">The callback to invoke with the <see cref="CancelEventArgs{T}" /> containing the proposed <see cref="Orientation" />.</param>
        /// <returns>The <see cref="Line" /> instance.</returns>
        public Line OnOrientationChanging(Action<CancelEventArgs<Orientation>> callback)
        {
            line.OrientationChanging += (_, e) => callback(e);
            return line;
        }
    }
}
