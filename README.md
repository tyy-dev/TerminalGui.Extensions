# TerminalGui.Extensions

# WORK IN PROGRESS

## Table of contents
*For more in depth documentation refer to the SourceCode listed under every TOC header*</br>
*Also refer to [Terminal.Gui Documentation](https://gui-cs.github.io/Terminal.Gui/)*

<!-- TOC-->
  - [View Extensions](#view-extensions)
    - [CheckBox Extensions](#checkbox-extensions)
      - [CheckState Extensions](#checkstate-extensions)
    - [ListView Extensions](#listview-extensions)
    - [NumericUpDown Extensions](#numericupdown-extensions)
    - [OptionSelector Extensions](#optionselector-extensions)
    - [TabView Extensions](#tabview-extensions)
    - [TextField Extensions](#textfield-extensions)
  - [Custom Views](#custom-views)
    - [NumericUpDownConstrained\<T\>](#numericupdownconstrainedt)
  - [ViewBuilder](#viewbuilder)
    - [Notes](#notes)
    - [Properties](#properties)
    - [Core Methods](#core-methods)
    - [Add Methods](#add-methods)
  - [MessageBox Extensions](#messagebox-extensions)
  - [ApplicationNavigationExtensions](#applicationnavigationextensions)
<!-- TOC -->

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

**MakeScrollable()**

Configures the view as scrollable content with a vertical scroll bar. Automatically tracks the content size based on subview positions.

```csharp
view.MakeScrollable();
```

**Command Event Wrappers**

| Method | Wraps |
|--------|-------|
| `OnAccepted(callback)` | `view.Accepted += ...` |
| `OnAccepting(callback)` | `view.Accepting += ...` |
| `OnActivating(callback)` | `view.Activating += ...` |
| `OnHandlingHotKey(callback)` | `view.HandlingHotKey += ...` |
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

**Focus Event Wrappers**

| Method | Wraps |
|--------|-------|
| `OnHasFocusChanged(callback)` | `view.HasFocusChanged += ...` |
| `OnFocusedChanged(callback)` | `view.FocusedChanged += ...` |

**Lifecycle Event Wrappers**

| Method | Wraps |
|--------|-------|
| `OnInitialized(callback)` | `view.Initialized += ...` |
| `OnDisposing(callback)` | `view.Disposing += ...` |

**State Change Event Wrappers**

| Method | Wraps |
|--------|-------|
| `OnVisibleChanged(callback)` | `view.VisibleChanged += ...` |
| `OnEnabledChanged(callback)` | `view.EnabledChanged += ...` |
| `OnTextChanged(callback)` | `view.TextChanged += ...` |
| `OnTitleChanged(callback)` | `view.TitleChanged += ...` |

**Layout Event Wrappers**

| Method | Wraps |
|--------|-------|
| `OnSubViewsLaidOut(callback)` | `view.SubViewsLaidOut += ...` |
| `OnFrameChanged(callback)` | `view.FrameChanged += ...` |

**Drawing Event Wrappers**

| Method | Wraps |
|--------|-------|
| `OnDrawComplete(callback)` | `view.DrawComplete += ...` |
| `OnDrawingContent(callback)` | `view.DrawingContent += ...` |

### CheckBox Extensions
[CheckBoxExtensions.cs](/Extensions/ViewExtensions/CheckBoxExtensions.cs)

**IsChecked**
```csharp
bool isChecked = checkbox.IsChecked; // Equal to checkbox.CheckedState == CheckState.Checked;
checkbox.IsChecked = true; // Equal to checkbox.CheckedState = CheckState.Checked;
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
| `OnCollectionChanged(callback)` | `listView.CollectionChanged += ...` |
| `OnSourceChanged(callback)` | `listView.SourceChanged += ...` |

### NumericUpDown Extensions
[NumericUpDownExtensions.cs](/Extensions/ViewExtensions/NumericUpDownExtensions.cs)

| Method | Wraps |
|--------|-------|
| `OnValueChanged(callback)` | `numericUpDown.ValueChanged += ...` |
| `OnValueChanging(callback)` | `numericUpDown.ValueChanging += ...` |

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

**OptionSelector\<TEnum\>**

| Method | Wraps |
|--------|-------|
| `OnValueChanged(callback)` | `optionSelector.ValueChanged += ...` |

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
| `OnTextChanging(callback)` | `textField.TextChanging += ...` |

```csharp
textField.OnValueChanged(e => Console.WriteLine($"New: {e.NewValue}"));
textField.OnTextChanging(e => {
    if (e.Result?.Contains("bad") == true) e.Result = null; // cancel
});
```

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

## ViewBuilder
[ViewBuilder.cs](/Core/Builders/ViewBuilder.cs)

### Notes

+ All `Add` methods return the `ViewBuilder` instance for fluent chaining.
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
| `AddStatusBar` | `StatusBar` | `shortcuts`, `orientation`, `alignmentModes` |
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

## ApplicationNavigationExtensions
[ApplicationNavigationExtensions.cs](/Extensions/ApplicationNavigationExtensions.cs)

**OnFocusChanged(...)**

wrapper for `navigation.FocusedChanged += (_, args) => ...`

```csharp
app.Navigation.OnFocusChanged(args => ...);
```

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