using System;
using System.Collections.Generic;

namespace SimpleEventBus
{
    public class SimpleEventBus : IEventBus
    {
        private readonly Dictionary<SubToken, EventRegNode> _nodesMap = new();
        private readonly Dictionary<Type, IEventContextSubKillRule> _killRules = new();
        
        public static SimpleEventBus CreateWithDefaultKillRules()
        {
            var simpleEventBus = new SimpleEventBus();
            simpleEventBus.AddKillRule<SceneEventContext>(new SceneEventContextSubKillRule());
            simpleEventBus.AddKillRule<GlobalEventContext>(new GlobalEventContextSubKillRule());
            return simpleEventBus;
        }

        public void AddKillRule<T>(IEventContextSubKillRule rule) where T : IEventContext
        {
            _killRules[typeof(T)] = rule;
        }
        
        public void KillSubs<T>() where T : IEventContext
        {
            KillSubs(typeof(T));
        }
        
        private void KillSubs(Type type) 
        {
            foreach (var (token, node) in _nodesMap)
            {
                if (node.ContextType == type)
                {
                    token.Kill();
                }
            }
        }
        
        public EventRegNodeBuilder RegisterEvent()
        {
            return new EventRegNodeBuilder(SubmitEvent);
        }
        
        private SubToken SubmitEvent(EventRegNode node)
        {
            var token = new SubToken(node.Listener);
            _nodesMap.Add(token, node);
            return token;
        }
        
        private void RemoveEventListener(SubToken token)
        {
            _nodesMap.Remove(token);
        }
        
        public void Fire(string eventKey)
        {
            ClearDeadSubs();
            
            foreach (var regNode in _nodesMap.Values)
            {
                if (regNode.IsKeyValid(eventKey))
                {
                    regNode.Listener.OnEventReceived(eventKey);
                }
            }
        }
        
        public void Fire<T>(string eventKey, T payload)
        {
            ClearDeadSubs();
            
            foreach (var regNode in _nodesMap.Values)
            {
                if (regNode.IsKeyValid(eventKey) && regNode.IsPayloadValid(payload))
                {
                    regNode.Listener.OnEventReceived(eventKey, payload);
                }
            }
        }
        
        private void CheckKillRules()
        {
            foreach (var (type, subKillRule) in _killRules)
            {
                if (subKillRule.IsNeedToKillSubs())
                {
                    KillSubs(type);
                }
            }
        }
        
        private void ClearDeadSubs()
        {
            CheckKillRules();
            
            var toRemove = new HashSet<SubToken>();
            
            foreach (var subToken in _nodesMap.Keys)
            {
                if (subToken.IsDead || subToken.HasNoTarget())
                {
                    toRemove.Add(subToken);
                }
            }
            
            foreach (var subToken in toRemove)
            {
                RemoveEventListener(subToken);
            }
        }
        
        public void Clear()
        {
            foreach (var subToken in _nodesMap.Keys)
            {
                subToken.Kill();
            }
            
            _nodesMap.Clear();
        }
    }
}