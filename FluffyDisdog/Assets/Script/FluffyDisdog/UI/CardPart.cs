using System;
using Script.FluffyDisdog.Managers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace FluffyDisdog.UI
{
    public class CardPart:MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private Text txtType;
        [SerializeField] private Button btnClickArea;
        [SerializeField] private Image imgSelected;
        [SerializeField] private Image imgUnselected;
        private int ID;
        private ToolType _toolType;
        private Action<int, ToolType> onClickedCb;
        private Action<int> onHoverCb;
        private Action<bool> onExitCb;
        private bool isSelected = false;
        public bool IsSelected => isSelected;

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
            Select(false);
        }

        public void Select(bool sel)
        {
            isSelected = sel;
            imgSelected.gameObject.SetActive(sel);
            imgUnselected.gameObject.SetActive(!sel);
            if(sel)
                Hovered();
            else
            {
                Exited();
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            Hovered();
        }

        private void Hovered()
        {
            var localPosition = transform.localPosition;
            localPosition = new Vector3( localPosition.x,20, 0);
            transform.localPosition = localPosition;
            
            onHoverCb?.Invoke(ID);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            Exited();
        }

        private void Exited()
        {
            if (!isSelected)
            {
                var localPosition = transform.localPosition;
                localPosition = new Vector3( localPosition.x,0, 0);
                transform.localPosition = localPosition;
                onExitCb?.Invoke(false);
            }
        }

        public void InitHandler(Action<int> onHover, Action<bool> onExit)
        {
            onHoverCb = onHover;
            onExitCb = onExit;
        }
    }
}