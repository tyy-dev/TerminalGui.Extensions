using Terminal.Gui.Views;

namespace TerminalGui.Extensions.Extensions.ViewExtensions;

/// <summary>
///     Extension methods for <see cref="TabView" />.
/// </summary>
public static class TabViewExtensions
{
    extension(TabView tabView)
    {
        /// <summary>
        ///     Subscribes to the <see cref="TabView.SelectedTabChanged" /> event.
        /// </summary>
        /// <param name="callback">The callback to invoke with the <see cref="TabChangedEventArgs" /> containing old and new tabs.</param>
        /// <returns>The <see cref="TabView" /> instance.</returns>
        public TabView OnSelectedTabChanged(Action<TabChangedEventArgs> callback)
        {
            tabView.SelectedTabChanged += (_, e) => callback(e);
            return tabView;
        }

        /// <summary>
        ///     Subscribes to the <see cref="TabView.TabClicked" /> event.
        /// </summary>
        /// <param name="callback">The callback to invoke with the <see cref="TabMouseEventArgs" />.</param>
        /// <returns>The <see cref="TabView" /> instance.</returns>
        public TabView OnTabClicked(Action<TabMouseEventArgs> callback)
        {
            tabView.TabClicked += (_, e) => callback(e);
            return tabView;
        }
    }
}
