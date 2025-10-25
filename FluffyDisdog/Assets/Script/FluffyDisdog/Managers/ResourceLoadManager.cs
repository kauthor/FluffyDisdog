using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using FluffyDisdog;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.U2D;

namespace Script.FluffyDisdog.Managers
{
    public class ResourceLoadManager:CustomSingleton<ResourceLoadManager>
    {
        private Dictionary<string, GameObject> cache = new Dictionary<string, GameObject>();
        private SpriteAtlas _cardImageAtlas;
        private SpriteAtlas _cardGridImageAtlas;
        private SpriteAtlas _cardTagIconAtlas;
        private SpriteAtlas _relicIconAtlas;

        protected override void Awake()
        {
            base.Awake();
            cache = new Dictionary<string, GameObject>();
        }

        
        
        public async UniTaskVoid LoadGameObjectResource(string key, Action<GameObject> loadEnd) 
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

        public async UniTaskVoid LoadCardImage(string key, Action<Sprite> loadEnd)
        {
            if (_cardImageAtlas == null)
            {
                var loader = Addressables.LoadAssetAsync<SpriteAtlas>("UI/CardImage");
                loader.Completed += _ =>
                {
                    var res = _.Result as SpriteAtlas;
                    _cardImageAtlas = res;
                };
                await loader.Task;
                Addressables.Release(loader);
            }
            
            var ret = _cardImageAtlas.GetSprite(key);
            loadEnd?.Invoke(ret);
            
        }
        
        public async UniTaskVoid LoadCardTagIconImage(string key, Action<Sprite> loadEnd)
        {
            if (_cardTagIconAtlas == null)
            {
                var loader = Addressables.LoadAssetAsync<SpriteAtlas>("UI/CardTagIcon");
                loader.Completed += _ =>
                {
                    var res = _.Result as SpriteAtlas;
                    _cardTagIconAtlas = res;
                };
                await loader.Task;
                Addressables.Release(loader);
            }
            
            var ret = _cardTagIconAtlas.GetSprite(key);
            loadEnd?.Invoke(ret);
            
        }
        
        public async UniTaskVoid LoadCardIconImage(string key, Action<Sprite> loadEnd)
        {
            if (_cardGridImageAtlas == null)
            {
                var loader = Addressables.LoadAssetAsync<SpriteAtlas>("UI/CardIcon");
                loader.Completed += _ =>
                {
                    var res = _.Result as SpriteAtlas;
                    _cardGridImageAtlas = res;
                };
                await loader.Task;
                Addressables.Release(loader);
            }
            
            var ret = _cardGridImageAtlas.GetSprite(key);
            loadEnd?.Invoke(ret);
            
        }
        public async UniTaskVoid LoadRelicIcon(string key, Action<Sprite> loadEnd)
        {
            if (_relicIconAtlas == null)
            {
                var loader = Addressables.LoadAssetAsync<SpriteAtlas>("Atlas/RelicIcon");
                loader.Completed += _ =>
                {
                    var res = _.Result as SpriteAtlas;
                    _relicIconAtlas = res;
                };
                await loader.Task;
                Addressables.Release(loader);
            }
            
            var ret = _relicIconAtlas.GetSprite(key);
            loadEnd?.Invoke(ret);
            
        }
        
    }
}