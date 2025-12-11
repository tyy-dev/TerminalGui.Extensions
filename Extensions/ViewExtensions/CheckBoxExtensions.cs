using Terminal.Gui.App;
using Terminal.Gui.ViewBase;
using Terminal.Gui.Views;

namespace TerminalGui.Extensions.Extensions.ViewExtensions;

public static class CheckBoxExtensions
{
    extension(CheckBox checkbox)
    {
        /// <summary>
        /// Whether the checkbox is currently checked.
        /// </summary>
        public bool IsChecked => checkbox.CheckedState == CheckState.Checked;

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
}
