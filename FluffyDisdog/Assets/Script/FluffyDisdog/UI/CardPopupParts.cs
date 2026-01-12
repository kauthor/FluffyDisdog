using System;
using Script.FluffyDisdog.Managers;
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
        [SerializeField] private Image cardRarityIcon;
        [SerializeField] private Text txtAmount;
        [SerializeField] private Image[] tagIcons;
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
            if(amount<=1)
                txtAmount.gameObject.SetActive(false);
            else txtAmount.text = $"x{amount}";
            gameObject.SetActive(true);

            var data = ExcelManager.I.GetToolExcelData(type);
            
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