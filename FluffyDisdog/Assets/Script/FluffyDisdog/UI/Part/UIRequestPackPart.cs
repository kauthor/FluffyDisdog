using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace FluffyDisdog.UI.Part
{
    public class UIRequestPackPart:MonoBehaviour,IPointerEnterHandler,IPointerExitHandler,ISelectableUiPart
    {
        [SerializeField] private Image cardPackImage;
        [SerializeField] private Button btnPackOpen;
        
        [SerializeField] private Sprite[] packResources;
        
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

        private Action<UIRequestPackPart> OnCLicked;
        
        private bool selected=false;
        public bool Selected => selected;
        
        private Action<bool> onHoveredGlobal;
        private void Start()
        {
            btnPackOpen.onClick.RemoveAllListeners();
            btnPackOpen.onClick.AddListener(OnClick);
        }
        
        private int gachaType;
        public int GachaType => gachaType;

        public void Init(int gachaType, Func<bool> indicator, Action<bool> hoveredGlobal, Action<UIRequestPackPart> cb)
        {
            this.gachaType = gachaType;
            selected=false;
            selectable = indicator;
            onHoveredGlobal=hoveredGlobal;
            OnCLicked = cb;
        }
        

        public void SetPackResource(int num)
        {
            cardPackImage.sprite = packResources[num];
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

        public SelectableUiType Type => SelectableUiType.Pack;
    }
}