using System;
using Script.FluffyDisdog.Managers;
using UnityEngine;

namespace FluffyDisdog
{
    public class SceneRegardableSingleton<T>:MonoBehaviour
    {
        private static T Instance;
        public static T GetInstance() => Instance;

        public static T I => GetInstance();

        public static bool ExistInstance() => Instance != null;

        protected virtual void Awake()
        {
            Instance ??= GetComponent<T>();
            
        }

        private void Start()
        {
            LoadSceneManager.I.BindSceneAction(OnSceneEnd);
        }

        private void OnSceneEnd()
          => Instance = default(T);
    }
}