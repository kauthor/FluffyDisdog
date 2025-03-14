using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace FluffyDisdog.UI
{
    public class RewardCardSlot: MonoBehaviour
    {
        [SerializeField] private Button btnClickArea;
        [SerializeField] private Text txtTool; //임시

        private ToolType _tool;
        private bool isRerollCard;

        private event Action<ToolType> OnClicked;
        private void Awake()
        {
            btnClickArea.onClick.RemoveAllListeners();
            btnClickArea.onClick.AddListener(()=> OnClicked?.Invoke(_tool));
        }

        public void Init(Action<ToolType> onclicked, ToolType tool)
        {
            BindHandler(onclicked);
            isRerollCard = false;
            SetCardData(tool);
            
        }

        public void InitAsReroll(Action<ToolType> onReroll, ToolType tool)
        {
            BindHandler(onReroll);
            isRerollCard = true;
            SetCardData(tool);
            
        }

        public void SetCardData(ToolType tool)
        {
            _tool = tool;
            
            if(!isRerollCard)
                txtTool.text = tool.ToString();
            else
            {
                txtTool.text = $"{tool.ToString()}카드 덱에서 한장 제거";
            }
        }

        private void BindHandler(Action<ToolType> cb)
        {
            OnClicked -= cb;
            OnClicked += cb;
        }
    }
}