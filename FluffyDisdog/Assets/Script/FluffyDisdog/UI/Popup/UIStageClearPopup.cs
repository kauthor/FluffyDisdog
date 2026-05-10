using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using FluffyDisdog.Manager;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;
using UnityEngine.UI;

namespace FluffyDisdog.UI
{

    public enum Phase
    {
        Phase1=0,
        Phase2=1,
        Phase3=2,
        Phase4=3,
    }
    public class UIStageClearPopup:PopupMonoBehavior
    {
        public override PopupType type => PopupType.StageClear;

        [SerializeField] private GoldCasher casher;
        [SerializeField] private OutlinedText txtGainedGold;

        [SerializeField] private GameObject[] CoinAnimPrefabs;
        [SerializeField] private Transform coinStartTr;
        [SerializeField] private Transform coinEndTr;
        [SerializeField] private GameObject coinShineAnim;
        [SerializeField] private Button btnOut;

        private Phase currentPhase;
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
        private List<Sequence> seqs = new List<Sequence>();
        
        public static void OpenPopup(int gold)
        {
            var pop = PopupManager.I.GetPopup(PopupType.StageClear);
            if (pop is UIStageClearPopup clr)
            {
                clr.Init(gold);
            }
        }

        protected override void Awake()
        {
            base.Awake();
            btnOut.onClick.RemoveAllListeners();
            btnOut.onClick.AddListener(OnOutClick);
        }

        private void DoPhaseFlow(Phase phase)
        {
            currentPhase = phase;
            switch (phase)
            {
                case Phase.Phase1:
                    coinShineAnim.gameObject.SetActive(true);
                    GoldGainCoroutine = StartCoroutine(Ph1Routine());
                    break;
                case Phase.Phase3:
                    GoldGainCoroutine = StartCoroutine(GoldRoutine());
                    break;
            }
        }

        private IEnumerator Ph1Routine()
        {
            float temp = 0;
            float dura = 1.0f;
            while (dura > temp)
            {
                temp +=  Time.fixedDeltaTime;
                int curGold = (int)(gainGold * temp/dura);
                txtGainedGold.SetText($"{curGold} G");
                
                yield return new WaitForFixedUpdate();
            }
            txtGainedGold.SetText($"{gainGold} G");
            currentPhase = Phase.Phase2;
            coinShineAnim.gameObject.SetActive(false);
        }

        private void Init(int gold)
        {
            casher.SyncGold(AccountManager.I.Gold);
            startCasherGold = AccountManager.I.Gold;
            gainGold = gold;
            txtGainedGold.SetText("0 G");
            goalGold = AccountManager.I.Gold + gold;
            GoldGainCoroutine = null;
            
            AccountManager.I.AddGold(gainGold);
            this.gameObject.SetActive(true);
            DoPhaseFlow(Phase.Phase1);
            //GoldGainCoroutine = StartCoroutine(GoldRoutine());
        }

        private List<GameObject> currentcoins;
        private IEnumerator GoldRoutine(float time=2.0f)
        {
            var startGold = startCasherGold;
            var currentUIGainGold =gainGold;
            currentcoins = new List<GameObject>();

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
                    var seq1 = DOTween.Sequence().Append(newCoin.transform.DOMoveX(coinEndTr.position.x, coinDuration)
                        .SetEase(easeCases[Random.Range(0, easeCases.Length)]));
                    var seq2 = DOTween.Sequence()
                        .Append(newCoin.transform.DOMoveY(coinEndTr.position.y, coinDuration)
                            .SetEase(easeCases[Random.Range(0, easeCases.Length)]));
                    seq2.onComplete += () => newCoin.gameObject.SetActive(false);
                    
                    currentcoins.Add(newCoin);
                    seqs.Add(seq1);
                    seqs.Add(seq2);
                }
            }
            
            casher.SyncGold(goalGold);
            txtGainedGold.SetText("0 G");
            currentPhase = Phase.Phase4;
        }

        protected override void OnCloseClick()
        {
            base.OnCloseClick();
            
            UIStageRewardPopup.OpenPopup(() =>
            {
                //TileGameManager.I.GoNextLevel();
                UIManager.I.ChangeView(UIType.Store);
            });
        }

        private void OnOutClick()
        {
            switch (currentPhase)
            {
                case Phase.Phase1:
                    StopCoroutine(GoldGainCoroutine);
                    txtGainedGold.SetText($"{gainGold} G");
                    currentPhase = Phase.Phase2;
                    coinShineAnim.gameObject.SetActive(false);
                    break;
                case Phase.Phase2:
                    DoPhaseFlow(Phase.Phase3);
                    break;
                case Phase.Phase3:
                    StopCoroutine(GoldGainCoroutine);
                    if(currentcoins !=null)
                        currentcoins.ForEach(x=>x.SetActive(false));
                    txtGainedGold.SetText("0 G");
                    casher.SyncGold(goalGold);
                    currentPhase=Phase.Phase4;
                    break;
                case Phase.Phase4:
                    Close();
                    break;
            }
        }
    }
}