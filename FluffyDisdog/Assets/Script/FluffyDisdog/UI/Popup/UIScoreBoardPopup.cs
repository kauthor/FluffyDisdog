using System;
using Script.FluffyDisdog.Managers;
using UnityEngine;
using UnityEngine.UI;

namespace FluffyDisdog.UI
{
    public class UIScoreBoardPopup:PopupMonoBehavior
    {
        public override PopupType type => PopupType.ScoreBoard;
        
        [SerializeField] private ScoreBoardTagPart[] tagParts;
        [SerializeField] private ScoreBoardTilePart[] tileParts;
        

        [SerializeField] private GameObject tagArea;
        [SerializeField] private GameObject tileArea;

        [SerializeField] private Button btnTileOn;
        [SerializeField] private Button btnTagOn;

        public static void OpenPopup()
        {
            var pop = PopupManager.I.GetPopup(PopupType.ScoreBoard);
            if (pop is UIScoreBoardPopup score)
            {
                score.Init();
                score.gameObject.SetActive(true);
            }
        }

        protected override void Awake()
        {
            base.Awake();
            btnTileOn.onClick.RemoveAllListeners();
            btnTagOn.onClick.RemoveAllListeners();
            
            btnTileOn.onClick.AddListener(() =>
            {
                tagArea.gameObject.SetActive(false);
                tileArea.gameObject.SetActive(true);
            });
            
            btnTagOn.onClick.AddListener(() =>
            {
                tagArea.gameObject.SetActive(true);
                tileArea.gameObject.SetActive(false);
            });
        }

        private void Init()
        {
            foreach (var tagPart in tagParts)
                tagPart.Init(Sort);
            
            foreach (var tilePart in tileParts)
                tilePart.Init(Sort);
            
        }
        
        
        private void Sort()
        {
            foreach (var tag in PlayerManager.I.TagBookmark)
            {
                var tagPart = Array.Find(tagParts, _=>_.Tag == tag);
                tagPart.transform.SetAsFirstSibling();
            }
            
            foreach (var node in PlayerManager.I.NodeSubstateBookmark)
            {
                var tagPart = Array.Find(tileParts, _=>_.Tag == node);
                tagPart.transform.SetAsFirstSibling();
            }
        }
    }
}