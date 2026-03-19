using Terminal.Gui.App;
using Terminal.Gui.Drawing;
using Terminal.Gui.Views;

namespace TerminalGui.Extensions.Extensions.ViewExtensions;

/// <summary>
///     Extension methods for <see cref="ColorPicker" />.
/// </summary>
public static class ColorPickerExtensions
{
    extension(ColorPicker colorPicker)
    {
        /// <summary>
        ///     Subscribes to the <see cref="ColorPicker.ValueChanged" /> event.
        /// </summary>
        /// <param name="callback">The callback to invoke with the <see cref="ValueChangedEventArgs{T}" /> containing old and new values.</param>
        /// <returns>The <see cref="ColorPicker" /> instance.</returns>
        public ColorPicker OnValueChanged(Action<ValueChangedEventArgs<Color?>> callback)
        {
            colorPicker.ValueChanged += (_, e) => callback(e);
            return colorPicker;
        }

        /// <summary>
        ///     Subscribes to the <see cref="ColorPicker.ValueChanging" /> event.
        ///     Set <see cref="ValueChangingEventArgs{T}.Handled" /> to <see langword="true" /> to cancel the change.
        /// </summary>
        /// <param name="callback">The callback to invoke with the <see cref="ValueChangingEventArgs{T}" /> containing current and proposed values.</param>
        /// <returns>The <see cref="ColorPicker" /> instance.</returns>
        public ColorPicker OnValueChanging(Action<ValueChangingEventArgs<Color?>> callback)
        {
            colorPicker.ValueChanging += (_, e) => callback(e);
            return colorPicker;
        }
    }
}
