using System;
using UnityEngine;
using UnityEngine.UI;

namespace FluffyDisdog.UI
{
    public class UIUpgradeSlotPart:MonoBehaviour
    {
        [SerializeField] private Button btnSelect;


        private event Action<bool> onDesired;

        private void Awake()
        {
            btnSelect.onClick.RemoveAllListeners();
            btnSelect.onClick.AddListener(() =>
            {
                UIUpgradeSelectPopup.OpenPopup(onDesired);
            });
        }

        public void Init(Action<bool> cb)
        {
            onDesired = cb;
        }
    }
}