using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using FluffyDisdog;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.U2D;

namespace Script.FluffyDisdog.Managers
{
    public static class ResourceAddress
    {
        public static string RelicIcon => "Atlas/RelicIcon";
        public static string CardEffect => "Atlas/CardEffect";
        public static string CardIllust => "Atlas/CardIllust";
        public static string CardRarity => "Atlas/CardRarity";
        public static string CardTag => "Atlas/CardTag";
    }
    public class ResourceLoadManager:CustomSingleton<ResourceLoadManager>
    {
        private Dictionary<string, GameObject> cache = new Dictionary<string, GameObject>();
        private Dictionary<string, SpriteAtlas> atlasCache = new Dictionary<string, SpriteAtlas>();

        protected override void Awake()
        {
            base.Awake();
            cache = new Dictionary<string, GameObject>();
            atlasCache = new Dictionary<string, SpriteAtlas>();
        }

        private async void Start()
        {
            string[] group = new string[5]
            {
                ResourceAddress.RelicIcon, ResourceAddress.CardEffect, ResourceAddress.CardIllust,
                ResourceAddress.CardRarity, ResourceAddress.CardTag
            };

            foreach (var address in group)
            {
                if(atlasCache.TryGetValue(address, out var value))
                    continue;
                else
                {
                    var loader = Addressables.LoadAssetAsync<SpriteAtlas>(address);
                    loader.Completed += _ =>
                    {
                        var res = _.Result as SpriteAtlas;
                        atlasCache.TryAdd(address, res);
                    };
                    await loader.Task;
                    Addressables.Release(loader);
                }
            }
            
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

        public async UniTaskVoid LoadSpriteAtlasResource(string address, string key, Action<Sprite> loadEnd)
        {
            if (key == "0" || key == "")
                return;
            
            SpriteAtlas ret = null;
            if(atlasCache.TryGetValue(address, out var value))
                ret = value;
            else
            {
                var loader = Addressables.LoadAssetAsync<SpriteAtlas>(address);
                loader.Completed += _ =>
                {
                    var res = _.Result as SpriteAtlas;
                    ret = res;
                    atlasCache.TryAdd(address, res);
                };
                await loader.Task;
                Addressables.Release(loader);
            }
            
            var sprite = ret.GetSprite(key);
            loadEnd?.Invoke(sprite);
        }

        public async UniTaskVoid LoadTagIcon(int key, Action<List<Sprite>> loadEnd)
        {
            SpriteAtlas ret = null;
            if(atlasCache.TryGetValue(ResourceAddress.CardTag, out var value))
                ret = value;
            else
            {
                var loader = Addressables.LoadAssetAsync<SpriteAtlas>(ResourceAddress.CardTag);
                loader.Completed += _ =>
                {
                    var res = _.Result as SpriteAtlas;
                    ret = res;
                    atlasCache.TryAdd(ResourceAddress.CardTag, res);
                };
                await loader.Task;
                Addressables.Release(loader);
            }

            int current = key;
            List<Sprite> onEnd = new List<Sprite>();

            for (int i = 0; current > 0; i++)
            {
                int bit = current % 2;
                current = current / 2;
                if (bit > 0)
                {
                    var rawData = ExcelManager.I.GetTagData(i + 1);
                    onEnd.Add(ret.GetSprite(rawData.tagImage));
                }
            }
            loadEnd?.Invoke(onEnd);
        }
        
    }
}