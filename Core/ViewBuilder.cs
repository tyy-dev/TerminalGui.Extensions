using System.Text;

using Terminal.Gui.ViewBase;
using Terminal.Gui.Views;

namespace TerminalGui.Extensions.Core;

public class ViewBuilder<TParent>(TParent parent) where TParent : View
{
    private View? _lastChildAdded;

    /// <summary>
    ///     A property used for automatically positioning children when added via <see cref="Add{T}" />.
    ///     This function determines how to position a new child relative to the previously added child.
    ///     Takes the last added child as input and returns the Y position to use for the new child,
    ///     or <see langword="null" /> to skip auto-positioning.
    ///     <br></br>
    ///     By default this is  <see cref="Pos.Bottom" />, which ensures that each new child is placed directly below the last
    ///     added child.
    /// </summary>
    public Func<View, Pos?>? NextPosY {
        get;
        set;
    } = Pos.Bottom;

    /// <summary>
    ///     Does the exact same as <see cref="NextPosY" />, but for the X position. By default no auto-positioning is applied
    ///     for X.
    /// </summary>
    public Func<View, Pos?>? NextPosX {
        get;
        set;
    } = null;

    /// <summary>
    ///     Returns the parent <see cref="View" /> associated with the current instance.
    /// </summary>
    /// <returns>The parent <see cref="View" /> of type <typeparamref name="TParent" /></returns>
    public TParent GetView() => parent;

    /// <summary>
    ///     Gets the most recently added view for this view builder instance.
    /// </summary>
    /// <returns>
    ///     The last child <see cref="View" /> that was added to the parent of this view builder instance, or
    ///     <see langword="null" />.
    /// </returns>
    public View? GetLastChildAdded() => _lastChildAdded;

    /// <summary>
    ///     Adds a <see cref="View" /> of type <typeparamref name="TChild" /> to the parent view,
    ///     retrieving it from the given <see cref="ViewBuilder{TChild}" />.
    /// </summary>
    /// <param name="viewBuilder">The <see cref="ViewBuilder{TChild}" /> to retrieve the view from</param>
    /// <param name="child">The newly added child view</param>
    /// <param name="configureBeforeAdd">Optional configuration callback before adding</param>
    /// <returns>The fluent <see cref="ViewBuilder{TParent}" /></returns>
    public ViewBuilder<TParent> Add<TChild>(ViewBuilder<TChild> viewBuilder, out TChild child, Action<TChild>? configureBeforeAdd = null)
        where TChild : View
    {
        child = viewBuilder.GetView();
        return Add(out _, child, configureBeforeAdd); // call the main Add below
    }

    /// <summary>
    ///     Adds a <see cref="View" /> of type <typeparamref name="TChild" /> to the parent view.
    /// </summary>
    /// <typeparam name="TChild">The type of <see cref="View" /> being added.</typeparam>
    /// <param name="addedChild">The child returned via out</param>
    /// <param name="child">The newly added child view</param>
    /// <param name="configureBeforeAdd">Optional configuration callback before adding</param>
    /// <returns>
    ///     <see cref="ViewBuilder{TParent}" />
    /// </returns>
    public ViewBuilder<TParent> Add<TChild>(out TChild addedChild, TChild child, Action<TChild>? configureBeforeAdd = null)
        where TChild : View
    {
        if (_lastChildAdded is not null && parent.SubViews.Contains(_lastChildAdded))
        {
            if (NextPosY?.Invoke(_lastChildAdded) is { } yPos)
            {
                child.Y = yPos;
            }

            if (NextPosX?.Invoke(_lastChildAdded) is { } xPos)
            {
                child.X = xPos;
            }
        }

        configureBeforeAdd?.Invoke(child);

        _lastChildAdded = child;
        parent.Add(child);

        addedChild = child;
        return this;
    }

    #region Bar

    /// <summary>
    ///     Adds a <see cref="Bar" /> to the parent view.
    /// </summary>
    /// <returns>The newly added <see cref="Bar" /> instance.</returns>
    public ViewBuilder<TParent> AddBar(Bar bar) => Add(out _, bar);

