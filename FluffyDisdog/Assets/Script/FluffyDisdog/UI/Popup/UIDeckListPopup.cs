using Script.FluffyDisdog.Managers;
using UnityEngine;
using UnityEngine.UI;

namespace FluffyDisdog.UI
{
    public class UIDeckListPopup:PopupMonoBehavior
    {
        public override PopupType type => PopupType.DeckList;

        [SerializeField] private Text[] txtCardList;
        
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
                if(loop >= txtCardList.Length)
                    break;

                txtCardList[loop++].text = $"{keypair.Key} x {keypair.Value}";
            }

            for (int i = loop; i < txtCardList.Length; i++)
            {
                txtCardList[i].gameObject.SetActive(false);
            }
        }
    }
}