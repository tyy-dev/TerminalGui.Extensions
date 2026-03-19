using Terminal.Gui.Views;

namespace TerminalGui.Extensions.Extensions.ViewExtensions;

/// <summary>
///     Extension methods for <see cref="HexView" />.
/// </summary>
public static class HexViewExtensions
{
    extension(HexView hexView)
    {
        /// <summary>
        ///     Subscribes to the <see cref="HexView.Edited" /> event.
        /// </summary>
        /// <param name="callback">The callback to invoke with the <see cref="HexViewEditEventArgs" />.</param>
        /// <returns>The <see cref="HexView" /> instance.</returns>
        public HexView OnEdited(Action<HexViewEditEventArgs> callback)
        {
            hexView.Edited += (_, e) => callback(e);
            return hexView;
        }

        /// <summary>
        ///     Subscribes to the <see cref="HexView.PositionChanged" /> event.
        /// </summary>
        /// <param name="callback">The callback to invoke with the <see cref="HexViewEventArgs" />.</param>
        /// <returns>The <see cref="HexView" /> instance.</returns>
        public HexView OnPositionChanged(Action<HexViewEventArgs> callback)
        {
            hexView.PositionChanged += (_, e) => callback(e);
            return hexView;
        }
    }
}
