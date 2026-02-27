using System;
using System.Collections.Generic;
using FluffyDisdog;
using FluffyDisdog.Manager;
using FluffyDisdog.UI;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Script.FluffyDisdog.Managers
{
    public class GameManager:CustomSingleton<GameManager>
    {
        private List<AsyncOperationHandle<GameObject>> handles;
        private async void Start()
        {
            //handles=new AsyncOperationHandle<GameObject>();
            handles = new List<AsyncOperationHandle<GameObject>>();
            var uiManager = Addressables.LoadAssetAsync<GameObject>("UIManager");
            uiManager.Completed += _ =>
            {
                var res = _.Result as GameObject;
                var obj = GameObject.Instantiate(res);
            };
            await uiManager.Task;
            //Addressables.Release(uiManager);
            handles.Add(uiManager);
            
            var popup = Addressables.LoadAssetAsync<GameObject>("PopupManager");
            popup.Completed += _ =>
            {
                var res = _.Result as GameObject;
                var obj = GameObject.Instantiate(res);
            };
            await popup.Task;
           // Addressables.Release(popup);
           handles.Add(popup);
            
            var resLoader = Addressables.LoadAssetAsync<GameObject>("ResourceLoadManager");
            resLoader.Completed += _ =>
            {
                var res = _.Result as GameObject;
                var obj = GameObject.Instantiate(res);
            };
            await resLoader.Task;
            //Addressables.Release(resLoader);
            handles.Add(resLoader);
            
            //UIManager
        }

        private void OnApplicationQuit()
        {
            foreach (var handle in handles)
            {
                Addressables.Release(handle);
            }
        }
    }
}