using UnityEngine;
using UnityEngine.UI;

namespace FluffyDisdog.UI
{
    public class UIStageResultPopup:PopupMonoBehavior
    {
        [SerializeField] private Text txtResult;
        [SerializeField] private Button btnNext;
        public override PopupType type => PopupType.StageResult;

        protected override void Awake()
        {
            base.Awake();
            btnNext.onClick.RemoveAllListeners();
            btnNext.onClick.AddListener(() =>
            {
                TileGameManager.I.GameStartRoute();
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
            txtResult.text = result ? "Clear" : "Fail";
        }
    }
}