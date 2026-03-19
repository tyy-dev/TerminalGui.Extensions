using Terminal.Gui.App;
using Terminal.Gui.ViewBase;
using Terminal.Gui.Views;

namespace TerminalGui.Extensions.Extensions.ViewExtensions;

/// <summary>
///     Extension methods for <see cref="Shortcut" />.
/// </summary>
public static class ShortcutExtensions
{
    extension(Shortcut shortcut)
    {
        /// <summary>
        ///     Subscribes to the <see cref="Shortcut.OrientationChanged" /> event.
        /// </summary>
        /// <param name="callback">The callback to invoke with the <see cref="EventArgs{T}" /> containing the new <see cref="Orientation" />.</param>
        /// <returns>The <see cref="Shortcut" /> instance.</returns>
        public Shortcut OnOrientationChanged(Action<EventArgs<Orientation>> callback)
        {
            shortcut.OrientationChanged += (_, e) => callback(e);
            return shortcut;
        }

        /// <summary>
        ///     Subscribes to the <see cref="Shortcut.OrientationChanging" /> event.
        ///     Set <see cref="CancelEventArgs{T}.Cancel" /> to <see langword="true" /> to cancel the change.
        /// </summary>
        /// <param name="callback">The callback to invoke with the <see cref="CancelEventArgs{T}" /> containing the proposed <see cref="Orientation" />.</param>
        /// <returns>The <see cref="Shortcut" /> instance.</returns>
        public Shortcut OnOrientationChanging(Action<CancelEventArgs<Orientation>> callback)
        {
            shortcut.OrientationChanging += (_, e) => callback(e);
            return shortcut;
        }
    }
}
