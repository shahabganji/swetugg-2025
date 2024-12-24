namespace CustomerManagementSystem.Domain.Fx;

public static class Maybe
{
    public static None None => None.Default;
    public static Maybe<T> Some<T>(T value) => new Some<T>(value);

}

public readonly struct Maybe<T> : IEquatable<None>, IEquatable<Maybe<T>>
{
    
    private readonly T _value;

    public T UnwrappedValue => this.IsNone
        ? throw new InvalidOperationException(
            $"{nameof(Unwrap)} without providing default value can only be called on " +
            $"Options with a not `None` values")
        : this._value;

    public bool IsSome { get; }
    public bool IsNone => !IsSome;

    private Maybe(T value)
    {
        _value = value;
        IsSome = true;
    }

    public static implicit operator Maybe<T>(T value)
        => value == null ? None.Default : Maybe.Some(value);

    public static implicit operator Maybe<T>(None _) => new();
    public static implicit operator Maybe<T>(Some<T> some) => new(some.Value);

    public TR Match<TR>(Func<TR> none, Func<T, TR> some)
        => IsSome ? some(_value) : none();

    public Task<TR> MatchAsync<TR>(Func<Task<TR>> noneAsync, Func<T, Task<TR>> someAsync)
        => IsSome ? someAsync(_value) : noneAsync();


    public Maybe<T> WhenNone(Action none)
    {
        if (this.IsNone) none();
        return this;
    }

    public async Task WhenNoneAsync(Func<Task> noneAsync)
    {
        if (this.IsNone)
            await noneAsync().ConfigureAwait(false);
    }

    public void WhenSome(Action<T> some)
    {
        if (this.IsSome) some(this._value);
    }

    public async Task WhenSomeAsync(Func<T, Task> someAsync)
    {
        if (this.IsSome)
            await someAsync(this._value).ConfigureAwait(false);
    }

    public IEnumerable<T> AsEnumerable()
    {
        if (this.IsSome)
            yield return this._value;
    }

    public T Unwrap(T defaultValue) => this.IsNone ? defaultValue : this._value;
    public T Unwrap(Func<T> defaultValueFunc) => this.IsNone ? defaultValueFunc() : this._value;

    public Task<T> UnwrapAsync(Func<Task<T>> defaultValueFuncAsync)
        => this.IsNone
            ? defaultValueFuncAsync()
            : Task.FromResult(this._value);


    public bool Equals(None other) => IsNone;

    public bool Equals(Maybe<T> other)
        => IsSome == other.IsSome && (IsNone || this._value!.Equals(other._value));

    public override bool Equals(object? obj)
    {
        return obj is Maybe<T> other && Equals(other);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            return (IsSome.GetHashCode() * 397) ^ EqualityComparer<T>.Default.GetHashCode(_value!);
        }
    }


    public override string ToString()
        => this.IsSome ? $"Some({_value})" : "None";

    #region Operators:

    public static bool operator ==(Maybe<T> @this, Maybe<T> other)
        => @this.Equals(other);

    public static bool operator !=(Maybe<T> @this, Maybe<T> other)
        => !(@this == other);

    public static bool operator ==(Maybe<T> @this, T other)
        => @this.Equals(other);

    public static bool operator !=(Maybe<T> @this, T other)
        => !(@this == other);

    public static bool operator ==(T other, Maybe<T> @this)
        => @this.Equals(other);

    public static bool operator !=(T other, Maybe<T> @this)
        => !(@this == other);

    #endregion
}
