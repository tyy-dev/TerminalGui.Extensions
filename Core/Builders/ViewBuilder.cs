using System.Drawing;
using System.Globalization;
using System.Text;

using Terminal.Gui.Drawing;
using Terminal.Gui.Drivers;
using Terminal.Gui.Input;
using Terminal.Gui.ViewBase;
using Terminal.Gui.Views;

using TerminalGui.Extensions.Core.Views;
using TerminalGui.Extensions.Extensions.ViewExtensions;

using Attribute = Terminal.Gui.Drawing.Attribute;
using Color = Terminal.Gui.Drawing.Color;

namespace TerminalGui.Extensions.Core.Builders;

public class ViewBuilder<TParent>(TParent parent) where TParent : View
{
    private View? _lastChildAdded;

    /// <summary>
    ///     A property used for automatically positioning children when added via
    ///     <see cref="Add{TChild}(out TChild, TChild, System.Action{TChild}?)" />.
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
    ///     If <see langword="true" />, skips any auto-positioning logic for any future added childam, unless it is set to
    ///     <see langword="fale" /> again.
    ///     It is suggested to set this property when using view.WithLayout if you do not want auto-positioning to interfere
    ///     with the layouting.
    /// </summary>
    public bool SkipAutoPositioning {
        get;
        set;
    } = false;

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
        if (!SkipAutoPositioning && _lastChildAdded is { } && parent.SubViews.Contains(_lastChildAdded))
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
        child = (TChild)parent.Add(child);

