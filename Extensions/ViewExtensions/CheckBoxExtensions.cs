using Terminal.Gui.App;
using Terminal.Gui.ViewBase;
using Terminal.Gui.Views;

namespace TerminalGui.Extensions.Extensions.ViewExtensions;

public static class CheckBoxExtensions {
    #region Checkbox
    extension(CheckBox checkbox) {
        /// <summary>
        /// Gets or sets the checked state of the checkbox control.
        /// </summary>
        /// <remarks>
        /// The value is <see langword="true"/> if the checkbox is checked,
        /// <see langword="false"/> if it is unchecked, or <see langword="null"/> if it is None.
        /// of the checkbox accordingly.
        /// </remarks>
        public bool? IsChecked {
            get => checkbox.CheckedState.IsChecked;
            set => checkbox.CheckedState = ConvertCheckState(value);
        }

        public View OnCheckedStateChanged(Action<EventArgs<CheckState>> callback)
        {
            checkbox.CheckedStateChanged += (_, e) => callback(e);
            return checkbox;
        }

        public View OnCheckedStateChanging(Action<ResultEventArgs<CheckState>> callback)
        {
            checkbox.CheckedStateChanging += (_, e) => callback(e);
            return checkbox;
        }
    }
#endregion

    #region CheckState
    extension(CheckState) {
        public static CheckState ConvertCheckState(bool? state) => state switch
        {
            true => CheckState.Checked,
            false => CheckState.UnChecked,
            null => CheckState.None,
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
        public bool? IsChecked => ConvertCheckState(checkState);
    }

#endregion
}
