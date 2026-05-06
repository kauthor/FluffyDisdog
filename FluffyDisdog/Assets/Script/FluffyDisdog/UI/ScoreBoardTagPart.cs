using System;
using System.Globalization;
using Script.FluffyDisdog.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FluffyDisdog.UI
{
    public class ScoreBoardTagPart:MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI txtBaseScore;
        [SerializeField] private TextMeshProUGUI txtMulti;

        [SerializeField] private int tag;

        [SerializeField] private Button btnBookmark;
        [SerializeField] private Image imgBookmark;
        public int Tag => this.tag;

        private Action OnBookmarkClicked;
        
        bool isBookmark=false;

        private void Awake()
        {
            btnBookmark.onClick.RemoveAllListeners();
            btnBookmark.onClick.AddListener(() =>
            {
                SetBookmark(!isBookmark);
                PlayerManager.I.TagBookmarkInsert(isBookmark,tag);
                OnBookmarkClicked?.Invoke();
            });

            OnBookmarkClicked = null;
        }

        public void Init(Action onBookmarkClicked)
        {
            var multi = TileGameManager.I.ScoreEmulator.GetTagMulti(tag);
            txtMulti.text = Math.Round(multi, 3).ToString();
            txtBaseScore.text = "100";
            onBookmarkClicked = onBookmarkClicked;
            bool on = PlayerManager.I.TagBookmark.Contains(tag);
            SetBookmark(on);
        }

        private void SetBookmark(bool on = false)
        {
            imgBookmark.gameObject.SetActive(on);
            isBookmark = on;
        }
    }
}