using System;
using Script.FluffyDisdog.Managers;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace FluffyDisdog.UI
{
    public enum CardSelectType
    {
        NONE=0,
        Selectable=1
    }
    public class CardPopupParts:MonoBehaviour,IPointerEnterHandler,IPointerExitHandler
    {
        [SerializeField] private Text txtType;
        [SerializeField] private Text txtShadow;
        [SerializeField] private Button btnClickArea;
        //[SerializeField] private Image imgSelected;
        [SerializeField] private Image imgUnselected;
        [SerializeField] private Image imgBackward;
        [SerializeField] private GameObject tagAndIconArea;
        [SerializeField] private Image cardImage;
        [SerializeField] private Image cardGridImage;
        [SerializeField] private Image cardRarityIcon;
        [SerializeField] private Text txtAmount;
        [SerializeField] private Image[] tagIcons;
        [SerializeField] private GameObject[] tags;
        [SerializeField] private GameObject soldOut;

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

        private Func<bool> selectable;

        [FoldoutGroup("local")] [SerializeField]
        private TextMeshProUGUI txtDesc;

        private bool selected = false;
        public bool Selected => selected;
        
        private int ID;
        private ToolType _toolType;
        public ToolType ToolType => _toolType;

        private Action<ToolType, CardPopupParts>  OnCLicked;
        
        private CardSelectType _selectType=CardSelectType.NONE;

        private void Awake()
        {
            btnClickArea.onClick.RemoveAllListeners();
            btnClickArea.onClick.AddListener(() =>
            {
                if (_selectType == CardSelectType.Selectable)
                {
                    if (selected || selectable())
                    {
                        selected = !selected;
                        txtSelect.gameObject.SetActive(!selected);
                        txtCancel.gameObject.SetActive(selected);
                        imgSelected.gameObject.SetActive(selected);
                        
                    }
                }
                OnCLicked?.Invoke(_toolType, this);
                txtSelectTMP?.SetColor(selectable() ? Color.white : Color.red);
                //imgSelected?.gameObject.SetActive(true);
                //imgUnselected?.gameObject.SetActive(false);
            });
            soldOut.SetActive(false);
        }

        public void Init(ToolType type, int amount, bool showTag=true)
        {
            selected = false;
            txtType.text = type.ToString();
            txtShadow.text = type.ToString();
            _toolType = type;
            if(amount<=1)
                txtAmount.gameObject.SetActive(false);
            else txtAmount.text = $"x{amount}";
            gameObject.SetActive(true);
            soldOut.SetActive(false);

            var data = ExcelManager.I.GetToolExcelData(type);
            var localDesc = ExcelManager.I.GetLocalizeData(data.CardDescKeyLocal);
            var localName = ExcelManager.I.GetLocalizeData(data.CardNameKeyLocal);
            txtType.text = localName.kor;
            txtShadow.text = localName.kor;
            txtDesc.SetText(localDesc.kor);
            
            if (data != null)
            {
                ResourceLoadManager.I.LoadSpriteAtlasResource(ResourceAddress.CardIllust, data.CardImgId, _ =>
                {
                    cardImage.sprite = _;
                }).Forget();
                ResourceLoadManager.I.LoadSpriteAtlasResource(ResourceAddress.CardEffect, data.CardGridId, _ =>
                {
                    cardGridImage.sprite = _;
                }).Forget();
                ResourceLoadManager.I.LoadSpriteAtlasResource(ResourceAddress.CardRarity, $"Card_Rarity_{data.RarityKey}",
                    _ =>
                    {
                        cardRarityIcon.sprite = _;
                    }).Forget();
                
                if (showTag)
                {
                    ResourceLoadManager.I.LoadTagIcon(data.CarBit, _ =>
                    {
                        for (int i = 0; i < tagIcons.Length; i++)
                        {
                            if (i >= _.Count)
                            {
                                tags[i].gameObject.SetActive(false);
                                continue;
                            }
                            tags[i].gameObject.SetActive(true);
                            tagIcons[i].sprite = _[i];
                        }
                    }).Forget();
                }
                else
                {
                    tags.ForEach(_=>_.gameObject.SetActive(false));
                }
            }
            hoverArea.SetActive(false);
            imgSelected.SetActive(false);
        }

        public void InitAsSelectable(Func<bool> indicator)
        {
            _selectType = CardSelectType.Selectable;
            selectable = indicator;
        }

        public void BindHandler(Action<ToolType, CardPopupParts> cb)
        {
            OnCLicked = cb;
        }

        public void Unselect()
        {
            //imgSelected?.gameObject.SetActive(false);
            imgUnselected?.gameObject.SetActive(true);
        }
        
        public void AfterBuy() => soldOut.SetActive(true);
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (_selectType == CardSelectType.Selectable)
            {
                hoverArea.SetActive(true);
                txtSelect.gameObject.SetActive(!selected);
                txtCancel.gameObject.SetActive(selected);
                
                txtSelectTMP.SetColor(selectable() ? Color.white : Color.red);
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            hoverArea.SetActive(false);
        }
    }
}