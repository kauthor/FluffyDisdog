using System.Collections;
using System.Collections.Generic;
using Script.FluffyDisdog.Managers;
using UnityEngine;
using UnityEngine.UI;

namespace FluffyDisdog.UI
{
    public class UIGameView:UIViewBehaviour
    {
        [SerializeField] private Text txtCurrentTool;
        [SerializeField] private Text txtCurrentScore;
        [SerializeField] private Text txtGoalScore;
        public override UIType type => UIType.InGame;

        
        public override void Init(UIViewParam param)
        {
            base.Init(param);
            DeckManager.I.BindHandler(_=> txtCurrentTool.text = _.ToString());
            txtCurrentScore.text = "0";
            txtGoalScore.text = TileGameManager.I.LevelData.Goal.ToString();
            TileGameManager.I.SubscribeCurrentScore(RefreshCurrentScore);
        }

        private void RefreshCurrentScore(int sc)
        {
            txtCurrentScore.text = sc.ToString();
        }
    }
}