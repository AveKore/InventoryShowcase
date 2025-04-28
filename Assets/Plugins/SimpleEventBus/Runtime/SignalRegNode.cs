using System;

namespace SimpleEventBus
{
    public class EventRegNode
    {
        internal readonly EventListener Listener = new();
        internal Type ContextType = typeof(GlobalEventContext);
        private Predicate<string> KeyFilter { get; set; }
        private Predicate<object> PayloadFilter { get; set; }

        public bool IsKeyValid(string key)
        {
            return KeyFilter == null || KeyFilter.Invoke(key);
        }
        
        public bool IsPayloadValid(object payload)
        {
            return PayloadFilter == null || PayloadFilter.Invoke(payload);
        }
        
        public void SetKeyFilter(Predicate<string> keyFilter)
        {
            KeyFilter = keyFilter;
        }
        
        public void SetPayloadFilter(Predicate<object> payloadFilter)
        {
            PayloadFilter = payloadFilter;
        }
    }
}