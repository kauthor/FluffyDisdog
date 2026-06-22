using System;
using FluffyDisdog.Data.RelicData;
using Script.FluffyDisdog.Managers;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace FluffyDisdog.UI
{
    public class RelicSlotTreasure:MonoBehaviour,IPointerEnterHandler,IPointerExitHandler
    {
        [SerializeField] private Image relicImage;
        [SerializeField] private Button btnRelicBuy;
        
        [SerializeField] private Text txtRelicName;
        [SerializeField] private Text txtRelicNameShadow;

        [SerializeField] private TextMeshProUGUI txtRelicDesc;
        
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

        private event Action<RelicName, RelicSlotTreasure> onCardPackOpen;
        private Func<bool> selectable;
        
        private bool selected = false;
        public bool Selected => selected;

        private void Start()
        {
            btnRelicBuy.onClick.RemoveAllListeners();
            btnRelicBuy.onClick.AddListener(TryClickRelic);
        }

        private int cost;
        private RelicName relicType;

        public void Init(RelicName relicN, Action<RelicName,RelicSlotTreasure> onCardPackOpen)
        {
            this.cost = cost;
            this.relicType = (RelicName)relicN;
             ResourceLoadManager.I.LoadSpriteAtlasResource(ResourceAddress.RelicIcon,
                $"Relic{relicN}",
                _ =>
                {
                    if (_ != null)
                        relicImage.sprite = _;
                }).Forget();
            this.onCardPackOpen -= onCardPackOpen;
            this.onCardPackOpen += onCardPackOpen;
            selected = false;
            
            var data = ExcelManager.I.GetRelicData(relicType);
            var nameTxt = ExcelManager.I.GetLocalizeData(data.localKey).kor;
            var descTxt = ExcelManager.I.GetLocalizeData(data.localDesc).kor;
            
            txtRelicName.text = nameTxt;
            txtRelicNameShadow.text = nameTxt;
            txtRelicDesc.SetText(descTxt);
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
            
            onCardPackOpen?.Invoke(relicType,this);
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