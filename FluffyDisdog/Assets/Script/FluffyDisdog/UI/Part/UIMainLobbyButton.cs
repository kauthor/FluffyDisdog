using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace FluffyDisdog.UI.Part
{
    public class UIMainLobbyButton:MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private Button btnClickArea;
        [SerializeField] private GameObject pnlNormalMask;
        [SerializeField] private GameObject pnlDisableMask;
        [SerializeField] private GameObject[] jams;

        private Action onClick;

        private void Awake()
        {
            btnClickArea.onClick.RemoveAllListeners();
            btnClickArea.onClick.AddListener(() =>
            {
                onClick?.Invoke();
            });
        }

        public void Enable(bool enable)
        {
            pnlDisableMask.SetActive(!enable);
            pnlNormalMask.SetActive(enable);
            foreach (var jam in jams)
                jam.SetActive(false);
            btnClickArea.interactable = enable;
        }

        public void Init(Action cb)
        {
            this.onClick = cb;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (btnClickArea.interactable)
            {
                pnlNormalMask.gameObject.SetActive(false);
                foreach (var jam in jams)
                    jam.SetActive(true);
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (btnClickArea.interactable)
            {
                pnlNormalMask.gameObject.SetActive(true);
                foreach (var jam in jams)
                    jam.SetActive(false);
            }
        }
    }
}