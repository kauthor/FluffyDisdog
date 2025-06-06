﻿using System;
using System.Linq;
using Script.FluffyDisdog.Managers;
using Sirenix.Utilities;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace FluffyDisdog.UI
{
    public class UIStageRewardPopup : PopupMonoBehavior
    {
        public override PopupType type => PopupType.Reward;

        [SerializeField] private CardPopupParts[] cards;
        [SerializeField] private Button btnSelectRemoveFromDeck;

        [SerializeField] private Button btnReroll;

        private Action OnClosedCb;

        public static void OpenPopup(Action onClosed)
        {
            var pop = PopupManager.I.GetPopup(PopupType.Reward);
            if (pop is UIStageRewardPopup re)
            {
                re.gameObject.SetActive(true);
                re.Init(onClosed);
            }
        }

        protected override void Awake()
        {
            base.Awake();
            btnReroll.onClick.RemoveAllListeners();
            btnReroll.onClick.AddListener(() =>
            {
                for (int i = 0; i < cards.Length ; i++)
                {
                    cards[i].Init((ToolType) (SeedManager.I.GetStoreSeed() %2), 0);
                    cards[i].BindHandler((a, b) =>
                    {
                        OnCardClicked(a);
                    });
                }
                //cards[^1].SetCardData(DeckManager.I.GetRandomCardFromDeck());
            });
            
            btnSelectRemoveFromDeck.onClick.RemoveAllListeners();
            btnSelectRemoveFromDeck.onClick.AddListener(() =>
            {
                UIDeckSelectPopup.OpenPopup(_ =>
                {
                    DeckManager.I.RemoveCard(_);
                    Close();
                });
            });
        }

        private void Init(Action onclosed)
        {
            for (int i = 0; i < cards.Length ; i++)
            {
                cards[i].Init( (ToolType) (SeedManager.I.GetStoreSeed() %2), 0);
                cards[i].BindHandler((a, b) =>
                {
                    OnCardClicked(a);
                });
            }
            //cards[^1].InitAsReroll(OnRerollCardClicked, DeckManager.I.GetRandomCardFromDeck());
            OnClosedCb = onclosed;
        }

        protected override void OnCloseClick()
        {
            base.OnCloseClick();
            OnClosedCb?.Invoke();
        }

        private void OnCardClicked(ToolType tool)
        {
            DeckManager.I.TryAddDeck(tool);
            Close();
        }

        private void OnRerollCardClicked(ToolType tool)
        {
            DeckManager.I.RemoveCard(tool);
            Close();
        }
    }
}