using System;
using System.Collections.Generic;
using FluffyDisdog.Manager;
using Script.FluffyDisdog.Managers;
using UnityEngine;
using UnityEngine.UI;

namespace FluffyDisdog.UI
{
    public class UIGameOverResultPopup:PopupMonoBehavior
    {
        public override PopupType type => PopupType.GameOver;
        
        [SerializeField] private Button btnOK;
        [SerializeField] private Transform relicParent;
        [SerializeField] private UIRelicInfoPart relicPrefab;
        
        private Queue<UIRelicInfoPart> relicPool;
        private Queue<UIRelicInfoPart> currentRelic;

        public static void OpenPopup()
        {
            var pop = PopupManager.I.GetPopup(PopupType.GameOver);
            if (pop is UIGameOverResultPopup stg)
            {
                stg.Init();
                stg.gameObject.SetActive(true);
            }
        }

        protected override void Awake()
        {
            base.Awake();
            relicPool = new Queue<UIRelicInfoPart>();
            currentRelic = new Queue<UIRelicInfoPart>();
        }

        private void Start()
        {
            btnOK.onClick.RemoveAllListeners();
            btnOK.onClick.AddListener(() =>
            {
                LoadSceneManager.I.LoadScene("Lobby", _ =>
                {
                    UIManager.I.ChangeView(UIType.Lobby);
                });
            });
        }

        
        private void Init()
        {
            //todo : 텍스트들이 사라졌다?
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