# TerminalGui.Extensions

## Table of contents

<!--TOC-->
  - [View Extensions](#view-extensions)
    - [CheckBox Extensions](#checkbox-extensions)
<!--/TOC-->

## View Extensions
[ViewBaseExtensions.cs](/Extensions/ViewExtensions/ViewBaseExtensions.cs)

**Builder()**

```
ViewBuilder builder = view.Builder();
builder.AddButton("Button Text");
```

**ConfigureWithBuilder(Action<ViewBuilder> callback)**

```
view.ConfigureWithBuilder((ViewBuilder viewBuilder) => 
{
    viewBuilder.AddButton("Button Text");
});

```

### CheckBox Extensions
[CheckBoxExtensions.cs](/Extensions/ViewExtensions/CheckBoxExtensions.cs)

**CheckedStateChanged(Action<EventArgs<CheckState>> callback)**

wrapper for `checkbox.CheckedStateChanged += (_, args) => ...`

```
checkbox.CheckedStateChanged(
    (EventArgs<CheckState> args) => ...
);
```

**OnCheckedStateChanging(Action<ResultEventArgs<CheckState>> callback)**

wrapper for `checkbox.OnCheckedStateChanging += (_, args) => ...`

```
checkbox.OnCheckedStateChanging(
    (ResultEventArgs<CheckState> args) => ...
);
```
