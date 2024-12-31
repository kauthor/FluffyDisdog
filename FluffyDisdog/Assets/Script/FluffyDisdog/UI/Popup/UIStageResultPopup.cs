using UnityEngine;
using UnityEngine.UI;

namespace FluffyDisdog.UI
{
    public class UIStageResultPopup:PopupMonoBehavior
    {
        [SerializeField] private Text txtResult;
        [SerializeField] private Button btnNext;
        public override PopupType type => PopupType.StageResult;

        private bool levelResult;
        protected override void Awake()
        {
            base.Awake();
            btnNext.onClick.RemoveAllListeners();
            btnNext.onClick.AddListener(() =>
            {
                if(levelResult)
                   TileGameManager.I.GoNextLevel();
                else TileGameManager.I.ResetLevel();
                Close();
            });
        }

        public static void OpenPopup(bool result)
        {
            var pop = PopupManager.I.GetPopup(PopupType.StageResult);
            if (pop is UIStageResultPopup stg)
            {
                stg.Init(result);
                stg.gameObject.SetActive(true);
            }
        }

        private void Init(bool result)
        {
            levelResult = result;
            txtResult.text = result ? "Clear" : "Fail";
        }
    }
}