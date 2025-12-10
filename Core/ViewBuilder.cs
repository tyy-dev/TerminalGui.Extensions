using System.Text;

using Terminal.Gui.ViewBase;
using Terminal.Gui.Views;

namespace TerminalGui.Extensions.Core;

public class ViewBuilder(View parent)
{
    private View? _lastChildAdded;

    /// <summary>
    ///     A property used for automatically positioning children when added via <see cref="Add{T}" />.
    ///     This function determines how to position a new child relative to the previously added child.
    ///     Takes the last added child as input and returns the Y position to use for the new child,
    ///     or <see langword="null" /> to skip auto-positioning.
    ///     <br></br>
    ///     By default this is  <see cref="Pos.Bottom" />, which ensures that each new child is placed directly below the last added child.
    /// </summary>
    public Func<View, Pos?>? NextPosY {
        get;
        set;
    } = Pos.Bottom;

    /// <summary>
    ///     Does the exact same as <see cref="NextPosY"/>, but for the X position. By default no auto-positioning is applied for X.
    /// </summary>
    public Func<View, Pos?>? NextPosX {
        get;
        set;
    } = null;

    /// <summary>
    ///     Gets the most recently added view for this view builder instance.
    /// </summary>
    /// <returns>
    ///     The last child <see cref="View" /> that was added to the parent of this view builder instance, or
    ///     <see langword="null" />.
    /// </returns>
    public View? GetLastChildAdded() => _lastChildAdded;

