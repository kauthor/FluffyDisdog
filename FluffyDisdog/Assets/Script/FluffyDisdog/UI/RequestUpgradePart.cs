using System;
using FluffyDisdog.UI.Part;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace FluffyDisdog.UI
{
    public class RequestUpgradePart:MonoBehaviour,IPointerEnterHandler,IPointerExitHandler,ISelectableUiPart
    {
        
        [SerializeField] private Button btnSelect;
        [FoldoutGroup("Selectable")] [SerializeField]
        private GameObject hoverArea;

        [FoldoutGroup("Selectable")] [SerializeField]
        private GameObject txtCancel;
        
        [FoldoutGroup("Selectable")] [SerializeField]
        private GameObject txtSelect;
        
        [FoldoutGroup("Selectable")] [SerializeField]
        private GameObject imgSelected;
        
        [FoldoutGroup("Selectable")] [SerializeField]
        private OutlinedText txtSelectTMP;

        [FoldoutGroup("Color Field")] [SerializeField]
        private Color selectableColor;
        
        [FoldoutGroup("Color Field")] [SerializeField]
        private Color selectableOutlineColor;
        
        [FoldoutGroup("Color Field")] [SerializeField]
        private Color unselectableColor;
        
        [FoldoutGroup("Color Field")] [SerializeField]
        private Color unselectableOutlineColor;

        private Func<bool> selectable;

        private Action<RequestUpgradePart> OnCLicked;
        
        private bool selected=false;
        public bool Selected => selected;
        
        private Action<bool> onHoveredGlobal;
        private void Start()
        {
            btnSelect.onClick.RemoveAllListeners();
            btnSelect.onClick.AddListener(OnClick);
        }
        
       

        public void Init(Func<bool> indicator, Action<bool> hoveredGlobal, Action<RequestUpgradePart> cb)
        {
            selected=false;
            selectable = indicator;
            onHoveredGlobal=hoveredGlobal;
            OnCLicked = cb;
        }
        
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            
            hoverArea.SetActive(true);
            txtSelect.gameObject.SetActive(!selected);
            txtCancel.gameObject.SetActive(selected);
            
            txtSelectTMP?.SetColor(selectable() ? selectableColor : unselectableColor);
            txtSelectTMP?.SetOutlineColor(selectable() ? selectableOutlineColor : unselectableOutlineColor);
            
            onHoveredGlobal?.Invoke(true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            hoverArea.SetActive(false);
            onHoveredGlobal?.Invoke(false);
        }

        private void OnClick()
        {
            
            if (selected || selectable())
            {
                selected = !selected;
                txtSelect.gameObject.SetActive(!selected);
                txtCancel.gameObject.SetActive(selected);
                imgSelected.gameObject.SetActive(selected);
                    
            }
            
            OnCLicked?.Invoke(this);
            if (selectable != null)
            {
                txtSelectTMP?.SetColor(selectable() ? selectableColor : unselectableColor);
                txtSelectTMP?.SetOutlineColor(selectable() ? selectableOutlineColor : unselectableOutlineColor);
            }
        }

        public SelectableUiType Type => SelectableUiType.Upgrade;
    }
}