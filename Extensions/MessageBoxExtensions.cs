using Terminal.Gui.App;
using Terminal.Gui.Views;

namespace TerminalGui.Extensions.Extensions;

public static class MessageBoxExtensions
{
    extension(MessageBox)
    {
        public static bool Confirm(IApplication application, string message, string title = "Confirm", string yesText = "Yes", string noText = "No")
            => MessageBox.Query(application, title, message, yesText, noText) == 0;

        /// <summary>
        /// Displays an error <see cref="MessageBox"/> with the specified values
        /// </summary>
        /// <param name="application"></param>
        /// <param name="message">The error message to display. if <see langword="null"/>, "An error has occured" is displayed.</param>
        /// <param name="title"></param>
        /// <param name="okText"></param>
        public static void Error(IApplication application, string? message = null, string title = "Error", string okText = "OK")
            => MessageBox.ErrorQuery(application, title, message ?? "An error has occured", okText);

        public static void Error(IApplication application, Exception exception, string title = "Error", string okText = "OK")
            => Error(application, exception.Message, okText);

        public static void Info(IApplication application, string message, string title = "Info", string okText = "OK")
            => MessageBox.Query(application, title, message, okText);
    }
}
