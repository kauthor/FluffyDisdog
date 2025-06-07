using Script.FluffyDisdog.Managers;
using UnityEngine;
using UnityEngine.UI;

namespace FluffyDisdog.UI
{
    public class UICardPackResultPopup:PopupMonoBehavior
    {
        public override PopupType type => PopupType.CardPackResult;


        [SerializeField] private CardPopupParts[] cards;
        [SerializeField] private Button[] btnSelect;
        [SerializeField] private Text[] txtBtns;

        private ToolType[] packResults;
        
        public static PopupMonoBehavior OpenPopup(int gachaType, int cost)
        {
            var pop = PopupManager.I.GetPopup(PopupType.CardPackResult);
            if (pop is UICardPackResultPopup pack)
            {
                pack.Init(gachaType, cost);
                //return pop;
            }

            return pop;
        }

        private void Init(int gachaType, int cost)
        {
            var result = CutPack(gachaType);
            if (result != null && result.Length > 0)
            {
                AccountManager.I.GoldConsume(cost);
                packResults = result;
                for (int i = 0; i < result.Length; i++)
                {
                    if (i >= cards.Length)
                        break;
                    
                    cards[i].Init(result[i],1);
                }
            }

            foreach (var btn in btnSelect)
            {
                btn.enabled = true;
            }
            
            btnSelect[0].onClick.RemoveAllListeners();
            btnSelect[0].onClick.AddListener(()=>OnSelectCard(0));
            
            btnSelect[1].onClick.RemoveAllListeners();
            btnSelect[1].onClick.AddListener(()=>OnSelectCard(1));
            
            btnSelect[2].onClick.RemoveAllListeners();
            btnSelect[2].onClick.AddListener(()=>OnSelectCard(2));

            foreach (var btn in txtBtns)
            {
                //todo : localize
                btn.text = "확정";
            }
        }

        private ToolType[] CutPack(int gachaType)
        {
            //todo:임시
            var ret = new ToolType[3];
            for (int i = 0; i < 3; i++)
            {
                var num = Random.Range(0, (int)ToolType.MAX);
                ret[i] = (ToolType)num;
            }

            return ret;
        }

        private void OnSelectCard(int num)
        {
            var type = cards[num].ToolType;
            DeckManager.I.TryAddDeck(type);
            
            btnSelect[num].enabled =false;
            txtBtns[num].text = "수령";
        }
        
    }
}