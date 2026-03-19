using Terminal.Gui.Views;

namespace TerminalGui.Extensions.Extensions.ViewExtensions;

/// <summary>
///     Extension methods for <see cref="TableView" />.
/// </summary>
public static class TableViewExtensions
{
    extension(TableView tableView)
    {
        /// <summary>
        ///     Subscribes to the <see cref="TableView.CellActivated" /> event.
        /// </summary>
        /// <param name="callback">The callback to invoke with the <see cref="CellActivatedEventArgs" />.</param>
        /// <returns>The <see cref="TableView" /> instance.</returns>
        public TableView OnCellActivated(Action<CellActivatedEventArgs> callback)
        {
            tableView.CellActivated += (_, e) => callback(e);
            return tableView;
        }

        /// <summary>
        ///     Subscribes to the <see cref="TableView.CellToggled" /> event.
        /// </summary>
        /// <param name="callback">The callback to invoke with the <see cref="CellToggledEventArgs" />.</param>
        /// <returns>The <see cref="TableView" /> instance.</returns>
        public TableView OnCellToggled(Action<CellToggledEventArgs> callback)
        {
            tableView.CellToggled += (_, e) => callback(e);
            return tableView;
        }

        /// <summary>
        ///     Subscribes to the <see cref="TableView.SelectedCellChanged" /> event.
        /// </summary>
        /// <param name="callback">The callback to invoke with the <see cref="SelectedCellChangedEventArgs" />.</param>
        /// <returns>The <see cref="TableView" /> instance.</returns>
        public TableView OnSelectedCellChanged(Action<SelectedCellChangedEventArgs> callback)
        {
            tableView.SelectedCellChanged += (_, e) => callback(e);
            return tableView;
        }
    }
}
