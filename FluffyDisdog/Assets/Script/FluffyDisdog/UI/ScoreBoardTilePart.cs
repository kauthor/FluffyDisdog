using System;
using Script.FluffyDisdog.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FluffyDisdog.UI
{
    public class ScoreBoardTilePart:MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI txtBaseScore;
        [SerializeField] private TextMeshProUGUI txtMulti;

        [SerializeField] private NodeSubstate tag;

        
        [SerializeField] private Button btnBookmark;
        [SerializeField] private Image imgBookmark;
        public NodeSubstate Tag => this.tag;

        
        private Action OnBookmarkClicked;

        bool isBookmark=false;
        
        private void Awake()
        {
            btnBookmark.onClick.RemoveAllListeners();
            btnBookmark.onClick.AddListener(() =>
            {
                SetBookmark(!isBookmark);
                PlayerManager.I.NodeSubstateBookmarkInsert(isBookmark,tag);
                OnBookmarkClicked?.Invoke();
            });

            OnBookmarkClicked = null;
        }

        public void Init(Action onBookmarkClicked)
        {
            var multi = TileGameManager.I.ScoreEmulator.GetNodeSubstateMulti(tag);
            txtMulti.text = Math.Round(multi, 3).ToString();
            txtBaseScore.text = "100";
            onBookmarkClicked = onBookmarkClicked;
            bool on = PlayerManager.I.NodeSubstateBookmark.Contains(tag);
            SetBookmark(on);
        }

        private void SetBookmark(bool on = false)
        {
            imgBookmark.gameObject.SetActive(on);
            isBookmark = on;
        }
    }
}