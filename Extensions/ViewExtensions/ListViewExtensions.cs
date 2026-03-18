using System.Collections.Specialized;

using Terminal.Gui.App;
using Terminal.Gui.Views;

namespace TerminalGui.Extensions.Extensions.ViewExtensions;

/// <summary>
///     Extension methods for <see cref="ListView" />.
/// </summary>
public static class ListViewExtensions
{
    extension(ListView listView)
    {
        /// <summary>
        ///     Returns the currently selected item from the given source list,
        ///     or <see langword="default" /> when no item is selected or the source is empty.
        /// </summary>
        /// <typeparam name="T">The type of items in the source list.</typeparam>
        /// <param name="source">The source list backing the <see cref="ListView" />.</param>
        /// <returns>The selected item, or <see langword="default" />.</returns>
        public T? GetSelectedItem<T>(IList<T> source)
        {
            if (source.Count == 0 || listView.SelectedItem is null)
            {
                return default;
            }

            return source[listView.SelectedItem!.Value];
        }

        /// <summary>
        ///     Subscribes to the <see cref="ListView.ValueChanged" /> event.
        /// </summary>
        /// <param name="callback">
        ///     The callback to invoke with the <see cref="ValueChangedEventArgs{T}" /> containing old and new
        ///     selected indices.
        /// </param>
        /// <returns>The <see cref="ListView" /> instance for fluent chaining.</returns>
        public ListView OnValueChanged(Action<ValueChangedEventArgs<int?>> callback)
        {
            listView.ValueChanged += (_, e) => callback(e);
            return listView;
        }

        /// <summary>
        ///     Subscribes to the <see cref="ListView.CollectionChanged" /> event.
        /// </summary>
        /// <param name="callback">The callback to invoke with the <see cref="NotifyCollectionChangedEventArgs" />.</param>
        /// <returns>The <see cref="ListView" /> instance for fluent chaining.</returns>
        public ListView OnCollectionChanged(Action<NotifyCollectionChangedEventArgs> callback)
        {
            listView.CollectionChanged += (_, e) => callback(e);
            return listView;
        }

        /// <summary>
        ///     Subscribes to the <see cref="ListView.SourceChanged" /> event.
        /// </summary>
        /// <param name="callback">The callback to invoke when the source changes.</param>
        /// <returns>The <see cref="ListView" /> instance for fluent chaining.</returns>
        public ListView OnSourceChanged(Action callback)
        {
            listView.SourceChanged += (_, _) => callback();
            return listView;
        }

        /// <summary>
        ///     Configures scroll bar visibility for the <see cref="ListView" />.
        /// </summary>
        /// <param name="vertical">Whether to show the vertical scroll bar.</param>
        /// <param name="horizontal">Whether to show the horizontal scroll bar.</param>
        /// <returns>The <see cref="ListView" /> instance for fluent chaining.</returns>
        public ListView WithScrollBars(bool vertical = true, bool horizontal = false)
        {
            listView.VerticalScrollBar.Visible = vertical;
            listView.HorizontalScrollBar.Visible = horizontal;
            return listView;
        }
    }
}
