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
    

    public class TurnEventHandler
    {
        private event Action _delegates;

        public void AddEvent(Action del)
        {
            _delegates += del;
        }
        public void RemoveEvent(Action del)
        {
            _delegates -= del;
        }

        public void FireEvent()
        {
            _delegates?.Invoke();
        }

        public void ClearEvent()
        {
            _delegates = null;
        }
    }
    
    public class TurnEventSystem
    {
        
        private Dictionary<TerrainNode, Dictionary<TurnEvent, TurnEventHandler>> _handler;

        public void Init()
        {
            _handler = new Dictionary<TerrainNode, Dictionary<TurnEvent, TurnEventHandler>>();
        }

        public void AddEvent(TurnEvent EventType, Action cb, TerrainNode unit)
        {
            if(!_handler.ContainsKey(unit))
                _handler.Add(unit, new Dictionary<TurnEvent, TurnEventHandler>());

            var dic = _handler[unit];
            if(!dic.ContainsKey(EventType))
                dic.Add(EventType, new TurnEventHandler());

            var handler = dic[EventType];
            handler.AddEvent(cb);
        }

        public void RemoveEvent(TurnEvent EventType, Action cb, TerrainNode unit)
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

        public void FireEvent(TurnEvent eventType)
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
                handlers.Pop().FireEvent();
            }
        }

        public void RemoveAllEventAsType(TerrainNode unit, TurnEvent EventType)
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