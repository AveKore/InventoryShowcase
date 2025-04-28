using System;
using UnityEngine;

namespace SimpleEventBus
{
    public class EventWrapper : IEventWrapper
    {
        private readonly SimpleEvent _simpleEvent;
        public string EventKey { get; }
        
        public EventWrapper(string eventKey, SimpleEvent action)
        {
            EventKey = eventKey;
            _simpleEvent = action;
        }
        
        public void Execute()
        {
            if (!CheckIfMethodHasTarget())
            {
                Debug.LogError(
                        $"SimpleEventBus >>> Can't execute event with key {EventKey}! " +
                        $"Target for method {_simpleEvent.Method.Name} is null!");
                
                return;
            }
            
            _simpleEvent?.Invoke();
        }
        
        // todo check with statics 
        public bool CheckIfMethodHasTarget()
        {
            switch (_simpleEvent.Target)
            {
                case null:
                case MonoBehaviour monoBehaviour when !monoBehaviour:
                    return false;
                default:
                    return true;
            }
        }
        
        public override bool Equals(object obj)
        {
            if (obj is EventWrapper eventWrapper)
            {
                return Equals(eventWrapper);
            }
            
            return ReferenceEquals(this, obj);
        }
        
        private bool Equals(EventWrapper other)
        {
            return _simpleEvent.Method == other._simpleEvent.Method
                   && ReferenceEquals( _simpleEvent.Target, other._simpleEvent.Target);
        }
        
        public override int GetHashCode()
        {
            return HashCode.Combine(_simpleEvent, EventKey);
        }
    }
}