using UnityEngine;

namespace FluffyDisdog.UI
{
    public class UIOptionPopup:PopupMonoBehavior
    {
        [SerializeField] private TextButton[] textButtons;
        public override PopupType type => PopupType.Option;

        public static void OpenPopup()
        {
            var pop = PopupManager.I.GetPopup(PopupType.Option);
            if (pop is UIOptionPopup stg)
            {
                stg.Init();
                stg.gameObject.SetActive(true);
            }
        }

        private void Init()
        {
            textButtons[0].BindHandler(Application.Quit);
        }
    }
}