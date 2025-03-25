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
        DeckSelect=6
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
                popupDic.Add(popups[i].type, popups[i]);
            }
        }


        public PopupMonoBehavior GetPopup(PopupType type) 
        {
            if (popupDic.TryGetValue(type, out var pop))
            {
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