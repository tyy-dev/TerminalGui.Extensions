using Terminal.Gui.Input;
using Terminal.Gui.Views;

namespace TerminalGui.Extensions.Extensions.ViewExtensions;

/// <summary>
///     Extension methods for <see cref="MenuBar" />.
/// </summary>
public static class MenuBarExtensions
{
    extension(MenuBar menuBar)
    {
        /// <summary>
        ///     Subscribes to the <see cref="MenuBar.KeyChanged" /> event.
        /// </summary>
        /// <param name="callback">The callback to invoke with the <see cref="KeyChangedEventArgs" /> containing the old and new keys.</param>
        /// <returns>The <see cref="MenuBar" /> instance.</returns>
        public MenuBar OnKeyChanged(Action<KeyChangedEventArgs> callback)
        {
            menuBar.KeyChanged += (_, e) => callback(e);
            return menuBar;
        }
    }
}
