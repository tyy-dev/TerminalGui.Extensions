using System.ComponentModel;
using System.Drawing;

using Terminal.Gui.App;
using Terminal.Gui.Input;
using Terminal.Gui.ViewBase;

using TerminalGui.Extensions.Core.Builders;

namespace TerminalGui.Extensions.Extensions.ViewExtensions;

public static class ViewBaseExtensions
{
    extension<T>(T view) where T : View
    {
        /// <summary>
        ///     Creates a <see cref="ViewBuilder{T}" /> for this view.
        /// </summary>
        /// <returns>A new <see cref="ViewBuilder{T}" /> instance.</returns>
        public ViewBuilder<T> Builder() => new(view);

        /// <summary>
        ///     Configures this view using a <see cref="ViewBuilder{T}" /> callback.
        /// </summary>
        /// <param name="callback">The configuration callback.</param>
        /// <returns>The <see cref="ViewBuilder{T}" /> created during configuration.</returns>
        public ViewBuilder<T> ConfigureWithBuilder(Action<ViewBuilder<T>> callback) => ViewBuilder<T>.Configure(view.Builder, callback);

        #region Layout

        /// <summary>
        ///     Sets the layout properties of the view and returns it.
        /// </summary>
        /// <param name="width">The width to set, or <see langword="null" /> to leave unchanged.</param>
        /// <param name="height">The height to set, or <see langword="null" /> to leave unchanged.</param>
        /// <param name="x">The X position to set, or <see langword="null" /> to leave unchanged.</param>
        /// <param name="y">The Y position to set, or <see langword="null" /> to leave unchanged.</param>
        /// <returns>The view instance.</returns>
        public T WithLayout(
            Dim? width = null,
            Dim? height = null,
            Pos? x = null,
            Pos? y = null
        )
        {
            if (width is { })
            {
                view.Width = width;
            }

            if (height is { })
            {
                view.Height = height;
            }

            if (x is { })
            {
                view.X = x;
            }

            if (y is { })
            {
                view.Y = y;
            }

            return view;
        }

        /// <summary>
        ///     Sets both width and height to <see cref="Dim.Fill()" /> and returns the view.
        /// </summary>
        /// <param name="widthAdjust">An optional value added to the width dimension.</param>
        /// <param name="heightAdjust">An optional value added to the height dimension.</param>
        /// <returns>The view instance.</returns>
        public T WithFill(int widthAdjust = 0, int heightAdjust = 0) =>
            view.WithLayout(width: Dim.Fill() + widthAdjust, height: Dim.Fill() + heightAdjust);

        /// <summary>
        ///     Sets both width and height to <see cref="Dim.Auto()" /> and returns the view.
        /// </summary>
        /// <param name="widthAdjust">An optional value added to the width dimension.</param>
        /// <param name="heightAdjust">An optional value added to the height dimension.</param>
        /// <returns>The view instance.</returns>
        public T WithAuto(int widthAdjust = 0, int heightAdjust = 0) =>
            view.WithLayout(width: Dim.Auto() + widthAdjust, height: Dim.Auto() + heightAdjust);

        /// <summary>
        ///     Sets width to <see cref="Dim.Fill()" /> and height to <see cref="Dim.Auto()" />,
        ///     and returns the view.
        /// </summary>
        /// <param name="widthAdjust">An optional value added to the width dimension.</param>
        /// <param name="heightAdjust">An optional value added to the height dimension.</param>
        /// <returns>The view instance.</returns>
        public T WithFillAuto(int widthAdjust = 0, int heightAdjust = 0) =>
            view.WithLayout(width: Dim.Fill() + widthAdjust, height: Dim.Auto() + heightAdjust);

        #endregion

        #region Scrollable Content

        /// <summary>
        ///     Configures this view as scrollable content with a vertical scroll bar.
        ///     Automatically tracks the content size based on subview positions so that
        ///     the scroll bar appears when content exceeds the viewport.
        /// </summary>
        /// <returns>The view instance</returns>
        public T MakeScrollable()
        {
            view.ViewportSettings |= ViewportSettingsFlags.HasVerticalScrollBar;

            view.SubViewsLaidOut += (_, _) => {
                int maxBottom = view.SubViews
                   .Select(subView => subView.Frame.Y + subView.Frame.Height)
                   .Prepend(0)
                   .Max();

                Size needed = new(view.Viewport.Width, Math.Max(maxBottom, view.Viewport.Height));

                if (view.GetContentSize() != needed)
                {
                    view.SetContentSize(needed);
                }
            };

            return view;
        }

