using Terminal.Gui.App;
using Terminal.Gui.Views;

namespace TerminalGui.Extensions.Extensions;

public static class MessageBoxExtensions
{
    extension(MessageBox)
    {
        public static bool Confirm(IApplication application, string message, string title = "Confirm", string yesText = "Yes", string noText = "No")
            => MessageBox.Query(application, title, message, yesText, noText) == 0;

        public static void Error(IApplication application, string message, string title = "ERROR", string okText = "OK")
            => MessageBox.ErrorQuery(application, title, message, okText);

        public static void Info(IApplication application, string message, string title = "Info", string okText = "OK")
            => MessageBox.Query(application, title, message, okText);
    }
}
