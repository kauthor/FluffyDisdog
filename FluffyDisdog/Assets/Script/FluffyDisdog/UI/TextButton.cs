using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace FluffyDisdog.UI
{
    public class TextButton:MonoBehaviour
    {
        [SerializeField] private ButtonExtension txtSelectable;
        [SerializeField] private GameObject[] onSelectedOnOff;


        private Action onHoveredCb;
        private Action onUnhoveredCb;
        private Action onClickCb;

        private void Awake()
        {
            if(txtSelectable == null)
                txtSelectable = GetComponentInChildren<ButtonExtension>();
        }

        private void Start()
        {
            if (onSelectedOnOff == null )
            {
                Debug.Log("ButtonExtension: onSelectedOnOff is null");
            }
            if(onSelectedOnOff.Length >0)
                foreach(var o in onSelectedOnOff)
                    o.gameObject.SetActive(false);

            if (txtSelectable == null || txtSelectable.onClick == null)
            {
                Debug.Log("ButtonExtension: txtSelectable is null");
            }
            txtSelectable.onClick.RemoveAllListeners();
            txtSelectable.onClick.AddListener(()=> onClickCb?.Invoke());
            
            txtSelectable.BindHoveredHandler(OnHovered, OnUnhovered);
        }
        
        public void BindHandler(Action onClick)
        => onClickCb = onClick;

        private void OnHovered()
        {
            onHoveredCb?.Invoke();
            foreach(var o in onSelectedOnOff)
                o.gameObject.SetActive(true);
        }

        private void OnUnhovered()
        {
            onUnhoveredCb?.Invoke();
            foreach(var o in onSelectedOnOff)
                o.gameObject.SetActive(false);
        }
    }
}