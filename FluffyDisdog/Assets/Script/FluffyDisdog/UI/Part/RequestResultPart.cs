using System;
using UnityEngine;
using UnityEngine.UI;

namespace FluffyDisdog.UI.Part
{
    public class RequestResultPart:MonoBehaviour
    {
        [SerializeField] private Button inputField;

        private Action<bool> OnClickCb;
        protected bool selected=false;
        
        
        public virtual void Init(RequestReward reward, Action<bool> onclick)
        {
            OnClickCb=onclick;
            selected=false;
        }

        private void Awake()
        {
            inputField.onClick.RemoveAllListeners();
            inputField.onClick.AddListener(() =>
            {
                selected=!selected;
                OnClickCb(selected);
            });
        }
    }
}