using Terminal.Gui.App;
using Terminal.Gui.Views;

namespace TerminalGui.Extensions.Extensions.ViewExtensions;

/// <summary>
///     Extension methods for <see cref="DatePicker" />.
/// </summary>
public static class DatePickerExtensions
{
    extension(DatePicker datePicker)
    {
        /// <summary>
        ///     Subscribes to the <see cref="DatePicker.ValueChanged" /> event.
        /// </summary>
        /// <param name="callback">The callback to invoke with the <see cref="ValueChangedEventArgs{T}" /> containing old and new values.</param>
        /// <returns>The <see cref="DatePicker" /> instance.</returns>
        public DatePicker OnValueChanged(Action<ValueChangedEventArgs<DateTime>> callback)
        {
            datePicker.ValueChanged += (_, e) => callback(e);
            return datePicker;
        }

        /// <summary>
        ///     Subscribes to the <see cref="DatePicker.ValueChanging" /> event.
        ///     Set <see cref="ValueChangingEventArgs{T}.Handled" /> to <see langword="true" /> to cancel the change.
        /// </summary>
        /// <param name="callback">The callback to invoke with the <see cref="ValueChangingEventArgs{T}" /> containing current and proposed values.</param>
        /// <returns>The <see cref="DatePicker" /> instance.</returns>
        public DatePicker OnValueChanging(Action<ValueChangingEventArgs<DateTime>> callback)
        {
            datePicker.ValueChanging += (_, e) => callback(e);
            return datePicker;
        }
    }
}
