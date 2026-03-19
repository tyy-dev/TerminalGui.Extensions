using Terminal.Gui.App;
using Terminal.Gui.Views;

namespace TerminalGui.Extensions.Extensions.ViewExtensions;

/// <summary>
///     Extension methods for <see cref="Menu" />.
/// </summary>
public static class MenuExtensions
{
    extension(Menu menu)
    {
        /// <summary>
        ///     Subscribes to the <see cref="Menu.SelectedMenuItemChanged" /> event.
        /// </summary>
        /// <param name="callback">The callback to invoke with the selected <see cref="MenuItem" />.</param>
        /// <returns>The <see cref="Menu" /> instance.</returns>
        public Menu OnSelectedMenuItemChanged(Action<MenuItem> callback)
        {
            menu.SelectedMenuItemChanged += (_, e) => callback(e);
            return menu;
        }

        /// <summary>
        ///     Subscribes to the <see cref="Menu.ValueChanged" /> event.
        /// </summary>
        /// <param name="callback">The callback to invoke with the <see cref="ValueChangedEventArgs{T}" /> containing old and new <see cref="MenuItem" /> values.</param>
        /// <returns>The <see cref="Menu" /> instance.</returns>
        public Menu OnValueChanged(Action<ValueChangedEventArgs<MenuItem>> callback)
        {
            menu.ValueChanged += (_, e) => callback(e);
            return menu;
        }

        /// <summary>
        ///     Subscribes to the <see cref="Menu.ValueChanging" /> event.
        ///     Set <see cref="ValueChangingEventArgs{T}.Handled" /> to <see langword="true" /> to cancel the change.
        /// </summary>
        /// <param name="callback">The callback to invoke with the <see cref="ValueChangingEventArgs{T}" /> containing current and proposed <see cref="MenuItem" /> values.</param>
        /// <returns>The <see cref="Menu" /> instance.</returns>
        public Menu OnValueChanging(Action<ValueChangingEventArgs<MenuItem>> callback)
        {
            menu.ValueChanging += (_, e) => callback(e);
            return menu;
        }
    }
}
