using Terminal.Gui;
using Terminal.Gui.App;
using Terminal.Gui.Drawing;
using Terminal.Gui.ViewBase;
using Terminal.Gui.Views;

namespace TerminalGui.Extensions.Core.Builders;

/// <summary>
///     Fluent builder for configuring and managing <see cref="Tabs" /> views.
/// </summary>
/// <typeparam name="TParent">
///     The type of the parent view that contains the <see cref="Tabs" />.
/// </typeparam>
public sealed class TabsBuilder<TParent> where TParent : View
{
    private readonly ViewBuilder<TParent>? _parentBuilder;
    private readonly Tabs _tabs;

    /// <summary>
    ///     Initializes a new instance of the <see cref="TabsBuilder{TParent}" /> class.
    /// </summary>
    /// <param name="parentBuilder">
    ///     The parent <see cref="ViewBuilder{TParent}" /> that owns this tabs instance.
    /// </param>
    /// <param name="tabs">
    ///     The <see cref="Tabs" /> instance to configure.
    /// </param>
    internal TabsBuilder(ViewBuilder<TParent>? parentBuilder, Tabs tabs)
    {
        _parentBuilder = parentBuilder;
        _tabs = tabs;
    }

    /// <summary>
    ///     The configured <see cref="Tabs" /> instance.
    /// </summary>
    public Tabs Tabs => _tabs;

    /// <summary>
    ///     Returns to the parent <see cref="ViewBuilder{TParent}" />.
    ///     Only available when this builder was created from a parent builder context.
    /// </summary>
    /// <returns>The parent builder instance.</returns>
    /// <exception cref="InvalidOperationException">Thrown when there is no parent builder (standalone Tabs instance).</exception>
    public ViewBuilder<TParent> Done()
    {
        if (_parentBuilder == null)
        {
            throw new InvalidOperationException(
                "Cannot call Done() on a TabsBuilder created from a standalone Tabs instance. " +
                "This method is only available when the builder was created via ViewBuilder<TParent>.AddTabs().");
        }
        return _parentBuilder;
    }

    /// <summary>
    ///     Configures the horizontal scroll offset for tab headers.
    /// </summary>
    /// <param name="scrollOffset">
    ///     <inheritdoc cref="Tabs.ScrollOffset" path="/summary" />
    /// </param>
    /// <returns>This <see cref="TabsBuilder{TParent}" /> instance for fluent chaining.</returns>
    public TabsBuilder<TParent> WithScrollOffset(int scrollOffset)
    {
        _tabs.ScrollOffset = scrollOffset;
        return this;
    }

    /// <summary>
    ///     Configures the height of the tab header area.
    /// </summary>
    /// <param name="tabDepth">
    ///     <inheritdoc cref="Tabs.TabDepth" path="/summary" />
    /// </param>
    /// <returns>This <see cref="TabsBuilder{TParent}" /> instance for fluent chaining.</returns>
    public TabsBuilder<TParent> WithTabDepth(int tabDepth)
    {
        _tabs.TabDepth = tabDepth;
        return this;
    }

    /// <summary>
    ///     Configures the line style for tab borders.
    /// </summary>
    /// <param name="lineStyle">
    ///     <inheritdoc cref="Tabs.TabLineStyle" path="/summary" />
    /// </param>
    /// <returns>This <see cref="TabsBuilder{TParent}" /> instance for fluent chaining.</returns>
    public TabsBuilder<TParent> WithLineStyle(LineStyle lineStyle)
    {
        _tabs.TabLineStyle = lineStyle;
        return this;
    }

    /// <summary>
    ///     Configures which side tabs appear on.
    /// </summary>
    /// <param name="side">
    ///     <inheritdoc cref="Tabs.TabSide" path="/summary" />
    /// </param>
    /// <returns>This <see cref="TabsBuilder{TParent}" /> instance for fluent chaining.</returns>
    public TabsBuilder<TParent> WithTabSide(Side side)
    {
        _tabs.TabSide = side;
        return this;
    }

    /// <summary>
    ///     Configures the space between tab headers.
    /// </summary>
    /// <param name="spacing">
    ///     <inheritdoc cref="Tabs.TabSpacing" path="/summary" />
    /// </param>
    /// <returns>This <see cref="TabsBuilder{TParent}" /> instance for fluent chaining.</returns>
    public TabsBuilder<TParent> WithTabSpacing(int spacing)
    {
        _tabs.TabSpacing = spacing;
        return this;
    }

