using Script.FluffyDisdog.Managers;
using UnityEngine;
using UnityEngine.UI;

namespace FluffyDisdog.UI
{
    public class UICharacterSelectPopup:PopupMonoBehavior
    {
        [SerializeField] private Button enterInstance;

        public override PopupType type => PopupType.CharacterSelect;

        
        public static void OpenPopup()
        {
            var pop = PopupManager.I.GetPopup(PopupType.CharacterSelect);
            if (pop is UICharacterSelectPopup stg)
            {
                stg.Init();
                stg.gameObject.SetActive(true);
            }
        }

        private void Init()
        {
            
        }
        
        
        protected override void Awake()
        {
            base.Awake();
            enterInstance.onClick.AddListener(() =>
            {
                Close();
                LoadSceneManager.I.LoadScene("GameScene",null);
            });
        }
    }
}