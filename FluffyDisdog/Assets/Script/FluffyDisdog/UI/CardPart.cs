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
        [SerializeField] private Image imgBackward;
        private int ID;
        private ToolType _toolType;
        private Action<int, ToolType> onClickedCb;
        private Action onClickCanceledCb;
        private Action<int> onHoverCb;
        private Action<bool> onExitCb;
        private bool isSelected = false;
        public bool IsSelected => isSelected;

        private void Awake()
        {
            btnClickArea.onClick.RemoveAllListeners();
            btnClickArea.onClick.AddListener(() =>
            {
                if (!isSelected)
                {
                    Select(true);
                    onClickedCb?.Invoke(ID, _toolType);
                }
                else
                {
                    Select(false);
                    onClickCanceledCb?.Invoke();
                }
            });
        }

        public void Init(int id, ToolType type, Action<int, ToolType> cb, Action onCancelCb)
        {
            txtType.text = type.ToString();
            onClickedCb = cb;
            _toolType = type;
            ID = id;
            gameObject.SetActive(true);
            onClickCanceledCb = onCancelCb;
            Select(false);
            Flip(false);
        }

        public void Flip(bool front)
        {
            imgBackward?.gameObject.SetActive(!front);
            
            //todo : 여기에 신규 프리팹 구성요소 온오프
        }

        public void Select(bool sel)
        {
            isSelected = sel;
            imgSelected?.gameObject.SetActive(sel);
            imgUnselected?.gameObject.SetActive(!sel);
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
            localPosition = new Vector3( localPosition.x,-190, 0);
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
                localPosition = new Vector3( localPosition.x,-300, 0);
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