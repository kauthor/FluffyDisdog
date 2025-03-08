using System;
using System.Collections.Generic;
using FluffyDisdog.UI;
using UnityEngine;

namespace FluffyDisdog.Manager
{
    public class UIManager: CustomSingleton<UIManager>
    {
        /// <summary>
        /// 어드레서블로 바꿀 필요가 있다...
        /// </summary>
        [SerializeField] private UIViewBehaviour[] Views;

        private Dictionary<UIType, UIViewBehaviour> uiDic = new Dictionary<UIType, UIViewBehaviour>();
        private UIViewBehaviour currentView;
        private event Action<UIManager> OnAwake;

        public UIType CurentViewType
        {
            get
            {
                if(currentView)
                    return currentView.type;
                return UIType.None;
            }
        }

        protected override void Awake()
        {
            base.Awake();
            uiDic = new Dictionary<UIType, UIViewBehaviour>();

            foreach (var v in Views)
            {
                uiDic.Add(v.type, v);
                v.gameObject.SetActive(false);
            }
            OnAwake?.Invoke(this);
            OnAwake = null;
        }

        public UIViewBehaviour ChangeView(UIType type, UIViewParam param=null)
        {
            if (currentView)
            {
                currentView.gameObject.SetActive(false);
                currentView.CallEnd();
            }

            if (uiDic.TryGetValue(type, out currentView))
            {
                currentView.gameObject.SetActive(true);
                currentView.Init(param);
            }

            return currentView;
        }

        public void CloseAllView()
        {
            if (currentView)
            {
                currentView.gameObject.SetActive(false);
                currentView.CallEnd();
            }
        }

        public void OnAwakeEnd(Action<UIManager> onEnd)
        {
            OnAwake += onEnd;
        }
    }
}