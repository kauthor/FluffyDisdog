using System;
using UnityEngine;

namespace FluffyDisdog.UI
{
    public class Sparkle:MonoBehaviour
    {
        private float temp = 0;
        private float life = 0.5f;
        private void Awake()
        {
            
        }

        private void Update()
        {
            temp += Time.deltaTime;
            if (life <= temp)
            {
                gameObject.SetActive(false);
                this.enabled = false;
            }
        }
    }
}