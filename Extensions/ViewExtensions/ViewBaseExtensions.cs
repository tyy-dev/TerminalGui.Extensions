using Terminal.Gui.Input;
using Terminal.Gui.ViewBase;

using TerminalGui.Extensions.Core;

namespace TerminalGui.Extensions.Extensions.ViewExtensions;

public static class ViewBaseExtensions
{
    extension<T>(T view) where T : View
    {
        public ViewBuilder<T> Builder() => new(view);

        public ViewBuilder<T> ConfigureWithBuilder(Action<ViewBuilder<T>> callback)
        {
            ViewBuilder<T> viewbuilder = view.Builder();
            callback.Invoke(viewbuilder);
            return viewbuilder;
        }
    }

    extension(View view)
    {
        #region Layout

        public View WithLayout(
            Dim? width = null,
            Dim? height = null,
            Pos? x = null,
            Pos? y = null
        )
        {
            if (width is not null)
            {
                view.Width = width;
            }

            if (height is not null)
            {
                view.Height = height;
            }

            if (x is not null)
            {
                view.X = x;
            }

            if (y is not null)
            {
                view.Y = y;
            }

            return view;
        }

        #endregion

        #region Command Event

        public View OnAccepted(Action<CommandEventArgs> callback)
        {
            view.Accepted += (_, e) => callback(e);
            return view;
        }

        public View OnAccepting(Action<CommandEventArgs> callback)
        {
            view.Accepting += (_, e) => callback(e);
            return view;
        }

        public View OnActivating(Action<CommandEventArgs> callback)
        {
            view.Activating += (_, e) => callback(e);
            return view;
        }

        public View OnHandlingHotKey(Action<CommandEventArgs> callback)
        {
            view.HandlingHotKey += (_, e) => callback(e);
            return view;
        }

        public View OnCommandNotBound(Action<CommandEventArgs> callback)
        {
            view.CommandNotBound += (_, e) => callback(e);
            return view;
        }

        #endregion
    }
}
