using System.Collections.Generic;
using UnityEngine;

namespace SimpleEventBus
{
    internal class EventListener
    {
        private readonly Dictionary<string, IEventWrapper> _wrappers
                = new Dictionary<string, IEventWrapper>();
        
        
        public void OnEventReceived(string signalKey)
        {
            if (_wrappers.TryGetValue(signalKey, out var wrapper))
            {
                wrapper.Execute();
            }
        }
        
        public void OnEventReceived<T>(string signalKey, T payload)
        {
            if (_wrappers.TryGetValue(signalKey, out var wrapper)
                && wrapper is IEventWrapper<T> genericWrapper)
            {
                genericWrapper.Execute(payload);
            }
        }
        
        public EventListener BindEvent(string signalKey, SimpleEvent simpleEvent)
        {
            return BindEventInternal(signalKey, new EventWrapper(signalKey, simpleEvent));
        }
        
        public EventListener BindEvent<T>(string signalKey, SimpleEvent<T> signalHandler)
        {
            return BindEventInternal(signalKey, new GenericEventWrapper<T>(signalKey, signalHandler));
        }
        
        private EventListener BindEventInternal(string signalKey, IEventWrapper signalWrapper)
        {
            if (_wrappers.TryGetValue(signalKey, out var wrapper))
            {
                if (wrapper.Equals(signalWrapper))
                {
                    Debug.LogError($"SimpleEventBus >>> Duplicated subscription is prevented for event with key {signalKey}!");
                    return this;
                }
                
                Debug.LogError(
                        $"SimpleEventBus >>> Event with key {signalKey} already added! " +
                        $"Old event will be replaced with new one.");
            }
            
            _wrappers[signalKey] = signalWrapper;
            return this;
        }
        
        public void Dispose()
        {
            _wrappers.Clear();
        }
        
        public bool AllActionsHasNotTarget()
        {
            foreach (var action in _wrappers)
            {
                if (action.Value.CheckIfMethodHasTarget())
                {
                    return false;
                }
            }
            
            return true;
        }
    }
}