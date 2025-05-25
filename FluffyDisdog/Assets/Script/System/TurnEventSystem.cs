using System;
using System.Collections.Generic;
using Script.FluffyDisdog.Managers;
using UnityEngine;

namespace FluffyDisdog
{
    public enum TurnEvent
    {
        None=0,
        TurnStart,
        TurnEnd
    }

    public class TurnEventOptionParam
    {
        
    }

    public class TurnEventHandler
    {
        private event Action<TurnEventOptionParam> _delegates;

        public void AddEvent(Action<TurnEventOptionParam> del)
        {
            _delegates += del;
        }
        public void RemoveEvent(Action<TurnEventOptionParam> del)
        {
            _delegates -= del;
        }

        public void FireEvent(TurnEventOptionParam param=null)
        {
            _delegates?.Invoke(param);
        }

        public void ClearEvent()
        {
            _delegates = null;
        }
    }
    
    public class TurnEventSystem
    {
        
        private Dictionary<IEventAffectable, Dictionary<TurnEvent, TurnEventHandler>> _handler;

        public void Init()
        {
            _handler = new Dictionary<IEventAffectable, Dictionary<TurnEvent, TurnEventHandler>>();
        }

        public void AddEvent(TurnEvent EventType, Action<TurnEventOptionParam> cb, IEventAffectable unit)
        {
            if(!_handler.ContainsKey(unit))
                _handler.Add(unit, new Dictionary<TurnEvent, TurnEventHandler>());

            var dic = _handler[unit];
            if(!dic.ContainsKey(EventType))
                dic.Add(EventType, new TurnEventHandler());

            var handler = dic[EventType];
            handler.AddEvent(cb);
        }

        public void RemoveEvent(TurnEvent EventType, Action<TurnEventOptionParam> cb, IEventAffectable unit)
        {
            if (!_handler.ContainsKey(unit))
            {
                Debug.LogError("이벤트 등록된 적 없음!");
                return;
            }
            var dic = _handler[unit];
            if(!dic.ContainsKey(EventType))
            {
                Debug.LogError("이벤트 등록된 적 없음!");
                return;
            }
            var handler = dic[EventType];
            handler.RemoveEvent(cb);
        }

        public void FireEvent(TurnEvent eventType, TurnEventOptionParam param =null)
        {
            Stack<TurnEventHandler> handlers = new Stack<TurnEventHandler>();
            foreach (var dic in _handler.Values)
            {
                if (dic.TryGetValue(eventType, out var handler))
                {
                    handlers.Push(handler);
                }
            }

            while (handlers.Count >0)
            {
                handlers.Pop().FireEvent(param);
            }
        }

        public void RemoveAllEventAsType(IEventAffectable unit, TurnEvent EventType)
        {
            if (!_handler.ContainsKey(unit))
            {
                Debug.LogError("이벤트 등록된 적 없음!");
                return;
            }
            var dic = _handler[unit];
            if(!dic.ContainsKey(EventType))
            {
                Debug.LogError("이벤트 등록된 적 없음!");
                return;
            }
            dic[EventType].ClearEvent();
        }
    }
}