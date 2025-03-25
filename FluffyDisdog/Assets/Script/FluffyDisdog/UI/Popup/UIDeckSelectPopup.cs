using System;
using Script.FluffyDisdog.Managers;
using UnityEngine;
using UnityEngine.UI;

namespace FluffyDisdog.UI
{
    public class UIDeckSelectPopup:PopupMonoBehavior
    {
        public override PopupType type => PopupType.DeckSelect;

        [SerializeField] private CardPopupParts cardPrefab;

        [SerializeField] private Transform cardParent;

        [SerializeField] private Button btnDecide;

        private Action<ToolType> OnCardDecided;


        private ToolType currentSelected;

        private CardPopupParts currentSelectedButton;


        public static void OpenPopup(Action<ToolType> cb)
        {
            var pop = PopupManager.I.GetPopup(PopupType.DeckSelect);
            if (pop is UIDeckSelectPopup stg)
            {
                stg.Init(cb);
                stg.gameObject.SetActive(true);
            }
        }
        
        protected override void Awake()
        {
            base.Awake();
            btnDecide.onClick.RemoveAllListeners();
            btnDecide.onClick.AddListener(() =>
            {
                Close();
                OnCardDecided?.Invoke(currentSelected);
            });
        }

        private void Init(Action<ToolType> toolCb)
        {
            var deck = DeckManager.I.GetDeckList();
            currentSelected = ToolType.None;
            currentSelectedButton = null;

            foreach (var pair in deck)
            {
                var newSlot = GameObject.Instantiate(cardPrefab, cardParent);
                newSlot.Init(pair.Key, pair.Value);
                newSlot.BindHandler(OnCardSelectedCB);
            }

            OnCardDecided = toolCb;
        }

        private void OnCardSelectedCB(ToolType tool, CardPopupParts part)
        {
            currentSelected = tool;
            if (currentSelectedButton != null)
            {
                currentSelectedButton.Unselect();
            }
            currentSelectedButton = part;
        }
    }
}