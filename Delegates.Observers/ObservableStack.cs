using System;
using System.Collections.Generic;
using System.Text;

namespace Delegates.Observers
{
    public class StackOperationsLogger
    {
        private readonly Observer _observer = new Observer();

        public void SubscribeOn<T>(ObservableStack<T> stack)
        {
            stack.Add(_observer);
        }

        public string GetLog()
        {
            return _observer.Log.ToString();
        }
    }

    public class Observer
    {
        public readonly StringBuilder Log = new StringBuilder();
    }

    public class ObservableStack<T>
    {
        public event Action<string> PushNotify =
            data => _observers.ForEach(observer => observer.Log.Append(data));

        public event Action<string> PopNotify =
            data => _observers.ForEach(observer => observer.Log.Append(data));

        private static readonly List<Observer> _observers = new List<Observer>();

        public void Add(Observer observer)
        {
            _observers.Add(observer);
        }

        public void Remove(Observer observer)
        {
            _observers.Remove(observer);
        }

        private readonly List<T> _data = new List<T>();

        public void Push(T obj)
        {
            _data.Add(obj);
            PushNotify?.Invoke("+" + obj);
        }

        public T Pop()
        {
            if (_data.Count == 0)
                throw new InvalidOperationException();
            var result = _data[_data.Count - 1];
            PopNotify?.Invoke("-" + result);
            return result;
        }
    }
}
