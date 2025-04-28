using System;

namespace SimpleEventBus
{
    public class EventRegNodeBuilder
    {
        private readonly EventRegNode _eventRegNode;
        private readonly Func<EventRegNode, SubToken> _onSubmit;
        
        public EventRegNodeBuilder(Func<EventRegNode, SubToken> onSubmit)
        {
            _onSubmit = onSubmit;
            _eventRegNode = new EventRegNode();
        }
        
        public EventRegNodeBuilder WithContext<T>() where T : IEventContext
        {
            _eventRegNode.ContextType = typeof(T);
            return this;
        }
        
        public EventRegNodeBuilder Bind(string eventKey, SimpleEvent simpleEvent)
        {
            _eventRegNode.Listener.BindEvent(eventKey, simpleEvent);
            return this;
        }
        
        public EventRegNodeBuilder Bind<T>(string eventKey, SimpleEvent<T> simpleEvent)
        {
            _eventRegNode.Listener.BindEvent(eventKey, simpleEvent);
            return this;
        }
        
        public EventRegNodeBuilder WithKeyFilter(Predicate<string> keyFilter)
        {
            _eventRegNode.SetKeyFilter(keyFilter);
            return this;
        }
        
        public EventRegNodeBuilder WithPayloadFilter(Predicate<object> payloadFilter)
        {
            _eventRegNode.SetPayloadFilter(payloadFilter);
            return this;
        }
        
        public SubToken Submit()
        {
            return _onSubmit?.Invoke(_eventRegNode);
        }
    }
}