using System;
using UnityEngine;
using UnityEngine.UI;

namespace FluffyDisdog.UI
{
    public class CardPopupParts:MonoBehaviour
    {
        [SerializeField] private Text txtType;
        [SerializeField] private Button btnClickArea;
        [SerializeField] private Image imgSelected;
        [SerializeField] private Image imgUnselected;
        [SerializeField] private Image imgBackward;
        [SerializeField] private GameObject tagAndIconArea;
        [SerializeField] private Image cardImage;
        [SerializeField] private Image cardGridImage;
        [SerializeField] private Text txtAmount;
        [SerializeField] private GameObject[] tags;
        private int ID;
        private ToolType _toolType;
        public ToolType ToolType => _toolType;

        private Action<ToolType, CardPopupParts>  OnCLicked;

        private void Awake()
        {
            btnClickArea.onClick.RemoveAllListeners();
            btnClickArea.onClick.AddListener(() =>
            {
                OnCLicked?.Invoke(_toolType, this);
                imgSelected?.gameObject.SetActive(true);
                imgUnselected?.gameObject.SetActive(false);
            });
        }

        public void Init(ToolType type, int amount)
        {
            txtType.text = type.ToString();
            _toolType = type;
            if(amount<=0)
                txtAmount.gameObject.SetActive(false);
            else txtAmount.text = $"x{amount}";
            gameObject.SetActive(true);
        }

        public void BindHandler(Action<ToolType, CardPopupParts> cb)
        {
            OnCLicked = cb;
        }

        public void Unselect()
        {
            imgSelected?.gameObject.SetActive(false);
            imgUnselected?.gameObject.SetActive(true);
        }
    }
}