using System;
using UnityEngine;
using UnityEngine.UI;

namespace FluffyDisdog.UI.Part
{
    public class RequestResultCardPart:RequestResultPart
    {
        public override void Init(RequestReward reward, Action<bool> onclick)
        {
            base.Init(reward, onclick);
        }
    }
}