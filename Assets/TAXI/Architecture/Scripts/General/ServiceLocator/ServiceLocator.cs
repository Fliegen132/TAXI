using System;
using System.Collections.Generic;
using UnityEngine;

public class ServiceLocator
{
    private Dictionary<string, IService> _services = new Dictionary<string, IService>();
    public static ServiceLocator current;

    public ServiceLocator()
    {
        if (current == null)
            current = this;
        else
            return;
    }

    public void Register<T>(T service) where T : IService
    {
        string key = typeof(T).Name;
        if (_services.ContainsKey(key))
        {
            Debug.LogError(key + " was register");
            return;
        }
        _services.Add(key, service);
    }
    public void Unregister<T>(T service) where T : IService
    {
        string key = typeof(T).Name;
        if (!_services.ContainsKey(key))
        {
            Debug.LogError(key + " was not register. Can't remove");
            return;
        }
        _services.Remove(key);
    }
    public T Get<T>() where T : IService
    {
        string key = typeof(T).Name;
        if (!_services.ContainsKey(key))
        {
            Debug.Log(key + " was not register. Can't get");
            throw new InvalidOperationException();
        }

        return (T)_services[key];
    }

    public void UnregisterAll()
    {
        _services.Clear();
    }
}
