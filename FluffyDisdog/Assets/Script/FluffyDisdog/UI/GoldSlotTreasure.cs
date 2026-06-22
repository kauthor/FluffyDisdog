using System;
using FluffyDisdog.Data.RelicData;
using Script.FluffyDisdog.Managers;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace FluffyDisdog.UI
{
    public class GoldSlotTreasure:MonoBehaviour,IPointerEnterHandler,IPointerExitHandler
    {
        [SerializeField] private Button btnSelect;

        [SerializeField] private TextMeshProUGUI txtAmount;
        
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

        private event Action<int, GoldSlotTreasure> onCardPackOpen;
        private Func<bool> selectable;
        
        private bool selected = false;
        public bool Selected => selected;

        private void Start()
        {
            btnSelect.onClick.RemoveAllListeners();
            btnSelect.onClick.AddListener(TryClickRelic);
        }

        private int Amount;

        public void Init(int cost, Action<int,GoldSlotTreasure> onCardPackOpen)
        {
            this.Amount = cost;
            this.onCardPackOpen -= onCardPackOpen;
            this.onCardPackOpen += onCardPackOpen;
            selected = false;
            
            txtAmount.SetText($"{cost} G");
        }
        
        public void InitAsSelectable(Func<bool> indicator)
        {
            //_selectType = CardSelectType.Selectable;
            selectable = indicator;
        }
        
        
        private void TryClickRelic()
        {
            if (selected || selectable())
            {
                selected = !selected;
                txtSelect.gameObject.SetActive(!selected);
                txtCancel.gameObject.SetActive(selected);
                imgSelected.gameObject.SetActive(selected);
                        
            }
            onCardPackOpen?.Invoke(Amount,this);
        }
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            hoverArea.SetActive(true);
            txtSelect.gameObject.SetActive(!selected);
            txtCancel.gameObject.SetActive(selected);
            
            txtSelectTMP?.SetColor(selectable() ? selectableColor : unselectableColor);
            txtSelectTMP?.SetOutlineColor(selectable() ? selectableOutlineColor : unselectableOutlineColor);
            
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            hoverArea.SetActive(false);
        }
    }
}