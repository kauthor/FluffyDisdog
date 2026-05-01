using Script.FluffyDisdog.Managers;
using UnityEngine;
using UnityEngine.UI;

namespace FluffyDisdog.UI
{
    public class UIDeckListPopup:PopupMonoBehavior
    {
        public override PopupType type => PopupType.DeckList;

        [SerializeField] private Text[] txtCardList;
        [SerializeField] private Transform cardHolder;
        [SerializeField] private CardPopupParts cardPrefab;
        
        public static void OpenPopup()
        {
            var pop = PopupManager.I.GetPopup(PopupType.DeckList);
            if (pop is UIDeckListPopup de)
            {
                de.gameObject.SetActive(true);
                de.Init();
            }
        }

        private void Init()
        {
            var list = DeckManager.I.GetDeckList();

            int loop = 0;
            foreach (var keypair in list)
            {
                var card = GameObject.Instantiate(cardPrefab, cardHolder) as CardPopupParts;
                card.transform.localScale = Vector3.one / 2.0f;
                card.Init(keypair.Key, keypair.Value);
            }
        }
    }
}