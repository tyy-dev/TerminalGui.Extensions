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
        public bool? IsChecked => CheckState.ConvertCheckState(checkState);
    }

    #endregion
}
