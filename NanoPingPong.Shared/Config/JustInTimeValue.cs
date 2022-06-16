using System;

namespace NanoPingPong.Shared.Config
{
    internal class JustInTimeValue<T>
    {

        private T _value;

        private bool _loaded = false;
        private readonly object _locker = new();
        public T GetValue()
        {
            if (!_loaded)
            {
                lock (_locker)
                {
                    if (!_loaded)
                    {
                        _value = _getter();
                        _loaded = true;
                    }
                }
            }
            return _value;
        }

        private readonly Func<T> _getter;
        public JustInTimeValue(Func<T> getter)
        {
            _getter = getter;
        }

    }
}
