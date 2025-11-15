using Script.FluffyDisdog.Managers;
using UnityEngine;
using UnityEngine.UI;

namespace FluffyDisdog.UI
{
    public class UIGraveyardList:PopupMonoBehavior
    {
        public override PopupType type => PopupType.GraveyardList;

        [SerializeField] private Text[] txtCardList;
        [SerializeField] private Transform cardHolder;
        [SerializeField] private CardPopupParts cardPrefab;
        
        public static void OpenPopup()
        {
            var pop = PopupManager.I.GetPopup(PopupType.GraveyardList);
            if (pop is UIGraveyardList de)
            {
                de.gameObject.SetActive(true);
                de.Init();
            }
        }

        private void Init()
        {
            var list = DeckManager.I.Graveyard;

            int loop = 0;
            foreach (var data in list)
            {
                var card = GameObject.Instantiate(cardPrefab, cardHolder) as CardPopupParts;
                card.Init(data.ToolType, 1);
            }
        }
    }
}