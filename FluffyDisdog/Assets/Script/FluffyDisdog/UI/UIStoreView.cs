using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace FluffyDisdog.UI
{
    public class UIStoreView : UIViewBehaviour
    {
        public override UIType type => UIType.Store;

        [SerializeField] private Text txtGold;

        [FoldoutGroup("Store")]
        [SerializeField] private CardPopupParts[] cardSlot;

        [FoldoutGroup("Store")] [SerializeField]
        private Button btnReroll;

        [FoldoutGroup("Store")] [SerializeField]
        private Button btnStoreAccept;

        [FoldoutGroup("Store")] [SerializeField]
        private GameObject pnlSoldOut;


        [SerializeField] private int storeValueEditor=20;

        private int currentAccountGold;

        private ToolType currentStoreSelected;

        public override void Init(UIViewParam param)
        {
            base.Init(param);
            
            SyncGold();

            
        }

        private void SyncGold()
        {
            currentAccountGold = AccountManager.I.Gold;
            txtGold.text = currentAccountGold.ToString();
        }


        private void RerollStoreCard()
        {
            foreach (var card in cardSlot)
            {
                card.Init((ToolType)(Random.Range(0,2)),0);
            }
            
            currentStoreSelected = ToolType.None;
        }


        private void OnCardSelect()
        {
            
        }

        private void OnCardBuy()
        {
            if (currentStoreSelected == ToolType.None)
                return;
            
            //if(currentAccountGold >= )
        }
    }
}