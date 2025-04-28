namespace SimpleEventBus
{
    public class GlobalEventContextSubKillRule : IEventContextSubKillRule
    {
        public bool IsNeedToKillSubs()
        {
            return false;
        }
    }
}