        addedChild = child;
        return this;
    }

    /// <summary>
    ///     Adds a <see cref="View" /> of type <typeparamref name="TChild" /> to the parent view,
    ///     retrieving it from the given <see cref="ViewBuilder{TChild}" />.
    /// </summary>
    /// <param name="viewBuilder">The <see cref="ViewBuilder{TChild}" /> to retrieve the view from</param>
    /// <param name="child">The newly added child view</param>
    /// <param name="configureBeforeAdd">Optional configuration callback before adding</param>
    /// <returns>
    ///     <see cref="ViewBuilder{TParent}" />
    /// </returns>
    public ViewBuilder<TParent> Add<TChild>(ViewBuilder<TChild> viewBuilder, out TChild child, Action<TChild>? configureBeforeAdd = null)
        where TChild : View
    {
        TChild viewBuilderView = viewBuilder.GetView();
        return Add(out child, viewBuilderView, configureBeforeAdd); // call the main Add below
    }

    /// <summary>
    ///     Adds a <see cref="View" /> of type <typeparamref name="TChild" /> to the parent view and returns its specialized
    ///     builder.
    /// </summary>
    /// <typeparam name="TChild">The type of <see cref="View" /> being added.</typeparam>
    /// <typeparam name="TBuilder">The type of the specialized Builder returned to the out parameter.</typeparam>
    /// <param name="addedChild">The child returned via out</param>
    /// <param name="addedChildBuilder">The child returned via out</param>
    /// <param name="builderFactory">
    ///     Called with the newly added child, and is expected to return it's specialized builder
    ///     instance
    /// </param>
    /// <param name="child">The newly added child view</param>
    /// <param name="configureBeforeAdd">Optional configuration callback before adding</param>
    /// <returns>
    ///     <see cref="ViewBuilder{TParent}" />
    /// </returns>
    internal ViewBuilder<TParent> Add<TChild, TBuilder>(
        out TChild addedChild,
        out TBuilder addedChildBuilder,
        Func<TChild, TBuilder> builderFactory,
        TChild child,
        Action<TChild>? configureBeforeAdd = null
    )
        where TChild : View
        where TBuilder : ViewBuilder<TChild>
    {
        ViewBuilder<TParent> viewBuilder = Add(out addedChild, child, configureBeforeAdd);
        addedChildBuilder = builderFactory(addedChild);

        return viewBuilder;
    }

    /// <summary>
    ///     Creates a builder instance via the provided factory and invokes the configuration callback.
    ///     Used internally by extension methods to reduce boilerplate in <c>ConfigureWithBuilder</c> implementations for View
    ///     Extensions.
    /// </summary>
    internal static TBuilder Configure<TBuilder>(Func<TBuilder> builderFactory, Action<TBuilder> callback)
    {
        TBuilder builder = builderFactory();
        callback(builder);
        return builder;
    }

    #region Bar

    /// <summary>
    ///     Adds a <see cref="Bar" /> to the parent view.
    /// </summary>
    /// <returns>The <see cref="ViewBuilder{TParent}" /> with The newly added <see cref="Bar" /> instance</returns>
    public ViewBuilder<TParent> AddBar(Bar bar) => Add(out _, bar);

    /// <inheritdoc cref="AddBar(Bar)" path="/summary" />
    /// <param name="barOut">
    ///     The newly added <see cref="Bar" /> instance
    /// </param>
    /// <param name="alignmentModes">
    ///     <inheritdoc cref="Bar.AlignmentModes" path="/summary" />
    /// </param>
    /// <param name="orientation">
    ///     <inheritdoc cref="Bar.Orientation" path="/summary" />
    /// </param>
    /// <inheritdoc cref="AddBar(Bar)" path="/returns" />
    public ViewBuilder<TParent> AddBar(
        out Bar barOut,
        AlignmentModes? alignmentModes = null,
        Orientation? orientation = null
    ) => Add(
        out barOut,
        new(),
        bar => {
            bar.AlignmentModes = alignmentModes ?? bar.AlignmentModes;
            bar.Orientation = orientation ?? bar.Orientation;
        }
    );

    #endregion

    #region Button

    /// <summary>
    ///     Adds a <see cref="Button" /> to the parent view.
    /// </summary>
    /// <returns>The <see cref="ViewBuilder{TParent}" /> with The newly added <see cref="Button" /> instance</returns>
    public ViewBuilder<TParent> AddButton(Button button) => Add(out _, button);

    /// <inheritdoc cref="AddButton(Button)" path="/summary" />
    /// <param name="buttonOut">
    ///     The newly added <see cref="Button" /> instance
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
    /// <inheritdoc cref="AddButton(Button)" path="/returns" />
    public ViewBuilder<TParent> AddButton(
        out Button buttonOut,
        string? text = null,
        bool? isDefault = null,
        bool? noDecorations = null,
        bool? noPadding = null,
        Rune? hotKeySpecifier = null
    ) => Add(
        out buttonOut,
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

    /// <inheritdoc cref="AddCharMap(CharMap)" path="/summary" />
    /// <param name="charMapOut">
    ///     The newly added <see cref="CharMap" /> instance
    /// </param>
    /// <param name="selectedCodePoint">
    ///     <inheritdoc cref="CharMap.SelectedCodePoint" path="/summary" />
    /// </param>
    /// <param name="showGlyphWidths">
    ///     <inheritdoc cref="CharMap.ShowGlyphWidths" path="/summary" />
    /// </param>
    /// <param name="startCodePoint">
    ///     <inheritdoc cref="CharMap.StartCodePoint" path="/summary" />
    /// </param>
    /// <param name="value">
    ///     <inheritdoc cref="CharMap.Value" path="/summary" />
    /// </param>
    /// <param name="showUnicodeCategory">
    ///     <inheritdoc cref="CharMap.ShowUnicodeCategory" path="/summary" />
    /// </param>
    /// <inheritdoc cref="AddCharMap(CharMap)" path="/returns" />
    public ViewBuilder<TParent> AddCharMap(
        out CharMap charMapOut,
        int? selectedCodePoint = null,
        bool? showGlyphWidths = null,
        int? startCodePoint = null,
        Rune? value = null,
        UnicodeCategory? showUnicodeCategory = null
    ) => Add(
        out charMapOut,
        new(),
        cm => {
            cm.SelectedCodePoint = selectedCodePoint ?? cm.SelectedCodePoint;
            cm.ShowGlyphWidths = showGlyphWidths ?? cm.ShowGlyphWidths;
            cm.StartCodePoint = startCodePoint ?? cm.StartCodePoint;
            cm.Value = value ?? cm.Value;
            cm.ShowUnicodeCategory = showUnicodeCategory ?? cm.ShowUnicodeCategory;
        }
    );

    #endregion

    #region CheckBox

    /// <summary>
    ///     Adds a <see cref="CheckBox" /> to the parent view.
    /// </summary>
    /// <returns>The <see cref="ViewBuilder{TParent}" /> with The newly added <see cref="CheckBox" /> instance</returns>
    public ViewBuilder<TParent> AddCheckBox(CheckBox checkBox) => Add(out _, checkBox);

    /// <inheritdoc cref="AddCheckBox(CheckBox)" path="/summary" />
    /// <param name="checkBox">
    ///     <inheritdoc cref="AddCheckBox(CheckBox)" path="/returns" />
    /// </param>
    /// <param name="text">
    ///     <inheritdoc cref="CheckBox.Text" path="/summary" />
    /// </param>
    /// <param name="checkedState">
    ///     <inheritdoc cref="CheckBox.Value" path="/summary" />
    /// </param>
    /// <param name="allowCheckedStateNone">
    ///     <inheritdoc cref="CheckBox.AllowCheckStateNone" path="/summary" />
    /// </param>
    /// <param name="hotKeySpecifier">
    ///     <inheritdoc cref="CheckBox.HotKeySpecifier" path="/summary" />
    /// </param>
    /// <inheritdoc cref="AddCheckBox(CheckBox)" path="/returns" />
    public ViewBuilder<TParent> AddCheckBox(
        out CheckBox checkBox,
        string? text = null,
        CheckState? checkedState = null,
        bool? allowCheckedStateNone = null,
        Rune? hotKeySpecifier = null
    ) =>
        AddCheckable(out checkBox, text, checkedState, allowCheckedStateNone, hotKeySpecifier: hotKeySpecifier);

    /// <summary>
    ///     Adds a <see cref="CheckBox" /> with radio style set to <see langword="true" /> to the parent view.
    /// </summary>
    /// <param name="checkBox">
    ///     <inheritdoc cref="AddCheckBox(CheckBox)" path="/returns" />
    /// </param>
    /// <param name="text">
    ///     <inheritdoc cref="CheckBox.Text" path="/summary" />
    /// </param>
    /// <param name="checkedState">
    ///     <inheritdoc cref="CheckBox.Value" path="/summary" />
    /// </param>
    /// <param name="allowCheckedStateNone">
    ///     <inheritdoc cref="CheckBox.AllowCheckStateNone" path="/summary" />
    /// </param>
    /// <param name="hotKeySpecifier">
    ///     <inheritdoc cref="CheckBox.HotKeySpecifier" path="/summary" />
    /// </param>
    /// <inheritdoc cref="AddCheckBox(CheckBox)" path="/returns" />
    public ViewBuilder<TParent> AddRadioButton(
        out CheckBox checkBox,
        string? text = null,
        CheckState? checkedState = null,
        bool? allowCheckedStateNone = null,
        Rune? hotKeySpecifier = null
    ) =>
        AddCheckable(out checkBox, text, checkedState, allowCheckedStateNone, true, hotKeySpecifier);

    /// <inheritdoc cref="AddCheckBox(CheckBox)" path="/summary" />
    /// <param name="checkBoxOut">
    ///     <inheritdoc cref="AddCheckBox(CheckBox)" path="/returns" />
    /// </param>
    /// <param name="text">
    ///     <inheritdoc cref="CheckBox.Text" path="/summary" />
    /// </param>
    /// <param name="checkedState">
    ///     <inheritdoc cref="CheckBox.Value" path="/summary" />
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
    private ViewBuilder<TParent> AddCheckable(
        out CheckBox checkBoxOut,
        string? text = null,
        CheckState? checkedState = null,
        bool? allowCheckedStateNone = null,
        bool radioStyle = false,
        Rune? hotKeySpecifier = null
    ) => Add(
        out checkBoxOut,
        new(),
        chBox => {
            string defaultName = radioStyle ? "RadioButton" : "CheckBox";

            chBox.Text = text ?? $"{defaultName} {parent.SubViews.Count}";
            chBox.Value = checkedState ?? chBox.Value;
            chBox.AllowCheckStateNone = allowCheckedStateNone ?? chBox.AllowCheckStateNone;
            chBox.RadioStyle = radioStyle;
            chBox.HotKeySpecifier = hotKeySpecifier ?? chBox.HotKeySpecifier;
        }
    );

    #endregion

    #region DropDownList

    /// <summary>
    ///     Adds a <see cref="DropDownList" /> to the parent view.
    /// </summary>
    /// <returns>The <see cref="ViewBuilder{TParent}" /> with The newly added <see cref="DropDownList" /> instance</returns>
    public ViewBuilder<TParent> AddDropDownList(DropDownList dropDownList) => Add(out _, dropDownList);

    /// <inheritdoc cref="AddDropDownList(DropDownList)" path="/summary" />
    /// <param name="dropDownListOut">
    ///     The newly added <see cref="DropDownList" /> instance
    /// </param>
    /// <param name="source">
    ///     <inheritdoc cref="DropDownList.Source" path="/summary" />
    /// </param>
    /// <param name="text">
    ///     <inheritdoc cref="TextField.Text" path="/summary" />
    /// </param>
    /// <param name="readOnly">
    ///     <inheritdoc cref="TextField.ReadOnly" path="/summary" />
    /// </param>
    /// <param name="secret">
    ///     <inheritdoc cref="TextField.Secret" path="/summary" />
    /// </param>
    /// <inheritdoc cref="AddDropDownList(DropDownList)" path="/returns" />
    public ViewBuilder<TParent> AddDropDownList(
        out DropDownList dropDownListOut,
        IListDataSource? source = null,
        string? text = null,
        bool? readOnly = null,
        bool? secret = null
    ) => Add(
        out dropDownListOut,
        new(),
        ddl => {
            ddl.Source = source ?? ddl.Source;
            ddl.Text = text ?? ddl.Text;
            ddl.ReadOnly = readOnly ?? ddl.ReadOnly;
            ddl.Secret = secret ?? ddl.Secret;
        }
    );

    #endregion

    #region DatePicker

    public ViewBuilder<TParent> AddDatePicker(DatePicker datePicker) => Add(out _, datePicker);

    /// <inheritdoc cref="AddDatePicker(DatePicker)" path="/summary" />
    /// <param name="datePickerOut">
    ///     The newly added <see cref="DatePicker" /> instance
    /// </param>
    /// <param name="date">
    ///     <inheritdoc cref="DatePicker.Value" path="/summary" />
    /// </param>
    /// <param name="culture">
    ///     <inheritdoc cref="DatePicker.Culture" path="/summary" />
    /// </param>
    /// <param name="text">
    ///     <inheritdoc cref="DatePicker.Text" path="/summary" />
    /// </param>
    /// <inheritdoc cref="AddDatePicker(DatePicker)" path="/returns" />
    public ViewBuilder<TParent> AddDatePicker(
        out DatePicker datePickerOut,
        DateTime? date = null,
        CultureInfo? culture = null,
        string? text = null
    ) => Add(
        out datePickerOut,
        new(),
        dp => {
            dp.Value = date ?? dp.Value;
            dp.Culture = culture ?? dp.Culture;
            dp.Text = text ?? dp.Text;
        }
    );

    #endregion

    #region Dialog

    public ViewBuilder<TParent> AddDialog(Dialog dialog) => Add(out _, dialog);

    /// <inheritdoc cref="AddDialog(Dialog)" path="/summary" />
    /// <param name="dialogOut">
    ///     The newly added <see cref="Dialog" /> instance
    /// </param>
    /// <param name="title">The title displayed by the Dialog.</param>
    /// <param name="buttons">
    ///     <inheritdoc cref="Dialog.Buttons" path="/summary" />
    /// </param>
    /// <param name="buttonAlignment">
    ///     <inheritdoc cref="Dialog.ButtonAlignment" path="/summary" />
    /// </param>
    /// <param name="buttonAlignmentModes">
    ///     <inheritdoc cref="Dialog.ButtonAlignmentModes" path="/summary" />
    /// </param>
    /// <param name="result">
    ///     <inheritdoc cref="Dialog.Result" path="/summary" />
    /// </param>
    /// <inheritdoc cref="AddDialog(Dialog)" path="/returns" />
    public ViewBuilder<TParent> AddDialog(
        out Dialog dialogOut,
        string? title = null,
        Button[]? buttons = null,
        Alignment? buttonAlignment = null,
        AlignmentModes? buttonAlignmentModes = null,
        int? result = null
    ) => Add(
        out dialogOut,
        new(),
        dlg => {
            dlg.Title = title ?? dlg.Title;
            dlg.Buttons = buttons ?? dlg.Buttons;
            dlg.ButtonAlignment = buttonAlignment ?? dlg.ButtonAlignment;
            dlg.ButtonAlignmentModes = buttonAlignmentModes ?? dlg.ButtonAlignmentModes;
            dlg.Result = result ?? dlg.Result;
        }
    );

    #endregion

    #region File Dialogs

    public ViewBuilder<TParent> AddFileDialog(FileDialog fileDialog) => Add(out _, fileDialog);

    /// <inheritdoc cref="AddFileDialog(FileDialog)" path="/summary" />
    /// <param name="fileDialogOut">
    ///     The newly added <see cref="FileDialog" /> instance
    /// </param>
    /// <param name="title">The title displayed by the FileDialog.</param>
    /// <param name="path">
    ///     <inheritdoc cref="FileDialog.Path" path="/summary" />
    /// </param>
    /// <param name="allowedTypes">
    ///     <inheritdoc cref="FileDialog.AllowedTypes" path="/summary" />
    /// </param>
    /// <param name="allowsMultipleSelection">
    ///     <inheritdoc cref="FileDialog.AllowsMultipleSelection" path="/summary" />
    /// </param>
    /// <param name="mustExist">
    ///     <inheritdoc cref="FileDialog.MustExist" path="/summary" />
    /// </param>
    /// <param name="openMode">
    ///     <inheritdoc cref="FileDialog.OpenMode" path="/summary" />
    /// </param>
    /// <inheritdoc cref="AddFileDialog(FileDialog)" path="/returns" />
    public ViewBuilder<TParent> AddFileDialog(
        out FileDialog fileDialogOut,
        string? title = null,
        string? path = null,
        List<IAllowedType>? allowedTypes = null,
        bool? allowsMultipleSelection = null,
        bool? mustExist = null,
        OpenMode? openMode = null
    ) => Add(
        out fileDialogOut,
        new(),
        fd => {
            fd.Title = title ?? fd.Title;
            fd.Path = path ?? fd.Path;
            fd.AllowedTypes = allowedTypes ?? fd.AllowedTypes;
            fd.AllowsMultipleSelection = allowsMultipleSelection ?? fd.AllowsMultipleSelection;
            fd.MustExist = mustExist ?? fd.MustExist;
            fd.OpenMode = openMode ?? fd.OpenMode;
        }
    );

    public ViewBuilder<TParent> AddOpenDialog(OpenDialog openDialog) => Add(out _, openDialog);

    /// <inheritdoc cref="AddOpenDialog(OpenDialog)" path="/summary" />
    /// <param name="openDialogOut">
    ///     The newly added <see cref="OpenDialog" /> instance
    /// </param>
    /// <param name="title">The title displayed by the OpenDialog.</param>
    /// <param name="path">
    ///     <inheritdoc cref="FileDialog.Path" path="/summary" />
    /// </param>
    /// <param name="allowedTypes">
    ///     <inheritdoc cref="FileDialog.AllowedTypes" path="/summary" />
    /// </param>
    /// <param name="allowsMultipleSelection">
    ///     <inheritdoc cref="FileDialog.AllowsMultipleSelection" path="/summary" />
    /// </param>
    /// <param name="mustExist">
    ///     <inheritdoc cref="FileDialog.MustExist" path="/summary" />
    /// </param>
    /// <param name="openMode">
    ///     <inheritdoc cref="OpenDialog.OpenMode" path="/summary" />
    /// </param>
    /// <inheritdoc cref="AddOpenDialog(OpenDialog)" path="/returns" />
    public ViewBuilder<TParent> AddOpenDialog(
        out OpenDialog openDialogOut,
        string? title = null,
        string? path = null,
        List<IAllowedType>? allowedTypes = null,
        bool? allowsMultipleSelection = null,
        bool? mustExist = null,
        OpenMode? openMode = null
    ) => Add(
        out openDialogOut,
        new(),
        od => {
            od.Title = title ?? od.Title;
            od.Path = path ?? od.Path;
            od.AllowedTypes = allowedTypes ?? od.AllowedTypes;
            od.AllowsMultipleSelection = allowsMultipleSelection ?? od.AllowsMultipleSelection;
            od.MustExist = mustExist ?? od.MustExist;
            od.OpenMode = openMode ?? od.OpenMode;
        }
    );

    public ViewBuilder<TParent> AddSaveDialog(SaveDialog saveDialog) => Add(out _, saveDialog);

    /// <inheritdoc cref="AddSaveDialog(SaveDialog)" path="/summary" />
    /// <param name="saveDialogOut">
    ///     The newly added <see cref="SaveDialog" /> instance
    /// </param>
    /// <param name="title">The title displayed by the SaveDialog.</param>
    /// <param name="path">
    ///     <inheritdoc cref="FileDialog.Path" path="/summary" />
    /// </param>
    /// <param name="allowedTypes">
    ///     <inheritdoc cref="FileDialog.AllowedTypes" path="/summary" />
    /// </param>
    /// <param name="mustExist">
    ///     <inheritdoc cref="FileDialog.MustExist" path="/summary" />
    /// </param>
    /// <inheritdoc cref="AddSaveDialog(SaveDialog)" path="/returns" />
    public ViewBuilder<TParent> AddSaveDialog(
        out SaveDialog saveDialogOut,
        string? title = null,
        string? path = null,
        List<IAllowedType>? allowedTypes = null,
        bool? mustExist = null
    ) => Add(
        out saveDialogOut,
        new(),
        sd => {
            sd.Title = title ?? sd.Title;
            sd.Path = path ?? sd.Path;
            sd.AllowedTypes = allowedTypes ?? sd.AllowedTypes;
            sd.MustExist = mustExist ?? sd.MustExist;
        }
    );

    #endregion

    #region FrameView

    public ViewBuilder<TParent> AddFrameView(FrameView frameView) => Add(out _, frameView);

    /// <inheritdoc cref="AddFrameView(FrameView)" path="/summary" />
    /// <param name="frameViewOut">
    ///     The newly added <see cref="FrameView" /> instance
    /// </param>
    /// <param name="title">The title displayed by the FrameView.</param>
    /// <inheritdoc cref="AddFrameView(FrameView)" path="/returns" />
    public ViewBuilder<TParent> AddFrameView(
        out FrameView frameViewOut,
        string? title = null
    ) => Add(
        out frameViewOut,
        new(),
        fv => fv.Title = title ?? fv.Title);

    #endregion

    #region GraphView

    public ViewBuilder<TParent> AddGraphView(GraphView graphView) => Add(out _, graphView);

    /// <inheritdoc cref="AddGraphView(GraphView)" path="/summary" />
    /// <param name="graphViewOut">
    ///     The newly added <see cref="GraphView" /> instance
    /// </param>
    /// <param name="cellSize">
    ///     <inheritdoc cref="GraphView.CellSize" path="/summary" />
    /// </param>
    /// <param name="scrollOffset">
    ///     <inheritdoc cref="GraphView.ScrollOffset" path="/summary" />
    /// </param>
    /// <param name="graphColor">
    ///     <inheritdoc cref="GraphView.GraphColor" path="/summary" />
    /// </param>
    /// <param name="marginLeft">
    ///     <inheritdoc cref="GraphView.MarginLeft" path="/summary" />
    /// </param>
    /// <param name="marginBottom">
    ///     <inheritdoc cref="GraphView.MarginBottom" path="/summary" />
    /// </param>
    /// <param name="axisX">
    ///     <inheritdoc cref="GraphView.AxisX" path="/summary" />
    /// </param>
    /// <param name="axisY">
    ///     <inheritdoc cref="GraphView.AxisY" path="/summary" />
    /// </param>
    /// <inheritdoc cref="AddGraphView(GraphView)" path="/returns" />
    public ViewBuilder<TParent> AddGraphView(
        out GraphView graphViewOut,
        PointF? cellSize = null,
        PointF? scrollOffset = null,
        Attribute? graphColor = null,
        uint? marginLeft = null,
        uint? marginBottom = null,
        HorizontalAxis? axisX = null,
        VerticalAxis? axisY = null
    ) => Add(
        out graphViewOut,
        new(),
        gv => {
            gv.CellSize = cellSize ?? gv.CellSize;
            gv.ScrollOffset = scrollOffset ?? gv.ScrollOffset;

            if (graphColor is { })
            {
                gv.GraphColor = graphColor;
            }

            gv.MarginLeft = marginLeft ?? gv.MarginLeft;
            gv.MarginBottom = marginBottom ?? gv.MarginBottom;
            gv.AxisX = axisX ?? gv.AxisX;
            gv.AxisY = axisY ?? gv.AxisY;
        }
    );

    #endregion

    #region HexView

    public ViewBuilder<TParent> AddHexView(HexView hexView) => Add(out _, hexView);

    /// <inheritdoc cref="AddHexView(HexView)" path="/summary" />
    /// <param name="hexViewOut">
    ///     The newly added <see cref="HexView" /> instance
    /// </param>
    /// <param name="source">
    ///     <inheritdoc cref="HexView.Source" path="/summary" />
    /// </param>
    /// <param name="readOnly">
    ///     <inheritdoc cref="HexView.ReadOnly" path="/summary" />
    /// </param>
    /// <param name="bytesPerLine">
    ///     <inheritdoc cref="HexView.BytesPerLine" path="/summary" />
    /// </param>
    /// <param name="addressWidth">
    ///     <inheritdoc cref="HexView.AddressWidth" path="/summary" />
    /// </param>
    /// <param name="address">
    ///     <inheritdoc cref="HexView.Address" path="/summary" />
    /// </param>
    /// <inheritdoc cref="AddHexView(HexView)" path="/returns" />
    public ViewBuilder<TParent> AddHexView(
        out HexView hexViewOut,
        Stream? source = null,
        bool? readOnly = null,
        int? bytesPerLine = null,
        int? addressWidth = null,
        long? address = null
    ) => Add(
        out hexViewOut,
        new(),
        hv => {
            hv.Source = source ?? hv.Source;
            hv.ReadOnly = readOnly ?? hv.ReadOnly;
            hv.BytesPerLine = bytesPerLine ?? hv.BytesPerLine;
            hv.AddressWidth = addressWidth ?? hv.AddressWidth;
            hv.Address = address ?? hv.Address;
        }
    );

    #endregion

    #region Label

    /// <summary>
    ///     Adds a <see cref="Label" /> to the parent view.
    /// </summary>
    /// <returns>The <see cref="ViewBuilder{TParent}" /> with The newly added <see cref="Label" /> instance</returns>
    public ViewBuilder<TParent> AddLabel(Label label) => Add(out _, label);

    /// <inheritdoc cref="AddLabel(Label)" path="/summary" />
    /// <param name="labelOut">
    ///     The newly added <see cref="Label" /> instance
    /// </param>
    /// <param name="text">
    ///     <inheritdoc cref="Label.Text" path="/summary" />
    /// </param>
    /// <param name="hotKeySpecifier">
    ///     <inheritdoc cref="Label.HotKeySpecifier" path="/summary" />
    /// </param>
    /// <inheritdoc cref="AddLabel(Label)" path="/returns" />
    public ViewBuilder<TParent> AddLabel(out Label labelOut, string? text = null, Rune? hotKeySpecifier = null) => Add(
        out labelOut,
        new(),
        lbl => {
            lbl.Text = text ?? $"Label {parent.SubViews.Count}";
            lbl.HotKeySpecifier = hotKeySpecifier ?? lbl.HotKeySpecifier;
        });

    #endregion

    #region Line

    /// <summary>
    ///     Adds a <see cref="Line" /> to the parent view.
    /// </summary>
    /// <returns>The <see cref="ViewBuilder{TParent}" /> with The newly added <see cref="Line" /> instance</returns>
    public ViewBuilder<TParent> AddLine(Line line) => Add(out _, line);

    /// <inheritdoc cref="AddLine(Line)" path="/summary" />
    /// <param name="lineOut">
    ///     The newly added <see cref="Line" /> instance
    /// </param>
    /// <param name="length">
    ///     <inheritdoc cref="Line.Length" path="/summary" />
    /// </param>
    /// <param name="orientation">
    ///     <inheritdoc cref="Line.Orientation" path="/summary" />
    /// </param>
    /// <param name="lineStyle">
    ///     <inheritdoc cref="Line.Style" path="/summary" />
    /// </param>
    /// <inheritdoc cref="AddLine(Line)" path="/returns" />
    public ViewBuilder<TParent> AddLine(out Line lineOut, Dim? length = null, Orientation? orientation = null, LineStyle? lineStyle = null) => Add(
        out lineOut,
        new(),
        line => {
            line.Length = length ?? line.Length;
            line.Orientation = orientation ?? line.Orientation;
            line.Style = lineStyle ?? line.Style;
        });

    #endregion

    #region ListView

    public ViewBuilder<TParent> AddListView(ListView listView) => Add(out _, listView);

    /// <inheritdoc cref="AddListView(ListView)" path="/summary" />
    /// <param name="listViewOut">
    ///     The newly added <see cref="ListView" /> instance
    /// </param>
    /// <param name="source">
    ///     <inheritdoc cref="ListView.Source" path="/summary" />
    /// </param>
    /// <param name="selectedItem">
    ///     <inheritdoc cref="ListView.SelectedItem" path="/summary" />
    /// </param>
    /// <param name="value">
    ///     <inheritdoc cref="ListView.Value" path="/summary" />
    /// </param>
    /// <param name="showMarks">
    ///     <inheritdoc cref="ListView.ShowMarks" path="/summary" />
    /// </param>
    /// <param name="markMultiple">
    ///     <inheritdoc cref="ListView.MarkMultiple" path="/summary" />
    /// </param>
    /// <inheritdoc cref="AddListView(ListView)" path="/returns" />
    public ViewBuilder<TParent> AddListView(
        out ListView listViewOut,
        IListDataSource? source = null,
        int? selectedItem = null,
        int? value = null,
        bool? showMarks = null,
        bool? markMultiple = null
    ) => Add(
        out listViewOut,
        new(),
        lv => {
            lv.Source = source ?? lv.Source;

            if (selectedItem is { })
            {
                lv.SelectedItem = selectedItem;
            }

            if (value is { })
            {
                lv.Value = value;
            }

            lv.ShowMarks = showMarks ?? lv.ShowMarks;
            lv.MarkMultiple = markMultiple ?? lv.MarkMultiple;
        }
    );

    #endregion

    #region Menu

    public ViewBuilder<TParent> AddMenu(Menu menu) => Add(out _, menu);

    /// <inheritdoc cref="AddMenu(Menu)" path="/summary" />
    /// <param name="menuOut">
    ///     The newly added <see cref="Menu" /> instance
    /// </param>
    /// <param name="menuItems">The menu items to add to the menu.</param>
    /// <param name="orientation">
    ///     <inheritdoc cref="Bar.Orientation" path="/summary" />
    /// </param>
    /// <param name="alignmentModes">
    ///     <inheritdoc cref="Bar.AlignmentModes" path="/summary" />
    /// </param>
    /// <param name="superMenuItem">
    ///     <inheritdoc cref="Menu.SuperMenuItem" path="/summary" />
    /// </param>
    /// <param name="value">
    ///     <inheritdoc cref="Menu.Value" path="/summary" />
    /// </param>
    /// <inheritdoc cref="AddMenu(Menu)" path="/returns" />
    public ViewBuilder<TParent> AddMenu(
        out Menu menuOut,
        IEnumerable<MenuItem>? menuItems = null,
        Orientation? orientation = null,
        AlignmentModes? alignmentModes = null,
        MenuItem? superMenuItem = null,
        MenuItem? value = null
    ) => Add(
        out menuOut,
        menuItems is { } ? new(menuItems) : new(),
        m => {
            m.Orientation = orientation ?? m.Orientation;
            m.AlignmentModes = alignmentModes ?? m.AlignmentModes;
            m.SuperMenuItem = superMenuItem ?? m.SuperMenuItem;
            m.Value = value ?? m.Value;
        }
    );

    public ViewBuilder<TParent> AddMenuItem(MenuItem menuItem) => Add(out _, menuItem);

    /// <inheritdoc cref="AddMenuItem(MenuItem)" path="/summary" />
    /// <param name="menuItemOut">
    ///     The newly added <see cref="MenuItem" /> instance
    /// </param>
    /// <param name="commandText">The text to display for the command.</param>
    /// <param name="helpText">The help text for the menu item.</param>
    /// <param name="action">The action to invoke when the menu item is activated.</param>
    /// <param name="key">
    ///     The key binding for the menu item.
    /// </param>
    /// <param name="targetView">
    ///     <inheritdoc cref="Shortcut.TargetView" path="/summary" />
    /// </param>
    /// <param name="command">
    ///     <inheritdoc cref="Shortcut.Command" path="/summary" />
    /// </param>
    /// <param name="subMenu">
    ///     <inheritdoc cref="MenuItem.SubMenu" path="/summary" />
    /// </param>
    /// <param name="bindKeyToApplication">
    ///     <inheritdoc cref="Shortcut.BindKeyToApplication" path="/summary" />
    /// </param>
    /// <param name="orientation">
    ///     <inheritdoc cref="Shortcut.Orientation" path="/summary" />
    /// </param>
    /// <param name="commandView">
    ///     <inheritdoc cref="Shortcut.CommandView" path="/summary" />
    /// </param>
    /// <param name="alignmentModes">
    ///     <inheritdoc cref="Shortcut.AlignmentModes" path="/summary" />
    /// </param>
    /// <param name="minimumKeyTextSize">
    ///     <inheritdoc cref="Shortcut.MinimumKeyTextSize" path="/summary" />
    /// </param>
    /// <inheritdoc cref="AddMenuItem(MenuItem)" path="/returns" />
    public ViewBuilder<TParent> AddMenuItem(
        out MenuItem menuItemOut,
        string? commandText = null,
        string? helpText = null,
        Action? action = null,
        Key? key = null,
        View? targetView = null,
        Command? command = null,
        Menu? subMenu = null,
        bool? bindKeyToApplication = null,
        Orientation? orientation = null,
        View? commandView = null,
        AlignmentModes? alignmentModes = null,
        int? minimumKeyTextSize = null
    ) => Add(
        out menuItemOut,
        new(),
        mi => {
            mi.Title = commandText ?? $"Menu Item {GetView().SubViews.Count}";
            mi.HelpText = helpText ?? mi.HelpText;
            mi.Action = action ?? mi.Action;
            mi.Key = key ?? mi.Key;
            mi.TargetView = targetView ?? mi.TargetView;
            mi.Command = command ?? mi.Command;
            mi.SubMenu = subMenu ?? mi.SubMenu;
            mi.BindKeyToApplication = bindKeyToApplication ?? mi.BindKeyToApplication;
            mi.Orientation = orientation ?? mi.Orientation;
            mi.CommandView = commandView ?? mi.CommandView;
            mi.AlignmentModes = alignmentModes ?? mi.AlignmentModes;
            mi.MinimumKeyTextSize = minimumKeyTextSize ?? mi.MinimumKeyTextSize;
        }
    );

    //public ViewBuilder<TParent> AddMenuBar(MenuBar menuBar, out MenuBarBuilder menuBarBuilder)
    //    => Add<MenuBar, MenuBarBuilder>(out _, out menuBarBuilder, m => m.Builder(), menuBar);
    //public ViewBuilder<TParent> AddMenuBar(out MenuBar menuBarOut, out MenuBarBuilder menuBarBuilder, string? text = null)
    //    => Add(out menuBarOut, out menuBarBuilder, m => m.Builder(), new(),
    //        bar => {
    //            bar.Text = text ?? bar.Text;
    //        });

    /// <summary>
    ///     Adds a <see cref="MenuBar" /> to the parent view.
    /// </summary>
    /// <returns>The <see cref="ViewBuilder{TParent}" /> with The newly added <see cref="MenuBar" /> instance</returns>
    public ViewBuilder<TParent> AddMenuBar(MenuBar menuBar) => Add(out _, menuBar);

    /// <inheritdoc cref="AddMenuBar(MenuBar)" path="/summary" />
    /// <param name="menuBarOut">
    ///     The newly added <see cref="MenuBar" /> instance
    /// </param>
    /// <param name="menus">
    ///     <inheritdoc cref="MenuBar.Menus" path="/summary" />
    /// </param>
    /// <param name="key">
    ///     <inheritdoc cref="MenuBar.Key" path="/summary" />
    /// </param>
    /// <inheritdoc cref="AddMenuBar(MenuBar)" path="/returns" />
    public ViewBuilder<TParent> AddMenuBar(out MenuBar menuBarOut, MenuBarItem[]? menus = null, Key? key = null) => Add(
        out menuBarOut,
        new(),
        menuBar => {
            menuBar.Menus = menus ?? [];
            menuBar.Key = key ?? menuBar.Key;
        });

    /// <summary>
    ///     Adds a <see cref="MenuBarItem" /> to the parent view.
    /// </summary>
    /// <returns>The <see cref="ViewBuilder{TParent}" /> with The newly added <see cref="MenuBarItem" /> instance</returns>
    public ViewBuilder<TParent> AddMenuBarItem(MenuBarItem menuBarItem) => Add(out _, menuBarItem);

    /// <inheritdoc cref="AddMenuBarItem(MenuBarItem)" path="/summary" />
    /// <param name="menuBarItemOut">
    ///     The newly added <see cref="MenuBarItem" /> instance
    /// </param>
    /// <param name="commandText">The text to display for the command.</param>
    /// <param name="targetView">
    ///     <inheritdoc cref="Shortcut.TargetView" path="/summary" />
    /// </param>
    /// <param name="command">
    ///     <inheritdoc cref="Shortcut.Command" path="/summary" />
    /// </param>
    /// <param name="popoverMenu">
    ///     <inheritdoc cref="MenuBarItem.PopoverMenu" path="/summary" />
    /// </param>
    /// <inheritdoc cref="AddMenuBarItem(MenuBarItem)" path="/returns" />
    public ViewBuilder<TParent> AddMenuBarItem(
        out MenuBarItem menuBarItemOut,
        string? commandText = null,
        View? targetView = null,
        Command? command = null,
        PopoverMenu? popoverMenu = null
    )
        => Add(
            out menuBarItemOut,
            new(),
            menuBarItem => {
                menuBarItem.Title = commandText ?? $"Menu Bar Item {GetView().SubViews.Count}";
                menuBarItem.TargetView = targetView ?? menuBarItem.TargetView;
                menuBarItem.Command = command ?? menuBarItem.Command;
                menuBarItem.PopoverMenu = popoverMenu ?? menuBarItem.PopoverMenu;
            });

    public ViewBuilder<TParent> AddPopoverMenu(PopoverMenu popoverMenu) => Add(out _, popoverMenu);

    /// <inheritdoc cref="AddPopoverMenu(PopoverMenu)" path="/summary" />
    /// <param name="popoverMenuOut">
    ///     The newly added <see cref="PopoverMenu" /> instance
    /// </param>
    /// <param name="root">
    ///     <inheritdoc cref="PopoverMenu.Root" path="/summary" />
    /// </param>
    /// <param name="key">
    ///     <inheritdoc cref="PopoverMenu.Key" path="/summary" />
    /// </param>
    /// <param name="mouseFlags">
    ///     <inheritdoc cref="PopoverMenu.MouseFlags" path="/summary" />
    /// </param>
    /// <inheritdoc cref="AddPopoverMenu(PopoverMenu)" path="/returns" />
    public ViewBuilder<TParent> AddPopoverMenu(
        out PopoverMenu popoverMenuOut,
        Menu? root = null,
        Key? key = null,
        MouseFlags? mouseFlags = null
    ) => Add(
        out popoverMenuOut,
        root is { } ? new(root) : new(),
        pm => {
            pm.Key = key ?? pm.Key;
            pm.MouseFlags = mouseFlags ?? pm.MouseFlags;
        }
    );

    #endregion

    #region NumericUpDown

    public ViewBuilder<TParent> AddNumericUpDown<TNumericType>(NumericUpDown<TNumericType> numericUpDown) where TNumericType : notnull => Add(out _, numericUpDown);

    /// <inheritdoc cref="AddNumericUpDown{TNumericType}(NumericUpDown{TNumericType})" path="/summary" />
    /// <param name="numericUpDownOut">
    ///     The newly added <see cref="NumericUpDownConstrained{TNumericType}" /> instance
    /// </param>
    /// <param name="value">
    ///     <inheritdoc cref="NumericUpDown{TNumericType}.Value" path="/summary" />
    /// </param>
    /// <param name="format">
    ///     <inheritdoc cref="NumericUpDown{TNumericType}.Format" path="/summary" />
    /// </param>
    /// <param name="step">
    ///     <inheritdoc cref="NumericUpDown{TNumericType}.Increment" path="/summary" />
    /// </param>
    /// <param name="max">
    ///     <inheritdoc cref="NumericUpDownConstrained{TNumericType}.Max" path="/summary" />
    /// </param>
    /// <param name="min">
    ///     <inheritdoc cref="NumericUpDownConstrained{TNumericType}.Min" path="/summary" />
    /// </param>
    /// <inheritdoc cref="AddNumericUpDown{TNumericType}(NumericUpDown{TNumericType})" path="/returns" />
    public ViewBuilder<TParent> AddNumericUpDown<TNumericType>(
        out NumericUpDownConstrained<TNumericType> numericUpDownOut,
        TNumericType? value = default,
        string? format = null,
        TNumericType? step = default,
        TNumericType? max = default,
        TNumericType? min = default
    ) where TNumericType : notnull
        => Add(
            out numericUpDownOut,
            new(),
            nud => {
                if (value is { })
                {
                    nud.Value = value;
                }

                if (format is { })
                {
                    nud.Format = format;
                }

                if (step is { })
                {
                    nud.Increment = step;
                }

                nud.Max = max;
                nud.Min = min;
            }
        );

    #endregion

    #region ProgressBar

    /// <summary>
    ///     Adds a <see cref="ProgressBar" /> to the parent view.
    /// </summary>
    /// <returns>The <see cref="ViewBuilder{TParent}" /> with The newly added <see cref="ProgressBar" /> instance</returns>
    public ViewBuilder<TParent> AddProgressBar(ProgressBar progressBar) => Add(out _, progressBar);

    /// <inheritdoc cref="AddProgressBar(ProgressBar)" path="/summary" />
    /// <param name="progressBarOut">
    ///     The newly added <see cref="ProgressBar" /> instance
    /// </param>
    /// <param name="text">
    ///     <inheritdoc cref="ProgressBar.Text" path="/summary" />
    /// </param>
    /// <param name="fraction">
    ///     <inheritdoc cref="ProgressBar.Fraction" path="/summary" />
    /// </param>
    /// <param name="format">
    ///     <inheritdoc cref="ProgressBar.ProgressBarFormat" path="/summary" />
    /// </param>
    /// <param name="style">
    ///     <inheritdoc cref="ProgressBar.ProgressBarStyle" path="/summary" />
    /// </param>
    /// <param name="segmentCharacter">
    ///     <inheritdoc cref="ProgressBar.SegmentCharacter" path="/summary" />
    /// </param>
    /// <param name="bidirectionalMarquee">
    ///     <inheritdoc cref="ProgressBar.BidirectionalMarquee" path="/summary" />
    /// </param>
    /// <inheritdoc cref="AddProgressBar(ProgressBar)" path="/returns" />
    public ViewBuilder<TParent> AddProgressBar(
        out ProgressBar progressBarOut,
        string? text = null,
        float? fraction = null,
        ProgressBarFormat? format = null,
        ProgressBarStyle? style = null,
        Rune? segmentCharacter = null,
        bool? bidirectionalMarquee = null
    ) => Add(
        out progressBarOut,
        new(),
        progressBar => {
            progressBar.Text = text ?? $"Progress Bar {GetView().SubViews.Count}";
            progressBar.Fraction = fraction ?? progressBar.Fraction;
            progressBar.ProgressBarFormat = format ?? progressBar.ProgressBarFormat;
            progressBar.ProgressBarStyle = style ?? progressBar.ProgressBarStyle;
            progressBar.SegmentCharacter = segmentCharacter ?? progressBar.SegmentCharacter;
            progressBar.BidirectionalMarquee = bidirectionalMarquee ?? progressBar.BidirectionalMarquee;
        });

    #endregion

    #region ScrollBar

    public ViewBuilder<TParent> AddScrollBar(ScrollBar scrollBar) => Add(out _, scrollBar);

    /// <inheritdoc cref="AddScrollBar(ScrollBar)" path="/summary" />
    /// <param name="scrollBarOut">
    ///     The newly added <see cref="ScrollBar" /> instance
    /// </param>
    /// <param name="orientation">
    ///     <inheritdoc cref="ScrollBar.Orientation" path="/summary" />
    /// </param>
    /// <param name="increment">
    ///     <inheritdoc cref="ScrollBar.Increment" path="/summary" />
    /// </param>
    /// <param name="visibleContentSize">
    ///     <inheritdoc cref="ScrollBar.VisibleContentSize" path="/summary" />
    /// </param>
    /// <param name="scrollableContentSize">
    ///     <inheritdoc cref="ScrollBar.ScrollableContentSize" path="/summary" />
    /// </param>
    /// <param name="value">
    ///     <inheritdoc cref="ScrollBar.Value" path="/summary" />
    /// </param>
    /// <param name="visibilityMode">
    ///     <inheritdoc cref="ScrollBar.VisibilityMode" path="/summary" />
    /// </param>
    /// <inheritdoc cref="AddScrollBar(ScrollBar)" path="/returns" />
    public ViewBuilder<TParent> AddScrollBar(
        out ScrollBar scrollBarOut,
        Orientation? orientation = null,
        int? increment = null,
        int? visibleContentSize = null,
        int? scrollableContentSize = null,
        int? value = null,
        ScrollBarVisibilityMode? visibilityMode = null
    ) => Add(
        out scrollBarOut,
        new(),
        sb => {
            sb.Orientation = orientation ?? sb.Orientation;
            sb.Increment = increment ?? sb.Increment;
            sb.VisibleContentSize = visibleContentSize ?? sb.VisibleContentSize;
            sb.ScrollableContentSize = scrollableContentSize ?? sb.ScrollableContentSize;
            sb.Value = value ?? sb.Value;
            sb.VisibilityMode = visibilityMode ?? sb.VisibilityMode;
        }
    );

    #endregion

    #region ScrollSlider

    public ViewBuilder<TParent> AddScrollSlider(ScrollSlider scrollSlider) => Add(out _, scrollSlider);

    /// <inheritdoc cref="AddScrollSlider(ScrollSlider)" path="/summary" />
    /// <param name="scrollSliderOut">
    ///     The newly added <see cref="ScrollSlider" /> instance
    /// </param>
    /// <param name="orientation">
    ///     <inheritdoc cref="ScrollSlider.Orientation" path="/summary" />
    /// </param>
    /// <param name="size">
    ///     <inheritdoc cref="ScrollSlider.Size" path="/summary" />
    /// </param>
    /// <param name="position">
    ///     <inheritdoc cref="ScrollSlider.Position" path="/summary" />
    /// </param>
    /// <param name="visibleContentSize">
    ///     <inheritdoc cref="ScrollSlider.VisibleContentSize" path="/summary" />
    /// </param>
    /// <param name="sliderPadding">
    ///     <inheritdoc cref="ScrollSlider.SliderPadding" path="/summary" />
    /// </param>
    /// <inheritdoc cref="AddScrollSlider(ScrollSlider)" path="/returns" />
    public ViewBuilder<TParent> AddScrollSlider(
        out ScrollSlider scrollSliderOut,
        Orientation? orientation = null,
        int? size = null,
        int? position = null,
        int? visibleContentSize = null,
        int? sliderPadding = null
    ) => Add(
        out scrollSliderOut,
        new(),
        ss => {
            ss.Orientation = orientation ?? ss.Orientation;
            ss.Size = size ?? ss.Size;
            ss.Position = position ?? ss.Position;
            ss.VisibleContentSize = visibleContentSize ?? ss.VisibleContentSize;
            ss.SliderPadding = sliderPadding ?? ss.SliderPadding;
        }
    );

    #endregion

    #region Shortcut

    public ViewBuilder<TParent> AddShortcut(Shortcut shortcut) => Add(out _, shortcut);

    /// <inheritdoc cref="AddShortcut(Shortcut)" path="/summary" />
    /// <param name="shortcutOut">
    ///     The newly added <see cref="Shortcut" /> instance
    /// </param>
    /// <param name="text">
    ///     <inheritdoc cref="Shortcut.Text" path="/summary" />
    /// </param>
    /// <param name="key">
    ///     <inheritdoc cref="Shortcut.Key" path="/summary" />
    /// </param>
    /// <param name="action">
    ///     <inheritdoc cref="Shortcut.Action" path="/summary" />
    /// </param>
    /// <param name="helpText">
    ///     <inheritdoc cref="Shortcut.HelpText" path="/summary" />
    /// </param>
    /// <param name="bindKeyToApplication">
    ///     <inheritdoc cref="Shortcut.BindKeyToApplication" path="/summary" />
    /// </param>
    /// <param name="orientation">
    ///     <inheritdoc cref="Shortcut.Orientation" path="/summary" />
    /// </param>
    /// <param name="commandView">
    ///     <inheritdoc cref="Shortcut.CommandView" path="/summary" />
    /// </param>
    /// <param name="alignmentModes">
    ///     <inheritdoc cref="Shortcut.AlignmentModes" path="/summary" />
    /// </param>
    /// <param name="command">
    ///     <inheritdoc cref="Shortcut.Command" path="/summary" />
    /// </param>
    /// <param name="targetView">
    ///     <inheritdoc cref="Shortcut.TargetView" path="/summary" />
    /// </param>
    /// <param name="minimumKeyTextSize">
    ///     <inheritdoc cref="Shortcut.MinimumKeyTextSize" path="/summary" />
    /// </param>
    /// <inheritdoc cref="AddShortcut(Shortcut)" path="/returns" />
    public ViewBuilder<TParent> AddShortcut(
        out Shortcut shortcutOut,
        string? text = null,
        Key? key = null,
        Action? action = null,
        string? helpText = null,
        bool? bindKeyToApplication = null,
        Orientation? orientation = null,
        View? commandView = null,
        AlignmentModes? alignmentModes = null,
        Command? command = null,
        View? targetView = null,
        int? minimumKeyTextSize = null
    ) => Add(
        out shortcutOut,
        new(),
        sc => {
            sc.Text = text ?? $"Shortcut {GetView().SubViews.Count}";
            sc.Key = key ?? sc.Key;
            sc.Action = action ?? sc.Action;
            sc.HelpText = helpText ?? sc.HelpText;
            sc.BindKeyToApplication = bindKeyToApplication ?? sc.BindKeyToApplication;
            sc.Orientation = orientation ?? sc.Orientation;
            sc.CommandView = commandView ?? sc.CommandView;
            sc.AlignmentModes = alignmentModes ?? sc.AlignmentModes;
            sc.Command = command ?? sc.Command;
            sc.TargetView = targetView ?? sc.TargetView;
            sc.MinimumKeyTextSize = minimumKeyTextSize ?? sc.MinimumKeyTextSize;
        }
    );

    /// <inheritdoc cref="AddShortcut(Shortcut)" path="/summary" />
    /// <param name="text">
    ///     <inheritdoc cref="Shortcut.Text" path="/summary" />
    /// </param>
    /// <param name="key">
    ///     <inheritdoc cref="Shortcut.Key" path="/summary" />
    /// </param>
    /// <param name="action">
    ///     <inheritdoc cref="Shortcut.Action" path="/summary" />
    /// </param>
    /// <param name="helpText">
    ///     <inheritdoc cref="Shortcut.HelpText" path="/summary" />
    /// </param>
    /// <param name="bindKeyToApplication">
    ///     <inheritdoc cref="Shortcut.BindKeyToApplication" path="/summary" />
    /// </param>
    /// <param name="orientation">
    ///     <inheritdoc cref="Shortcut.Orientation" path="/summary" />
    /// </param>
    /// <param name="commandView">
    ///     <inheritdoc cref="Shortcut.CommandView" path="/summary" />
    /// </param>
    /// <param name="alignmentModes">
    ///     <inheritdoc cref="Shortcut.AlignmentModes" path="/summary" />
    /// </param>
    /// <param name="command">
    ///     <inheritdoc cref="Shortcut.Command" path="/summary" />
    /// </param>
    /// <param name="targetView">
    ///     <inheritdoc cref="Shortcut.TargetView" path="/summary" />
    /// </param>
    /// <param name="minimumKeyTextSize">
    ///     <inheritdoc cref="Shortcut.MinimumKeyTextSize" path="/summary" />
    /// </param>
    /// <inheritdoc cref="AddShortcut(Shortcut)" path="/returns" />
    public ViewBuilder<TParent> AddShortcut(
        string? text = null,
        Key? key = null,
        Action? action = null,
        string? helpText = null,
        bool? bindKeyToApplication = null,
        Orientation? orientation = null,
        View? commandView = null,
        AlignmentModes? alignmentModes = null,
        Command? command = null,
        View? targetView = null,
        int? minimumKeyTextSize = null
    ) => AddShortcut(
        out _,
        text,
        key,
        action,
        helpText,
        bindKeyToApplication,
        orientation,
        commandView,
        alignmentModes,
        command,
        targetView,
        minimumKeyTextSize
    );

    #endregion

    #region Linear Range

    public ViewBuilder<TParent> AddLinearRange(LinearRange linearRange) => Add(out _, linearRange);
    public ViewBuilder<TParent> AddLinearRange<T>(LinearRange<T> linearRange) => Add(out _, linearRange);

    /// <inheritdoc cref="AddLinearRange{T}(LinearRange{T})" path="/summary" />
    /// <param name="linearRangeOut">
    ///     The newly added <see cref="LinearRange{T}" /> instance
    /// </param>
    /// <param name="options">
    ///     <inheritdoc cref="LinearRange{T}.Options" path="/summary" />
    /// </param>
    /// <param name="orientation">
    ///     <inheritdoc cref="LinearRange{T}.Orientation" path="/summary" />
    /// </param>
    /// <param name="allowEmpty">
    ///     <inheritdoc cref="LinearRange{T}.AllowEmpty" path="/summary" />
    /// </param>
    /// <param name="rangeAllowSingle">
    ///     <inheritdoc cref="LinearRange{T}.RangeAllowSingle" path="/summary" />
    /// </param>
    /// <param name="showLegends">
    ///     <inheritdoc cref="LinearRange{T}.ShowLegends" path="/summary" />
    /// </param>
    /// <param name="showEndSpacing">
    ///     <inheritdoc cref="LinearRange{T}.ShowEndSpacing" path="/summary" />
    /// </param>
    /// <param name="text">
    ///     <inheritdoc cref="LinearRange{T}.Text" path="/summary" />
    /// </param>
    /// <param name="type">
    ///     <inheritdoc cref="LinearRange{T}.Type" path="/summary" />
    /// </param>
    /// <param name="legendsOrientation">
    ///     <inheritdoc cref="LinearRange{T}.LegendsOrientation" path="/summary" />
    /// </param>
    /// <param name="style">
    ///     <inheritdoc cref="LinearRange{T}.Style" path="/summary" />
    /// </param>
    /// <param name="minimumInnerSpacing">
    ///     <inheritdoc cref="LinearRange{T}.MinimumInnerSpacing" path="/summary" />
    /// </param>
    /// <param name="useMinimumSize">
    ///     <inheritdoc cref="LinearRange{T}.UseMinimumSize" path="/summary" />
    /// </param>
    /// <param name="focusedOption">
    ///     <inheritdoc cref="LinearRange{T}.FocusedOption" path="/summary" />
    /// </param>
    /// <inheritdoc cref="AddLinearRange{T}(LinearRange{T})" path="/returns" />
    public ViewBuilder<TParent> AddLinearRange<T>(
        out LinearRange<T> linearRangeOut,
        List<LinearRangeOption<T>>? options = null,
        Orientation? orientation = null,
        bool? allowEmpty = null,
        bool? rangeAllowSingle = null,
        bool? showLegends = null,
        bool? showEndSpacing = null,
        string? text = null,
        LinearRangeType? type = null,
        Orientation? legendsOrientation = null,
        LinearRangeStyle? style = null,
        int? minimumInnerSpacing = null,
        bool? useMinimumSize = null,
        int? focusedOption = null
    ) => Add(
        out linearRangeOut,
        new(),
        lr => {
            lr.Options = options ?? lr.Options;
            lr.Orientation = orientation ?? lr.Orientation;
            lr.AllowEmpty = allowEmpty ?? lr.AllowEmpty;
            lr.RangeAllowSingle = rangeAllowSingle ?? lr.RangeAllowSingle;
            lr.ShowLegends = showLegends ?? lr.ShowLegends;
            lr.ShowEndSpacing = showEndSpacing ?? lr.ShowEndSpacing;
            lr.Text = text ?? lr.Text;
            lr.Type = type ?? lr.Type;
            lr.LegendsOrientation = legendsOrientation ?? lr.LegendsOrientation;
            lr.Style = style ?? lr.Style;
            lr.MinimumInnerSpacing = minimumInnerSpacing ?? lr.MinimumInnerSpacing;
            lr.UseMinimumSize = useMinimumSize ?? lr.UseMinimumSize;
            lr.FocusedOption = focusedOption ?? lr.FocusedOption;
        }
    );

    #endregion

    #region SpinnerView

    public ViewBuilder<TParent> AddSpinnerView(SpinnerView spinnerView) => Add(out _, spinnerView);

    /// <inheritdoc cref="AddSpinnerView(SpinnerView)" path="/summary" />
    /// <param name="spinnerViewOut">
    ///     The newly added <see cref="SpinnerView" /> instance
    /// </param>
    /// <param name="style">
    ///     <inheritdoc cref="SpinnerView.Style" path="/summary" />
    /// </param>
    /// <param name="autoSpin">
    ///     <inheritdoc cref="SpinnerView.AutoSpin" path="/summary" />
    /// </param>
    /// <param name="spinDelay">
    ///     <inheritdoc cref="SpinnerView.SpinDelay" path="/summary" />
    /// </param>
    /// <param name="spinBounce">
    ///     <inheritdoc cref="SpinnerView.SpinBounce" path="/summary" />
    /// </param>
    /// <param name="spinReverse">
    ///     <inheritdoc cref="SpinnerView.SpinReverse" path="/summary" />
    /// </param>
    /// <param name="sequence">
    ///     <inheritdoc cref="SpinnerView.Sequence" path="/summary" />
    /// </param>
    /// <inheritdoc cref="AddSpinnerView(SpinnerView)" path="/returns" />
    public ViewBuilder<TParent> AddSpinnerView(
        out SpinnerView spinnerViewOut,
        SpinnerStyle? style = null,
        bool? autoSpin = null,
        int? spinDelay = null,
        bool? spinBounce = null,
        bool? spinReverse = null,
        string[]? sequence = null
    ) => Add(
        out spinnerViewOut,
        new(),
        sv => {
            sv.Style = style ?? sv.Style;
            sv.AutoSpin = autoSpin ?? sv.AutoSpin;
            sv.SpinDelay = spinDelay ?? sv.SpinDelay;
            sv.SpinBounce = spinBounce ?? sv.SpinBounce;
            sv.SpinReverse = spinReverse ?? sv.SpinReverse;
            sv.Sequence = sequence ?? sv.Sequence;
        }
    );

    #endregion

    #region StatusBar

    public ViewBuilder<TParent> AddStatusBar(StatusBar statusBar) => Add(out _, statusBar);

    /// <inheritdoc cref="AddStatusBar(StatusBar)" path="/summary" />
    /// <param name="statusBarOut">
    ///     The newly added <see cref="StatusBar" /> instance
    /// </param>
    /// <param name="shortcuts">The shortcuts to add to the status bar.</param>
    /// <param name="orientation">
    ///     <inheritdoc cref="Bar.Orientation" path="/summary" />
    /// </param>
    /// <param name="alignmentModes">
    ///     <inheritdoc cref="Bar.AlignmentModes" path="/summary" />
    /// </param>
    /// <inheritdoc cref="AddStatusBar(StatusBar)" path="/returns" />
    public ViewBuilder<TParent> AddStatusBar(
        out StatusBar statusBarOut,
        IEnumerable<Shortcut>? shortcuts = null,
        Orientation? orientation = null,
        AlignmentModes? alignmentModes = null
    ) => Add(
        out statusBarOut,
        shortcuts is { } ? new(shortcuts) : new(),
        sb => {
            sb.Orientation = orientation ?? sb.Orientation;
            sb.AlignmentModes = alignmentModes ?? sb.AlignmentModes;
        }
    );

    /// <inheritdoc cref="AddStatusBar(StatusBar)" path="/summary" />
    /// <param name="statusBarOut">
    ///     The newly added <see cref="StatusBar" /> instance
    /// </param>
    /// <param name="configureShortcuts">
    ///     A callback that receives a <see cref="ViewBuilder{TParent}" /> for the <see cref="StatusBar" />,
    ///     allowing shortcuts to be added via <see cref="AddShortcut(string?, Key?, Action?, string?, bool?, Orientation?, View?, AlignmentModes?, Command?, View?, int?)" />.
    /// </param>
    /// <param name="orientation">
    ///     <inheritdoc cref="Bar.Orientation" path="/summary" />
    /// </param>
    /// <param name="alignmentModes">
    ///     <inheritdoc cref="Bar.AlignmentModes" path="/summary" />
    /// </param>
    /// <inheritdoc cref="AddStatusBar(StatusBar)" path="/returns" />
    public ViewBuilder<TParent> AddStatusBar(
        out StatusBar statusBarOut,
        Action<ViewBuilder<StatusBar>> configureShortcuts,
        Orientation? orientation = null,
        AlignmentModes? alignmentModes = null
    ) => Add(
        out statusBarOut,
        new(),
        sb => {
            sb.Orientation = orientation ?? sb.Orientation;
            sb.AlignmentModes = alignmentModes ?? sb.AlignmentModes;
            configureShortcuts(sb.Builder());
        }
    );

    #endregion

    #region TreeView

    public ViewBuilder<TParent> AddTreeView(TreeView treeView) => Add(out _, treeView);

    /// <inheritdoc cref="AddTreeView(TreeView)" path="/summary" />
    /// <param name="treeViewOut">
    ///     The newly added <see cref="TreeView" /> instance
    /// </param>
    /// <param name="multiSelect">
    ///     <inheritdoc cref="TreeView{T}.MultiSelect" path="/summary" />
    /// </param>
    /// <param name="allowLetterBasedNavigation">
    ///     <inheritdoc cref="TreeView{T}.AllowLetterBasedNavigation" path="/summary" />
    /// </param>
    /// <param name="maxDepth">
    ///     <inheritdoc cref="TreeView{T}.MaxDepth" path="/summary" />
    /// </param>
    /// <param name="treeBuilder">
    ///     <inheritdoc cref="TreeView{T}.TreeBuilder" path="/summary" />
    /// </param>
    /// <param name="style">
    ///     <inheritdoc cref="TreeView{T}.Style" path="/summary" />
    /// </param>
    /// <param name="selectedObject">
    ///     <inheritdoc cref="TreeView{T}.SelectedObject" path="/summary" />
    /// </param>
    /// <param name="objectActivationButton">
    ///     <inheritdoc cref="TreeView{T}.ObjectActivationButton" path="/summary" />
    /// </param>
    /// <param name="objectActivationKey">
    ///     <inheritdoc cref="TreeView{T}.ObjectActivationKey" path="/summary" />
    /// </param>
    /// <param name="scrollOffsetHorizontal">
    ///     <inheritdoc cref="TreeView{T}.ScrollOffsetHorizontal" path="/summary" />
    /// </param>
    /// <param name="scrollOffsetVertical">
    ///     <inheritdoc cref="TreeView{T}.ScrollOffsetVertical" path="/summary" />
    /// </param>
    /// <inheritdoc cref="AddTreeView(TreeView)" path="/returns" />
    public ViewBuilder<TParent> AddTreeView(
        out TreeView treeViewOut,
        bool? multiSelect = null,
        bool? allowLetterBasedNavigation = null,
        int? maxDepth = null,
        ITreeBuilder<ITreeNode>? treeBuilder = null,
        TreeStyle? style = null,
        ITreeNode? selectedObject = null,
        MouseFlags? objectActivationButton = null,
        KeyCode? objectActivationKey = null,
        int? scrollOffsetHorizontal = null,
        int? scrollOffsetVertical = null
    ) => Add(
        out treeViewOut,
        new(),
        tv => {
            tv.MultiSelect = multiSelect ?? tv.MultiSelect;
            tv.AllowLetterBasedNavigation = allowLetterBasedNavigation ?? tv.AllowLetterBasedNavigation;
            tv.MaxDepth = maxDepth ?? tv.MaxDepth;
            tv.TreeBuilder = treeBuilder ?? tv.TreeBuilder;
            tv.Style = style ?? tv.Style;

            if (selectedObject is { })
            {
                tv.SelectedObject = selectedObject;
            }

            if (objectActivationButton is { })
            {
                tv.ObjectActivationButton = objectActivationButton;
            }

            tv.ObjectActivationKey = objectActivationKey ?? tv.ObjectActivationKey;
            tv.ScrollOffsetHorizontal = scrollOffsetHorizontal ?? tv.ScrollOffsetHorizontal;
            tv.ScrollOffsetVertical = scrollOffsetVertical ?? tv.ScrollOffsetVertical;
        }
    );

    #endregion

    #region Window

    /// <summary>
    ///     Adds a <see cref="Window" /> to the parent view.
    /// </summary>
    /// <returns>The newly added <see cref="Window" /> instance.</returns>
    public ViewBuilder<TParent> AddWindow(Window window) => Add(out _, window);

    /// <inheritdoc cref="AddWindow(Window)" path="/summary" />
    /// <param name="windowOut">The newly added <see cref="Window" /> instance</param>
    /// <param name="addedViews">The list of child views that were added to the window.</param>
    /// <param name="views">The child views to add to the window.</param>
    /// <param name="title">The title displayed by the Window.</param>
    /// <inheritdoc cref="AddWindow(Window)" path="/returns" />
    public ViewBuilder<TParent> AddWindow(out Window windowOut, out List<View> addedViews, List<View>? views = null, string? title = null)
    {
        List<View> tempAddedViews = []; // local variable

        ViewBuilder<TParent> parentWithWindow = Add(
            out windowOut,
            new(),
            window => {
                window.Title = title ?? window.Title;

                if (views == null || views.Count == 0)
                {
                    return;
                }

                ViewBuilder<Window> builder = window.Builder();

                foreach (View view in views)
                {
                    builder.Add(out View outView, view);
                    tempAddedViews.Add(outView);
                }
            });

        addedViews = tempAddedViews;
        return parentWithWindow;
    }

    #endregion

    #region Wizard

    public ViewBuilder<TParent> AddWizard(Wizard wizard) => Add(out _, wizard);

    /// <inheritdoc cref="AddWizard(Wizard)" path="/summary" />
    /// <param name="wizardOut">
    ///     The newly added <see cref="Wizard" /> instance
    /// </param>
    /// <param name="title">The title displayed by the Wizard.</param>
    /// <param name="currentStep">
    ///     <inheritdoc cref="Wizard.CurrentStep" path="/summary" />
    /// </param>
    /// <param name="buttons">
    ///     <inheritdoc cref="Dialog.Buttons" path="/summary" />
    /// </param>
    /// <param name="buttonAlignment">
    ///     <inheritdoc cref="Dialog.ButtonAlignment" path="/summary" />
    /// </param>
    /// <param name="buttonAlignmentModes">
    ///     <inheritdoc cref="Dialog.ButtonAlignmentModes" path="/summary" />
    /// </param>
    /// <inheritdoc cref="AddWizard(Wizard)" path="/returns" />
    public ViewBuilder<TParent> AddWizard(
        out Wizard wizardOut,
        string? title = null,
        WizardStep? currentStep = null,
        Button[]? buttons = null,
        Alignment? buttonAlignment = null,
        AlignmentModes? buttonAlignmentModes = null
    ) => Add(
        out wizardOut,
        new(),
        wiz => {
            wiz.Title = title ?? wiz.Title;
            wiz.CurrentStep = currentStep ?? wiz.CurrentStep;
            wiz.Buttons = buttons ?? wiz.Buttons;
            wiz.ButtonAlignment = buttonAlignment ?? wiz.ButtonAlignment;
            wiz.ButtonAlignmentModes = buttonAlignmentModes ?? wiz.ButtonAlignmentModes;
        }
    );

    #endregion

    #region TextField

    /// <summary>
    ///     Adds a <see cref="TextField" /> to the parent view.
    /// </summary>
    /// <returns>The <see cref="ViewBuilder{TParent}" /> with The newly added <see cref="TextField" /> instance</returns>
    public ViewBuilder<TParent> AddTextField(TextField textField) => Add(out _, textField);

    /// <inheritdoc cref="AddTextField(TextField)" path="/summary" />
    /// <param name="textFieldOut">
    ///     The newly added <see cref="TextField" /> instance
    /// </param>
    /// <param name="text">
    ///     <inheritdoc cref="TextField.Text" path="/summary" />
    /// </param>
    /// <param name="readOnly">
    ///     <inheritdoc cref="TextField.ReadOnly" path="/summary" />
    /// </param>
    /// <param name="secret">
    ///     <inheritdoc cref="TextField.Secret" path="/summary" />
    /// </param>
    /// <param name="insertionPoint">
    ///     <inheritdoc cref="TextField.InsertionPoint" path="/summary" />
    /// </param>
    /// <param name="selectWordOnlyOnDoubleClick">
    ///     <inheritdoc cref="TextField.SelectWordOnlyOnDoubleClick" path="/summary" />
    /// </param>
    /// <param name="useSameRuneTypeForWords">
    ///     <inheritdoc cref="TextField.UseSameRuneTypeForWords" path="/summary" />
    /// </param>
    /// <inheritdoc cref="AddTextField(TextField)" path="/returns" />
    public ViewBuilder<TParent> AddTextField(
        out TextField textFieldOut,
        string? text = null,
        bool? readOnly = null,
        bool? secret = null,
        int? insertionPoint = null,
        bool? selectWordOnlyOnDoubleClick = null,
        bool? useSameRuneTypeForWords = null
    ) => Add(
        out textFieldOut,
        new(),
        tf => {
            tf.Text = text ?? tf.Text;
            tf.ReadOnly = readOnly ?? tf.ReadOnly;
            tf.Secret = secret ?? tf.Secret;
            tf.InsertionPoint = insertionPoint ?? tf.InsertionPoint;
            tf.SelectWordOnlyOnDoubleClick = selectWordOnlyOnDoubleClick ?? tf.SelectWordOnlyOnDoubleClick;
            tf.UseSameRuneTypeForWords = useSameRuneTypeForWords ?? tf.UseSameRuneTypeForWords;
        }
    );

    #endregion

    #region TextView

    /// <summary>
    ///     Adds a <see cref="TextView" /> to the parent view.
    /// </summary>
    /// <returns>The <see cref="ViewBuilder{TParent}" /> with The newly added <see cref="TextView" /> instance</returns>
    public ViewBuilder<TParent> AddTextView(TextView textView) => Add(out _, textView);

    /// <inheritdoc cref="AddTextView(TextView)" path="/summary" />
    /// <param name="textViewOut">
    ///     The newly added <see cref="TextView" /> instance
    /// </param>
    /// <param name="text">
    ///     <inheritdoc cref="TextView.Text" path="/summary" />
    /// </param>
    /// <param name="readOnly">
    ///     <inheritdoc cref="TextView.ReadOnly" path="/summary" />
    /// </param>
    /// <param name="multiline">
    ///     <inheritdoc cref="TextView.Multiline" path="/summary" />
    /// </param>
    /// <param name="wordWrap">
    ///     <inheritdoc cref="TextView.WordWrap" path="/summary" />
    /// </param>
    /// <param name="tabWidth">
    ///     <inheritdoc cref="TextView.TabWidth" path="/summary" />
    /// </param>
    /// <param name="scrollBars">
    ///     <inheritdoc cref="TextView.ScrollBars" path="/summary" />
    /// </param>
    /// <param name="enterKeyAddsLine">
    ///     <inheritdoc cref="TextView.EnterKeyAddsLine" path="/summary" />
    /// </param>
    /// <param name="tabKeyAddsTab">
    ///     <inheritdoc cref="TextView.TabKeyAddsTab" path="/summary" />
    /// </param>
    /// <param name="inheritsPreviousAttribute">
    ///     <inheritdoc cref="TextView.InheritsPreviousAttribute" path="/summary" />
    /// </param>
    /// <param name="selectWordOnlyOnDoubleClick">
    ///     <inheritdoc cref="TextView.SelectWordOnlyOnDoubleClick" path="/summary" />
    /// </param>
    /// <param name="useSameRuneTypeForWords">
    ///     <inheritdoc cref="TextView.UseSameRuneTypeForWords" path="/summary" />
    /// </param>
    /// <inheritdoc cref="AddTextView(TextView)" path="/returns" />
    public ViewBuilder<TParent> AddTextView(
        out TextView textViewOut,
        string? text = null,
        bool? readOnly = null,
        bool? multiline = null,
        bool? wordWrap = null,
        int? tabWidth = null,
        bool? scrollBars = null,
        bool? enterKeyAddsLine = null,
        bool? tabKeyAddsTab = null,
        bool? inheritsPreviousAttribute = null,
        bool? selectWordOnlyOnDoubleClick = null,
        bool? useSameRuneTypeForWords = null
    ) => Add(
        out textViewOut,
        new(),
        tv => {
            tv.Text = text ?? tv.Text;
            tv.ReadOnly = readOnly ?? tv.ReadOnly;
            tv.Multiline = multiline ?? tv.Multiline;
            tv.WordWrap = wordWrap ?? tv.WordWrap;
            tv.TabWidth = tabWidth ?? tv.TabWidth;
            tv.ScrollBars = scrollBars ?? tv.ScrollBars;
            tv.EnterKeyAddsLine = enterKeyAddsLine ?? tv.EnterKeyAddsLine;
            tv.TabKeyAddsTab = tabKeyAddsTab ?? tv.TabKeyAddsTab;
            tv.InheritsPreviousAttribute = inheritsPreviousAttribute ?? tv.InheritsPreviousAttribute;
            tv.SelectWordOnlyOnDoubleClick = selectWordOnlyOnDoubleClick ?? tv.SelectWordOnlyOnDoubleClick;
            tv.UseSameRuneTypeForWords = useSameRuneTypeForWords ?? tv.UseSameRuneTypeForWords;
        }
    );

    #endregion

    #region TableView

    /// <summary>
    ///     Adds a <see cref="TableView" /> to the parent view.
    /// </summary>
    /// <returns>The <see cref="ViewBuilder{TParent}" /> with The newly added <see cref="TableView" /> instance</returns>
    public ViewBuilder<TParent> AddTableView(TableView tableView) => Add(out _, tableView);

    /// <inheritdoc cref="AddTableView(TableView)" path="/summary" />
    /// <param name="tableViewOut">
    ///     The newly added <see cref="TableView" /> instance
    /// </param>
    /// <param name="table">
    ///     <inheritdoc cref="TableView.Table" path="/summary" />
    /// </param>
    /// <param name="fullRowSelect">
    ///     <inheritdoc cref="TableView.FullRowSelect" path="/summary" />
    /// </param>
    /// <param name="multiSelect">
    ///     <inheritdoc cref="TableView.MultiSelect" path="/summary" />
    /// </param>
    /// <param name="style">
    ///     <inheritdoc cref="TableView.Style" path="/summary" />
    /// </param>
    /// <param name="selectedRow">
    ///     <inheritdoc cref="TableView.SelectedRow" path="/summary" />
    /// </param>
    /// <param name="selectedColumn">
    ///     <inheritdoc cref="TableView.SelectedColumn" path="/summary" />
    /// </param>
    /// <param name="nullSymbol">
    ///     <inheritdoc cref="TableView.NullSymbol" path="/summary" />
    /// </param>
    /// <param name="maxCellWidth">
    ///     <inheritdoc cref="TableView.MaxCellWidth" path="/summary" />
    /// </param>
    /// <param name="minCellWidth">
    ///     <inheritdoc cref="TableView.MinCellWidth" path="/summary" />
    /// </param>
    /// <param name="rowOffset">
    ///     <inheritdoc cref="TableView.RowOffset" path="/summary" />
    /// </param>
    /// <param name="columnOffset">
    ///     <inheritdoc cref="TableView.ColumnOffset" path="/summary" />
    /// </param>
    /// <param name="separatorSymbol">
    ///     <inheritdoc cref="TableView.SeparatorSymbol" path="/summary" />
    /// </param>
    /// <param name="cellActivationKey">
    ///     <inheritdoc cref="TableView.CellActivationKey" path="/summary" />
    /// </param>
    /// <param name="useAllRowsForContentCalculation">
    ///     <inheritdoc cref="TableView.UseAllRowsForContentCalculation" path="/summary" />
    /// </param>
    /// <inheritdoc cref="AddTableView(TableView)" path="/returns" />
    public ViewBuilder<TParent> AddTableView(
        out TableView tableViewOut,
        ITableSource? table = null,
        bool? fullRowSelect = null,
        bool? multiSelect = null,
        TableStyle? style = null,
        int? selectedRow = null,
        int? selectedColumn = null,
        string? nullSymbol = null,
        int? maxCellWidth = null,
        int? minCellWidth = null,
        int? rowOffset = null,
        int? columnOffset = null,
        char? separatorSymbol = null,
        KeyCode? cellActivationKey = null,
        bool? useAllRowsForContentCalculation = null
    ) => Add(
        out tableViewOut,
        new(),
        tv => {
            tv.Table = table ?? tv.Table;
            tv.FullRowSelect = fullRowSelect ?? tv.FullRowSelect;
            tv.MultiSelect = multiSelect ?? tv.MultiSelect;
            tv.Style = style ?? tv.Style;
            tv.SelectedRow = selectedRow ?? tv.SelectedRow;
            tv.SelectedColumn = selectedColumn ?? tv.SelectedColumn;
            tv.NullSymbol = nullSymbol ?? tv.NullSymbol;
            tv.MaxCellWidth = maxCellWidth ?? tv.MaxCellWidth;
            tv.MinCellWidth = minCellWidth ?? tv.MinCellWidth;
            tv.RowOffset = rowOffset ?? tv.RowOffset;
            tv.ColumnOffset = columnOffset ?? tv.ColumnOffset;
            tv.SeparatorSymbol = separatorSymbol ?? tv.SeparatorSymbol;
            tv.CellActivationKey = cellActivationKey ?? tv.CellActivationKey;
            tv.UseAllRowsForContentCalculation = useAllRowsForContentCalculation ?? tv.UseAllRowsForContentCalculation;
        }
    );

    #endregion

    #region Tab

    /// <summary>
    ///     Adds a <see cref="Tab" /> to the parent view.
    /// </summary>
    /// <returns>The <see cref="ViewBuilder{TParent}" /> with The newly added <see cref="Tab" /> instance</returns>
    public ViewBuilder<TParent> AddTab(Tab tab) => Add(out _, tab);

    /// <inheritdoc cref="AddTab(Tab)" path="/summary" />
    /// <param name="tabOut">
    ///     The newly added <see cref="Tab" /> instance
    /// </param>
    /// <param name="text">The text displayed by the Tab. Defaults to "Tab {n}" where n is the subview count.</param>
    /// <param name="view">
    ///     <inheritdoc cref="Tab.View" path="/summary" />
    /// </param>
    /// <param name="displayText">
    ///     <inheritdoc cref="Tab.DisplayText" path="/summary" />
    /// </param>
    /// <inheritdoc cref="AddTab(Tab)" path="/returns" />
    public ViewBuilder<TParent> AddTab(
        out Tab tabOut,
        string? text = null,
        View? view = null,
        string? displayText = null
    ) => Add(
        out tabOut,
        new(),
        tab => {
            tab.Text = text ?? $"Tab {parent.SubViews.Count}";
            tab.View = view ?? tab.View;
            tab.DisplayText = displayText ?? tab.DisplayText;
        }
    );

    #endregion

    #region TabView

    /// <summary>
    ///     Adds a <see cref="TabView" /> to the parent view.
    /// </summary>
    /// <returns>The <see cref="ViewBuilder{TParent}" /> with The newly added <see cref="TabView" /> instance</returns>
    public ViewBuilder<TParent> AddTabView(TabView tabView) => Add(out _, tabView);

    /// <inheritdoc cref="AddTabView(TabView)" path="/summary" />
    /// <param name="tabViewOut">
    ///     The newly added <see cref="TabView" /> instance
    /// </param>
    /// <param name="maxTabTextWidth">
    ///     <inheritdoc cref="TabView.MaxTabTextWidth" path="/summary" />
    /// </param>
    /// <param name="style">
    ///     <inheritdoc cref="TabView.Style" path="/summary" />
    /// </param>
    /// <param name="selectedTab">
    ///     <inheritdoc cref="TabView.SelectedTab" path="/summary" />
    /// </param>
    /// <param name="tabScrollOffset">
    ///     <inheritdoc cref="TabView.TabScrollOffset" path="/summary" />
    /// </param>
    /// <inheritdoc cref="AddTabView(TabView)" path="/returns" />
    public ViewBuilder<TParent> AddTabView(
        out TabView tabViewOut,
        uint? maxTabTextWidth = null,
        TabStyle? style = null,
        Tab? selectedTab = null,
        int? tabScrollOffset = null
    ) => Add(
        out tabViewOut,
        new(),
        tv => {
            tv.MaxTabTextWidth = maxTabTextWidth ?? tv.MaxTabTextWidth;
            tv.Style = style ?? tv.Style;
            tv.SelectedTab = selectedTab ?? tv.SelectedTab;
            tv.TabScrollOffset = tabScrollOffset ?? tv.TabScrollOffset;
        }
    );

    #endregion

    #region OptionSelector

    /// <summary>
    ///     Adds an <see cref="OptionSelector" /> to the parent view.
    /// </summary>
    /// <returns>The <see cref="ViewBuilder{TParent}" /> with The newly added <see cref="OptionSelector" /> instance</returns>
    public ViewBuilder<TParent> AddOptionSelector(OptionSelector optionSelector) => Add(out _, optionSelector);

    /// <inheritdoc cref="AddOptionSelector(OptionSelector)" path="/summary" />
    /// <param name="optionSelectorOut">
    ///     The newly added <see cref="OptionSelector" /> instance
    /// </param>
    /// <param name="text">
    ///     <inheritdoc cref="SelectorBase.Text" path="/summary" />
    /// </param>
    /// <param name="orientation">
    ///     <inheritdoc cref="SelectorBase.Orientation" path="/summary" />
    /// </param>
    /// <param name="styles">
    ///     <inheritdoc cref="SelectorBase.Styles" path="/summary" />
    /// </param>
    /// <param name="doubleClickAccepts">
    ///     <inheritdoc cref="SelectorBase.DoubleClickAccepts" path="/summary" />
    /// </param>
    /// <param name="labels">
    ///     <inheritdoc cref="SelectorBase.Labels" path="/summary" />
    /// </param>
    /// <param name="horizontalSpace">
    ///     <inheritdoc cref="SelectorBase.HorizontalSpace" path="/summary" />
    /// </param>
    /// <param name="focusedItem">
    ///     <inheritdoc cref="OptionSelector.FocusedItem" path="/summary" />
    /// </param>
    /// <param name="value">
    ///     <inheritdoc cref="SelectorBase.Value" path="/summary" />
    /// </param>
    /// <param name="values">
    ///     <inheritdoc cref="SelectorBase.Values" path="/summary" />
    /// </param>
    /// <inheritdoc cref="AddOptionSelector(OptionSelector)" path="/returns" />
    public ViewBuilder<TParent> AddOptionSelector(
        out OptionSelector optionSelectorOut,
        string? text = null,
        Orientation? orientation = null,
        SelectorStyles? styles = null,
        bool? doubleClickAccepts = null,
        IReadOnlyList<string>? labels = null,
        int? horizontalSpace = null,
        int? focusedItem = null,
        int? value = null,
        IReadOnlyList<int>? values = null
    ) => Add(
        out optionSelectorOut,
        new(),
        os => {
            os.Text = text ?? os.Text;
            os.Orientation = orientation ?? os.Orientation;
            os.Styles = styles ?? os.Styles;
            os.DoubleClickAccepts = doubleClickAccepts ?? os.DoubleClickAccepts;
            os.Labels = labels ?? os.Labels;
            os.HorizontalSpace = horizontalSpace ?? os.HorizontalSpace;
            os.FocusedItem = focusedItem ?? os.FocusedItem;
            os.Value = value ?? os.Value;
            os.Values = values ?? os.Values;
        }
    );

    /// <summary>
    ///     Adds an <see cref="OptionSelector{TEnum}" /> to the parent view.
    /// </summary>
    /// <returns>The <see cref="ViewBuilder{TParent}" /> with The newly added <see cref="OptionSelector{TEnum}" /> instance</returns>
    public ViewBuilder<TParent> AddOptionSelector<TEnum>(OptionSelector<TEnum> optionSelector) where TEnum : struct, Enum
        => Add(out _, optionSelector);

    /// <inheritdoc cref="AddOptionSelector{TEnum}(OptionSelector{TEnum})" path="/summary" />
    /// <param name="optionSelectorOut">
    ///     The newly added <see cref="OptionSelector{TEnum}" /> instance
    /// </param>
    /// <param name="text">
    ///     <inheritdoc cref="SelectorBase.Text" path="/summary" />
    /// </param>
    /// <param name="orientation">
    ///     <inheritdoc cref="SelectorBase.Orientation" path="/summary" />
    /// </param>
    /// <param name="styles">
    ///     <inheritdoc cref="SelectorBase.Styles" path="/summary" />
    /// </param>
    /// <param name="doubleClickAccepts">
    ///     <inheritdoc cref="SelectorBase.DoubleClickAccepts" path="/summary" />
    /// </param>
    /// <param name="horizontalSpace">
    ///     <inheritdoc cref="SelectorBase.HorizontalSpace" path="/summary" />
    /// </param>
    /// <param name="value">
    ///     <inheritdoc cref="OptionSelector{TEnum}.Value" path="/summary" />
    /// </param>
    /// <inheritdoc cref="AddOptionSelector{TEnum}(OptionSelector{TEnum})" path="/returns" />
    public ViewBuilder<TParent> AddOptionSelector<TEnum>(
        out OptionSelector<TEnum> optionSelectorOut,
        string? text = null,
        Orientation? orientation = null,
        SelectorStyles? styles = null,
        bool? doubleClickAccepts = null,
        int? horizontalSpace = null,
        TEnum? value = null
    ) where TEnum : struct, Enum => Add(
        out optionSelectorOut,
        new(),
        os => {
            os.Text = text ?? os.Text;
            os.Orientation = orientation ?? os.Orientation;
            os.Styles = styles ?? os.Styles;
            os.DoubleClickAccepts = doubleClickAccepts ?? os.DoubleClickAccepts;
            os.HorizontalSpace = horizontalSpace ?? os.HorizontalSpace;
            os.Value = value ?? os.Value;
        }
    );

    #endregion

    #region ColorPicker

    /// <summary>
    ///     Adds a <see cref="ColorPicker" /> to the parent view.
    /// </summary>
    /// <returns>The <see cref="ViewBuilder{TParent}" /> with The newly added <see cref="ColorPicker" /> instance</returns>
    public ViewBuilder<TParent> AddColorPicker(ColorPicker colorPicker) => Add(out _, colorPicker);

    /// <inheritdoc cref="AddColorPicker(ColorPicker)" path="/summary" />
    /// <param name="colorPickerOut">
    ///     The newly added <see cref="ColorPicker" /> instance
    /// </param>
    /// <param name="selectedColor">
    ///     <inheritdoc cref="ColorPicker.SelectedColor" path="/summary" />
    /// </param>
    /// <param name="style">
    ///     <inheritdoc cref="ColorPicker.Style" path="/summary" />
    /// </param>
    /// <param name="text">
    ///     <inheritdoc cref="ColorPicker.Text" path="/summary" />
    /// </param>
    /// <inheritdoc cref="AddColorPicker(ColorPicker)" path="/returns" />
    public ViewBuilder<TParent> AddColorPicker(
        out ColorPicker colorPickerOut,
        Color? selectedColor = null,
        ColorPickerStyle? style = null,
        string? text = null
    ) => Add(
        out colorPickerOut,
        new(),
        cp => {
            if (selectedColor is { })
            {
                cp.SelectedColor = selectedColor.Value;
            }

            cp.Style = style ?? cp.Style;
            cp.Text = text ?? cp.Text;
        }
    );

    /// <summary>
    ///     Adds a <see cref="ColorPicker16" /> to the parent view.
    /// </summary>
    /// <returns>The <see cref="ViewBuilder{TParent}" /> with The newly added <see cref="ColorPicker16" /> instance</returns>
    public ViewBuilder<TParent> AddColorPicker16(ColorPicker16 colorPicker16) => Add(out _, colorPicker16);

    /// <inheritdoc cref="AddColorPicker16(ColorPicker16)" path="/summary" />
    /// <param name="colorPicker16Out">
    ///     The newly added <see cref="ColorPicker16" /> instance
    /// </param>
    /// <param name="selectedColor">
    ///     <inheritdoc cref="ColorPicker16.SelectedColor" path="/summary" />
    /// </param>
    /// <param name="boxWidth">
    ///     <inheritdoc cref="ColorPicker16.BoxWidth" path="/summary" />
    /// </param>
    /// <param name="boxHeight">
    ///     <inheritdoc cref="ColorPicker16.BoxHeight" path="/summary" />
    /// </param>
    /// <inheritdoc cref="AddColorPicker16(ColorPicker16)" path="/returns" />
    public ViewBuilder<TParent> AddColorPicker16(
        out ColorPicker16 colorPicker16Out,
        ColorName16? selectedColor = null,
        int? boxWidth = null,
        int? boxHeight = null
    ) => Add(
        out colorPicker16Out,
        new(),
        cp16 => {
            cp16.SelectedColor = selectedColor ?? cp16.SelectedColor;
            cp16.BoxWidth = boxWidth ?? cp16.BoxWidth;
            cp16.BoxHeight = boxHeight ?? cp16.BoxHeight;
        }
    );

    #endregion
}
