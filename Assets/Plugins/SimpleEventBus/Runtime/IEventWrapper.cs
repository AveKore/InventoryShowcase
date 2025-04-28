namespace SimpleEventBus
{
    public interface IEventWrapper 
    {
        string EventKey { get; }
        void Execute();
        bool CheckIfMethodHasTarget();
    }
    
    public interface IEventWrapper<in T> : IEventWrapper
    {
        void Execute(T payload);
    }
}