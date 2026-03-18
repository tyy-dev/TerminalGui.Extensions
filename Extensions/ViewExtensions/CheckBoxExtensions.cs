using Terminal.Gui.App;
using Terminal.Gui.Views;

namespace TerminalGui.Extensions.Extensions.ViewExtensions;

public static class CheckBoxExtensions
{
    #region Checkbox

    extension(CheckBox checkbox)
    {
        /// <summary>
        ///     Gets or sets the checked state of the checkbox control.
        /// </summary>
        /// <remarks>
        ///     The value is <see langword="true" /> if the checkbox is checked,
        ///     <see langword="false" /> if it is unchecked, or <see langword="null" /> if it is None.
        ///     of the checkbox accordingly.
        /// </remarks>
        public bool? IsChecked {
            get => checkbox.Value.IsChecked;
            set => checkbox.Value = CheckState.ConvertCheckState(value);
        }

        /// <summary>
        ///     Subscribes to the <see cref="CheckBox.ValueChanged" /> event.
        /// </summary>
        /// <param name="callback">
        ///     The callback to invoke with the <see cref="ValueChangedEventArgs{T}" /> containing old and new
        ///     values.
        /// </param>
        /// <returns>The <see cref="CheckBox" /> instance.</returns>
        public CheckBox OnValueChanged(Action<ValueChangedEventArgs<CheckState>> callback)
        {
            checkbox.ValueChanged += (_, e) => callback(e);
            return checkbox;
        }

        /// <summary>
        ///     Subscribes to the <see cref="CheckBox.ValueChanging" /> event.
        ///     Set <see cref="ValueChangingEventArgs{T}.Handled" /> to <see langword="true" /> in the callback to cancel the
        ///     change.
        /// </summary>
        /// <param name="callback">
        ///     The callback to invoke with the <see cref="ValueChangingEventArgs{T}" /> containing current and
        ///     proposed values.
        /// </param>
        /// <returns>The <see cref="CheckBox" /> instance.</returns>
        public CheckBox OnValueChanging(Action<ValueChangingEventArgs<CheckState>> callback)
        {
            checkbox.ValueChanging += (_, e) => callback(e);
            return checkbox;
        }
    }

    #endregion

    #region CheckState

    extension(CheckState)
    {
        public static CheckState ConvertCheckState(bool? state) => state switch
        {
            true => CheckState.Checked,
            false => CheckState.UnChecked,
            null => CheckState.None
        };

        public static bool? ConvertCheckState(CheckState state) => state switch
        {
            CheckState.Checked => true,
            CheckState.UnChecked => false,
            _ => null
        };
    }

    extension(CheckState checkState)
    {
        /// <summary>
        ///     The <see cref="CheckState" /> converted to a nullable <see cref="bool" />.
        ///     <see cref="CheckState.Checked" /> returns <see langword="true" />,
        ///     <see cref="CheckState.UnChecked" /> returns <see langword="false" />,
        ///     and <see cref="CheckState.None" /> returns <see langword="null" />.
        /// </summary>
        public bool? IsChecked => CheckState.ConvertCheckState(checkState);
    }

    #endregion
}
