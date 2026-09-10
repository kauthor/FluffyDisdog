using System;
using System.Collections.Generic;
using FluffyDisdog.Data.RelicData;
using FluffyDisdog.UI.Part;
using Script.FluffyDisdog.Managers;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace FluffyDisdog.UI
{
    public class UiRequestResultPopup:PopupMonoBehavior
    {
        public override PopupType type => PopupType.Request;

        [SerializeField] private Transform selectableTr;
        [SerializeField] private CardPopupParts cardPrefab;
        [SerializeField] private UIRequestPackPart packPrefab;
        [SerializeField] private RelicSlotTreasure relicPrefab;
        [SerializeField] private GoldSlotTreasure goldPrefab;
        [SerializeField] private RequestUpgradePart upgradePrefab;

        [FormerlySerializedAs("btnSkipAndNext")] 
        [SerializeField] private Button btnSkip;
        [SerializeField] private Button btnNext;
        
        private ISelectableUiPart[] runtimeRequestParts;
        private List<ISelectableUiPart> currentSelected;
        
        [SerializeField] private Button pnlSkipArea;
        [SerializeField] private Button pnlNextArea;
        //[SerializeField] private Sprite[] skipOrNextSprite;
        [SerializeField] private OutlinedText cardSelectTextSkip;
        [SerializeField] private OutlinedText cardSelectTextNext;
        [SerializeField] private GameObject skipLine;
        [SerializeField] private GameObject nextLine;

        protected override void Awake()
        {
            base.Awake();
            btnSkip.onClick.RemoveAllListeners();
            btnSkip.onClick.AddListener(() =>
            {
                
                Close();
            });
            btnNext.onClick.RemoveAllListeners();
            btnNext.onClick.AddListener(() =>
            {
                
                Close();
            });
        }
        
        public static PopupMonoBehavior OpenPopup(List<RequestReward> dataList)
        {
            var pop = PopupManager.I.GetPopup(PopupType.Request);
            if (pop is UiRequestResultPopup res)
            {
                res.Init(dataList);
                res.gameObject.SetActive(true);
                //return pop;
            }

            return pop;
        }

        private void Init(List<RequestReward> dataList)
        {
            currentSelected = new List<ISelectableUiPart>();
            cardSelectTextSkip.SetText($"(0/{3})");
            cardSelectTextNext.SetText($"(0/{3})");
            
            runtimeRequestParts = new ISelectableUiPart[dataList.Count];

            int temp = 0;
            foreach (var item in dataList)
            {
                ISelectableUiPart part = null;
                switch (item.Type)
                {
                    case RequestRewardType.Gold:
                        var n = GameObject.Instantiate(goldPrefab,selectableTr);
                        n.InitAsSelectable(Indicator);
                        n.Init(item.count, OnGoldClick);
                        part = n;
                        break;
                    case RequestRewardType.Card:
                        var c = GameObject.Instantiate(cardPrefab,selectableTr);
                        c.Init((ToolType)item.value, 0);
                        c.BindHandler(OnCardClicked);
                        c.InitAsSelectable(Indicator);
                        part = c;
                        break;
                    case RequestRewardType.Relic:
                        var r = GameObject.Instantiate(relicPrefab,selectableTr);
                        r.InitAsSelectable(Indicator);
                        r.Init((RelicName)item.value, OnRelicClick);
                        part = r;
                        break;
                    case RequestRewardType.Gacha:
                        var g = GameObject.Instantiate(packPrefab,selectableTr);
                        g.Init(item.value, Indicator, null, OnPackClicked);
                        part = g;
                        break;
                    case RequestRewardType.RemoveOrUpgrade:
                    default:
                        var u = GameObject.Instantiate(upgradePrefab,selectableTr);
                        u.Init(Indicator, null, OnUpgradeClicked);
                        part = u;
                        break;
                }
                runtimeRequestParts[temp] = part;
                temp++;
            }
        }

        private bool Indicator()
        {
            return currentSelected != null && currentSelected.Count < 3;
        }
        private void OnCardClicked(ToolType t, CardPopupParts c)
        {
            if (c.Selected)
            {
                if(currentSelected.Count < 3)
                    currentSelected.Add(c);
            }
            else
            {
                currentSelected.Remove(c);
            }
            
            cardSelectTextSkip.SetText($"({currentSelected.Count}/{3})");
            cardSelectTextNext.SetText($"({currentSelected.Count}/{3})");
            bool activeSkip = currentSelected.Count <= 0;
            pnlSkipArea.gameObject.SetActive(activeSkip);
            pnlNextArea.gameObject.SetActive(!activeSkip);
        }
        
        private void OnUpgradeClicked(RequestUpgradePart c)
        {
            if (c.Selected)
            {
                if(currentSelected.Count < 3)
                    currentSelected.Add(c);
            }
            else
            {
                currentSelected.Remove(c);
            }
            
            cardSelectTextSkip.SetText($"({currentSelected.Count}/{3})");
            cardSelectTextNext.SetText($"({currentSelected.Count}/{3})");
            bool activeSkip = currentSelected.Count <= 0;
            pnlSkipArea.gameObject.SetActive(activeSkip);
            pnlNextArea.gameObject.SetActive(!activeSkip);
        }

        private void OnPackClicked(UIRequestPackPart c)
        {
            if (c.Selected)
            {
                if(currentSelected.Count < 3)
                    currentSelected.Add(c);
            }
            else
            {
                currentSelected.Remove(c);
            }
            
            cardSelectTextSkip.SetText($"({currentSelected.Count}/{3})");
            cardSelectTextNext.SetText($"({currentSelected.Count}/{3})");
            bool activeSkip = currentSelected.Count <= 0;
            pnlSkipArea.gameObject.SetActive(activeSkip);
            pnlNextArea.gameObject.SetActive(!activeSkip);
        }
        
        private void OnGoldClick(int amount, GoldSlotTreasure c)
        {
            if (c.Selected)
            {
                if(currentSelected.Count < 3)
                    currentSelected.Add(c);
            }
            else
            {
                currentSelected.Remove(c);
            }
            
            cardSelectTextSkip.SetText($"({currentSelected.Count}/{3})");
            cardSelectTextNext.SetText($"({currentSelected.Count}/{3})");
            bool activeSkip = currentSelected.Count <= 0;
            pnlSkipArea.gameObject.SetActive(activeSkip);
            pnlNextArea.gameObject.SetActive(!activeSkip);
        }

        private void OnRelicClick(RelicName t, RelicSlotTreasure c)
        {
            if (c.Selected)
            {
                if(currentSelected.Count < 3)
                    currentSelected.Add(c);
            }
            else
            {
                currentSelected.Remove(c);
            }
            
            cardSelectTextSkip.SetText($"({currentSelected.Count}/{3})");
            cardSelectTextNext.SetText($"({currentSelected.Count}/{3})");
            bool activeSkip = currentSelected.Count <= 0;
            pnlSkipArea.gameObject.SetActive(activeSkip);
            pnlNextArea.gameObject.SetActive(!activeSkip);
        }
        
        

        public void MouseHoverSkip(bool hover)
        {
            skipLine.SetActive(hover);
        }
        public void MouseHoverNext(bool hover)
        {
            nextLine.SetActive(hover);
        }
    }
}