using Terminal.Gui.App;
using Terminal.Gui.ViewBase;
using Terminal.Gui.Views;

namespace TerminalGui.Extensions.Extensions.ViewExtensions;

public static class CheckBoxExtensions
{
    extension(CheckBox checkbox)
    {
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