    /// <inheritdoc cref="AddBar(Bar)" path="/summary" />
    /// <param name="bar">
    ///     <inheritdoc cref="AddBar(Bar)" path="/returns" />
    /// </param>
    /// <param name="shortcuts"></param>
    /// <param name="alignmentMode">
    ///     <inheritdoc cref="Bar.AlignmentModes" path="/summary" />
    /// </param>
    /// <param name="orientation">
    ///     <inheritdoc cref="Bar.Orientation" path="/summary" />
    /// </param>
    /// <returns>The<see cref="ViewBuilder{TParent}" /> with <inheritdoc cref="AddBar(Bar)" path="/returns" /></returns>
    public ViewBuilder<TParent> AddBar(out Bar bar, IEnumerable<View>? shortcuts = null, AlignmentModes? alignmentMode = null, Orientation? orientation = null) => Add(
        out bar,
        new(),
        btn => {
            btn.AlignmentModes = alignmentMode ?? btn.AlignmentModes;
            btn.Orientation = orientation ?? btn.Orientation;
        }
    );

    #endregion

    #region Button

    /// <summary>
    ///     Adds a <see cref="Button" /> to the parent view.
    /// </summary>
    /// <returns>The newly added <see cref="Button" /> instance.</returns>
    public ViewBuilder<TParent> AddButton(Button button) => Add(out _, button);

    /// <inheritdoc cref="AddButton(Button)" path="/summary" />
    /// <param name="button">
    ///     <inheritdoc cref="AddButton(Button)" path="/returns" />
    /// </param>
    /// <param name="text">The text displayed by the Button. Defaults to "Button {n}" where n is the subview count.</param>
    /// <param name="isDefault">
    ///     <inheritdoc cref="Button.IsDefault" path="/summary" />
    /// </param>
    /// <param name="noDecorations">
    ///     <inheritdoc cref="Button.NoDecorations" path="/summary" />
    /// </param>
    /// <param name="noPadding">
    ///     <inheritdoc cref="Button.NoPadding" path="/summary" />
    /// </param>
    /// <param name="hotKeySpecifier">
    ///     <inheritdoc cref="Button.HotKeySpecifier" path="/summary" />
    /// </param>
    /// <returns>The<see cref="ViewBuilder{TParent}" /> with <inheritdoc cref="AddButton(Button)" path="/returns" /></returns>
    public ViewBuilder<TParent> AddButton(
        out Button button,
        string? text = null,
        bool? isDefault = null,
        bool? noDecorations = null,
        bool? noPadding = null,
        Rune? hotKeySpecifier = null
    ) => Add(
        out button,
        new(),
        btn => {
            btn.Text = text ?? $"Button {parent.SubViews.Count}";
            btn.IsDefault = isDefault ?? btn.IsDefault;
            btn.NoDecorations = noDecorations ?? btn.NoDecorations;
            btn.NoPadding = noPadding ?? btn.NoPadding;
            btn.HotKeySpecifier = hotKeySpecifier ?? btn.HotKeySpecifier;
        }
    );

    #endregion

    #region CharMap

    public ViewBuilder<TParent> AddCharMap(CharMap charMap) => Add(out _, charMap);

    public ViewBuilder<TParent> AddCharMap() => throw new NotImplementedException();

    #endregion

    #region CheckBox

    /// <summary>
    ///     Adds a <see cref="CheckBox" /> to the parent view.
    /// </summary>
    /// <returns>The newly added <see cref="CheckBox" /> instance.</returns>
    public ViewBuilder<TParent> AddCheckBox(CheckBox checkBox) => Add(out _, checkBox);

