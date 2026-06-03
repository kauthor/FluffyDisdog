using System;
using Script.FluffyDisdog.Managers;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace FluffyDisdog.UI
{
    public class UIUpgradeSelectPopup:PopupMonoBehavior
    {
        public override PopupType type => PopupType.UpgradeSelect;

        [SerializeField] private CardPopupParts cardPrefab;

        [SerializeField] private Transform cardParent;

        [SerializeField] private Button btnSkip;
        

        [FoldoutGroup("Upgrade Desire")]
        [SerializeField] private GameObject pnlUpgrade;

        [FoldoutGroup("Upgrade Desire")] [SerializeField]
        private CardPopupParts before;
        
        [FoldoutGroup("Upgrade Desire")]
        [SerializeField] private CardPopupParts after;
        
        [FoldoutGroup("Upgrade Desire")]
        [SerializeField] private Button btnAccept;
        
        [FoldoutGroup("Upgrade Desire")]
        [SerializeField] private Button btnDecline;

        private ToolType currentSelected;

        private CardPopupParts currentSelectedButton;

        private Action<bool> onResult;


        public static void OpenPopup(Action<bool> cb)
        {
            var pop = PopupManager.I.GetPopup(PopupType.UpgradeSelect);
            if (pop is UIUpgradeSelectPopup stg)
            {
                stg.Init(cb);
                stg.gameObject.SetActive(true);
            }
        }
        
        protected override void Awake()
        {
            base.Awake();
            btnAccept.onClick.RemoveAllListeners();
            btnDecline.onClick.RemoveAllListeners();
            btnAccept.onClick.AddListener(()=>
            {
                var card = DeckManager.I.GetDeckList().Find(_=>_.ToolType == currentSelected);
                DeckManager.I.RemoveCard(card);
                var newCard = ExcelManager.I.GetToolExcelData(currentSelected).UpgradeKey;
                DeckManager.I.TryAddDeck(newCard);
                onResult?.Invoke(true);
                Close();
            });
            btnDecline.onClick.AddListener(() =>
            {
                onResult?.Invoke(false);
                Close();
            });
        }

        private void Init(Action<bool> cb)
        {
            var deck = DeckManager.I.GetDeckList();
            currentSelected = ToolType.None;
            currentSelectedButton = null;

            foreach (var pair in deck)
            {
                var newSlot = GameObject.Instantiate(cardPrefab, cardParent);
                newSlot.transform.localScale = Vector3.one / 2.0f;
                newSlot.Init(pair.ToolType, 0);
                newSlot.InitAsUpgradeHandler();
                newSlot.BindHandler(OnCardSelectedCB);
            }

            onResult = cb;
        }
        
        
        
        private void OnCardSelectedCB(ToolType tool, CardPopupParts part)
        {
            currentSelected = tool;
            before.Init(tool,0);
            var beforeData = ExcelManager.I.GetToolExcelData(tool);
            var afterData = ExcelManager.I.GetToolExcelData((ToolType)(beforeData.UpgradeKey));
            after.Init(afterData.CardKey,0);
        }
    }
}