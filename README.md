# TerminalGui.Extensions

# WORK IN PROGRESS

<details>
<summary>Full Example — Building a settings window with multiple panels</summary>

```csharp
public class SettingsWindow : Window
{
    public SettingsWindow()
    {
        Title = "Settings";
        this.WithFill();

        ViewBuilder<SettingsWindow> builder = this.Builder();

        builder.AddFrameView(out FrameView audioFrame, title: "Audio");
        audioFrame.WithFillAuto();

        ViewBuilder<FrameView> audioBuilder = audioFrame.Builder();

        audioBuilder.AddLabel(out _, text: "Master Volume:");

        audioBuilder.AddNumericUpDown(
            out NumericUpDownConstrained<int> nudVolume,
            value: 80,
            step: 5,
            min: 0,
            max: 100,
            format: "{0}%");

        audioBuilder.AddCheckBox(out CheckBox chkMute, text: "Mute All");
        chkMute.IsChecked = false;

        builder.AddFrameView(out FrameView displayFrame, title: "Display");
        displayFrame.WithFillAuto();

        ViewBuilder<FrameView> displayBuilder = displayFrame.Builder();

        displayBuilder.AddOptionSelector(
            out OptionSelector themeSelector,
            labels: ["Dark", "Light", "Solarized"],
            value: 0);

        themeSelector.OnValueChanged(e =>
            ApplyTheme(e.NewValue ?? 0));

        displayBuilder.AddCheckBox(
            out CheckBox chkAnimations,
            text: "Enable Animations",
            checkedState: CheckState.Checked);

        builder.AddTabView(out TabView tabView);
        tabView.WithFill();

        View generalContent = new View().WithFill();
        generalContent.Builder()
           .AddLabel(out _, text: "Player Name:")
           .AddTextField(out TextField tfName, text: "Player 1")
           .AddLabel(out _, text: "Difficulty:")
           .AddOptionSelector(out OptionSelector<Difficulty> diffSelector, value: Difficulty.Normal);

        tfName.OnValueChanged(e =>
            Console.WriteLine($"Name changed to: {e.NewValue}"));

        View keybindsContent = new View().WithFill();
        keybindsContent.MakeScrollable();
        ViewBuilder<View> keybindsBuilder = keybindsContent.Builder();

        keybindsBuilder.AddLabel(out _, text: "Move Up:");
        keybindsBuilder.AddTextField(out _, text: "W", readOnly: true);
        keybindsBuilder.AddLabel(out _, text: "Move Down:");
        keybindsBuilder.AddTextField(out _, text: "S", readOnly: true);
        keybindsBuilder.AddLabel(out _, text: "Interact:");
        keybindsBuilder.AddTextField(out _, text: "E", readOnly: true);

        tabView.Builder()
           .AddTab(out _, text: "General", view: generalContent)
           .AddTab(out _, text: "Keybinds", view: keybindsContent);

        builder.AddStatusBar(out _, statusBar => statusBar
           .AddShortcut(text: "Save", key: Key.S.WithCtrl)
           .AddShortcut(text: "Close", key: Key.Esc)
        );

        chkMute.OnValueChanging(e => {
            bool isChecked = e.NewValue == CheckState.Checked;
            nudVolume.Max = isChecked ? 0 : 100;

            if (isChecked)
            {
                nudVolume.Value = 0;
            }
        });
    }

    private static void ApplyTheme(int themeIndex)
    { /* ... */
    }
}
```

</details>

