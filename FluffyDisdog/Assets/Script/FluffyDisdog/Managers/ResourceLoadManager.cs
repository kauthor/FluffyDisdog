using System;
using System.Collections.Generic;
using FluffyDisdog;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Script.FluffyDisdog.Managers
{
    public class ResourceLoadManager:CustomSingleton<ResourceLoadManager>
    {
        private Dictionary<string, GameObject> cache = new Dictionary<string, GameObject>();

        protected override void Awake()
        {
            base.Awake();
            cache = new Dictionary<string, GameObject>();
        }

        
        
        public async void LoadGameObjectResource(string key, Action<GameObject> loadEnd) 
        {
            if (cache.TryGetValue(key, out var value))
            {
                var obj = GameObject.Instantiate(value);
                loadEnd?.Invoke(obj);
                return;
            }
            
            var loader = Addressables.LoadAssetAsync<GameObject>(key);
            loader.Completed += _ =>
            {
                var res = _.Result as GameObject;
                cache.Add(key, res);
            };
            await loader.Task;
            Addressables.Release(loader);
            
            var ob = GameObject.Instantiate(cache[key]);
            loadEnd?.Invoke(ob);
        }
    }
}