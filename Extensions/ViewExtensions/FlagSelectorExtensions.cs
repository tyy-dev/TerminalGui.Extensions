using Terminal.Gui.App;
using Terminal.Gui.ViewBase;
using Terminal.Gui.Views;

namespace TerminalGui.Extensions.Extensions.ViewExtensions;

/// <summary>
///     Extension methods for <see cref="FlagSelector" /> and <see cref="FlagSelector{TFlagsEnum}" />.
/// </summary>
public static class FlagSelectorExtensions
{
    extension(FlagSelector flagSelector)
    {
        /// <summary>
        ///     Subscribes to the <see cref="SelectorBase.ValueChanged" /> event.
        /// </summary>
        /// <param name="callback">
        ///     The callback to invoke with the <see cref="ValueChangedEventArgs{T}" /> containing old and new values.
        /// </param>
        /// <returns>The <see cref="FlagSelector" /> instance.</returns>
        public FlagSelector OnValueChanged(Action<ValueChangedEventArgs<int?>> callback)
        {
            flagSelector.ValueChanged += (_, e) => callback(e);
            return flagSelector;
        }

        /// <summary>
        ///     Subscribes to the <see cref="SelectorBase.ValueChanging" /> event.
        ///     Set <see cref="ValueChangingEventArgs{T}.Handled" /> to <see langword="true" /> to cancel the change.
        /// </summary>
        /// <param name="callback">
        ///     The callback to invoke with the <see cref="ValueChangingEventArgs{T}" /> containing current and proposed values.
        /// </param>
        /// <returns>The <see cref="FlagSelector" /> instance.</returns>
        public FlagSelector OnValueChanging(Action<ValueChangingEventArgs<int?>> callback)
        {
            flagSelector.ValueChanging += (_, e) => callback(e);
            return flagSelector;
        }

        /// <summary>
        ///     Subscribes to the <see cref="SelectorBase.OrientationChanged" /> event.
        /// </summary>
        /// <param name="callback">The callback to invoke with the <see cref="EventArgs{T}" /> containing the new <see cref="Orientation" />.</param>
        /// <returns>The <see cref="FlagSelector" /> instance.</returns>
        public FlagSelector OnOrientationChanged(Action<EventArgs<Orientation>> callback)
        {
            flagSelector.OrientationChanged += (_, e) => callback(e);
            return flagSelector;
        }

        /// <summary>
        ///     Subscribes to the <see cref="SelectorBase.OrientationChanging" /> event.
        ///     Set <see cref="CancelEventArgs{T}.Cancel" /> to <see langword="true" /> to cancel the change.
        /// </summary>
        /// <param name="callback">The callback to invoke with the <see cref="CancelEventArgs{T}" /> containing the proposed <see cref="Orientation" />.</param>
        /// <returns>The <see cref="FlagSelector" /> instance.</returns>
        public FlagSelector OnOrientationChanging(Action<CancelEventArgs<Orientation>> callback)
        {
            flagSelector.OrientationChanging += (_, e) => callback(e);
            return flagSelector;
        }
    }

    extension<TFlagsEnum>(FlagSelector<TFlagsEnum> flagSelector) where TFlagsEnum : struct, Enum
    {
        /// <summary>
        ///     Subscribes to the <see cref="FlagSelector{TFlagsEnum}.ValueChanged" /> event.
        /// </summary>
        /// <param name="callback">The callback to invoke with the <see cref="EventArgs{T}" /> containing the new value.</param>
        /// <returns>The <see cref="FlagSelector{TFlagsEnum}" /> instance.</returns>
        public FlagSelector<TFlagsEnum> OnValueChanged(Action<EventArgs<TFlagsEnum?>> callback)
        {
            flagSelector.ValueChanged += (_, e) => callback(e);
            return flagSelector;
        }
    }
}
