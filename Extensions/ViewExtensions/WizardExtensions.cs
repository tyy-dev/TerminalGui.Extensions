using System.ComponentModel;

using Terminal.Gui.App;
using Terminal.Gui.Views;

namespace TerminalGui.Extensions.Extensions.ViewExtensions;

/// <summary>
///     Extension methods for <see cref="Wizard" />.
/// </summary>
public static class WizardExtensions
{
    extension(Wizard wizard)
    {
        /// <summary>
        ///     Subscribes to the <see cref="Wizard.MovingBack" /> event.
        ///     Set <see cref="CancelEventArgs.Cancel" /> to <see langword="true" /> to prevent navigating back.
        /// </summary>
        /// <param name="callback">The callback to invoke with the <see cref="CancelEventArgs" />.</param>
        /// <returns>The <see cref="Wizard" /> instance.</returns>
        public Wizard OnMovingBack(Action<CancelEventArgs> callback)
        {
            wizard.MovingBack += (_, e) => callback(e);
            return wizard;
        }

        /// <summary>
        ///     Subscribes to the <see cref="Wizard.MovingNext" /> event.
        ///     Set <see cref="CancelEventArgs.Cancel" /> to <see langword="true" /> to prevent navigating forward.
        /// </summary>
        /// <param name="callback">The callback to invoke with the <see cref="CancelEventArgs" />.</param>
        /// <returns>The <see cref="Wizard" /> instance.</returns>
        public Wizard OnMovingNext(Action<CancelEventArgs> callback)
        {
            wizard.MovingNext += (_, e) => callback(e);
            return wizard;
        }

        /// <summary>
        ///     Subscribes to the <see cref="Wizard.StepChanged" /> event.
        /// </summary>
        /// <param name="callback">The callback to invoke with the <see cref="ValueChangedEventArgs{T}" /> containing the old and new <see cref="WizardStep" />.</param>
        /// <returns>The <see cref="Wizard" /> instance.</returns>
        public Wizard OnStepChanged(Action<ValueChangedEventArgs<WizardStep>> callback)
        {
            wizard.StepChanged += (_, e) => callback(e);
            return wizard;
        }

        /// <summary>
        ///     Subscribes to the <see cref="Wizard.StepChanging" /> event.
        ///     Set <see cref="ValueChangingEventArgs{T}.Handled" /> to <see langword="true" /> to cancel the step change.
        /// </summary>
        /// <param name="callback">The callback to invoke with the <see cref="ValueChangingEventArgs{T}" /> containing the current and proposed <see cref="WizardStep" />.</param>
        /// <returns>The <see cref="Wizard" /> instance.</returns>
        public Wizard OnStepChanging(Action<ValueChangingEventArgs<WizardStep>> callback)
        {
            wizard.StepChanging += (_, e) => callback(e);
            return wizard;
        }
    }
}
