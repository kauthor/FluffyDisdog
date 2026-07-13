using System;
using FluffyDisdog.Manager;
using FluffyDisdog.UI.Part;
using Script.FluffyDisdog.Managers;
using UnityEngine;
using UnityEngine.UI;

namespace FluffyDisdog.UI
{
    public class UILoginView : UIViewBehaviour
    {
        public override UIType type => UIType.Login;
        [SerializeField] private Button btnExit;
        [SerializeField] private UIMainLobbyButton[] btnVariables;

        private void Awake()
        {
            btnExit.onClick.RemoveAllListeners();
            btnExit.onClick.AddListener(Application.Quit);

            foreach (var btn in btnVariables)
            {
                btn.Enable(false);
            }
            
            btnVariables[0].Enable(true);
            btnVariables[0].Init(() =>
            {
                if(ExcelManager.ExistInstance() && ExcelManager.I.Initialized)
                    LoadSceneManager.I.LoadScene("GameScene", null);
            });
            
            
            btnVariables[3].Enable(true);
        }
    }
}