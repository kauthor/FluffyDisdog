using System;
using UnityEngine;
using UnityEngine.UI;

namespace FluffyDisdog.UI
{
    public class PopupMonoBehavior:MonoBehaviour
    {
        public virtual PopupType type { get; }
        [SerializeField] private Button BtnClose;
        [SerializeField] private Button BtnOutClose;

        private Action OnCloseCb;
        protected virtual void Awake()
        {
            if(BtnClose)
               BtnClose.onClick.AddListener(OnCloseClick);
            if(BtnOutClose!=null)
               BtnOutClose.onClick.AddListener(OnCloseClick);
        }

        protected virtual void Dispose()
        {
            
        }

        public virtual void CallEnd()
        {
            Dispose();
        }

        protected virtual void OnCloseClick()
        {
            OnCloseCb?.Invoke();
            PopupManager.GetInstance().ClosePopup(type);
        }

        public void Close()
        {
            OnCloseClick();
        }

        public void SetOnCloseHandler(Action cb)
        {
            OnCloseCb = cb;
        }
    }
}