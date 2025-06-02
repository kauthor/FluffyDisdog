using System;
using System.Collections.Generic;
using System.Linq;
using FluffyDisdog.Data.RelicData;
using Script.FluffyDisdog.Managers;
using UnityEngine;

namespace FluffyDisdog
{
    public enum TurnEvent
    {
        None=0,   
        TurnStart,      //턴시작
        TurnEnd,        //턴끝
        TileDigged,     //타일 파괴시
        GameStart,      //게임 시작시
        GameEnd,        //게임 끝날시
        ToolConsumed,    //도구 소진 시
        EndCrack,     //도구 효과처리 끝날 시.
        ToolConsumeDesire, //도구 소진 판정
        TileClicked,    //타일 클릭시
        DistanceDesire, //클릭 위치와의 거리에 의한 확률계산시
        Draw,             //드로우
        ToolCalculateStart, //도구별 타일 계산 시작
        DigFail            //타일 파괴 실패시
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

        private List<RelicCommandData.RelicCommandData> commandList;

        public void Init()
        {
            _handler = new Dictionary<IEventAffectable, Dictionary<TurnEvent, TurnEventHandler>>();
            commandList = new List<RelicCommandData.RelicCommandData>();
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

            if (unit is RelicCommandData.RelicCommandData data)
            {
                commandList.Add(data);
            }
        }

        public bool HasRelicCommand(RelicName name)
        {
            return commandList.Exists(_ => _.relicType == name);
        }

        public void RemoveEvent(IEventAffectable unit)
        {
            if (!_handler.ContainsKey(unit))
            {
                Debug.LogError("이벤트 등록된 적 없음!");
                return;
            } 
            _handler[unit].Clear();
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