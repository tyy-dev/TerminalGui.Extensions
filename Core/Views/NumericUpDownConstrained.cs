using Terminal.Gui.App;
using Terminal.Gui.Views;

namespace TerminalGui.Extensions.Core.Views;

/// <summary>
///     A <see cref="NumericUpDown{T}" /> with settable <see cref="Min" /> and <see cref="Max" /> constraints
///     that are enforced dynamically via the <see cref="NumericUpDown{T}.ValueChanging" /> event.
/// </summary>
/// <typeparam name="T">The numeric type.</typeparam>
public class NumericUpDownConstrained<T> : NumericUpDown<T> where T : notnull
{
    private static readonly Comparer<T> _comparer = Comparer<T>.Default;

    private T? _max;
    private T? _min;
    private bool _hasMax;
    private bool _hasMin;

    /// <summary>
    ///     The maximum allowed value, or <see langword="null" /> if unconstrained.
    /// </summary>
    public T? Max
    {
        get => _max;
        set
        {
            _max = value;
            _hasMax = value is { };
        }
    }

    /// <summary>
    ///     The minimum allowed value, or <see langword="null" /> if unconstrained.
    /// </summary>
    public T? Min
    {
        get => _min;
        set
        {
            _min = value;
            _hasMin = value is { };
        }
    }

    /// <inheritdoc />
    public NumericUpDownConstrained() => ValueChanging += OnValueChanging;

    private void OnValueChanging(object? sender, ValueChangingEventArgs<T?> e)
    {
        if (e.NewValue is null)
        {
            return;
        }

        if (_hasMax && _comparer.Compare(e.NewValue, _max) > 0)
        {
            e.Handled = true;
            return;
        }

        if (_hasMin && _comparer.Compare(e.NewValue, _min) < 0)
        {
            e.Handled = true;
        }
    }
}
