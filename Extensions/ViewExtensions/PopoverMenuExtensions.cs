using Terminal.Gui.Input;
using Terminal.Gui.Views;

namespace TerminalGui.Extensions.Extensions.ViewExtensions;

/// <summary>
///     Extension methods for <see cref="PopoverMenu" />.
/// </summary>
public static class PopoverMenuExtensions
{
    extension(PopoverMenu popoverMenu)
    {
        /// <summary>
        ///     Subscribes to the <see cref="PopoverMenu.KeyChanged" /> event.
        /// </summary>
        /// <param name="callback">The callback to invoke with the <see cref="KeyChangedEventArgs" /> containing the old and new keys.</param>
        /// <returns>The <see cref="PopoverMenu" /> instance.</returns>
        public PopoverMenu OnKeyChanged(Action<KeyChangedEventArgs> callback)
        {
            popoverMenu.KeyChanged += (_, e) => callback(e);
            return popoverMenu;
        }
    }
}
