namespace SimpleEventBus
{
    public delegate void SimpleEvent<in T>(T payload);
    
    public delegate void SimpleEvent();
}