    /// <inheritdoc cref="AddCheckBox(CheckBox)" path="/summary" />
    /// <param name="checkBox">
    ///     <inheritdoc cref="AddCheckBox(CheckBox)" path="/returns" />
    /// </param>
    /// <param name="text">
    ///     <inheritdoc cref="CheckBox.Text" path="/summary" />
    /// </param>
    /// <param name="checkedState">
    ///     <inheritdoc cref="CheckBox.CheckedState" path="/summary" />
    /// </param>
    /// <param name="allowCheckedStateNone">
    ///     <inheritdoc cref="CheckBox.AllowCheckStateNone" path="/summary" />
    /// </param>
    /// <param name="hotKeySpecifier">
    ///     <inheritdoc cref="CheckBox.HotKeySpecifier" path="/summary" />
    /// </param>
    /// <returns>
    ///     The<see cref="ViewBuilder{TParent}" /> with <inheritdoc cref="AddCheckBox(CheckBox)" path="/returns" />
    /// </returns>
    public ViewBuilder<TParent> AddCheckBox(
        out CheckBox checkBox,
        string? text = null,
        CheckState? checkedState = null,
        bool? allowCheckedStateNone = null,
        Rune? hotKeySpecifier = null
    ) =>
        AddCheckable(out checkBox, text, checkedState, allowCheckedStateNone, hotKeySpecifier: hotKeySpecifier);

    /// <summary>
    ///     Adds a <see cref="CheckBox" /> with radio style set to true to the parent view.
    /// </summary>
    /// <param name="checkBox">
    ///     <inheritdoc cref="AddCheckBox(CheckBox)" path="/returns" />
    /// </param>
    /// <param name="text">
    ///     <inheritdoc cref="CheckBox.Text" path="/summary" />
    /// </param>
    /// <param name="checkedState">
    ///     <inheritdoc cref="CheckBox.CheckedState" path="/summary" />
    /// </param>
    /// <param name="allowCheckedStateNone">
    ///     <inheritdoc cref="CheckBox.AllowCheckStateNone" path="/summary" />
    /// </param>
    /// <param name="hotKeySpecifier">
    ///     <inheritdoc cref="CheckBox.HotKeySpecifier" path="/summary" />
    /// </param>
    /// <returns>
    ///     The<see cref="ViewBuilder{TParent}" /> with <inheritdoc cref="AddCheckBox(CheckBox)" path="/returns" />
    /// </returns>
    public ViewBuilder<TParent> AddRadioButton(
        out CheckBox checkBox,
        string? text = null,
        CheckState? checkedState = null,
        bool? allowCheckedStateNone = null,
        Rune? hotKeySpecifier = null
    ) =>
        AddCheckable(out checkBox, text, checkedState, allowCheckedStateNone, true, hotKeySpecifier);

    /// <inheritdoc cref="AddCheckBox(CheckBox)" path="/summary" />
    /// <param name="checkBox">
    ///     <inheritdoc cref="AddCheckBox(CheckBox)" path="/returns" />
    /// </param>
    /// <param name="text">
    ///     <inheritdoc cref="CheckBox.Text" path="/summary" />
    /// </param>
    /// <param name="checkedState">
    ///     <inheritdoc cref="CheckBox.CheckedState" path="/summary" />
    /// </param>
    /// <param name="allowCheckedStateNone">
    ///     <inheritdoc cref="CheckBox.AllowCheckStateNone" path="/summary" />
    /// </param>
    /// <param name="radioStyle">
    ///     <inheritdoc cref="CheckBox.RadioStyle" path="/summary" />
    /// </param>
    /// <param name="hotKeySpecifier">
    ///     <inheritdoc cref="CheckBox.HotKeySpecifier" path="/summary" />
    /// </param>
    /// <returns>
    ///     The<see cref="ViewBuilder{TParent}" /> with <inheritdoc cref="AddCheckBox(CheckBox)" path="/returns" />
    /// </returns>
    private ViewBuilder<TParent> AddCheckable(
        out CheckBox checkBox,
        string? text = null,
        CheckState? checkedState = null,
        bool? allowCheckedStateNone = null,
        bool radioStyle = false,
        Rune? hotKeySpecifier = null
    ) => Add(
        out checkBox,
        new(),
        chBox => {
            string defaultName = radioStyle ? "RadioButton" : "CheckBox";

            chBox.Text = text ?? $"{defaultName} {parent.SubViews.Count}";
            chBox.CheckedState = checkedState ?? chBox.CheckedState;
            chBox.AllowCheckStateNone = allowCheckedStateNone ?? chBox.AllowCheckStateNone;
            chBox.RadioStyle = radioStyle;
            chBox.HotKeySpecifier = hotKeySpecifier ?? chBox.HotKeySpecifier;
        }
    );

