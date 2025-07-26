using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace FluffyDisdog.Data
{
    [Serializable]
    public class ToolExcelData
    {
        
        
        [SerializeField] ToolType cardKeyNew;
        [SerializeField] private string cardKeyString;
        [SerializeField] private string cardNameKeyLocal;
        [SerializeField] private string cardDescKeyLocal;
        [SerializeField] private int cardTypeId;
        [SerializeField] private int charaTypeId;
        [SerializeField] private int upgradeTypeId;
        [SerializeField] private string upgradeKey;
        [SerializeField] private int rarityKey;
        [SerializeField] private int carBit;
        [SerializeField] private int cardTagBit;
        [SerializeField] private int manaCost;
        [SerializeField] private int[] addedOptionIds;
        [SerializeField] private int cardDescId;
        [SerializeField] private int sortId;
        [SerializeField] private string cardGridId;
        [SerializeField] private string cardImgId; 
        
        public ToolType CardKey => cardKeyNew;
        public string CardKeyString => cardKeyString;
        public string CardNameKeyLocal => cardNameKeyLocal;
        public string CardDescKeyLocal => cardDescKeyLocal;
        public int CardTypeId => cardTypeId;
        public int CharaTypeId => charaTypeId;
        public int UpgradeTypeId => upgradeTypeId;
        public string UpgradeKey => upgradeKey;
        public int RarityKey => rarityKey;
        public int CarBit => carBit;
        public ToolTag ToolTag => (ToolTag)cardTagBit;
        public int ManaCost => manaCost;
        public int[] AddedOptionIds => addedOptionIds;
        public int CardDescId => cardDescId;
        public int SortId => sortId;
        public string CardGridId => cardGridId;
        public string CardImgId => cardImgId;
        //public ToolAdditionalOption Option => option;
        //public int OptionValue => optionValue;
        
        public ToolExcelData(ToolType type, ToolTag tag, ToolAdditionalOption op, int val)
        {
           /* this.cardKey = type;
            this.toolTag = tag;
            this.option = op;
            this.optionValue = val;*/
        }

        public ToolExcelData(int id,string key,string localNKey,string localDKey,int cardType,int charType, int upType, string upKey, int rarity, int bit, int tagBit, int mana, int[] addOpId1,
            int cardDexId, int sort, string gridId, string imgId)
        {
            this.cardKeyNew = (ToolType)(id - 101);
            cardKeyString = key;
            cardNameKeyLocal = localNKey;
            cardDescKeyLocal = localDKey;
            this.cardTypeId = cardType;
            this.charaTypeId = charType;
            this.upgradeTypeId = upType;
            upgradeKey = upKey;
            rarityKey = rarity;
            carBit = bit;
            cardTagBit = cardDexId;
            manaCost = mana;
            addedOptionIds = addOpId1;
            cardDescId = cardDexId;
            sortId = sort;
            cardGridId = imgId;
            cardImgId = imgId;
        }
        
    }
    
    
    [CreateAssetMenu]
    public class ToolExcelDataTable : ScriptableObject
    {
        [SerializeField] private ToolExcelData[] _toolExcelDatas;
        
        public void SetToolExcelData(ToolExcelData[] arr) => _toolExcelDatas = arr;
        
        public Dictionary<ToolType, ToolExcelData> TryCache()
        {
            var ret = new Dictionary<ToolType, ToolExcelData>();
            for (int i = 0; i < _toolExcelDatas.Length; i++)
            {
                ret.Add(_toolExcelDatas[i].CardKey, _toolExcelDatas[i]);
            }

            return ret;
        }
    }
}