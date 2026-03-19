using System.Collections.Concurrent;
using System.Drawing;

using Terminal.Gui.App;

namespace TerminalGui.Extensions.Extensions;

/// <summary>
///     Extension methods for <see cref="IApplication" />.
/// </summary>
public static class ApplicationExtensions
{
    extension(IApplication application)
    {
        #region Session Stack

        /// <summary>
        ///     The number of active sessions on the <see cref="IApplication.SessionStack" />.
        /// </summary>
        public int SessionCount => application.SessionStack?.Count ?? 0;

        /// <summary>
        ///     Whether the <see cref="IApplication.SessionStack" /> has any active sessions.
        /// </summary>
        public bool HasActiveSessions => application.SessionStack is { IsEmpty: false };

        /// <summary>
        ///     Peeks at the top of the <see cref="IApplication.SessionStack" /> without removing it.
        /// </summary>
        /// <returns>The current <see cref="SessionToken" />, or <see langword="null" /> if the stack is empty.</returns>
        public SessionToken? GetCurrentSession()
        {
            if (application.SessionStack is { } stack && stack.TryPeek(out SessionToken? token))
            {
                return token;
            }

            return null;
        }

        /// <summary>
        ///     Finds a <see cref="SessionToken" /> on the <see cref="IApplication.SessionStack" /> whose
        ///     <see cref="SessionToken.Runnable" /> matches the specified <paramref name="runnable" />.
        /// </summary>
        /// <param name="runnable">The <see cref="IRunnable" /> to search for.</param>
        /// <returns>The matching <see cref="SessionToken" />, or <see langword="null" /> if not found.</returns>
        public SessionToken? FindSession(IRunnable runnable)
        {
            ArgumentNullException.ThrowIfNull(runnable);

            if (application.SessionStack is not { } stack)
            {
                return null;
            }

            foreach (SessionToken token in stack)
            {
                if (ReferenceEquals(token.Runnable, runnable))
                {
                    return token;
                }
            }

            return null;
        }

        #endregion

        #region Events

        /// <summary>
        ///     Subscribes to the <see cref="IApplication.InitializedChanged" /> event,
        ///     raised after <see cref="IApplication.Init" /> or <see cref="IDisposable.Dispose" /> has been called.
        /// </summary>
        /// <param name="callback">The callback to invoke with the <see cref="EventArgs{T}" /> containing the initialization state.</param>
        /// <returns>The <see cref="IApplication" /> instance.</returns>
        public IApplication OnInitializedChanged(Action<EventArgs<bool>> callback)
        {
            application.InitializedChanged += (_, e) => callback(e);
            return application;
        }

        /// <summary>
        ///     Subscribes to the <see cref="IApplication.Iteration" /> event,
        ///     raised on each iteration of the main loop.
        /// </summary>
        /// <param name="callback">The callback to invoke with the <see cref="EventArgs{T}" /> containing the <see cref="IApplication" />.</param>
        /// <returns>The <see cref="IApplication" /> instance.</returns>
        public IApplication OnIteration(Action<EventArgs<IApplication>> callback)
        {
            application.Iteration += (_, e) => callback(e);
            return application;
        }

        /// <summary>
        ///     Subscribes to the <see cref="IApplication.SessionBegun" /> event,
        ///     raised when <see cref="IApplication.Begin" /> has created a new <see cref="SessionToken" />.
        /// </summary>
        /// <param name="callback">The callback to invoke with the <see cref="SessionTokenEventArgs" /> containing the new session token.</param>
        /// <returns>The <see cref="IApplication" /> instance.</returns>
        public IApplication OnSessionBegun(Action<SessionTokenEventArgs> callback)
        {
            application.SessionBegun += (_, e) => callback(e);
            return application;
        }

        /// <summary>
        ///     Subscribes to the <see cref="IApplication.SessionEnded" /> event,
        ///     raised when <see cref="IApplication.End" /> was called and the session is stopping.
        /// </summary>
        /// <param name="callback">The callback to invoke with the <see cref="SessionTokenEventArgs" /> containing the ended session token.</param>
        /// <returns>The <see cref="IApplication" /> instance.</returns>
        public IApplication OnSessionEnded(Action<SessionTokenEventArgs> callback)
        {
            application.SessionEnded += (_, e) => callback(e);
            return application;
        }

        /// <summary>
        ///     Subscribes to the <see cref="IApplication.ScreenChanged" /> event,
        ///     raised when the terminal's size has changed.
        /// </summary>
        /// <param name="callback">The callback to invoke with the <see cref="EventArgs{T}" /> containing the new screen <see cref="Rectangle" />.</param>
        /// <returns>The <see cref="IApplication" /> instance.</returns>
        public IApplication OnScreenChanged(Action<EventArgs<Rectangle>> callback)
        {
            application.ScreenChanged += (_, e) => callback(e);
            return application;
        }

        #endregion
    }
}