    #endregion

    #region ComboBox

    public ViewBuilder<TParent> AddComboBox(ComboBox comboBox) => Add(out _, comboBox);

    public ViewBuilder<TParent> AddComboBox() => throw new NotImplementedException();

    #endregion

    #region DatePicker

    public ViewBuilder<TParent> AddDatePicker(DatePicker datePicker) => Add(out _, datePicker);

    public ViewBuilder<TParent> AddDatePicker() => throw new NotImplementedException();

    #endregion

    #region Dialog

    public ViewBuilder<TParent> AddDialog(Dialog dialog) => Add(out _, dialog);

    public ViewBuilder<TParent> AddDialog() => throw new NotImplementedException();

    #endregion

    #region File Dialogs

    public ViewBuilder<TParent> AddFileDialog(FileDialog fileDialog) => Add(out _, fileDialog);

    public ViewBuilder<TParent> AddFileDialog() => throw new NotImplementedException();

    public ViewBuilder<TParent> AddOpenDialog(OpenDialog openDialog) => Add(out _, openDialog);

    public ViewBuilder<TParent> AddOpenDialog() => throw new NotImplementedException();

    public ViewBuilder<TParent> AddSaveDialog(SaveDialog saveDialog) => Add(out _, saveDialog);

    public ViewBuilder<TParent> AddSaveDialog() => throw new NotImplementedException();

    #endregion

    #region FrameView

    public ViewBuilder<TParent> AddFrameView(FrameView frameView) => Add(out _, frameView);

    public ViewBuilder<TParent> AddFrameView() => throw new NotImplementedException();

    #endregion

    #region GraphView

    public ViewBuilder<TParent> AddGraphView(GraphView graphView) => Add(out _, graphView);

    public ViewBuilder<TParent> AddGraphView() => throw new NotImplementedException();

    #endregion

    #region HexView

    public ViewBuilder<TParent> AddHexView(HexView hexView) => Add(out _, hexView);

    public ViewBuilder<TParent> AddHexView() => throw new NotImplementedException();

    #endregion

    #region Label

    /// <summary>
    ///     Adds a <see cref="Label" /> to the parent view.
    /// </summary>
    /// <returns>The newly added <see cref="Label" /> instance.</returns>
    public ViewBuilder<TParent> AddLabel(Label label) => Add(out _, label);

    /// <inheritdoc cref="AddLabel(Label)" path="/summary" />
    /// <param name="label">
    ///     <inheritdoc cref="AddLabel(Label)" path="/returns" />
    /// </param>
    /// <param name="text">
    ///     <inheritdoc cref="Label.Text" path="/summary" />
    /// </param>
    /// <param name="hotKeySpecifier">
    ///     <inheritdoc cref="Label.HotKeySpecifier" path="/summary" />
    /// </param>
    /// <returns>The<see cref="ViewBuilder{TParent}" /> with <inheritdoc cref="AddLabel(Label)" path="/returns" /></returns>
    public ViewBuilder<TParent> AddLabel(out Label label, string? text = null, Rune? hotKeySpecifier = null) => Add(
        out label,
        new(),
        lbl => {
            lbl.Text = text ?? $"Label {parent.SubViews.Count}";
            lbl.HotKeySpecifier = hotKeySpecifier ?? lbl.HotKeySpecifier;
        });

    #endregion

    #region Line

    public ViewBuilder<TParent> AddLine(Line line) => Add(out _, line);

    public ViewBuilder<TParent> AddLine() => throw new NotImplementedException();

    #endregion

    #region ListView

    public ViewBuilder<TParent> AddListView(ListView listView) => Add(out _, listView);

    public ViewBuilder<TParent> AddListView() => throw new NotImplementedException();

    #endregion

    #region Menu

    public ViewBuilder<TParent> AddMenu(Menu menu) => Add(out _, menu);

    public ViewBuilder<TParent> AddMenu() => throw new NotImplementedException();

    public ViewBuilder<TParent> AddMenuItem(MenuItem menuItem) => Add(out _, menuItem);

    public ViewBuilder<TParent> AddMenuItem() => throw new NotImplementedException();

