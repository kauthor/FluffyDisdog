using System.Collections.Generic;
using UnityEngine;

namespace FluffyDisdog.UI
{
    public enum PopupType
    {
        NONE=-1,
        StageResult=0,
        CharacterSelect=1,
        DungeonSelect=2,
        Loading=3,
        Reward=4,
        DeckList=5,
        DeckSelect=6,
        CardPackResult=7,
        Option=8,
        GraveyardList=9,
        GameOver=10,
    }

    public class PopupManager:CustomSingleton<PopupManager>
    {
        private Stack<PopupMonoBehavior> popupStack;
        [SerializeField] private PopupMonoBehavior[] popups;
        private Dictionary<PopupType, PopupMonoBehavior> popupDic;

        protected override void Awake()
        {
            base.Awake();
            popupStack = new Stack<PopupMonoBehavior>();
            popupDic = new Dictionary<PopupType, PopupMonoBehavior>();

            for (int i = 0; i < popups.Length; i++)
            {
                var p = popups[i];
                Debug.Log($"[PopupInit] i={i} type={(p ? p.type.ToString() : "NULL")} ref={(p ? p.name : "MISSING/NULL")}");

                // 방어: null이면 여기서 바로 알 수 있음
                if (!p) Debug.LogError($"[PopupInit] popups[{i}] is NULL/MISSING in build");
                else //popupDic[p.type] = p;
                   popupDic.Add(popups[i].type, popups[i]);
            }
        }


        public PopupMonoBehavior GetPopup(PopupType type) 
        {
            if (popupDic.TryGetValue(type, out var pop))
            {
                Debug.Log(pop==null? "Null!!" : pop.ToString());
                var newPop = Instantiate(pop);
                popupStack.Push(newPop);
                newPop.gameObject.SetActive(false);
                newPop.transform.SetParent(transform);
                return newPop;
            }

            return null;
        }

        public void ClosePopup(PopupType type)
        {
            if (popupStack.Peek().type == type)
            {
                var pop = popupStack.Pop();
                Destroy(pop.gameObject);
            }
        }
    }
}