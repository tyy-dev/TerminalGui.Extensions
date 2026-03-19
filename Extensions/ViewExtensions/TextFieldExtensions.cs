using Terminal.Gui.App;
using Terminal.Gui.Views;

namespace TerminalGui.Extensions.Extensions.ViewExtensions;

/// <summary>
///     Extension methods for <see cref="TextField" />.
/// </summary>
public static class TextFieldExtensions
{
    extension(TextField textField)
    {
        /// <summary>
        ///     Subscribes to the <see cref="TextField.ValueChanged" /> event.
        /// </summary>
        /// <param name="callback">
        ///     The callback to invoke with the <see cref="ValueChangedEventArgs{T}" /> containing old and new
        ///     values.
        /// </param>
        /// <returns>The <see cref="TextField" /> instance.</returns>
        public TextField OnValueChanged(Action<ValueChangedEventArgs<string?>> callback)
        {
            textField.ValueChanged += (_, e) => callback(e);
            return textField;
        }

        /// <summary>
        ///     Subscribes to the <see cref="TextField.TextChanging" /> event.
        ///     Set <see cref="ResultEventArgs{T}.Result" /> to <see langword="null" /> in the callback to cancel the change.
        /// </summary>
        /// <param name="callback">The callback to invoke with the <see cref="ResultEventArgs{T}" /> containing the proposed text.</param>
        /// <returns>The <see cref="TextField" /> instance.</returns>
        public TextField OnTextChanging(Action<ResultEventArgs<string>> callback)
        {
            textField.TextChanging += (_, e) => callback(e);
            return textField;
        }

        /// <summary>
        ///     Subscribes to the <see cref="TextField.ValueChanging" /> event.
        ///     Set <see cref="ValueChangingEventArgs{T}.Handled" /> to <see langword="true" /> to cancel the change.
        /// </summary>
        /// <param name="callback">
        ///     The callback to invoke with the <see cref="ValueChangingEventArgs{T}" /> containing current and
        ///     proposed values.
        /// </param>
        /// <returns>The <see cref="TextField" /> instance.</returns>
        public TextField OnValueChanging(Action<ValueChangingEventArgs<string?>> callback)
        {
            textField.ValueChanging += (_, e) => callback(e);
            return textField;
        }
    }
}