    public ViewBuilder<TParent> AddPopoverMenu(PopoverMenu popoverMenu) => Add(out _, popoverMenu);

    public ViewBuilder<TParent> AddPopoverMenu() => throw new NotImplementedException();

    #endregion

    #region Menu Bar

    public ViewBuilder<TParent> AddMenuBar(MenuBar menuBar) => Add(out _, menuBar);

    public ViewBuilder<TParent> AddMenuBar() => throw new NotImplementedException();

    public ViewBuilder<TParent> AddMenuBarItem(MenuBarItem menuBarItem) => Add(out _, menuBarItem);

    public ViewBuilder<TParent> AddMenuBarItem() => throw new NotImplementedException();

    #endregion

    #region NumericUpDown

    public ViewBuilder<TParent> AddNumericUpDown<TNumericType>(NumericUpDown<TNumericType> numericUpDown) where TNumericType : notnull => Add(out _, numericUpDown);
    public ViewBuilder<TParent> AddNumericUpDown<TNumericType>() where TNumericType : notnull => throw new NotImplementedException();

    #endregion

    #region ProgressBar

    public ViewBuilder<TParent> AddProgressBar(ProgressBar progressBar) => Add(out _, progressBar);

    public ViewBuilder<TParent> AddProgressBar() => throw new NotImplementedException();

    #endregion

    #region ScrollBar

    public ViewBuilder<TParent> AddScrollBar(ScrollBar scrollBar) => Add(out _, scrollBar);

    public ViewBuilder<TParent> AddScrollBar() => throw new NotImplementedException();

    #endregion

    #region ScrollSlider

    public ViewBuilder<TParent> AddScrollSlider(ScrollSlider scrollSlider) => Add(out _, scrollSlider);

    public ViewBuilder<TParent> AddScrollSlider() => throw new NotImplementedException();

    #endregion

    #region Shortcut

    public ViewBuilder<TParent> AddShortcut(Shortcut shortcut) => Add(out _, shortcut);

    public ViewBuilder<TParent> AddShortcut() => throw new NotImplementedException();

    #endregion

    #region Slider

    public ViewBuilder<TParent> AddSlider(Slider slider) => Add(out _, slider);

    public ViewBuilder<TParent> AddSlider() => throw new NotImplementedException();

    #endregion

    #region SpinnerView

    public ViewBuilder<TParent> AddSpinnerView(SpinnerView spinnerView) => Add(out _, spinnerView);

    public ViewBuilder<TParent> AddSpinnerView() => throw new NotImplementedException();

    #endregion

    #region StatusBar

    public ViewBuilder<TParent> AddStatusBar(StatusBar statusBar) => Add(out _, statusBar);

    public ViewBuilder<TParent> AddStatusBar() => throw new NotImplementedException();

    #endregion

    #region TreeView

    public ViewBuilder<TParent> AddTreeView(TreeView treeView) => Add(out _, treeView);

    public ViewBuilder<TParent> AddTreeView() => throw new NotImplementedException();

    #endregion

    #region Window

    public ViewBuilder<TParent> AddWindow(Window window) => Add(out _, window);

    public ViewBuilder<TParent> AddWindow() => throw new NotImplementedException();

    #endregion

    #region Wizard

    public ViewBuilder<TParent> AddWizard(Wizard wizard) => Add(out _, wizard);

    public ViewBuilder<TParent> AddWizard() => throw new NotImplementedException();

    #endregion

    // todo https://github.com/gui-cs/Terminal.Gui/tree/v2_develop/Terminal.Gui/Views/TextInput

    // todo https://github.com/gui-cs/Terminal.Gui/tree/v2_develop/Terminal.Gui/Views/TableView

    // todo https://github.com/gui-cs/Terminal.Gui/tree/v2_develop/Terminal.Gui/Views/TabView

    // todo https://github.com/gui-cs/Terminal.Gui/tree/v2_develop/Terminal.Gui/Views/Selectors

    // todo https://github.com/gui-cs/Terminal.Gui/tree/v2_develop/Terminal.Gui/Views/Color

    // todo https://github.com/gui-cs/Terminal.Gui/tree/v2_develop/Terminal.Gui/Views/Autocomplete
}
