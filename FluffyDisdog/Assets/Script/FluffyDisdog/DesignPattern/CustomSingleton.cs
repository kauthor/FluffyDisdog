using System;
using UnityEngine;

namespace FluffyDisdog
{
    public class CustomSingleton<T> : MonoBehaviour
    {
        private static T Instance;
        public static T GetInstance() => Instance;

        public static T I => GetInstance();

        public static bool ExistInstance() => Instance != null;

        protected virtual void Awake()
        {
            if (Instance == null)
            {
                Instance = GetComponent<T>();
                DontDestroyOnLoad(this.gameObject);
            }
            else
            {
                this.enabled = false;
            }
        }
    }
}