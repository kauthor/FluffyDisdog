using System;
using Script.FluffyDisdog.Managers;
using UnityEngine;
using UnityEngine.UI;

namespace FluffyDisdog.UI
{
    public class CardPart:MonoBehaviour
    {
        [SerializeField] private Text txtType;
        [SerializeField] private Button btnClickArea;
        [SerializeField] private Image imgSelected;
        [SerializeField] private Image imgUnselected;
        private int ID;
        private ToolType _toolType;
        private Action<int, ToolType> onClickedCb;

        private void Awake()
        {
            btnClickArea.onClick.RemoveAllListeners();
            btnClickArea.onClick.AddListener(() =>
            {
                Select(true);
                onClickedCb?.Invoke(ID, _toolType);
            });
        }

        public void Init(int id, ToolType type, Action<int, ToolType> cb)
        {
            txtType.text = type.ToString();
            onClickedCb = cb;
            _toolType = type;
            ID = id;
            gameObject.SetActive(true);
        }

        public void Select(bool sel)
        {
            imgSelected.gameObject.SetActive(sel);
            imgUnselected.gameObject.SetActive(!sel);
        }
    }
}