        #endregion

        #region Command Events

        /// <summary>
        ///     Subscribes to the <see cref="View.Accepted" /> event.
        /// </summary>
        /// <param name="callback">The callback to invoke when the event fires.</param>
        /// <returns>The view instance</returns>
        public T OnAccepted(Action<CommandEventArgs> callback)
        {
            view.Accepted += (_, e) => callback(e);
            return view;
        }

        /// <summary>
        ///     Subscribes to the <see cref="View.Accepting" /> event.
        /// </summary>
        /// <param name="callback">The callback to invoke when the event fires.</param>
        /// <returns>The view instance.</returns>
        public T OnAccepting(Action<CommandEventArgs> callback)
        {
            view.Accepting += (_, e) => callback(e);
            return view;
        }

        /// <summary>
        ///     Subscribes to the <see cref="View.Activating" /> event.
        /// </summary>
        /// <param name="callback">The callback to invoke when the event fires.</param>
        /// <returns>The view instance.</returns>
        public T OnActivating(Action<CommandEventArgs> callback)
        {
            view.Activating += (_, e) => callback(e);
            return view;
        }

        /// <summary>
        ///     Subscribes to the <see cref="View.HandlingHotKey" /> event.
        /// </summary>
        /// <param name="callback">The callback to invoke when the event fires.</param>
        /// <returns>The view instance.</returns>
        public T OnHandlingHotKey(Action<CommandEventArgs> callback)
        {
            view.HandlingHotKey += (_, e) => callback(e);
            return view;
        }

        /// <summary>
        ///     Subscribes to the <see cref="View.CommandNotBound" /> event.
        /// </summary>
        /// <param name="callback">The callback to invoke when the event fires.</param>
        /// <returns>The view instance.</returns>
        public T OnCommandNotBound(Action<CommandEventArgs> callback)
        {
            view.CommandNotBound += (_, e) => callback(e);
            return view;
        }

        #endregion

        #region Keyboard Events

        /// <summary>
        ///     Subscribes to the <see cref="View.KeyDown" /> event.
        /// </summary>
        /// <param name="callback">The callback to invoke with the <see cref="Key" /> event args.</param>
        /// <returns>The view instance.</returns>
        public T OnKeyDown(Action<Key> callback)
        {
            view.KeyDown += (_, e) => callback(e);
            return view;
        }

        /// <summary>
        ///     Subscribes to the <see cref="View.KeyDownNotHandled" /> event.
        /// </summary>
        /// <param name="callback">The callback to invoke with the <see cref="Key" /> event args.</param>
        /// <returns>The view instance.</returns>
        public T OnKeyDownNotHandled(Action<Key> callback)
        {
            view.KeyDownNotHandled += (_, e) => callback(e);
            return view;
        }

        #endregion

        #region Mouse Events

        /// <summary>
        ///     Subscribes to the <see cref="View.MouseEvent" /> event.
        /// </summary>
        /// <param name="callback">The callback to invoke with the <see cref="Mouse" /> event args.</param>
        /// <returns>The view instance.</returns>
        public T OnMouseEvent(Action<Mouse> callback)
        {
            view.MouseEvent += (_, e) => callback(e);
            return view;
        }

        /// <summary>
        ///     Subscribes to the <see cref="View.MouseEnter" /> event.
        /// </summary>
        /// <param name="callback">The callback to invoke with the <see cref="CancelEventArgs" />.</param>
        /// <returns>The view instance.</returns>
        public T OnMouseEnter(Action<CancelEventArgs> callback)
        {
            view.MouseEnter += (_, e) => callback(e);
            return view;
        }

        /// <summary>
        ///     Subscribes to the <see cref="View.MouseLeave" /> event.
        /// </summary>
        /// <param name="callback">The callback to invoke when the mouse leaves.</param>
        /// <returns>The view instance.</returns>
        public T OnMouseLeave(Action callback)
        {
            view.MouseLeave += (_, _) => callback();
            return view;
        }

        #endregion

        #region Focus Events

        /// <summary>
        ///     Subscribes to the <see cref="View.HasFocusChanged" /> event.
        /// </summary>
        /// <param name="callback">The callback to invoke with the <see cref="HasFocusEventArgs" />.</param>
        /// <returns>The view instance.</returns>
        public T OnHasFocusChanged(Action<HasFocusEventArgs> callback)
        {
            view.HasFocusChanged += (_, e) => callback(e);
            return view;
        }

