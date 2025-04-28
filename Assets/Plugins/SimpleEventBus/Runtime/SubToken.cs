using System;

namespace SimpleEventBus
{
    public class SubToken : IDisposable
    {
        private EventListener Listener { get; }
        
        public bool IsDead { get; set; }
        
        internal SubToken(EventListener listener)
        {
            Listener = listener;
        }
        
        public void Dispose()
        {
            if (IsDead)
            {
                return;
            }
            
            IsDead = true;
            Listener?.Dispose();
        }
        
        public void Kill()
        {
            Dispose();
        }
        
        public bool HasNoTarget()
        {
            return Listener.AllActionsHasNotTarget();
        }
    }
}