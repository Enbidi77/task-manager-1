using System;

namespace TaskManager.Core.Events;

public interface IEventAggregator
{
    void Publish<TEvent>(TEvent eventToPublish) where TEvent : class;
    IDisposable Subscribe<TEvent>(Action<TEvent> action) where TEvent : class;
}