    /// <summary>
    ///     Adds a new tab with the specified title and builds its content using a fluent builder.
    /// </summary>
    /// <param name="title">The title text to display on the tab header.</param>
    /// <param name="configure">
    ///     A configuration action that receives a <see cref="ViewBuilder{TView}" /> for the tab's content view.
    /// </param>
    /// <returns>This <see cref="TabsBuilder{TParent}" /> instance for fluent chaining.</returns>
    public TabsBuilder<TParent> AddTab(string title, Action<ViewBuilder<View>> configure)
    {
        var view = new View { Title = title };
        configure(new ViewBuilder<View>(view));
        _tabs.Add(view);
        return this;
    }

    /// <summary>
    ///     Adds a new tab with the specified title and an existing view as content.
    /// </summary>
    /// <param name="title">The title text to display on the tab header.</param>
    /// <param name="view">The view to display as the tab's content.</param>
    /// <returns>This <see cref="TabsBuilder{TParent}" /> instance for fluent chaining.</returns>
    public TabsBuilder<TParent> AddTab(string title, View view)
    {
        view.Title = title;
        _tabs.Add(view);
        return this;
    }

    /// <summary>
    ///     Adds a new tab with the specified title and returns a builder for the tab's content.
    /// </summary>
    /// <typeparam name="TView">The type of view to use as the tab's content.</typeparam>
    /// <param name="title">The title text to display on the tab header.</param>
    /// <param name="tabView">The created view instance for the tab's content.</param>
    /// <returns>
    ///     A <see cref="ViewBuilder{TView}" /> for configuring the tab's content view.
    /// </returns>
    public ViewBuilder<TView> AddTab<TView>(string title, out TView tabView) where TView : View, new()
    {
        tabView = new TView { Title = title };
        _tabs.Add(tabView);
        return new ViewBuilder<TView>(tabView);
    }

    /// <summary>
    ///     Removes the tab at the specified index.
    /// </summary>
    /// <param name="index">The zero-based index of the tab to remove.</param>
    /// <returns>This <see cref="TabsBuilder{TParent}" /> instance for fluent chaining.</returns>
    public TabsBuilder<TParent> RemoveTabAt(int index)
    {
        var tabs = _tabs.TabCollection.ToList();
        if (index >= 0 && index < tabs.Count)
        {
            _tabs.Remove(tabs[index]);
        }
        return this;
    }

    /// <summary>
    ///     Removes all tabs from the collection.
    /// </summary>
    /// <returns>This <see cref="TabsBuilder{TParent}" /> instance for fluent chaining.</returns>
    public TabsBuilder<TParent> ClearTabs()
    {
        _tabs.RemoveAll();
        return this;
    }

    /// <summary>
    ///     Sets the currently selected tab by index.
    /// </summary>
    /// <param name="index">The zero-based index of the tab to select.</param>
    /// <returns>This <see cref="TabsBuilder{TParent}" /> instance for fluent chaining.</returns>
    public TabsBuilder<TParent> SelectTab(int index)
    {
        var tabs = _tabs.TabCollection.ToList();
        if (index >= 0 && index < tabs.Count)
        {
            _tabs.Value = tabs[index];
        }
        return this;
    }

    /// <summary>
    ///     Sets the currently selected tab by finding the tab with the specified title.
    /// </summary>
    /// <param name="title">The title of the tab to select.</param>
    /// <returns>This <see cref="TabsBuilder{TParent}" /> instance for fluent chaining.</returns>
    public TabsBuilder<TParent> SelectTab(string title)
    {
        var tab = _tabs.TabCollection.FirstOrDefault(t => t.Title == title);
        if (tab != null)
        {
            _tabs.Value = tab;
        }
        return this;
    }

    /// <summary>
    ///     Attaches a handler to the <see cref="Tabs.ValueChanged" /> event.
    /// </summary>
    /// <param name="handler">
    ///     The event handler to invoke when the selected tab changes.
    /// </param>
    /// <returns>This <see cref="TabsBuilder{TParent}" /> instance for fluent chaining.</returns>
    public TabsBuilder<TParent> OnValueChanged(Action<ValueChangedEventArgs<View?>> handler)
    {
        _tabs.ValueChanged += (sender, args) => handler(args);
        return this;
    }

    /// <summary>
    ///     Attaches a handler to the <see cref="Tabs.ValueChanging" /> event.
    /// </summary>
    /// <param name="handler">
    ///     The event handler to invoke before the selected tab changes. Set <see cref="ValueChangingEventArgs{T}.Cancel" /> to prevent the change.
    /// </param>
    /// <returns>This <see cref="TabsBuilder{TParent}" /> instance for fluent chaining.</returns>
    public TabsBuilder<TParent> OnValueChanging(Action<ValueChangingEventArgs<View?>> handler)
    {
        _tabs.ValueChanging += (sender, args) => handler(args);
        return this;
    }
}