        /// <summary>
        ///     Subscribes to the <see cref="View.FocusedChanged" /> event.
        /// </summary>
        /// <param name="callback">The callback to invoke with the <see cref="HasFocusEventArgs" />.</param>
        /// <returns>The view instance.</returns>
        public T OnFocusedChanged(Action<HasFocusEventArgs> callback)
        {
            view.FocusedChanged += (_, e) => callback(e);
            return view;
        }

        #endregion

        #region Lifecycle Events

        /// <summary>
        ///     Subscribes to the <see cref="View.Initialized" /> event.
        /// </summary>
        /// <param name="callback">The callback to invoke when the view is initialized.</param>
        /// <returns>The view instance.</returns>
        public T OnInitialized(Action callback)
        {
            view.Initialized += (_, _) => callback();
            return view;
        }

        /// <summary>
        ///     Subscribes to the <see cref="View.Disposing" /> event.
        /// </summary>
        /// <param name="callback">The callback to invoke when the view is being disposed.</param>
        /// <returns>The view instance.</returns>
        public T OnDisposing(Action callback)
        {
            view.Disposing += (_, _) => callback();
            return view;
        }

        #endregion

        #region State Change Events

        /// <summary>
        ///     Subscribes to the <see cref="View.VisibleChanged" /> event.
        /// </summary>
        /// <param name="callback">The callback to invoke when visibility changes.</param>
        /// <returns>The view instance.</returns>
        public T OnVisibleChanged(Action callback)
        {
            view.VisibleChanged += (_, _) => callback();
            return view;
        }

        /// <summary>
        ///     Subscribes to the <see cref="View.EnabledChanged" /> event.
        /// </summary>
        /// <param name="callback">The callback to invoke when the enabled state changes.</param>
        /// <returns>The view instance.</returns>
        public T OnEnabledChanged(Action callback)
        {
            view.EnabledChanged += (_, _) => callback();
            return view;
        }

        /// <summary>
        ///     Subscribes to the <see cref="View.TextChanged" /> event.
        /// </summary>
        /// <param name="callback">The callback to invoke when the text changes.</param>
        /// <returns>The view instance.</returns>
        public T OnTextChanged(Action callback)
        {
            view.TextChanged += (_, _) => callback();
            return view;
        }

        /// <summary>
        ///     Subscribes to the <see cref="View.TitleChanged" /> event.
        /// </summary>
        /// <param name="callback">The callback to invoke with the <see cref="EventArgs{T}" /> containing the new title.</param>
        /// <returns>The view instance.</returns>
        public T OnTitleChanged(Action<EventArgs<string>> callback)
        {
            view.TitleChanged += (_, e) => callback(e);
            return view;
        }

        #endregion

        #region Layout Events

        /// <summary>
        ///     Subscribes to the <see cref="View.SubViewsLaidOut" /> event.
        /// </summary>
        /// <param name="callback">The callback to invoke with the <see cref="LayoutEventArgs" />.</param>
        /// <returns>The view instance.</returns>
        public T OnSubViewsLaidOut(Action<LayoutEventArgs> callback)
        {
            view.SubViewsLaidOut += (_, e) => callback(e);
            return view;
        }

        /// <summary>
        ///     Subscribes to the <see cref="View.FrameChanged" /> event.
        /// </summary>
        /// <param name="callback">The callback to invoke with the <see cref="EventArgs{T}" /> containing the new frame rectangle.</param>
        /// <returns>The view instance.</returns>
        public T OnFrameChanged(Action<EventArgs<Rectangle>> callback)
        {
            view.FrameChanged += (_, e) => callback(e);
            return view;
        }

        #endregion

        #region Drawing Events

        /// <summary>
        ///     Subscribes to the <see cref="View.DrawComplete" /> event.
        /// </summary>
        /// <param name="callback">The callback to invoke with the <see cref="DrawEventArgs" />.</param>
        /// <returns>The view instance.</returns>
        public T OnDrawComplete(Action<DrawEventArgs> callback)
        {
            view.DrawComplete += (_, e) => callback(e);
            return view;
        }

        /// <summary>
        ///     Subscribes to the <see cref="View.DrawingContent" /> event.
        /// </summary>
        /// <param name="callback">The callback to invoke with the <see cref="DrawEventArgs" />.</param>
        /// <returns>The view instance.</returns>
        public T OnDrawingContent(Action<DrawEventArgs> callback)
        {
            view.DrawingContent += (_, e) => callback(e);
            return view;
        }

        #endregion
    }
}
