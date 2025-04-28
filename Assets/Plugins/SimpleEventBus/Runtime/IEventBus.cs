namespace SimpleEventBus
{
    public interface IEventBus
    {
        void Fire(string eventKey);
        void Fire<T>(string eventKey, T payload);
    }
}