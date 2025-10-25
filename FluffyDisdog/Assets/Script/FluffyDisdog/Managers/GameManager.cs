using System;
using FluffyDisdog;
using FluffyDisdog.Manager;
using FluffyDisdog.UI;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Script.FluffyDisdog.Managers
{
    public class GameManager:CustomSingleton<GameManager>
    {
        private async void Start()
        {
            var uiManager = Addressables.LoadAssetAsync<GameObject>("UIManager");
            uiManager.Completed += _ =>
            {
                var res = _.Result as GameObject;
                var obj = GameObject.Instantiate(res);
            };
            await uiManager.Task;
            Addressables.Release(uiManager);
            
            var popup = Addressables.LoadAssetAsync<GameObject>("PopupManager");
            popup.Completed += _ =>
            {
                var res = _.Result as GameObject;
                var obj = GameObject.Instantiate(res);
            };
            await popup.Task;
            Addressables.Release(popup);
            
            var resLoader = Addressables.LoadAssetAsync<GameObject>("ResourceLoadManager");
            resLoader.Completed += _ =>
            {
                var res = _.Result as GameObject;
                var obj = GameObject.Instantiate(res);
            };
            await resLoader.Task;
            Addressables.Release(resLoader);
            
            //UIManager
            UIManager.I.ChangeView(UIType.Login);
        }

       
    }
}