using System;
using FluffyDisdog.Data;
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
        [SerializeField] private GameObject tagAndIconArea;
        [SerializeField] private Image cardImage;
        [SerializeField] private Image cardGridImage;
        [SerializeField] private Image cardRarityIcon;


        [SerializeField] private Image[] tagIcons;
        [SerializeField] private GameObject[] tags;
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

        private void Start()
        {
            //ResourceLoadManager.I.LoadSprite(" ", sprite => cardImage.sprite = sprite).Forget() ;
        }

        public void Init(int id, ToolType type, Action<int, ToolType> cb, Action onCancelCb, ToolExcelData data=null)
        {
            txtType.text = type.ToString();
            onClickedCb = cb;
            _toolType = type;
            ID = id;
            gameObject.SetActive(true);
            onClickCanceledCb = onCancelCb;
            Select(false);
            Flip(false);

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

        public void Flip(bool front)
        {
            imgBackward?.gameObject.SetActive(!front);
            tagAndIconArea?.SetActive(front);
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
            localPosition = new Vector3( localPosition.x,-140, 0);
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
                localPosition = new Vector3( localPosition.x,-250, 0);
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