    /// <summary>
    ///     Adds a <see cref="View" /> of type <typeparamref name="T" /> to the parent view.
    /// </summary>
    /// <typeparam name="T">The type of <see cref="View" /> being added.</typeparam>
    /// <param name="child">The <see cref="View" /> being added.</param>
    /// <param name="configureBeforeAdd">An optional callback to apply configuration to the child before it is added to the parent viwe.</param>
    /// <returns>The newly added <see cref="View" /> instance of type <typeparamref name="T" />.</returns>
    public T Add<T>(T child, Action<T>? configureBeforeAdd = null) where T : View
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
        return child;
    }

    #region Bar

    /// <summary>
    ///     Adds a <see cref="Bar" /> to the parent view.
    /// </summary>
    /// <returns>The newly added <see cref="Bar" /> instance.</returns>
    public Bar AddBar(Bar bar) => Add(bar);

    /// <inheritdoc cref="AddBar(Bar)" path="/summary" />
    /// <param name="shortcuts"></param>
    /// <param name="alignmentMode">
    ///     <inheritdoc cref="Bar.AlignmentModes" path="/summary" />
    /// </param>
    /// <param name="orientation">
    ///     <inheritdoc cref="Bar.Orientation" path="/summary" />
    /// </param>
    /// <inheritdoc cref="AddBar(Bar)" path="/returns" />
    public Bar AddBar(IEnumerable<View>? shortcuts = null, AlignmentModes? alignmentMode = null, Orientation? orientation = null) => Add(
        new Bar(shortcuts),
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
    public Button AddButton(Button button) => Add(button);

    /// <inheritdoc cref="AddButton(Button)" path="/summary" />
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
    /// <inheritdoc cref="AddButton(Button)" path="/returns" />
    public Button AddButton(
        string? text = null,
        bool? isDefault = null,
        bool? noDecorations = null,
        bool? noPadding = null,
        Rune? hotKeySpecifier = null
    ) => Add(
        new Button(),
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

    public CharMap AddCharMap(CharMap charMap) => Add(charMap);
    public CharMap AddCharMap() => throw new NotImplementedException();

    #endregion

    #region CheckBox

    /// <summary>
    ///     Adds a <see cref="CheckBox" /> to the parent view.
    /// </summary>
    /// <returns>The newly added <see cref="CheckBox" /> instance.</returns>
    public CheckBox AddCheckBox(CheckBox checkBox) => Add(checkBox);

    /// <inheritdoc cref="AddCheckBox(CheckBox)" path="/summary" />
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
    /// <inheritdoc cref="AddCheckBox(CheckBox)" path="/returns" />
    public CheckBox AddCheckBox(string? text = null, CheckState? checkedState = null, bool? allowCheckedStateNone = null, Rune? hotKeySpecifier = null) =>
        AddCheckable(text, checkedState, allowCheckedStateNone, hotKeySpecifier: hotKeySpecifier);

    /// <summary>
    ///     Adds a <see cref="CheckBox" /> with radio style set to true to the parent view.
    /// </summary>
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
    /// <inheritdoc cref="AddCheckBox(CheckBox)" path="/returns" />
    public CheckBox AddRadioButton(string? text = null, CheckState? checkedState = null, bool? allowCheckedStateNone = null, Rune? hotKeySpecifier = null) =>
        AddCheckable(text, checkedState, allowCheckedStateNone, true, hotKeySpecifier);

    /// <inheritdoc cref="AddCheckBox(CheckBox)" path="/summary" />
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
    /// <inheritdoc cref="AddCheckBox(CheckBox)" path="/returns" />
    private CheckBox AddCheckable(
        string? text = null,
        CheckState? checkedState = null,
        bool? allowCheckedStateNone = null,
        bool radioStyle = false,
        Rune? hotKeySpecifier = null
    ) => Add(
        new CheckBox(),
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

    public ComboBox AddComboBox(ComboBox comboBox) => Add(comboBox);

    public ComboBox AddComboBox() => throw new NotImplementedException();

    #endregion

    #region DatePicker

    public DatePicker AddDatePicker(DatePicker datePicker) => Add(datePicker);

    public DatePicker AddDatePicker() => throw new NotImplementedException();

    #endregion

    #region Dialog

    public Dialog AddDialog(Dialog dialog) => Add(dialog);

    public Dialog AddDialog() => throw new NotImplementedException();

    #endregion

    #region File Dialogs

    public FileDialog AddFileDialog(FileDialog fileDialog) => Add(fileDialog);

    public FileDialog AddFileDialog() => throw new NotImplementedException();

    public OpenDialog AddOpenDialog(OpenDialog openDialog) => Add(openDialog);
    public OpenDialog AddOpenDialog() => throw new NotImplementedException();

    public SaveDialog AddSaveDialog(SaveDialog saveDialog) => Add(saveDialog);
    public SaveDialog AddSaveDialog() => throw new NotImplementedException();

    #endregion

    #region FrameView

    public FrameView AddFrameView(FrameView frameView) => Add(frameView);

    public FrameView AddFrameView() => throw new NotImplementedException();

    #endregion

    #region GraphView

    public GraphView AddGraphView(GraphView graphView) => Add(graphView);
    public GraphView AddGraphView() => throw new NotImplementedException();

    #endregion

    #region HexView

    public HexView AddHexView(HexView hexView) => Add(hexView);

    public HexView AddHexView() => throw new NotImplementedException();

    #endregion

    #region Label

    /// <summary>
    ///     Adds a <see cref="Label" /> to the parent view.
    /// </summary>
    /// <returns>The newly added <see cref="Label" /> instance.</returns>
    public Label AddLabel(Label label) => Add(label);

    /// <inheritdoc cref="AddLabel(Label)" path="/summary" />
    /// <param name="text">
    ///     <inheritdoc cref="Label.Text" path="/summary" />
    /// </param>
    /// <param name="hotKeySpecifier">
    ///     <inheritdoc cref="Label.HotKeySpecifier" path="/summary" />
    /// </param>
    /// <inheritdoc cref="AddButton(Button)" path="/returns" />
    public Label AddLabel(string? text = null, Rune? hotKeySpecifier = null) => Add(
        new Label(),
        lbl => {
            lbl.Text = text ?? $"Label {parent.SubViews.Count}";
            lbl.HotKeySpecifier = hotKeySpecifier ?? lbl.HotKeySpecifier;
        });

    #endregion

    #region Line

    public Line AddLine(Line line) => Add(line);
    public Line AddLine() => throw new NotImplementedException();

    #endregion

    #region ListView

    public ListView AddListView(ListView listView) => Add(listView);
    public ListView AddListView() => throw new NotImplementedException();

    #endregion

    #region NumericUpDown

    public NumericUpDown<T> AddNumericUpDown<T>(NumericUpDown<T> numericUpDown) where T : notnull => Add(numericUpDown);
    public NumericUpDown<T> AddNumericUpDown<T>() where T : notnull => throw new NotImplementedException();

    #endregion

    #region ProgressBar

    public ProgressBar AddProgressBar(ProgressBar progressBar) => Add(progressBar);
    public ProgressBar AddProgressBar() => throw new NotImplementedException();

    #endregion

    #region ScrollBar

    public ScrollBar AddScrollBar(ScrollBar scrollBar) => Add(scrollBar);
    public ScrollBar AddScrollBar() => throw new NotImplementedException();

    #endregion

    #region ScrollSlider

    public ScrollSlider AddScrollSlider(ScrollSlider scrollSlider) => Add(scrollSlider);
    public ScrollSlider AddScrollSlider() => throw new NotImplementedException();

    #endregion

    #region Shortcut

    public Shortcut AddShortcut(Shortcut shortcut) => Add(shortcut);
    public Shortcut AddShortcut() => throw new NotImplementedException();

    #endregion

    #region Slider

    public Slider AddSlider(Slider slider) => Add(slider);

    public Slider AddSlider() => throw new NotImplementedException();

    #endregion

    #region SpinnerView

    public SpinnerView AddSpinnerView(SpinnerView spinnerView) => Add(spinnerView);

    public SpinnerView AddSpinnerView() => throw new NotImplementedException();

    #endregion

    #region StatusBar

    public StatusBar AddStatusBar(StatusBar statusBar) => Add(statusBar);

    public StatusBar AddStatusBar() => throw new NotImplementedException();

    #endregion

    #region TreeView

    public TreeView AddTreeView(TreeView treeView) => Add(treeView);

    public TreeView AddTreeView() => throw new NotImplementedException();

    #endregion

    #region Window

    public Window AddWindow(Window window) => Add(window);

    public Window AddWindow() => throw new NotImplementedException();

    #endregion

    #region Wizard

    public Wizard AddWizard(Wizard wizard) => Add(wizard);

    public Wizard AddWizard() => throw new NotImplementedException();

    #endregion

    // todo https://github.com/gui-cs/Terminal.Gui/tree/v2_develop/Terminal.Gui/Views/TextInput

    // todo https://github.com/gui-cs/Terminal.Gui/tree/v2_develop/Terminal.Gui/Views/TableView

    // todo https://github.com/gui-cs/Terminal.Gui/tree/v2_develop/Terminal.Gui/Views/TabView

    // todo https://github.com/gui-cs/Terminal.Gui/tree/v2_develop/Terminal.Gui/Views/Selectors

    // todo https://github.com/gui-cs/Terminal.Gui/tree/v2_develop/Terminal.Gui/Views/Menu

    // todo https://github.com/gui-cs/Terminal.Gui/tree/v2_develop/Terminal.Gui/Views/Color

    // todo https://github.com/gui-cs/Terminal.Gui/tree/v2_develop/Terminal.Gui/Views/Autocomplete
}
