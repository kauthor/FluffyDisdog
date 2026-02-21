using System;
using System.Collections;
using System.Collections.Generic;
using FluffyDisdog.Manager;
using FluffyDisdog.UI;
using GifImporter;
using Script.FluffyDisdog.Managers;
using UnityEngine;

public class LogoScene : MonoBehaviour
{
    [SerializeField] private GifPlayer player;

    private void Start()
    {
        player?.BindHandler(OnEndGif);
    }

    private void OnEndGif()
    {
        LoadSceneManager.I.LoadScene("Login", _ =>
        {
            UIManager.I.ChangeView(UIType.Login);
        });
    }
}
