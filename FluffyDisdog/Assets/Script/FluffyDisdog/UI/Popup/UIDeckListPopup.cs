using System.Collections.Generic;
using Script.FluffyDisdog.Managers;
using UnityEngine;
using UnityEngine.UI;

namespace FluffyDisdog.UI
{
    public class UIDeckListPopup:PopupMonoBehavior
    {
        public override PopupType type => PopupType.DeckList;
        
        [SerializeField] private Transform cardHolder;
        [SerializeField] private CardPopupParts cardPrefab;
        
        [SerializeField] private Transform relicParent;
        [SerializeField] private UIRelicInfoPart relicPrefab;

        [SerializeField] private Transform exceptHandCardHolder;

        [SerializeField] private GameObject includeHandArea;
        [SerializeField] private GameObject excludeHandArea;
        [SerializeField] private Button btnShowExceptHand;
        [SerializeField] private Button btnReturn;
        
        private Queue<UIRelicInfoPart> relicPool;
        private Queue<UIRelicInfoPart> currentRelic;
        public static void OpenPopup(bool showExceptHand=false)
        {
            var pop = PopupManager.I.GetPopup(PopupType.DeckList);
            if (pop is UIDeckListPopup de)
            {
                de.gameObject.SetActive(true);
                de.Init(showExceptHand);
            }
        }

        protected override void Awake()
        {
            base.Awake();
            relicPool = new Queue<UIRelicInfoPart>();
            currentRelic = new Queue<UIRelicInfoPart>();
            
            btnShowExceptHand.onClick.RemoveAllListeners();
            btnShowExceptHand.onClick.AddListener(() =>
            {
                btnShowExceptHand.gameObject.SetActive(false);
                btnReturn.gameObject.SetActive(true);
                includeHandArea.gameObject.SetActive(false);
                excludeHandArea.gameObject.SetActive(true);
            });
            btnReturn.onClick.RemoveAllListeners();
            btnReturn.onClick.AddListener(() =>
            {
                btnShowExceptHand.gameObject.SetActive(true);
                btnReturn.gameObject.SetActive(false);
                includeHandArea.gameObject.SetActive(true);
                excludeHandArea.gameObject.SetActive(false);
            });
        }

        private void Init(bool showExceptHand=false)
        {
            var list = DeckManager.I.GetDeckList();
            includeHandArea.SetActive(true);
            excludeHandArea.SetActive(false);
            
            btnShowExceptHand.gameObject.SetActive(showExceptHand);
            btnReturn.gameObject.SetActive(false);

            int loop = 0;
            foreach (var keypair in list)
            {
                var card = GameObject.Instantiate(cardPrefab, cardHolder) as CardPopupParts;
                card.transform.localScale = Vector3.one / 2.0f;
                card.Init(keypair.ToolType, 1);
            }

            if (showExceptHand)
            {
                var eHand = DeckManager.I.GetDeckExceptHand();
                foreach (var keypair in eHand)
                {
                    var card = GameObject.Instantiate(cardPrefab, exceptHandCardHolder) as CardPopupParts;
                    card.transform.localScale = Vector3.one / 2.0f;
                    card.Init(keypair.ToolType, 1);
                }
            }
            
            var relics = TileGameManager.I.RelicSystem.currentRelicDatas;
            if(relics != null && relics.Length > 0)
                foreach (var relic in relics)
                {
                    if (relicPool.Count > 0)
                    {
                        var current = relicPool.Dequeue();
                        current.gameObject.SetActive(true);
                        current.InitData(relic.relicName);
                        currentRelic.Enqueue(current);
                    }
                    else
                    {
                        var newRelic = GameObject.Instantiate(relicPrefab, relicParent);
                        newRelic.InitData(relic.relicName);
                        currentRelic.Enqueue(newRelic);
                    }
                }
        }
    }
}