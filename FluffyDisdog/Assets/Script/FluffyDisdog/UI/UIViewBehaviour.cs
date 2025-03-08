using System;
using UnityEngine;

namespace FluffyDisdog.UI
{
    public enum UIType
    {
        None=-1,
        InGame=0,
        Login=1,
        Lobby=2
    }

    public class UIViewParam
    {
        
    }
    public class UIViewBehaviour : MonoBehaviour
    {
        public virtual UIType type { get; }

        public virtual void Init(UIViewParam param)
        {
            
        }

        protected virtual void Dispose()
        {
            
        }

        public virtual void CallEnd()
        {
            Dispose();
        }
    }
}