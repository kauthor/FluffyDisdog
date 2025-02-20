using FluffyDisdog.Manager;
using Script.FluffyDisdog.Managers;
using UnityEngine;
using UnityEngine.UI;

namespace FluffyDisdog.UI
{
    public class UILobbyView : UIViewBehaviour
    {
        public override UIType type => UIType.Lobby;
        [SerializeField] private Button btnEnter;

        private void Awake()
        {
            btnEnter.onClick.AddListener(() =>
            {
                LoadSceneManager.I.LoadScene("GameScene",null);
            });
        }
    }
}