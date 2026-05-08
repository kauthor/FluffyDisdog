using System;
using System.Collections.Generic;
using FluffyDisdog.Manager;
using Script.FluffyDisdog.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FluffyDisdog.UI
{
    public class UIGameOverResultPopup:PopupMonoBehavior
    {
        public override PopupType type => PopupType.GameOver;
        
        [SerializeField] private Button btnOK;
        
        [SerializeField] private Button btnDeck;

        [SerializeField] private TextMeshProUGUI txtStage;
        [SerializeField] private TextMeshProUGUI txtAttackedTile;
        [SerializeField] private TextMeshProUGUI txtDestroyTIle;
        [SerializeField] private TextMeshProUGUI txtOwnGold;
        
        
        

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
            
            btnDeck.onClick.RemoveAllListeners();
            btnDeck.onClick.AddListener(() =>
            {
                UIDeckListPopup.OpenPopup();
            });
        }

        private void Start()
        {
            btnOK.onClick.RemoveAllListeners();
            btnOK.onClick.AddListener(() =>
            {
                this.Close();
                LoadSceneManager.I.LoadScene("Lobby", _ =>
                {
                    UIManager.I.ChangeView(UIType.Login);
                });
            });
        }

        
        private void Init()
        {
            txtStage.text = $"{TileGameManager.I.currentLevel}-8";
            txtAttackedTile.text = TileGameManager.I.GameLog.AttackedTile.ToString();
            txtDestroyTIle.text = TileGameManager.I.GameLog.DestroyedTile.ToString();
            txtOwnGold.text = AccountManager.I.Gold.ToString() + " G";
            //todo : 텍스트들이 사라졌다?
            
        }
    }
}