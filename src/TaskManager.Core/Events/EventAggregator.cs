using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace TaskManager.Core.Events;

public class EventAggregator : IEventAggregator
{
    private readonly ConcurrentDictionary<Type, List<object>> _subscribers = new();

    public void Publish<TEvent>(TEvent eventToPublish) where TEvent : class
    {
        if (_subscribers.TryGetValue(typeof(TEvent), out var handlers))
        {
            var handlersCopy = handlers.ToList();
            foreach (var handler in handlersCopy)
            {
                if (handler is Action<TEvent> action)
                {
                    action(eventToPublish);
                }
            }
        }
    }

    public IDisposable Subscribe<TEvent>(Action<TEvent> action) where TEvent : class
    {
        var eventType = typeof(TEvent);
        var handlers = _subscribers.GetOrAdd(eventType, _ => new List<object>());
        
        lock (handlers)
        {
            handlers.Add(action);
        }

        return new Subscription(() =>
        {
            lock (handlers)
            {
                handlers.Remove(action);
            }
        });
    }

    private class Subscription : IDisposable
    {
        private readonly Action _unsubscribe;
        public Subscription(Action unsubscribe) => _unsubscribe = unsubscribe;
        public void Dispose() => _unsubscribe?.Invoke();
    }
}
