﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

namespace FluffyDisdog.Data
{
    [Flags]
    public enum ToolTag : uint
    {
        NONE=0,
        First=1<<0,
        Sec=1<<1,
        Third=1<<2,
        Fourth=1<<3,
        Fifth=1<<4,
        Sixth=1<<5,
        Seventh=1<<6,
        Eighth=1<<7,
        Ninth=1<<8,
        Tenth=1<<9
    }

    public enum ToolAdditionalOption
    {
        [Description("그런 거 없다.")]
        None=0,
        [Description("파괴 실패 시 확률로 균열.")]
        ChangeCrackWhenFail=1,
        [Description("파괴 실패 시 확률로 전염.")]
        ChangeFlagueWhenFail=2,
        [Description("인접 랜덤타일 전염.")]
        ChangeFlagueRandomNearTile=3
    }
    
    [Serializable]
    public class ToolData
    {
        public ToolType type;
        public int cellWidth;
        public int cellHeight;
        public int[] ratio;
        public bool[] interact;
        public int Center;
        public ToolTag tag;
        public ToolAdditionalOption option;
        public int optionValue;

        public int CenterRow => Center % cellWidth;
        public int CenterColumn => Center / cellWidth;

        public int[] GetRatioValues()
        {
            int[] newArr = new int[ratio.Length];
            for (int i = 0; i < ratio.Length; i++)
            {
                newArr[i] = ratio[i];
            }

            return newArr;
        }

        public int GetRatioValue(int row, int col)
        {
            return ratio[col * cellWidth + row];
        }
        
        public bool[] GetInteractable()
        {
            if (interact == null)
                return new bool[cellHeight*cellWidth];
            bool[] newArr = new bool[ratio.Length];
            for (int i = 0; i < interact.Length; i++)
            {
                newArr[i] = interact[i];
            }

            return newArr;
        }

        public bool GetInteractable(int row, int col)
        {
            return interact[col * cellWidth + row];
        }
        
        public ToolData Copy()
        {
            var newArr = GetRatioValues();
            var newInteract = GetInteractable();
            return new ToolData()
            {
                type = this.type,
                cellHeight = this.cellHeight,
                cellWidth = this.cellWidth,
                ratio = newArr,
                Center = this.Center,
                tag = this.tag,
                option = this.option,
                optionValue = this.optionValue,
                interact = newInteract
            };
        }
    }
    
    [CreateAssetMenu]
    public class ToolTable:ScriptableObject
    {
        [SerializeField] private ToolData[] _datas;

#if UNITY_EDITOR
        public void SetData(ToolData[] d)
        {
            _datas = d;
        }
#endif

        public Dictionary<ToolType, ToolData> TryCache()
        {
            var ret = new Dictionary<ToolType, ToolData>();

            foreach (var d in _datas)
            {
                ret.Add(d.type, d.Copy());
            }

            return ret;
        }
    }
}