using System;

namespace SimpleEventBus
{
    public class GenericEventWrapper<T> : IEventWrapper<T>
    {
        public string EventKey { get; }
        private readonly SimpleEvent<T> _simpleEvent;
        
        public GenericEventWrapper(string eventKey, SimpleEvent<T> action)
        {
            EventKey = eventKey;
            _simpleEvent = action;
        }
        
        public void Execute()
        {
            Execute(default);
        }
        
        public void Execute(T payload)
        {
            _simpleEvent?.Invoke(payload);
        }
        
        public bool CheckIfMethodHasTarget()
        {
            return _simpleEvent.Target != null;
        }
        
        public override bool Equals(object obj)
        {
            if (obj is GenericEventWrapper<T> eventWrapper)
            {
                return Equals(eventWrapper);
            }
            
            return ReferenceEquals(this, obj);
        }
        
        private bool Equals(GenericEventWrapper<T> other)
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