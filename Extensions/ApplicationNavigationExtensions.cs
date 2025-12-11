using Terminal.Gui.App;
using Terminal.Gui.ViewBase;
using Terminal.Gui.Views;

using TerminalGui.Extensions.Extensions.ViewExtensions;

namespace TerminalGui.Extensions.Extensions;

public static class ApplicationNavigationExtensions
{
    extension(ApplicationNavigation navigation)
    {
        public ApplicationNavigation OnFocusChanged(Action<EventArgs> callback)
        {
            navigation.FocusedChanged += (_, args) => callback(args);
            return navigation;
        }

        #region Navigation

        /// <inheritdoc cref="NavigatesTo{TRunnableTo}(ApplicationNavigation, View, TRunnableTo, bool)"
        ///     path="/param[@name='targetView']" />
        /// <param name="runnableToFactory">An factory returning the runnable instance to navigate to.</param>
        public View NavigatesTo<TRunnableTo>(View targetView, Func<TRunnableTo> runnableToFactory, bool closeCurrent = false)
            where TRunnableTo : Runnable => navigation.SetupNavigation(
            targetView,
            closeCurrent,
            () => navigation.App?.Run(runnableToFactory())); // Invoke factory each time

        /// <param name="targetView">The runnable instance whose Accepting event will trigger navigation to the new runnable.</param>
        /// <param name="runnableTo">The runnable instance to navigate to.</param>
        public View NavigatesTo<TRunnableTo>(View targetView, TRunnableTo runnableTo, bool closeCurrent = false)
            where TRunnableTo : Runnable => navigation.SetupNavigation(targetView, closeCurrent, () => navigation.App?.Run(runnableTo));

        /// <inheritdoc cref="NavigatesTo{TRunnableTo}(ApplicationNavigation, View, TRunnableTo, bool)"
        ///     path="/param[@name='targetView']" />
        public View NavigatesTo<TRunnableTo>(View targetView, bool closeCurrent = false)
            where TRunnableTo : Runnable, new() => navigation.SetupNavigation(targetView, closeCurrent, () => navigation.App?.Run<TRunnableTo>());

        /// <inheritdoc cref="NavigatesTo{TRunnableTo}(ApplicationNavigation, View, TRunnableTo, bool)"
        ///     path="/param[@name='targetView']" />
        internal View SetupNavigation(View targetView, bool closeCurrent, Action runAction)
        {
            ArgumentNullException.ThrowIfNull(navigation.App, nameof(navigation.App));

            targetView.OnActivating(
                _ => {
                    if (closeCurrent)
                    {
                        navigation.App.RequestStop();
                    }

                    runAction();
                });
            return targetView;
        }

        #endregion
    }
}