## Table of contents
*For more in depth documentation refer to the SourceCode listed under every TOC header*</br>
*Also refer to [Terminal.Gui Documentation](https://gui-cs.github.io/Terminal.Gui/)*

<!-- TOC-->
  - [ViewBuilder](#viewbuilder)
    - [Notes](#notes)
    - [Properties](#properties)
    - [Core Methods](#core-methods)
    - [Add Methods](#add-methods)
  - [View Extensions](#view-extensions)
    - [Bar Extensions](#bar-extensions)
    - [CheckBox Extensions](#checkbox-extensions)
      - [CheckState Extensions](#checkstate-extensions)
    - [ColorPicker Extensions](#colorpicker-extensions)
    - [DatePicker Extensions](#datepicker-extensions)
    - [FlagSelector Extensions](#flagselector-extensions)
    - [HexView Extensions](#hexview-extensions)
    - [Line Extensions](#line-extensions)
    - [ListView Extensions](#listview-extensions)
    - [Menu Extensions](#menu-extensions)
    - [MenuBar Extensions](#menubar-extensions)
    - [NumericUpDown Extensions](#numericupdown-extensions)
    - [OptionSelector Extensions](#optionselector-extensions)
    - [ScrollBar Extensions](#scrollbar-extensions)
    - [ScrollSlider Extensions](#scrollslider-extensions)
    - [PopoverMenu Extensions](#popovermenu-extensions)
    - [Shortcut Extensions](#shortcut-extensions)
    - [TabView Extensions](#tabview-extensions)
    - [TextField Extensions](#textfield-extensions)
    - [TableView Extensions](#tableview-extensions)
    - [TextView Extensions](#textview-extensions)
    - [Wizard Extensions](#wizard-extensions)
  - [Custom Views](#custom-views)
    - [NumericUpDownConstrained\<T\>](#numericupdownconstrainedt)
  - [MessageBox Extensions](#messagebox-extensions)
  - [ApplicationExtensions](#applicationextensions)
  - [ApplicationNavigationExtensions](#applicationnavigationextensions)
  - [ApplicationPopoverExtensions](#applicationpopoverextensions)
  - [RunnableExtensions](#runnableextensions)
<!-- TOC -->


## ViewBuilder
[ViewBuilder.cs](/Core/Builders/ViewBuilder.cs)

### Notes

+ All `Add` methods return the `ViewBuilder` instance.
+ All `Add(...)` methods return the added child via an `out` parameter as the first out parameter.
+ Parameters with `null` defaults retain their class's initialization values.
Except for the `Text` parameter which defaults to `"{typeName} {parent.SubViews.Count}"` (e.g., `"Button 3"`) if not provided.

### Properties

**NextPosY**

A function that determines how to position a new child relative to the previously added child.</br>
Takes the last added child as input and returns the Y position to use for the new child, or null to skip auto-positioning.

By default this is `Pos.Bottom`, which ensures that each new child is placed directly below the last added child.

**NextPosX**

Does the exact same as NextPosY, but for the X position. By default no auto-positioning is applied for X.

**SkipAutoPositioning**

If true, skips any auto-positioning logic for any future added children, unless set to false again.
It is suggested to set this property when using `view.WithLayout(...)` if you do not want auto-positioning to interfere with the layout.

### Core Methods

**GetView()**

Returns the parent `View` associated with the builder.

**GetLastChildAdded()**

```csharp
viewBuilder.AddButton(out _, "a button");
Button? button = viewBuilder.GetLastChildAdded() as Button;
```

**Add(...)**

```csharp
windowBuilder.Add(out Button button, new(), btn => btn.Text = "Click me");

// You can also add a ViewBuilder directly:
ViewBuilder<Menu> menuBuilder = new Menu().Builder();
menuBuilder.AddLabel(out Label label, "Menu 1");
windowBuilder.Add(menuBuilder, out Menu menu);
```

### Add Methods

Every view type follows the same pattern — two overloads:
1. Pass a pre-constructed instance: `AddXxx(Xxx instance)`
2. Use named parameters with an `out` reference: `AddXxx(out Xxx xxxOut, ...)`

| Method | View Type | Notable Parameters |
|--------|-----------|-------------------|
| `AddBar` | `Bar` | `alignmentModes`, `orientation` |
| `AddButton` | `Button` | `text`, `isDefault`, `noDecorations`, `noPadding`, `hotKeySpecifier` |
| `AddCharMap` | `CharMap` | `selectedCodePoint`, `showGlyphWidths`, `startCodePoint`, `value`, `showUnicodeCategory` |
| `AddCheckBox` | `CheckBox` | `text`, `checkedState`, `allowCheckedStateNone`, `hotKeySpecifier` |
| `AddRadioButton` | `CheckBox` | Same as CheckBox but with `RadioStyle = true` |
| `AddColorPicker` | `ColorPicker` | `selectedColor`, `style`, `text` |
| `AddColorPicker16` | `ColorPicker16` | `selectedColor`, `boxWidth`, `boxHeight` |
| `AddDatePicker` | `DatePicker` | `date`, `culture`, `text` |
| `AddDialog` | `Dialog` | `title`, `buttons`, `buttonAlignment`, `buttonAlignmentModes`, `result` |
| `AddDropDownList` | `DropDownList` | `source`, `text`, `readOnly`, `secret` |
| `AddFileDialog` | `FileDialog` | `title`, `path`, `allowedTypes`, `allowsMultipleSelection`, `mustExist`, `openMode` |
| `AddFrameView` | `FrameView` | `title` |
| `AddGraphView` | `GraphView` | `cellSize`, `scrollOffset`, `graphColor`, `marginLeft`, `marginBottom`, `axisX`, `axisY` |
| `AddHexView` | `HexView` | `source`, `readOnly`, `bytesPerLine`, `addressWidth`, `address` |
| `AddLabel` | `Label` | `text`, `hotKeySpecifier` |
| `AddLine` | `Line` | `length`, `orientation`, `lineStyle` |
| `AddLinearRange<T>` | `LinearRange<T>` | `options`, `orientation`, `allowEmpty`, `rangeAllowSingle`, `showLegends`, `type`, `style`, ... |
| `AddListView` | `ListView` | `source`, `selectedItem`, `value`, `showMarks`, `markMultiple` |
| `AddMenu` | `Menu` | `menuItems`, `orientation`, `alignmentModes`, `superMenuItem`, `value` |
| `AddMenuItem` | `MenuItem` | `commandText`, `helpText`, `action`, `key`, `subMenu`, ... |
| `AddMenuBar` | `MenuBar` | `menus`, `key` |
| `AddMenuBarItem` | `MenuBarItem` | `commandText`, `targetView`, `command`, `popoverMenu` |
| `AddNumericUpDown<T>` | `NumericUpDownConstrained<T>` | `value`, `format`, `step`, `min`, `max` |
| `AddOpenDialog` | `OpenDialog` | `title`, `path`, `allowedTypes`, `allowsMultipleSelection`, `mustExist`, `openMode` |
| `AddOptionSelector` | `OptionSelector` | `text`, `orientation`, `styles`, `labels`, `focusedItem`, `value`, `values`, ... |
| `AddOptionSelector<TEnum>` | `OptionSelector<TEnum>` | `text`, `orientation`, `styles`, `value`, ... |
| `AddPopoverMenu` | `PopoverMenu` | `root`, `key`, `mouseFlags` |
| `AddProgressBar` | `ProgressBar` | `text`, `fraction`, `format`, `style`, `segmentCharacter`, `bidirectionalMarquee` |
| `AddSaveDialog` | `SaveDialog` | `title`, `path`, `allowedTypes`, `mustExist` |
| `AddScrollBar` | `ScrollBar` | `orientation`, `increment`, `visibleContentSize`, `scrollableContentSize`, `value`, `visibilityMode` |
| `AddScrollSlider` | `ScrollSlider` | `orientation`, `size`, `position`, `visibleContentSize`, `sliderPadding` |
| `AddShortcut` | `Shortcut` | `text`, `key`, `action`, `helpText`, `bindKeyToApplication`, `command`, `targetView`, ... |
| `AddSpinnerView` | `SpinnerView` | `style`, `autoSpin`, `spinDelay`, `spinBounce`, `spinReverse`, `sequence` |
| `AddStatusBar` | `StatusBar` | `shortcuts` or `configureShortcuts` callback, `orientation`, `alignmentModes` |
| `AddTab` | `Tab` | `text`, `view`, `displayText` |
| `AddTabView` | `TabView` | `maxTabTextWidth`, `style`, `selectedTab`, `tabScrollOffset` |
| `AddTableView` | `TableView` | `table`, `fullRowSelect`, `multiSelect`, `style`, `selectedRow`, `selectedColumn`, ... |
| `AddTextField` | `TextField` | `text`, `readOnly`, `secret`, `insertionPoint`, ... |
| `AddTextView` | `TextView` | `text`, `readOnly`, `multiline`, `wordWrap`, `tabWidth`, `scrollBars`, ... |
| `AddTreeView` | `TreeView` | `multiSelect`, `allowLetterBasedNavigation`, `maxDepth`, `treeBuilder`, `style`, ... |
| `AddWindow` | `Window` | `title`, `views` (auto-added children), returns `addedViews` via second out parameter |
| `AddWizard` | `Wizard` | `title`, `currentStep`, `buttons`, `buttonAlignment`, `buttonAlignmentModes` |

<details>
<summary>Examples</summary>

**AddButton**

```csharp
viewBuilder.AddButton(new() { Text = "Button 1" });

viewBuilder.AddButton(
    out Button button,
    text: "Button 2",
    isDefault: false,
    noDecorations: false,
    noPadding: false,
    hotKeySpecifier: new(';'));
```

**AddCheckBox / AddRadioButton**

```csharp
viewBuilder.AddCheckBox(
    out CheckBox checkBox,
    text: "Checkbox 1",
    checkedState: CheckState.Checked,
    allowCheckedStateNone: false);

viewBuilder.AddRadioButton(
    out CheckBox radioButton,
    text: "Option A",
    checkedState: CheckState.Checked);
```

**AddLabel**

```csharp
viewBuilder.AddLabel(out Label label, text: "Hello, World!");
```

**AddNumericUpDown**

```csharp
viewBuilder.AddNumericUpDown(
    out NumericUpDownConstrained<double> nud,
    value: 50.0,
    step: 1.0,
    format: "Value: {0}",
    min: 0.0,
    max: 100.0);
```

**AddWindow**

```csharp
viewBuilder.AddWindow(new());

viewBuilder.AddWindow(
    out Window subWindow,
    out List<View> addedViews,
    views: [new Label() { Text = "Hello, World!" }]);
```

**AddMenuBar**

```csharp
viewBuilder.AddMenuBar(out MenuBar menuBar, menus: [
    new MenuBarItem() { Title = "File" }
]);
```

**AddProgressBar**

```csharp
viewBuilder.AddProgressBar(
    out ProgressBar progressBar,
    text: "Loading",
    fraction: 0.5f,
    style: ProgressBarStyle.Continuous);
```

**AddTabView / AddTab**

```csharp
viewBuilder.AddTabView(out TabView tabView);
tabView.Builder()
    .AddTab(out Tab tab1, text: "Tab 1", view: new Label { Text = "Content 1" })
    .AddTab(out Tab tab2, text: "Tab 2", view: new Label { Text = "Content 2" });
```

**AddOptionSelector**

```csharp
viewBuilder.AddOptionSelector(
    out OptionSelector os,
    labels: ["Option A", "Option B", "Option C"],
    value: 0);

viewBuilder.AddOptionSelector(
    out OptionSelector<MyEnum> enumSelector,
    value: MyEnum.FirstValue);
```

</details>

## View Extensions
[ViewBaseExtensions.cs](/Extensions/ViewExtensions/ViewBaseExtensions.cs)

**Builder()**

Creates a `ViewBuilder<T>` instance for any View.

```csharp
ViewBuilder<Window> builder = view.Builder();
builder.AddButton(out _, "Button Text");
```

**ConfigureWithBuilder(...)**

Creates a builder via `Builder()` and passes it to the callback.

```csharp
view.ConfigureWithBuilder(viewBuilder =>
{
    viewBuilder.AddButton(out _, "Button Text");
});
```

**WithLayout(...)**

```csharp
view.WithLayout(
    width: Dim.Fill(),
    height: Dim.Fill(),
    x: Pos.Center(),
    y: Pos.Center()
);

view.WithLayout(
    width: Dim.Auto(),
    height: Dim.Auto()
);
```

**WithFill(...)**

Sets both width and height to `Dim.Fill()`.

```csharp
view.WithFill();
view.WithFill(widthAdjust: -2, heightAdjust: -1);
```

**WithAuto(...)**

Sets both width and height to `Dim.Auto()`.

```csharp
view.WithAuto();
view.WithAuto(widthAdjust: 2);
```

**WithFillAuto(...)**

```csharp
view.WithFillAuto();
view.WithFillAuto(widthAdjust: -1);
```

**MakeScrollable()**

Configures the view as scrollable content with a vertical scroll bar. Automatically tracks the content size based on subview positions.

```csharp
view.MakeScrollable();
```

**Subview Search**

`FindSubView<T>()` — Finds the first direct child of the given type.

`FindAllSubViews<T>()` — Recursively finds all descendants of the given type.

```csharp
Button? btn = view.FindSubView<Button>();
IEnumerable<Label> allLabels = view.FindAllSubViews<Label>();
```

**Command Event Wrappers**

| Method | Wraps |
|--------|-------|
| `OnAccepted(callback)` | `view.Accepted += ...` |
| `OnAccepting(callback)` | `view.Accepting += ...` |
| `OnActivating(callback)` | `view.Activating += ...` |
| `OnActivated(callback)` | `view.Activated += ...` |
| `OnHandlingHotKey(callback)` | `view.HandlingHotKey += ...` |
| `OnHotKeyCommand(callback)` | `view.HotKeyCommand += ...` |
| `OnCommandNotBound(callback)` | `view.CommandNotBound += ...` |

```csharp
view.OnAccepting(args => ...);
```

**Keyboard Event Wrappers**

| Method | Wraps |
|--------|-------|
| `OnKeyDown(callback)` | `view.KeyDown += ...` |
| `OnKeyDownNotHandled(callback)` | `view.KeyDownNotHandled += ...` |

**Mouse Event Wrappers**

| Method | Wraps |
|--------|-------|
| `OnMouseEvent(callback)` | `view.MouseEvent += ...` |
| `OnMouseEnter(callback)` | `view.MouseEnter += ...` |
| `OnMouseLeave(callback)` | `view.MouseLeave += ...` |
| `OnMouseStateChanged(callback)` | `view.MouseStateChanged += ...` |
| `OnMouseHoldRepeatChanged(callback)` | `view.MouseHoldRepeatChanged += ...` |
| `OnMouseHoldRepeatChanging(callback)` | `view.MouseHoldRepeatChanging += ...` |

**Focus Event Wrappers**

| Method | Wraps |
|--------|-------|
| `OnHasFocusChanged(callback)` | `view.HasFocusChanged += ...` |
| `OnHasFocusChanging(callback)` | `view.HasFocusChanging += ...` |
| `OnFocusedChanged(callback)` | `view.FocusedChanged += ...` |
| `OnAdvancingFocus(callback)` | `view.AdvancingFocus += ...` |
| `OnCanFocusChanged(callback)` | `view.CanFocusChanged += ...` |

**Lifecycle Event Wrappers**

| Method | Wraps |
|--------|-------|
| `OnInitialized(callback)` | `view.Initialized += ...` |
| `OnDisposing(callback)` | `view.Disposing += ...` |
| `OnRemoved(callback)` | `view.Removed += ...` |
| `OnSuperViewChanged(callback)` | `view.SuperViewChanged += ...` |
| `OnSuperViewChanging(callback)` | `view.SuperViewChanging += ...` |
| `OnSubViewAdded(callback)` | `view.SubViewAdded += ...` |
| `OnSubViewRemoved(callback)` | `view.SubViewRemoved += ...` |

**State Change Event Wrappers**

| Method | Wraps |
|--------|-------|
| `OnVisibleChanged(callback)` | `view.VisibleChanged += ...` |
| `OnVisibleChanging(callback)` | `view.VisibleChanging += ...` |
| `OnEnabledChanged(callback)` | `view.EnabledChanged += ...` |
| `OnTextChanged(callback)` | `view.TextChanged += ...` |
| `OnTitleChanged(callback)` | `view.TitleChanged += ...` |
| `OnTitleChanging(callback)` | `view.TitleChanging += ...` |
| `OnBorderStyleChanged(callback)` | `view.BorderStyleChanged += ...` |
| `OnHotKeyChanged(callback)` | `view.HotKeyChanged += ...` |
| `OnSchemeChanged(callback)` | `view.SchemeChanged += ...` |
| `OnSchemeChanging(callback)` | `view.SchemeChanging += ...` |
| `OnSchemeNameChanged(callback)` | `view.SchemeNameChanged += ...` |
| `OnSchemeNameChanging(callback)` | `view.SchemeNameChanging += ...` |
| `OnContentSizeChanged(callback)` | `view.ContentSizeChanged += ...` |
| `OnContentSizeChanging(callback)` | `view.ContentSizeChanging += ...` |

**Layout Event Wrappers**

| Method | Wraps |
|--------|-------|
| `OnSubViewsLaidOut(callback)` | `view.SubViewsLaidOut += ...` |
| `OnSubViewLayout(callback)` | `view.SubViewLayout += ...` |
| `OnFrameChanged(callback)` | `view.FrameChanged += ...` |
| `OnHeightChanged(callback)` | `view.HeightChanged += ...` |
| `OnHeightChanging(callback)` | `view.HeightChanging += ...` |
| `OnWidthChanged(callback)` | `view.WidthChanged += ...` |
| `OnWidthChanging(callback)` | `view.WidthChanging += ...` |

**Drawing Event Wrappers**

| Method | Wraps |
|--------|-------|
| `OnDrawComplete(callback)` | `view.DrawComplete += ...` |
| `OnDrawingContent(callback)` | `view.DrawingContent += ...` |
| `OnDrawingSubViews(callback)` | `view.DrawingSubViews += ...` |
| `OnDrawingText(callback)` | `view.DrawingText += ...` |
| `OnDrewText(callback)` | `view.DrewText += ...` |
| `OnClearingViewport(callback)` | `view.ClearingViewport += ...` |
| `OnClearedViewport(callback)` | `view.ClearedViewport += ...` |
| `OnViewportChanged(callback)` | `view.ViewportChanged += ...` |
| `OnGettingAttributeForRole(callback)` | `view.GettingAttributeForRole += ...` |
| `OnGettingScheme(callback)` | `view.GettingScheme += ...` |

### Bar Extensions
[BarExtensions.cs](/Extensions/ViewExtensions/BarExtensions.cs)

| Method | Wraps |
|--------|-------|
| `OnOrientationChanged(callback)` | `bar.OrientationChanged += ...` |
| `OnOrientationChanging(callback)` | `bar.OrientationChanging += ...` |

### CheckBox Extensions
[CheckBoxExtensions.cs](/Extensions/ViewExtensions/CheckBoxExtensions.cs)

**IsChecked**
```csharp
bool isChecked = checkbox.IsChecked; // Equal to checkbox.CheckedState == CheckState.Checked;
checkbox.IsChecked = true; // Equal to checkbox.CheckedState = CheckState.Checked;
```

**Event Wrappers**

| Method | Equivalent |
|--------|-----------|
| `OnValueChanged(callback)` | `checkbox.ValueChanged += ...` |
| `OnValueChanging(callback)` | `checkbox.ValueChanging += ...` |

```csharp
checkbox.OnValueChanged(e => Console.WriteLine($"Changed from {e.OldValue} to {e.NewValue}"));
checkbox.OnValueChanging(e => {
    if (e.NewValue == CheckState.None)
        e.Handled = true; // cancel the change
});
```

#### CheckState Extensions
[CheckBoxExtensions.cs](/Extensions/ViewExtensions/CheckBoxExtensions.cs)

**static ConvertCheckState(...)**
```csharp
CheckState state1 = CheckState.ConvertCheckState(true);  // CheckState.Checked
CheckState state2 = CheckState.ConvertCheckState(false); // CheckState.UnChecked
CheckState state3 = CheckState.ConvertCheckState(null);  // CheckState.None
```

```csharp
bool? value1 = CheckState.ConvertCheckState(CheckState.Checked);   // true
bool? value2 = CheckState.ConvertCheckState(CheckState.UnChecked); // false
bool? value3 = CheckState.ConvertCheckState(CheckState.None);      // null
```

**IsChecked**
```csharp
CheckState state = CheckState.UnChecked;
bool? isChecked = state.IsChecked; // false
```

### ColorPicker Extensions
[ColorPickerExtensions.cs](/Extensions/ViewExtensions/ColorPickerExtensions.cs)

| Method | Wraps |
|--------|-------|
| `OnValueChanged(callback)` | `colorPicker.ValueChanged += ...` |
| `OnValueChanging(callback)` | `colorPicker.ValueChanging += ...` |

### DatePicker Extensions
[DatePickerExtensions.cs](/Extensions/ViewExtensions/DatePickerExtensions.cs)

| Method | Wraps |
|--------|-------|
| `OnValueChanged(callback)` | `datePicker.ValueChanged += ...` |
| `OnValueChanging(callback)` | `datePicker.ValueChanging += ...` |

### FlagSelector Extensions
[FlagSelectorExtensions.cs](/Extensions/ViewExtensions/FlagSelectorExtensions.cs)

**FlagSelector**

| Method | Wraps |
|--------|-------|
| `OnValueChanged(callback)` | `flagSelector.ValueChanged += ...` |
| `OnValueChanging(callback)` | `flagSelector.ValueChanging += ...` |
| `OnOrientationChanged(callback)` | `flagSelector.OrientationChanged += ...` |
| `OnOrientationChanging(callback)` | `flagSelector.OrientationChanging += ...` |

**FlagSelector\<TFlagsEnum\>**

| Method | Wraps |
|--------|-------|
| `OnValueChanged(callback)` | `flagSelector.ValueChanged += ...` |

### HexView Extensions
[HexViewExtensions.cs](/Extensions/ViewExtensions/HexViewExtensions.cs)

| Method | Wraps |
|--------|-------|
| `OnEdited(callback)` | `hexView.Edited += ...` |
| `OnPositionChanged(callback)` | `hexView.PositionChanged += ...` |

### Line Extensions
[LineExtensions.cs](/Extensions/ViewExtensions/LineExtensions.cs)

| Method | Wraps |
|--------|-------|
| `OnOrientationChanged(callback)` | `line.OrientationChanged += ...` |
| `OnOrientationChanging(callback)` | `line.OrientationChanging += ...` |

### ListView Extensions
[ListViewExtensions.cs](/Extensions/ViewExtensions/ListViewExtensions.cs)

**GetSelectedItem\<T\>(...)**

Returns the currently selected item from the source list, or `default` when nothing is selected.

```csharp
IList<string> items = ["Apple", "Banana", "Cherry"];
string? selected = listView.GetSelectedItem(items);
```

**WithScrollBars(...)**

```csharp
listView.WithScrollBars(vertical: true, horizontal: false);
```

| Method | Wraps |
|--------|-------|
| `OnValueChanged(callback)` | `listView.ValueChanged += ...` |
| `OnValueChanging(callback)` | `listView.ValueChanging += ...` |
| `OnCollectionChanged(callback)` | `listView.CollectionChanged += ...` |
| `OnSourceChanged(callback)` | `listView.SourceChanged += ...` |
| `OnRowRender(callback)` | `listView.RowRender += ...` |

### Menu Extensions
[MenuExtensions.cs](/Extensions/ViewExtensions/MenuExtensions.cs)

| Method | Wraps |
|--------|-------|
| `OnSelectedMenuItemChanged(callback)` | `menu.SelectedMenuItemChanged += ...` |
| `OnValueChanged(callback)` | `menu.ValueChanged += ...` |
| `OnValueChanging(callback)` | `menu.ValueChanging += ...` |

### MenuBar Extensions
[MenuBarExtensions.cs](/Extensions/ViewExtensions/MenuBarExtensions.cs)

| Method | Wraps |
|--------|-------|
| `OnKeyChanged(callback)` | `menuBar.KeyChanged += ...` |

### NumericUpDown Extensions
[NumericUpDownExtensions.cs](/Extensions/ViewExtensions/NumericUpDownExtensions.cs)

| Method | Wraps |
|--------|-------|
| `OnValueChanged(callback)` | `numericUpDown.ValueChanged += ...` |
| `OnValueChanging(callback)` | `numericUpDown.ValueChanging += ...` |
| `OnFormatChanged(callback)` | `numericUpDown.FormatChanged += ...` |
| `OnIncrementChanged(callback)` | `numericUpDown.IncrementChanged += ...` |

```csharp
numericUpDown.OnValueChanged(e => Console.WriteLine($"New: {e.NewValue}"));
numericUpDown.OnValueChanging(e => {
    if (e.NewValue < 0) e.Handled = true; // cancel negative values
});
```

### OptionSelector Extensions
[OptionSelectorExtensions.cs](/Extensions/ViewExtensions/OptionSelectorExtensions.cs)

**OptionSelector**

| Method | Wraps |
|--------|-------|
| `OnValueChanged(callback)` | `optionSelector.ValueChanged += ...` |
| `OnValueChanging(callback)` | `optionSelector.ValueChanging += ...` |
| `OnOrientationChanged(callback)` | `optionSelector.OrientationChanged += ...` |
| `OnOrientationChanging(callback)` | `optionSelector.OrientationChanging += ...` |

**OptionSelector\<TEnum\>**

| Method | Wraps |
|--------|-------|
| `OnValueChanged(callback)` | `optionSelector.ValueChanged += ...` |

### ScrollBar Extensions
[ScrollBarExtensions.cs](/Extensions/ViewExtensions/ScrollBarExtensions.cs)

| Method | Wraps |
|--------|-------|
| `OnOrientationChanged(callback)` | `scrollBar.OrientationChanged += ...` |
| `OnOrientationChanging(callback)` | `scrollBar.OrientationChanging += ...` |
| `OnScrollableContentSizeChanged(callback)` | `scrollBar.ScrollableContentSizeChanged += ...` |
| `OnScrolled(callback)` | `scrollBar.Scrolled += ...` |
| `OnSliderPositionChanged(callback)` | `scrollBar.SliderPositionChanged += ...` |
| `OnValueChanged(callback)` | `scrollBar.ValueChanged += ...` |
| `OnValueChanging(callback)` | `scrollBar.ValueChanging += ...` |

### ScrollSlider Extensions
[ScrollSliderExtensions.cs](/Extensions/ViewExtensions/ScrollSliderExtensions.cs)

| Method | Wraps |
|--------|-------|
| `OnOrientationChanged(callback)` | `scrollSlider.OrientationChanged += ...` |
| `OnOrientationChanging(callback)` | `scrollSlider.OrientationChanging += ...` |
| `OnPositionChanged(callback)` | `scrollSlider.PositionChanged += ...` |
| `OnPositionChanging(callback)` | `scrollSlider.PositionChanging += ...` |
| `OnScrolled(callback)` | `scrollSlider.Scrolled += ...` |

### PopoverMenu Extensions
[PopoverMenuExtensions.cs](/Extensions/ViewExtensions/PopoverMenuExtensions.cs)

| Method | Wraps |
|--------|-------|
| `OnKeyChanged(callback)` | `popoverMenu.KeyChanged += ...` |

### Shortcut Extensions
[ShortcutExtensions.cs](/Extensions/ViewExtensions/ShortcutExtensions.cs)

| Method | Wraps |
|--------|-------|
| `OnOrientationChanged(callback)` | `shortcut.OrientationChanged += ...` |
| `OnOrientationChanging(callback)` | `shortcut.OrientationChanging += ...` |

### TabView Extensions
[TabViewExtensions.cs](/Extensions/ViewExtensions/TabViewExtensions.cs)

| Method | Wraps |
|--------|-------|
| `OnSelectedTabChanged(callback)` | `tabView.SelectedTabChanged += ...` |
| `OnTabClicked(callback)` | `tabView.TabClicked += ...` |

### TextField Extensions
[TextFieldExtensions.cs](/Extensions/ViewExtensions/TextFieldExtensions.cs)

| Method | Wraps |
|--------|-------|
| `OnValueChanged(callback)` | `textField.ValueChanged += ...` |
| `OnValueChanging(callback)` | `textField.ValueChanging += ...` |
| `OnTextChanging(callback)` | `textField.TextChanging += ...` |

```csharp
textField.OnValueChanged(e => Console.WriteLine($"New: {e.NewValue}"));
textField.OnTextChanging(e => {
    if (e.Result?.Contains("bad") == true) e.Result = null; // cancel
});
```

### TableView Extensions
[TableViewExtensions.cs](/Extensions/ViewExtensions/TableViewExtensions.cs)

| Method | Wraps |
|--------|-------|
| `OnCellActivated(callback)` | `tableView.CellActivated += ...` |
| `OnCellToggled(callback)` | `tableView.CellToggled += ...` |
| `OnSelectedCellChanged(callback)` | `tableView.SelectedCellChanged += ...` |

### TextView Extensions
[TextViewExtensions.cs](/Extensions/ViewExtensions/TextViewExtensions.cs)

| Method | Wraps |
|--------|-------|
| `OnContentsChanged(callback)` | `textView.ContentsChanged += ...` |
| `OnDrawNormalColor(callback)` | `textView.DrawNormalColor += ...` |
| `OnDrawReadOnlyColor(callback)` | `textView.DrawReadOnlyColor += ...` |
| `OnDrawSelectionColor(callback)` | `textView.DrawSelectionColor += ...` |
| `OnDrawUsedColor(callback)` | `textView.DrawUsedColor += ...` |
| `OnUnwrappedCursorPositionChanged(callback)` | `textView.UnwrappedCursorPositionChanged += ...` |

### Wizard Extensions
[WizardExtensions.cs](/Extensions/ViewExtensions/WizardExtensions.cs)

| Method | Wraps |
|--------|-------|
| `OnMovingBack(callback)` | `wizard.MovingBack += ...` |
| `OnMovingNext(callback)` | `wizard.MovingNext += ...` |
| `OnStepChanged(callback)` | `wizard.StepChanged += ...` |
| `OnStepChanging(callback)` | `wizard.StepChanging += ...` |

## Custom Views

### NumericUpDownConstrained\<T\>
[NumericUpDownConstrained.cs](/Core/Views/NumericUpDownConstrained.cs)

A `NumericUpDown<T>` subclass with dynamically settable `Min` and `Max` constraints. The constraints are enforced via the `ValueChanging` event and update immediately when the properties change.

Setting `Min` or `Max` to `null` disables the corresponding constraint.

```csharp
var nud = new NumericUpDownConstrained<int> { Min = 0, Max = 100, Value = 50 };

// Constraints are dynamic — update them at any time:
nud.Max = 200;
nud.Min = null; // remove lower bound
```

The `AddNumericUpDown(...)` builder method creates a `NumericUpDownConstrained<T>` by default:

```csharp
viewBuilder.AddNumericUpDown(
    out NumericUpDownConstrained<double> nud,
    value: 10.0,
    step: 0.5,
    min: 0.0,
    max: 100.0);

// Later, adjust the constraint dynamically:
nud.Max = newCapacity;
```

## MessageBox Extensions
[MessageBoxExtensions.cs](/Extensions/MessageBoxExtensions.cs)

**static MessageBox.Confirm(...)**
```csharp
bool confirmed = MessageBox.Confirm(App, "Are you sure?");
bool confirmed2 = MessageBox.Confirm(App,
    message: "Proceed?",
    yesText: "Confirm",
    noText: "Cancel");
```

**static MessageBox.Error(...)**
```csharp
MessageBox.Error(App, "Something went wrong.");
MessageBox.Error(App,
    message: "Something went wrong.",
    title: "Error",
    okText: "Close");

// ...
catch (Exception e)
{
    MessageBox.Error(App, exception: e);
    MessageBox.Error(App,
        exception: e,
        title: e.GetType().Name);
}
```

**static MessageBox.Info(...)**
```csharp
MessageBox.Info(App, "Done.");
MessageBox.Info(App,
    message: "Saved.",
    okText: "Proceed");
```

## ApplicationExtensions
[ApplicationExtensions.cs](/Extensions/ApplicationExtensions.cs)

**Session Stack**

Convenience methods for inspecting the `IApplication.SessionStack`.

| Member | Description |
|--------|-------------|
| `SessionCount` | The number of active sessions. |
| `HasActiveSessions` | Whether the session stack has any active sessions. |
| `GetCurrentSession()` | Peeks at the top session without removing it. |
| `FindSession(IRunnable)` | Finds a session by its runnable. |

```csharp
public static Dialog? GetDialog()
    => Application.GetCurrentSession() is {} token
        ? token.Runnable as Dialog
        : null;

MessageBox.Info("Hello, World");
Dialog? messageBox = GetDialog();
```

```csharp
int count = app.SessionCount;
bool hasActive = app.HasActiveSessions;

var myRunnable = ...
SessionToken? found = app.FindSession(myRunnable);
```

**Event Wrappers**

| Method | Wraps |
|--------|-------|
| `OnInitializedChanged(callback)` | `app.InitializedChanged += ...` |
| `OnIteration(callback)` | `app.Iteration += ...` |
| `OnSessionBegun(callback)` | `app.SessionBegun += ...` |
| `OnSessionEnded(callback)` | `app.SessionEnded += ...` |
| `OnScreenChanged(callback)` | `app.ScreenChanged += ...` |

```csharp
app.OnSessionBegun(e => Console.WriteLine($"Session started: {e.State.Runnable}"));
app.OnScreenChanged(e => Console.WriteLine($"New size: {e.Value}"));
```

## ApplicationNavigationExtensions
[ApplicationNavigationExtensions.cs](/Extensions/ApplicationNavigationExtensions.cs)

**OnFocusChanged(...)**

wrapper for `navigation.FocusedChanged += (_, args) => ...`

```csharp
app.Navigation.OnFocusChanged(args => ...);
```
<br>DONT USE [WILL BE REMOVED]</br>
**NavigatesTo(...)**

Sets the targetView's Activating handler to navigate to the given runnable.

<details>
<summary>Example using instance</summary>

```csharp
ViewBuilder<MainWindow> windowBuilder = this.Builder();
windowBuilder.AddButton(out Button settingsBtn, "Go to settings Menu");

Window settings = new();
settings.Builder()
    .AddButton(out Button backButton, "Back to Main Menu");

backButton.OnActivating(_ => App.RequestStop());
app.Navigation.NavigatesTo<Window>(targetView: settingsBtn, runnableTo: settings, closeCurrent: false);
```
</details>

<details>
<summary>Example using class (creates fresh instance each navigation)</summary>

```csharp
public class SettingsMenu : Window
{
    public SettingsMenu()
    {
        this.Builder()
            .AddButton(out Button backButton, "Back to Main Menu");
        backButton.OnActivating(_ => App.RequestStop());
    }
}
public class MainWindow : Window
{
    public MainWindow(IApplication app)
    {
        ViewBuilder<MainWindow> windowBuilder = this.Builder();
        windowBuilder.AddButton(out Button settingsBtn, "Go to settings Menu");

        app.Navigation.NavigatesTo<SettingsMenu>(targetView: settingsBtn, closeCurrent: false);
    }
}
using IApplication app = Application.Create().Init();
app.Run(new MainWindow(app));
```
</details>

<details>
<summary>Example using factory (creates fresh instance each navigation)</summary>

```csharp
public class MainWindow : Window
{
    public MainWindow(IApplication app)
    {
        ViewBuilder<MainWindow> windowBuilder = this.Builder();
        windowBuilder.AddButton(out Button settingsBtn, "Go to settings Menu");
        app.Navigation.NavigatesTo(
            targetView: settingsBtn,
            runnableFactory: () => new SettingsMenu(),
            closeCurrent: false);
    }
}
```
</details>

## ApplicationPopoverExtensions
[ApplicationPopoverExtensions.cs](/Extensions/ApplicationPopoverExtensions.cs)

| Method | Wraps |
|--------|-------|
| `OnPopoverRegistered(callback)` | `popover.PopoverRegistered += ...` |
| `OnPopoverUnregistered(callback)` | `popover.PopoverDeRegistered += ...` |

```csharp
app.Popovers.OnPopoverRegistered(e => Console.WriteLine($"Registered: {e.Value}"));
```

## RunnableExtensions
[RunnableExtensions.cs](/Extensions/RunnableExtensions.cs)

| Method | Wraps |
|--------|-------|
| `OnRunningChanged(callback)` | `runnable.IsRunningChanged += ...` |
| `OnRunningChanging(callback)` | `runnable.IsRunningChanging += ...` |
| `OsModalChanged(callback)` | `runnable.IsModalChanged += ...` |

````csharp
window.OnIsRunningChanged(e => Console.WriteLine($"Running: {e.Value}"));
window.OnIsRunningChanging(e => {
    if (!canStop) e.Cancel = true;
});
````