# TerminalGui.Extensions

# WORK IN PROGRESS

## Table of contents
*For more in depth documentation refer to the SourceCode listed under every TOC header*</br>
*Also refer to [Terminal.Gui Documentation](https://gui-cs.github.io/Terminal.Gui/)*

<!--TOC-->
  - [View Extensions](#view-extensions)
    - [CheckBox Extensions](#checkbox-extensions)
  - [ViewBuilder](#viewbuilder)
    - [MessageBox Extensions](#messagebox-extensions)
<!--/TOC-->

## View Extensions
[ViewBaseExtensions.cs](/Extensions/ViewExtensions/ViewBaseExtensions.cs)

**Builder()**

```csharp
ViewBuilder<Window> builder = view.Builder();
builder.AddButton("Button Text");
```

**ConfigureWithBuilder(...)**

```csharp
view.ConfigureWithBuilder(viewBuilder => 
{
    viewBuilder.AddButton("Button Text");
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

+ All `Add` methods return the added view. 
+ Parameters with `null` defaults retain their classes's initialization values.
Except for the `Text` parameter which default to `"{typeName} {parent.SubViews.Count}"` (e.g., `"Button 3"`) if not provided.

**NextPosY**

A property used for automatically positioning children when added via the ViewBuilder.</br>
This property should be a function that determines how to position a new child relative to the previously added child. </br>
The function takes the last added child as input and returns the Y position to use for the new child, or null to skip auto-positioning.

By default this is Pos.Bottom, which ensures that each new child is placed directly below the last added child.

**NextPosX**

Does the exact same as NextPosY, but for the X position. By default no auto-positioning is applied for X.

**GetLastChildAdded()**

```csharp
viewBuilder.AddButton("a button");
Button? button = viewBuilder.GetLastChildAdded() as Button;
```

**Add(...)**

```csharp
Button button = viewBuilder.Add(new Button(), btn => btn.Text = "Click me");

// You're also able to directly add ViewBuilder's
ViewBuilder<MainWindow> windowBuilder = this.Builder();

ViewBuilder<Menu> menuBuilder = new Menu().Builder();
Label label = menuBuilder.AddLabel("Menu 1");

Menu menu = windowBuilder.Add(menuBuilder);
```

**AddBar(...)**

```csharp
Bar bar = viewBuilder.AddBar(new());

Bar bar2 = viewBuilder.AddBar(
    shortcuts: [
        new Shortcut
        {
            Title = "Shortcut"
        }
    ],
    alignmentMode: AlignmentModes.EndToStart,
    orientation: Orientation.Vertical);
```

**AddButton(...)**

```csharp
Button button = viewBuilder.AddButton(
    new()
    {
        Text = "Button 1"
    });

Button button2 = viewBuilder.AddButton(
    text: "Button 2",
    isDefault: false,
    noDecorations: false,
    noPadding: false,
    hotKeySpecifier: new(';'))
```

**AddCheckBox(...)**

```csharp
CheckBox checkBox = viewBuilder.AddCheckBox(
            new()
            {
                Text = "Checkbox 1"
            });

CheckBox checkBox2 = viewBuilder.AddCheckBox(
    text: "Checkbox 2",
    checkedState: CheckState.Checked,
    allowCheckedStateNone: false, 
    hotKeySpecifier: null);
```

**AddRadioButton(...)**

The same as AddCheckBox but with [RadioStyle](https://gui-cs.github.io/Terminal.Gui/api/Terminal.Gui.Views.CheckBox.RadioStyle.html#Terminal_Gui_Views_CheckBox_RadioStyle) set to true.

Furthermore the `Text` parameter will default to `"RadioButton {parent.SubViews.Count}"` if not provided.

```csharp
CheckBox radioButton = viewBuilder.AddRadioButton(
    text: null, 
    checkedState: CheckState.Checked, 
    allowCheckedStateNone: false, 
    hotKeySpecifier: null);
```

**AddLabel(...)**

```csharp
Label label = viewBuilder.AddLabel(new());
Label label = viewBuilder.AddLabel("Text", hotKeySpecifier: null);
```

### MessageBox Extensions
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