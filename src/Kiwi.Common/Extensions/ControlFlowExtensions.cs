using System;

namespace Kiwi.Common.Extensions
{
    public static class Select<TResult>
    {
        public static SelectResult<T> Case<T>(Func<T> getter)
        {
            var result = getter();
            return new SelectResult<T>(result);
        }

        public class SelectResult<T>
        {
            private T _current;
            private bool _isMatch;
            private TResult _result;
            private bool _resultSet;

            public SelectResult(T current)
            {
                _current = current;
            }

            public SelectResult<T> Match(Func<T, bool> matcher)
            {
                if (!_isMatch)
                {
                    _isMatch = matcher(_current);
                }

                return this;
            }

            public SelectResult<T> Case(Func<T> getter)
            {
                if (!_isMatch)
                {
                    _current = getter();
                }

                return this;
            }

            public SelectResult<T> Do(Func<T, TResult> action)
            {
                if (_isMatch && !_resultSet)
                {
                    _resultSet = true;
                    _result = action(_current);
                }
                return this;
            }

            public TResult Done()
            {
                return _result;
            }

            public SelectResult<T> Else(Action action)
            {
                if (!_isMatch)
                {
                    action();
                }
                return this;
            }
        }
    }

    public static class ControlFlowExtensions
    {
        public static Switch<TSource> TypeSwitch<TSource>(this TSource value)
        {
            return new Switch<TSource>(value);
        }

        public sealed class Switch<TSource>
        {
            private readonly TSource _value;
            private bool _handled;

            internal Switch(TSource value)
            {
                _value = value;
            }

            public Switch<TSource> Case<TTarget>(Action<TTarget> action)
                where TTarget : TSource
            {
                if (!_handled && _value is TTarget)
                {
                    action((TTarget)_value);
                    _handled = true;
                }
                return this;
            }

            public void Default(Action<TSource> action)
            {
                if (!_handled)
                    action(_value);
            }

            public void Default(Action action)
            {
                if (!_handled)
                    action();
            }
        }

        public static SwitchExpression<TSource, TResult> TypeSwitchExpression<TSource, TResult>(this TSource value)
        {
            return new SwitchExpression<TSource, TResult>(value);
        }

        public sealed class SwitchExpression<TSource, TResult>
        {
            private readonly TSource _value;
            private bool _handled;
            private TResult _result;

            internal SwitchExpression(TSource value)
            {
                _value = value;
            }

            public SwitchExpression<TSource, TResult> Case<TTarget>(Func<TTarget, TResult> function)
                where TTarget : TSource
            {
                if (!_handled && _value is TTarget)
                {
                    _result = function((TTarget)_value);
                    _handled = true;
                }
                return this;
            }

            public SwitchExpression<TSource, TResult> Default(Func<TSource, TResult> function)
            {
                if (!_handled)
                {
                    _result = function(_value);
                    _handled = true;
                }
                return this;
            }

            public SwitchExpression<TSource, TResult> Default(Action<TSource> action)
            {
                if (!_handled)
                {
                    action(_value);
                    _handled = true;
                }
                return this;
            }

            public SwitchExpression<TSource, TResult> Default(Action action)
            {
                if (!_handled)
                {
                    action();
                    _handled = true;
                }
                return this;
            }

            public TResult Done()
            {
                return _result;
            }
        }
    }
}