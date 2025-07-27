using System;
using UnityEngine;

namespace FluffyDisdog.Data
{
    [Serializable]
    public class RequestData
    {
        
    }
    public class RequestTable:ScriptableObject
    {
        [SerializeField] private RequestData datas;
    }
}