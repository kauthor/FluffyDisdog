using System.Collections;
using DG.Tweening;
using FluffyDisdog.Manager;
using Sirenix.OdinInspector;
using UnityEngine;

namespace FluffyDisdog.UI
{
    public class UIStageClearPopup:PopupMonoBehavior
    {
        public override PopupType type => PopupType.StageClear;

        [SerializeField] private GoldCasher casher;
        [SerializeField] private OutlinedText txtGainedGold;

        [SerializeField] private GameObject[] CoinAnimPrefabs;
        [SerializeField] private Transform coinStartTr;
        [SerializeField] private Transform coinEndTr;

        /// <summary>
        /// 이것은 코인의 경로들
        /// </summary>
        [FoldoutGroup("To PD")]
        [SerializeField] private Ease[] easeCases;

        /// <summary>
        /// 이것은 코인 생성 주기
        /// </summary>
        [FoldoutGroup("To PD")]
        [SerializeField] private float coinTerm=0.1f;
        
        /// <summary>
        /// 이것은 코인 수명
        /// </summary>
        [FoldoutGroup("To PD")]
        [SerializeField] private float coinDuration=0.5f;

        private int gainGold;
        private int goalGold;
        private int startCasherGold;
        
        private Coroutine GoldGainCoroutine;
        
        public static void OpenPopup(int gold)
        {
            var pop = PopupManager.I.GetPopup(PopupType.StageClear);
            if (pop is UIStageClearPopup clr)
            {
                clr.Init(gold);
            }
        }

        private void Init(int gold)
        {
            casher.SyncGold(AccountManager.I.Gold);
            startCasherGold = AccountManager.I.Gold;
            gainGold = gold;
            txtGainedGold.SetText(gainGold.ToString()+" G");
            goalGold = AccountManager.I.Gold + gold;
            GoldGainCoroutine = null;
            
            AccountManager.I.AddGold(gainGold);
            this.gameObject.SetActive(true);
            GoldGainCoroutine = StartCoroutine(GoldRoutine());
        }
        
        private IEnumerator GoldRoutine(float time=2.0f)
        {
            var startGold = startCasherGold;
            var currentUIGainGold =gainGold;

            float temp = 0;
            float coinTemp = 0;
            while (temp < time)
            {
                casher.SyncGold(startGold + (int)((float)gainGold* temp/time));
                currentUIGainGold = gainGold -(int)((float)gainGold* temp/time);
                txtGainedGold.SetText(currentUIGainGold.ToString()+" G");
                yield return new WaitForFixedUpdate();
                temp += Time.fixedDeltaTime;
                coinTemp += Time.fixedDeltaTime;
                if (coinTemp > coinTerm)
                {
                    coinTemp = 0;
                    var newCoin = GameObject.Instantiate(CoinAnimPrefabs[Random.Range(0,3)], coinStartTr);
                    newCoin.transform.DOMoveX(coinEndTr.position.x, coinDuration).SetEase(easeCases[Random.Range(0,easeCases.Length)]);
                    newCoin.transform.DOMoveY(coinEndTr.position.y, coinDuration).SetEase(easeCases[Random.Range(0,easeCases.Length)]).onComplete += ()=>newCoin.gameObject.SetActive(false);
                }
            }
            
            casher.SyncGold(goalGold);
            txtGainedGold.SetText("0 G");
        }

        protected override void OnCloseClick()
        {
            base.OnCloseClick();
            if(GoldGainCoroutine!=null)
                StopCoroutine(GoldGainCoroutine);

            GoldGainCoroutine = null;
            UIStageRewardPopup.OpenPopup(() =>
            {
                //TileGameManager.I.GoNextLevel();
                UIManager.I.ChangeView(UIType.Store);
            });
        }
    }
}