﻿using System.Collections.Concurrent;
using Slithin.Modules.Events.Models;

namespace Slithin.Modules.Events;

internal class EventServiceImpl : IEventService
{
    private readonly ConcurrentDictionary<string, Action<object>> _subscriptions = new();

    public void Invoke<T>(string name, T argument)
    {
        if (_subscriptions.ContainsKey(name))
        {
            _subscriptions[name].Invoke(argument);
        }
    }

    public void Subscribe<T>(string name, Action<T> action)
    {
        if (!_subscriptions.ContainsKey(name))
        {
            _subscriptions.TryAdd(name, _ => action((T)_));
            return;
        }

        var oldSubscriptions = _subscriptions[name];
        _subscriptions[name] = (Action<object>)Delegate.Combine(oldSubscriptions, action);
    }
}