# TerminalGui.Extensions

# WORK IN PROGRESS

## Table of contents
*For more in depth documentation refer to the SourceCode listed under every TOC header*</br>
*Also refer to [Terminal.Gui Documentation](https://gui-cs.github.io/Terminal.Gui/)*

<!--TOC-->
  - [View Extensions](#view-extensions)
    - [CheckBox Extensions](#checkbox-extensions)
  - [ViewBuilder](#viewbuilder)
    - [Notes](#notes)
  - [MessageBox Extensions](#messagebox-extensions)
  - [ApplicationNavigationExtensions](#applicationnavigationextensions)
<!--/TOC-->

## View Extensions
[ViewBaseExtensions.cs](/Extensions/ViewExtensions/ViewBaseExtensions.cs)

**Builder()**

```csharp
ViewBuilder<Window> builder = view.Builder();
builder.AddButton(out _, "Button Text");
```

**ConfigureWithBuilder(...)**

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

**OnAccepted(...)**

wrapper for `view.Accepted += (_, args) => ...`

```csharp
view.OnAccepted(args => ...);
```

**OnAccepting(...)**

wrapper for `view.Accepting += (_, args) => ...`

```csharp
view.OnAccepting(args => ...);
```

**OnActivating(...)**

wrapper for `view.Activating += (_, args) => ...`

```csharp
view.OnActivating(args => ...);
```

**OnHandlingHotKey(...)**

wrapper for `view.HandlingHotKey += (_, args) => ...`

```csharp
view.OnHandlingHotKey(args => ...);
```

**OnCommandNotBound(...)**

wrapper for `view.CommandNotBound += (_, args) => ...`

```csharp
view.OnCommandNotBound(args => ...);
```

### CheckBox Extensions
[CheckBoxExtensions.cs](/Extensions/ViewExtensions/CheckBoxExtensions.cs)

**IsChecked**
```csharp
bool isChecked = checkbox.IsChecked; // Equal to checkbox.CheckedState == CheckState.Checked;
```

**OnCheckedStateChanged(...)**

wrapper for `checkbox.CheckedStateChanged += (_, args) => ...`

```csharp
checkbox.OnCheckedStateChanged(args => ...);
```

**OnCheckedStateChanging(...)**

wrapper for `checkbox.CheckedStateChanging += (_, args) => ...`

```csharp
checkbox.OnCheckedStateChanging(args => ...);
```

## ViewBuilder

### Notes

+ All `Add` methods return the ViewBuilder instance for fluent chaining.
+ All `Add` methods return which aren't AddX(X instance) return the added view through an the first out parameter. 
+ Parameters with `null` defaults retain their classes's initialization values.
Except for the `Text` parameter which default to `"{typeName} {parent.SubViews.Count}"` (e.g., `"Button 3"`) if not provided.

**NextPosY**

A property used for automatically positioning children when added via the ViewBuilder.</br>
This property should be a function that determines how to position a new child relative to the previously added child. </br>
The function takes the last added child as input and returns the Y position to use for the new child, or null to skip auto-positioning.

By default this is Pos.Bottom, which ensures that each new child is placed directly below the last added child.

**NextPosX**

Does the exact same as NextPosY, but for the X position. By default no auto-positioning is applied for X.

**SkipAutoPositioning**

If true, skips any auto-positioning logic for any future added childam, unless it is set to false again.
It is suggested to set this property when using view.WithLayout if you do not want auto-positioning to interfere with the layouting.

**GetLastChildAdded()**

```csharp
viewBuilder.AddButton(out _, "a button");
Button? button = viewBuilder.GetLastChildAdded() as Button; // also in this case equal to AddButton(out Button button, ...);
```

**Add(...)**

```csharp
windowBuilder.Add(out Button button, new(), btn => btn.Text = "Click me"); // => ViewBuilder<Window>

// You're also able to directly add ViewBuilder's
ViewBuilder<Window> windowBuilder = this.Builder();

ViewBuilder<Menu> menuBuilder = new Menu().Builder();
menuBuilder.AddLabel(out Label label, "Menu 1"); // => ViewBuilder<Menu>

windowBuilder.Add(menuBuilder, out Menu Menu); // => ViewBuilder<Window>
```

**AddBar(...)**

```csharp
viewBuilder.AddBar(new()); // => ViewBuilder<Window>

viewBuilder.AddBar(
    out Bar bar,
    shortcuts: [
        new Shortcut
        {
            Title = "Shortcut"
        }
    ],
    alignmentMode: AlignmentModes.EndToStart,
    orientation: Orientation.Vertical); // => ViewBuilder<Window>
```

**AddButton(...)**

```csharp
viewBuilder.AddButton(
    new()
    {
        Text = "Button 1"
    }); // => ViewBuilder<Window>

viewBuilder.AddButton(
    out Button button,
    text: "Button 2",
    isDefault: false,
    noDecorations: false,
    noPadding: false,
    hotKeySpecifier: new(';')) / => ViewBuilder<Window>
```

**AddCheckBox(...)**

```csharp
viewBuilder.AddCheckBox(
    new()
    {
        Text = "Checkbox 1"
    }); // => ViewBuilder<Window>

viewBuilder.AddCheckBox(
    out CheckBox checkBox,
    text: "Checkbox 2",
    checkedState: CheckState.Checked,
    allowCheckedStateNone: false, 
    hotKeySpecifier: null); // => ViewBuilder<Window>
```

**AddRadioButton(...)**

The same as AddCheckBox but with [RadioStyle](https://gui-cs.github.io/Terminal.Gui/api/Terminal.Gui.Views.CheckBox.RadioStyle.html#Terminal_Gui_Views_CheckBox_RadioStyle) set to true.

Furthermore the `Text` parameter will default to `"RadioButton {parent.SubViews.Count}"` if not provided.

```csharp
viewBuilder.AddRadioButton(
    out CheckBox radioButton,
    text: null, 
    checkedState: CheckState.Checked, 
    allowCheckedStateNone: false, 
    hotKeySpecifier: null); // => ViewBuilder<Window>
```

**AddLabel(...)**

```csharp
viewBuilder.AddLabel(new()); // => ViewBuilder<Window>
viewBuilder.AddLabel(out Label label, "Text", hotKeySpecifier: null); // => ViewBuilder<Window>
```

## MessageBox Extensions
[MessageBoxExtensions.cs](/Extensions/ViewExtensions/MessageBoxExtensions.cs)

**MessageBox.Confirm(...)**
```csharp
bool confirmed = MessageBox.Confirm(App, "Are you sure?");
bool confirmed2 = MessageBox.Confirm(App,
    message: "Proceed?",
    yesText: "Confirm",
    noText: "Cancel");
```

**MessageBox.Error(...)**
```csharp
MessageBox.Error(App, "Something went wrong.");
MessageBox.Error(App,
    message: "Something went wrong.",
    title: "Error",
    okText: "Close");
```

**MessageBox.Info(...)**
```csharp
MessageBox.Info(App, "Done.");
MessageBox.Info(App,
    message: "Saved."
    okText: "Proceed");
```

## ApplicationNavigationExtensions
[ApplicationNavigationExtensions.cs](/Extensions/ApplicationNavigationExtensions.cs)

**NavigatesTo(...)**
Sets the targetView's Accepting handler to navigate to the given runnable.

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
        ViewBuilder windowBuilder = this.Builder();
        windowBuilder.AddButton(out Button settingsBtn, "Go to settings Menu");
        app.Navigation.NavigatesTo(
            targetView: settingsBtn, 
            runnableFactory: () => new SettingsMenu(), 
            closeCurrent: false);
    }
}
```
</details>

**OnFocusChanged(...)**

wrapper for `view.FocusedChanged += (_, args) => ...`

```csharp
view.OnFocusChanged(args => ...);
```