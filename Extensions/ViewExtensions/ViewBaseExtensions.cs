using System.ComponentModel;
using System.Drawing;

using Terminal.Gui.App;
using Terminal.Gui.Drawing;
using Terminal.Gui.Input;
using Terminal.Gui.ViewBase;
using Terminal.Gui.Views;

using TerminalGui.Extensions.Core.Builders;

namespace TerminalGui.Extensions.Extensions.ViewExtensions;

public static partial class ViewBaseExtensions
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

        #region Subview Search

        /// <summary>
        ///     Finds the first subview of type <typeparamref name="TView" /> in the view's direct children.
        /// </summary>
        /// <typeparam name="TView">The type of subview to search for.</typeparam>
        /// <returns>The first matching subview, or <see langword="null" /> if not found.</returns>
        public TView? FindSubView<TView>() where TView : View
            => view.SubViews.OfType<TView>().FirstOrDefault();

        /// <summary>
        ///     Finds all subviews of type <typeparamref name="TView" /> in the view's hierarchy recursively.
        /// </summary>
        /// <typeparam name="TView">The type of subview to search for.</typeparam>
        /// <returns>An enumerable of all matching subviews in the hierarchy.</returns>
        public IEnumerable<TView> FindAllSubViews<TView>() where TView : View
            => FindAllSubViewsRecursive<TView>(view);

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

        /// <summary>
        ///     Subscribes to the <see cref="View.Activated" /> event.
        /// </summary>
        /// <param name="callback">The callback to invoke with the <see cref="EventArgs{T}" /> containing the command context.</param>
        /// <returns>The view instance.</returns>
        public T OnActivated(Action<EventArgs<ICommandContext>> callback)
        {
            view.Activated += (_, e) => callback(e);
            return view;
        }

        /// <summary>
        ///     Subscribes to the <see cref="View.HotKeyCommand" /> event.
        /// </summary>
        /// <param name="callback">The callback to invoke with the <see cref="EventArgs{T}" /> containing the command context.</param>
        /// <returns>The view instance.</returns>
        public T OnHotKeyCommand(Action<EventArgs<ICommandContext>> callback)
        {
            view.HotKeyCommand += (_, e) => callback(e);
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

        /// <summary>
        ///     Subscribes to the <see cref="View.MouseStateChanged" /> event.
        /// </summary>
        /// <param name="callback">The callback to invoke with the <see cref="EventArgs{T}" /> containing the new <see cref="MouseState" />.</param>
        /// <returns>The view instance.</returns>
        public T OnMouseStateChanged(Action<EventArgs<MouseState>> callback)
        {
            view.MouseStateChanged += (_, e) => callback(e);
            return view;
        }

        /// <summary>
        ///     Subscribes to the <see cref="View.MouseHoldRepeatChanged" /> event.
        /// </summary>
        /// <param name="callback">The callback to invoke with the <see cref="ValueChangedEventArgs{T}" /> containing the old and new <see cref="MouseFlags" /> values.</param>
        /// <returns>The view instance.</returns>
        public T OnMouseHoldRepeatChanged(Action<ValueChangedEventArgs<MouseFlags?>> callback)
        {
            view.MouseHoldRepeatChanged += (_, e) => callback(e);
            return view;
        }

        /// <summary>
        ///     Subscribes to the <see cref="View.MouseHoldRepeatChanging" /> event.
        ///     Set <see cref="ValueChangingEventArgs{T}.Handled" /> to <see langword="true" /> to cancel the change.
        /// </summary>
        /// <param name="callback">The callback to invoke with the <see cref="ValueChangingEventArgs{T}" /> containing the current and proposed <see cref="MouseFlags" /> values.</param>
        /// <returns>The view instance.</returns>
        public T OnMouseHoldRepeatChanging(Action<ValueChangingEventArgs<MouseFlags?>> callback)
        {
            view.MouseHoldRepeatChanging += (_, e) => callback(e);
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

        /// <summary>
        ///     Subscribes to the <see cref="View.HasFocusChanging" /> event.
        /// </summary>
        /// <param name="callback">The callback to invoke with the <see cref="HasFocusEventArgs" />.</param>
        /// <returns>The view instance.</returns>
        public T OnHasFocusChanging(Action<HasFocusEventArgs> callback)
        {
            view.HasFocusChanging += (_, e) => callback(e);
            return view;
        }

        /// <summary>
        ///     Subscribes to the <see cref="View.AdvancingFocus" /> event.
        /// </summary>
        /// <param name="callback">The callback to invoke with the <see cref="AdvanceFocusEventArgs" />.</param>
        /// <returns>The view instance.</returns>
        public T OnAdvancingFocus(Action<AdvanceFocusEventArgs> callback)
        {
            view.AdvancingFocus += (_, e) => callback(e);
            return view;
        }

        /// <summary>
        ///     Subscribes to the <see cref="View.CanFocusChanged" /> event.
        /// </summary>
        /// <param name="callback">The callback to invoke when <see cref="View.CanFocus" /> changes.</param>
        /// <returns>The view instance.</returns>
        public T OnCanFocusChanged(Action callback)
        {
            view.CanFocusChanged += (_, _) => callback();
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

        /// <summary>
        ///     Subscribes to the <see cref="View.Removed" /> event.
        /// </summary>
        /// <param name="callback">The callback to invoke with the <see cref="SuperViewChangedEventArgs" />.</param>
        /// <returns>The view instance.</returns>
        public T OnRemoved(Action<SuperViewChangedEventArgs> callback)
        {
            view.Removed += (_, e) => callback(e);
            return view;
        }

        /// <summary>
        ///     Subscribes to the <see cref="View.SuperViewChanged" /> event.
        /// </summary>
        /// <param name="callback">The callback to invoke with the <see cref="ValueChangedEventArgs{T}" /> containing the old and new <see cref="View" />.</param>
        /// <returns>The view instance.</returns>
        public T OnSuperViewChanged(Action<ValueChangedEventArgs<View?>> callback)
        {
            view.SuperViewChanged += (_, e) => callback(e);
            return view;
        }

        /// <summary>
        ///     Subscribes to the <see cref="View.SuperViewChanging" /> event.
        ///     Set <see cref="ValueChangingEventArgs{T}.Handled" /> to <see langword="true" /> to cancel the change.
        /// </summary>
        /// <param name="callback">The callback to invoke with the <see cref="ValueChangingEventArgs{T}" /> containing the current and proposed <see cref="View" />.</param>
        /// <returns>The view instance.</returns>
        public T OnSuperViewChanging(Action<ValueChangingEventArgs<View?>> callback)
        {
            view.SuperViewChanging += (_, e) => callback(e);
            return view;
        }

        /// <summary>
        ///     Subscribes to the <see cref="View.SubViewAdded" /> event.
        /// </summary>
        /// <param name="callback">The callback to invoke with the <see cref="SuperViewChangedEventArgs" />.</param>
        /// <returns>The view instance.</returns>
        public T OnSubViewAdded(Action<SuperViewChangedEventArgs> callback)
        {
            view.SubViewAdded += (_, e) => callback(e);
            return view;
        }

        /// <summary>
        ///     Subscribes to the <see cref="View.SubViewRemoved" /> event.
        /// </summary>
        /// <param name="callback">The callback to invoke with the <see cref="SuperViewChangedEventArgs" />.</param>
        /// <returns>The view instance.</returns>
        public T OnSubViewRemoved(Action<SuperViewChangedEventArgs> callback)
        {
            view.SubViewRemoved += (_, e) => callback(e);
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

        /// <summary>
        ///     Subscribes to the <see cref="View.TitleChanging" /> event.
        ///     Set <see cref="CancelEventArgs{T}.Cancel" /> to <see langword="true" /> to cancel the change.
        /// </summary>
        /// <param name="callback">The callback to invoke with the <see cref="CancelEventArgs{T}" /> containing the proposed title.</param>
        /// <returns>The view instance.</returns>
        public T OnTitleChanging(Action<CancelEventArgs<string>> callback)
        {
            view.TitleChanging += (_, e) => callback(e);
            return view;
        }

        /// <summary>
        ///     Subscribes to the <see cref="View.VisibleChanging" /> event.
        ///     Set <see cref="CancelEventArgs{T}.Cancel" /> to <see langword="true" /> to cancel the change.
        /// </summary>
        /// <param name="callback">The callback to invoke with the <see cref="CancelEventArgs{T}" /> containing the proposed visibility.</param>
        /// <returns>The view instance.</returns>
        public T OnVisibleChanging(Action<CancelEventArgs<bool>> callback)
        {
            view.VisibleChanging += (_, e) => callback(e);
            return view;
        }

        /// <summary>
        ///     Subscribes to the <see cref="View.BorderStyleChanged" /> event.
        /// </summary>
        /// <param name="callback">The callback to invoke when the border style changes.</param>
        /// <returns>The view instance.</returns>
        public T OnBorderStyleChanged(Action callback)
        {
            view.BorderStyleChanged += (_, _) => callback();
            return view;
        }

        /// <summary>
        ///     Subscribes to the <see cref="View.HotKeyChanged" /> event.
        /// </summary>
        /// <param name="callback">The callback to invoke with the <see cref="KeyChangedEventArgs" /> containing the old and new keys.</param>
        /// <returns>The view instance.</returns>
        public T OnHotKeyChanged(Action<KeyChangedEventArgs> callback)
        {
            view.HotKeyChanged += (_, e) => callback(e);
            return view;
        }

        /// <summary>
        ///     Subscribes to the <see cref="View.SchemeChanged" /> event.
        /// </summary>
        /// <param name="callback">The callback to invoke with the <see cref="ValueChangedEventArgs{T}" /> containing the old and new <see cref="Scheme" />.</param>
        /// <returns>The view instance.</returns>
        public T OnSchemeChanged(Action<ValueChangedEventArgs<Scheme>> callback)
        {
            view.SchemeChanged += (_, e) => callback(e);
            return view;
        }

        /// <summary>
        ///     Subscribes to the <see cref="View.SchemeChanging" /> event.
        ///     Set <see cref="ValueChangingEventArgs{T}.Handled" /> to <see langword="true" /> to cancel the change.
        /// </summary>
        /// <param name="callback">The callback to invoke with the <see cref="ValueChangingEventArgs{T}" /> containing the current and proposed <see cref="Scheme" />.</param>
        /// <returns>The view instance.</returns>
        public T OnSchemeChanging(Action<ValueChangingEventArgs<Scheme>> callback)
        {
            view.SchemeChanging += (_, e) => callback(e);
            return view;
        }

        /// <summary>
        ///     Subscribes to the <see cref="View.SchemeNameChanged" /> event.
        /// </summary>
        /// <param name="callback">The callback to invoke with the <see cref="ValueChangedEventArgs{T}" /> containing the old and new scheme names.</param>
        /// <returns>The view instance.</returns>
        public T OnSchemeNameChanged(Action<ValueChangedEventArgs<string>> callback)
        {
            view.SchemeNameChanged += (_, e) => callback(e);
            return view;
        }

        /// <summary>
        ///     Subscribes to the <see cref="View.SchemeNameChanging" /> event.
        ///     Set <see cref="ValueChangingEventArgs{T}.Handled" /> to <see langword="true" /> to cancel the change.
        /// </summary>
        /// <param name="callback">The callback to invoke with the <see cref="ValueChangingEventArgs{T}" /> containing the current and proposed scheme names.</param>
        /// <returns>The view instance.</returns>
        public T OnSchemeNameChanging(Action<ValueChangingEventArgs<string>> callback)
        {
            view.SchemeNameChanging += (_, e) => callback(e);
            return view;
        }

        /// <summary>
        ///     Subscribes to the <see cref="View.ContentSizeChanged" /> event.
        /// </summary>
        /// <param name="callback">The callback to invoke with the <see cref="ValueChangedEventArgs{T}" /> containing the old and new content sizes.</param>
        /// <returns>The view instance.</returns>
        public T OnContentSizeChanged(Action<ValueChangedEventArgs<Size?>> callback)
        {
            view.ContentSizeChanged += (_, e) => callback(e);
            return view;
        }

        /// <summary>
        ///     Subscribes to the <see cref="View.ContentSizeChanging" /> event.
        ///     Set <see cref="ValueChangingEventArgs{T}.Handled" /> to <see langword="true" /> to cancel the change.
        /// </summary>
        /// <param name="callback">The callback to invoke with the <see cref="ValueChangingEventArgs{T}" /> containing the current and proposed content sizes.</param>
        /// <returns>The view instance.</returns>
        public T OnContentSizeChanging(Action<ValueChangingEventArgs<Size?>> callback)
        {
            view.ContentSizeChanging += (_, e) => callback(e);
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

        /// <summary>
        ///     Subscribes to the <see cref="View.SubViewLayout" /> event.
        /// </summary>
        /// <param name="callback">The callback to invoke with the <see cref="LayoutEventArgs" />.</param>
        /// <returns>The view instance.</returns>
        public T OnSubViewLayout(Action<LayoutEventArgs> callback)
        {
            view.SubViewLayout += (_, e) => callback(e);
            return view;
        }

        /// <summary>
        ///     Subscribes to the <see cref="View.HeightChanged" /> event.
        /// </summary>
        /// <param name="callback">The callback to invoke with the <see cref="ValueChangedEventArgs{T}" /> containing the old and new <see cref="Dim" /> values.</param>
        /// <returns>The view instance.</returns>
        public T OnHeightChanged(Action<ValueChangedEventArgs<Dim>> callback)
        {
            view.HeightChanged += (_, e) => callback(e);
            return view;
        }

        /// <summary>
        ///     Subscribes to the <see cref="View.HeightChanging" /> event.
        ///     Set <see cref="ValueChangingEventArgs{T}.Handled" /> to <see langword="true" /> to cancel the change.
        /// </summary>
        /// <param name="callback">The callback to invoke with the <see cref="ValueChangingEventArgs{T}" /> containing the current and proposed <see cref="Dim" /> values.</param>
        /// <returns>The view instance.</returns>
        public T OnHeightChanging(Action<ValueChangingEventArgs<Dim>> callback)
        {
            view.HeightChanging += (_, e) => callback(e);
            return view;
        }

        /// <summary>
        ///     Subscribes to the <see cref="View.WidthChanged" /> event.
        /// </summary>
        /// <param name="callback">The callback to invoke with the <see cref="ValueChangedEventArgs{T}" /> containing the old and new <see cref="Dim" /> values.</param>
        /// <returns>The view instance.</returns>
        public T OnWidthChanged(Action<ValueChangedEventArgs<Dim>> callback)
        {
            view.WidthChanged += (_, e) => callback(e);
            return view;
        }

        /// <summary>
        ///     Subscribes to the <see cref="View.WidthChanging" /> event.
        ///     Set <see cref="ValueChangingEventArgs{T}.Handled" /> to <see langword="true" /> to cancel the change.
        /// </summary>
        /// <param name="callback">The callback to invoke with the <see cref="ValueChangingEventArgs{T}" /> containing the current and proposed <see cref="Dim" /> values.</param>
        /// <returns>The view instance.</returns>
        public T OnWidthChanging(Action<ValueChangingEventArgs<Dim>> callback)
        {
            view.WidthChanging += (_, e) => callback(e);
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

        /// <summary>
        ///     Subscribes to the <see cref="View.DrawingSubViews" /> event.
        /// </summary>
        /// <param name="callback">The callback to invoke with the <see cref="DrawEventArgs" />.</param>
        /// <returns>The view instance.</returns>
        public T OnDrawingSubViews(Action<DrawEventArgs> callback)
        {
            view.DrawingSubViews += (_, e) => callback(e);
            return view;
        }

        /// <summary>
        ///     Subscribes to the <see cref="View.DrawingText" /> event.
        /// </summary>
        /// <param name="callback">The callback to invoke with the <see cref="DrawEventArgs" />.</param>
        /// <returns>The view instance.</returns>
        public T OnDrawingText(Action<DrawEventArgs> callback)
        {
            view.DrawingText += (_, e) => callback(e);
            return view;
        }

        /// <summary>
        ///     Subscribes to the <see cref="View.DrewText" /> event.
        /// </summary>
        /// <param name="callback">The callback to invoke after text has been drawn.</param>
        /// <returns>The view instance.</returns>
        public T OnDrewText(Action callback)
        {
            view.DrewText += (_, _) => callback();
            return view;
        }

        /// <summary>
        ///     Subscribes to the <see cref="View.ClearingViewport" /> event.
        /// </summary>
        /// <param name="callback">The callback to invoke with the <see cref="DrawEventArgs" />.</param>
        /// <returns>The view instance.</returns>
        public T OnClearingViewport(Action<DrawEventArgs> callback)
        {
            view.ClearingViewport += (_, e) => callback(e);
            return view;
        }

        /// <summary>
        ///     Subscribes to the <see cref="View.ClearedViewport" /> event.
        /// </summary>
        /// <param name="callback">The callback to invoke with the <see cref="DrawEventArgs" />.</param>
        /// <returns>The view instance.</returns>
        public T OnClearedViewport(Action<DrawEventArgs> callback)
        {
            view.ClearedViewport += (_, e) => callback(e);
            return view;
        }

        /// <summary>
        ///     Subscribes to the <see cref="View.ViewportChanged" /> event.
        /// </summary>
        /// <param name="callback">The callback to invoke with the <see cref="DrawEventArgs" />.</param>
        /// <returns>The view instance.</returns>
        public T OnViewportChanged(Action<DrawEventArgs> callback)
        {
            view.ViewportChanged += (_, e) => callback(e);
            return view;
        }

        /// <summary>
        ///     Subscribes to the <see cref="View.GettingAttributeForRole" /> event.
        /// </summary>
        /// <param name="callback">The callback to invoke with the <see cref="VisualRoleEventArgs" />.</param>
        /// <returns>The view instance.</returns>
        public T OnGettingAttributeForRole(Action<VisualRoleEventArgs> callback)
        {
            view.GettingAttributeForRole += (_, e) => callback(e);
            return view;
        }

        /// <summary>
        ///     Subscribes to the <see cref="View.GettingScheme" /> event.
        /// </summary>
        /// <param name="callback">The callback to invoke with the <see cref="ResultEventArgs{T}" /> containing the <see cref="Scheme" />.</param>
        /// <returns>The view instance.</returns>
        public T OnGettingScheme(Action<ResultEventArgs<Scheme>> callback)
        {
            view.GettingScheme += (_, e) => callback(e);
            return view;
        }

        #endregion
    }
}

/// <summary>
/// Extension methods specifically for <see cref="Tabs"/> views.
/// </summary>
public static class TabsExtensions
{
    /// <summary>
    ///     Creates a <see cref="TabsBuilder{TParent}" /> for fluent tab management.
    /// </summary>
    /// <param name="tabs">The <see cref="Tabs"/> instance to configure.</param>
    /// <returns>A <see cref="TabsBuilder{Tabs}" /> for configuring tabs.</returns>
    public static TabsBuilder<Tabs> TabsBuilder(this Tabs tabs) => new(null, tabs);
}

public static partial class ViewBaseExtensions
{
    private static IEnumerable<TView> FindAllSubViewsRecursive<TView>(View parent) where TView : View
    {
        foreach (View child in parent.SubViews)
        {
            if (child is TView match)
            {
                yield return match;
            }

            foreach (TView nested in FindAllSubViewsRecursive<TView>(child))
            {
                yield return nested;
            }
        }
    }
}
