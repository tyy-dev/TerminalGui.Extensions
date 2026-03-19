using Terminal.Gui.App;

namespace TerminalGui.Extensions.Extensions;

/// <summary>
///     Extension methods for <see cref="ApplicationPopover" />.
/// </summary>
public static class ApplicationPopoverExtensions
{
    extension(ApplicationPopover popover)
    {
        /// <summary>
        ///     Subscribes to the <see cref="ApplicationPopover.PopoverRegistered" /> event.
        /// </summary>
        /// <param name="callback">The callback to invoke with the <see cref="EventArgs{T}" /> containing the registered <see cref="IPopoverView" />.</param>
        /// <returns>The <see cref="ApplicationPopover" /> instance.</returns>
        public ApplicationPopover OnPopoverRegistered(Action<EventArgs<IPopoverView>> callback)
        {
            popover.PopoverRegistered += (_, e) => callback(e);
            return popover;
        }

        /// <summary>
        ///     Subscribes to the <see cref="ApplicationPopover.PopoverDeRegistered" /> event.
        /// </summary>
        /// <param name="callback">The callback to invoke with the <see cref="EventArgs{T}" /> containing the de-registered <see cref="IPopoverView" />.</param>
        /// <returns>The <see cref="ApplicationPopover" /> instance.</returns>
        public ApplicationPopover OnPopoverUnregistered(Action<EventArgs<IPopoverView>> callback)
        {
            popover.PopoverDeRegistered += (_, e) => callback(e);
            return popover;
        }
    }
}
