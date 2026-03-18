using Terminal.Gui.App;
using Terminal.Gui.Views;

namespace TerminalGui.Extensions.Extensions.ViewExtensions;

/// <summary>
///     Extension methods for <see cref="NumericUpDown{T}" />.
/// </summary>
public static class NumericUpDownExtensions
{
    extension<T>(NumericUpDown<T> numericUpDown) where T : notnull
    {
        /// <summary>
        ///     Subscribes to the <see cref="NumericUpDown{T}.ValueChanged" /> event.
        /// </summary>
        /// <param name="callback">
        ///     The callback to invoke with the <see cref="ValueChangedEventArgs{T}" /> containing old and new
        ///     values.
        /// </param>
        /// <returns>The <see cref="NumericUpDown{T}" /> instance.</returns>
        public NumericUpDown<T> OnValueChanged(Action<ValueChangedEventArgs<T?>> callback)
        {
            numericUpDown.ValueChanged += (_, e) => callback(e);
            return numericUpDown;
        }

        /// <summary>
        ///     Subscribes to the <see cref="NumericUpDown{T}.ValueChanging" /> event.
        ///     Set <see cref="ValueChangingEventArgs{T}.Handled" /> to <see langword="true" /> in the callback to cancel the
        ///     change.
        /// </summary>
        /// <param name="callback">
        ///     The callback to invoke with the <see cref="ValueChangingEventArgs{T}" /> containing current and
        ///     proposed values.
        /// </param>
        /// <returns>The <see cref="NumericUpDown{T}" /> instance.</returns>
        public NumericUpDown<T> OnValueChanging(Action<ValueChangingEventArgs<T?>> callback)
        {
            numericUpDown.ValueChanging += (_, e) => callback(e);
            return numericUpDown;
        }
    }
}
