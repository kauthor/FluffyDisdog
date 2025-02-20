using System;
using FluffyDisdog;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;

namespace Script.FluffyDisdog.Managers
{
    public class LoadSceneManager:CustomSingleton<LoadSceneManager>
    {
        public async void LoadScene(string sceneName, Action<AsyncOperationHandle<SceneInstance>> onComplete=null)
        {
            var handler = Addressables.LoadSceneAsync(sceneName);
            if(onComplete!=null)
                handler.Completed += onComplete;

            await handler.Task;
            
        }
        
    }
}