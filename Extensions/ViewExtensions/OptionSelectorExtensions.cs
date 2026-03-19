using Terminal.Gui.App;
using Terminal.Gui.ViewBase;
using Terminal.Gui.Views;

namespace TerminalGui.Extensions.Extensions.ViewExtensions;

/// <summary>
///     Extension methods for <see cref="OptionSelector" /> and <see cref="OptionSelector{TEnum}" />.
/// </summary>
public static class OptionSelectorExtensions
{
    extension(OptionSelector optionSelector)
    {
        /// <summary>
        ///     Subscribes to the <see cref="SelectorBase.ValueChanged" /> event.
        /// </summary>
        /// <param name="callback">
        ///     The callback to invoke with the <see cref="ValueChangedEventArgs{T}" /> containing old and new
        ///     values.
        /// </param>
        /// <returns>The <see cref="OptionSelector" /> instance.</returns>
        public OptionSelector OnValueChanged(Action<ValueChangedEventArgs<int?>> callback)
        {
            optionSelector.ValueChanged += (_, e) => callback(e);
            return optionSelector;
        }

        /// <summary>
        ///     Subscribes to the <see cref="SelectorBase.ValueChanging" /> event.
        ///     Set <see cref="ValueChangingEventArgs{T}.Handled" /> to <see langword="true" /> in the callback to cancel the
        ///     change.
        /// </summary>
        /// <param name="callback">
        ///     The callback to invoke with the <see cref="ValueChangingEventArgs{T}" /> containing current and
        ///     proposed values.
        /// </param>
        /// <returns>The <see cref="OptionSelector" /> instance.</returns>
        public OptionSelector OnValueChanging(Action<ValueChangingEventArgs<int?>> callback)
        {
            optionSelector.ValueChanging += (_, e) => callback(e);
            return optionSelector;
        }

        /// <summary>
        ///     Subscribes to the <see cref="SelectorBase.OrientationChanged" /> event.
        /// </summary>
        /// <param name="callback">The callback to invoke with the <see cref="EventArgs{T}" /> containing the new <see cref="Orientation" />.</param>
        /// <returns>The <see cref="OptionSelector" /> instance.</returns>
        public OptionSelector OnOrientationChanged(Action<EventArgs<Orientation>> callback)
        {
            optionSelector.OrientationChanged += (_, e) => callback(e);
            return optionSelector;
        }

        /// <summary>
        ///     Subscribes to the <see cref="SelectorBase.OrientationChanging" /> event.
        ///     Set <see cref="CancelEventArgs{T}.Cancel" /> to <see langword="true" /> to cancel the change.
        /// </summary>
        /// <param name="callback">The callback to invoke with the <see cref="CancelEventArgs{T}" /> containing the proposed <see cref="Orientation" />.</param>
        /// <returns>The <see cref="OptionSelector" /> instance.</returns>
        public OptionSelector OnOrientationChanging(Action<CancelEventArgs<Orientation>> callback)
        {
            optionSelector.OrientationChanging += (_, e) => callback(e);
            return optionSelector;
        }
    }

    extension<TEnum>(OptionSelector<TEnum> optionSelector) where TEnum : struct, Enum
    {
        /// <summary>
        ///     Subscribes to the <see cref="OptionSelector{TEnum}.ValueChanged" /> event.
        /// </summary>
        /// <param name="callback">The callback to invoke with the <see cref="EventArgs{T}" /> containing the new value.</param>
        /// <returns>The <see cref="OptionSelector{TEnum}" /> instance.</returns>
        public OptionSelector<TEnum> OnValueChanged(Action<EventArgs<TEnum?>> callback)
        {
            optionSelector.ValueChanged += (_, e) => callback(e);
            return optionSelector;
        }
    }
}
