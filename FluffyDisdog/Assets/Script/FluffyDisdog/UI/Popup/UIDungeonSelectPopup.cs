using UnityEngine;
using UnityEngine.UI;

namespace FluffyDisdog.UI
{
    public class UIDungeonSelectPopup:PopupMonoBehavior
    {
        /// <summary>
        /// 임시
        /// </summary>
        [SerializeField] private Button enterInstance;

        public override PopupType type => PopupType.DungeonSelect;

        public static void OpenPopup()
        {
            var pop = PopupManager.I.GetPopup(PopupType.DungeonSelect);
            if (pop is UIDungeonSelectPopup stg)
            {
                stg.Init();
                stg.gameObject.SetActive(true);
            }
        }

        protected override void Awake()
        {
            base.Awake();
            enterInstance.onClick.AddListener(() =>
            {
                Close();
                UICharacterSelectPopup.OpenPopup();
            });
        }

        private void Init()
        {
            
        }
    }
}