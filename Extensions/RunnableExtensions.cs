using Terminal.Gui.App;
using Terminal.Gui.Views;

namespace TerminalGui.Extensions.Extensions;

/// <summary>
///     Extension methods for <see cref="Runnable" />.
/// </summary>
public static class RunnableExtensions
{
    extension(Runnable runnable)
    {
        /// <summary>
        ///     Subscribes to the <see cref="Runnable.IsRunningChanged" /> event.
        /// </summary>
        /// <param name="callback">The callback to invoke with the <see cref="EventArgs{T}" /> containing the new running state.</param>
        /// <returns>The <see cref="Runnable" /> instance.</returns>
        public Runnable OnRunningChanged(Action<EventArgs<bool>> callback)
        {
            runnable.IsRunningChanged += (_, e) => callback(e);
            return runnable;
        }

        /// <summary>
        ///     Subscribes to the <see cref="Runnable.IsRunningChanging" /> event.
        ///     Set <see cref="CancelEventArgs{T}.Cancel" /> to <see langword="true" /> in the callback to cancel the change.
        /// </summary>
        /// <param name="callback">The callback to invoke with the <see cref="CancelEventArgs{T}" /> containing the proposed running state.</param>
        /// <returns>The <see cref="Runnable" /> instance.</returns>
        public Runnable OnRunningChanging(Action<CancelEventArgs<bool>> callback)
        {
            runnable.IsRunningChanging += (_, e) => callback(e);
            return runnable;
        }

        /// <summary>
        ///     Subscribes to the <see cref="Runnable.IsModalChanged" /> event.
        /// </summary>
        /// <param name="callback">The callback to invoke with the <see cref="EventArgs{T}" /> containing the new modal state.</param>
        /// <returns>The <see cref="Runnable" /> instance.</returns>
        public Runnable OnModalChanged(Action<EventArgs<bool>> callback)
        {
            runnable.IsModalChanged += (_, e) => callback(e);
            return runnable;
        }
    }
}
