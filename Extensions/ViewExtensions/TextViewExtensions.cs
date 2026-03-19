using System.Drawing;

using Terminal.Gui.Drawing;
using Terminal.Gui.Views;

namespace TerminalGui.Extensions.Extensions.ViewExtensions;

/// <summary>
///     Extension methods for <see cref="TextView" />.
/// </summary>
public static class TextViewExtensions
{
    extension(TextView textView)
    {
        /// <summary>
        ///     Subscribes to the <see cref="TextView.ContentsChanged" /> event.
        /// </summary>
        /// <param name="callback">The callback to invoke with the <see cref="ContentsChangedEventArgs" />.</param>
        /// <returns>The <see cref="TextView" /> instance.</returns>
        public TextView OnContentsChanged(Action<ContentsChangedEventArgs> callback)
        {
            textView.ContentsChanged += (_, e) => callback(e);
            return textView;
        }

        /// <summary>
        ///     Subscribes to the <see cref="TextView.DrawNormalColor" /> event.
        /// </summary>
        /// <param name="callback">The callback to invoke with the <see cref="CellEventArgs" />.</param>
        /// <returns>The <see cref="TextView" /> instance.</returns>
        public TextView OnDrawNormalColor(Action<CellEventArgs> callback)
        {
            textView.DrawNormalColor += (_, e) => callback(e);
            return textView;
        }

        /// <summary>
        ///     Subscribes to the <see cref="TextView.DrawReadOnlyColor" /> event.
        /// </summary>
        /// <param name="callback">The callback to invoke with the <see cref="CellEventArgs" />.</param>
        /// <returns>The <see cref="TextView" /> instance.</returns>
        public TextView OnDrawReadOnlyColor(Action<CellEventArgs> callback)
        {
            textView.DrawReadOnlyColor += (_, e) => callback(e);
            return textView;
        }

        /// <summary>
        ///     Subscribes to the <see cref="TextView.DrawSelectionColor" /> event.
        /// </summary>
        /// <param name="callback">The callback to invoke with the <see cref="CellEventArgs" />.</param>
        /// <returns>The <see cref="TextView" /> instance.</returns>
        public TextView OnDrawSelectionColor(Action<CellEventArgs> callback)
        {
            textView.DrawSelectionColor += (_, e) => callback(e);
            return textView;
        }

        /// <summary>
        ///     Subscribes to the <see cref="TextView.DrawUsedColor" /> event.
        /// </summary>
        /// <param name="callback">The callback to invoke with the <see cref="CellEventArgs" />.</param>
        /// <returns>The <see cref="TextView" /> instance.</returns>
        public TextView OnDrawUsedColor(Action<CellEventArgs> callback)
        {
            textView.DrawUsedColor += (_, e) => callback(e);
            return textView;
        }

        /// <summary>
        ///     Subscribes to the <see cref="TextView.UnwrappedCursorPositionChanged" /> event.
        /// </summary>
        /// <param name="callback">The callback to invoke with the <see cref="Point" /> representing the unwrapped cursor position.</param>
        /// <returns>The <see cref="TextView" /> instance.</returns>
        public TextView OnUnwrappedCursorPositionChanged(Action<Point> callback)
        {
            textView.UnwrappedCursorPositionChanged += (_, e) => callback(e);
            return textView;
        }
    }
}
