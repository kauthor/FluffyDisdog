using System;
using System.Collections.Generic;
using System.ComponentModel;
using FluffyDisdog;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace Script.FluffyDisdog.Managers
{
    public enum SoundTypeEnum
    {
        NONE=0,
        BGM=1,
        ENVIRONMENT=2,
        SFX=3
    }

    public enum SoundDesc
    {
        NONE=0,
        InGame1Bgm=1,
        Env1=2,
        StoreBgm=3,
        StageClearSfx=4,
        CoinSfx=5,
        CardSfx1=6,
        CardSfx2=7,
        PopupOpenSfx=8,
        TileDestroy1Sfx=9,
        TileDestroy2Sfx=10,
        TileCrackSfx=11,
        TileFailSfx=12,
        MultiTileDestroy1Sfx=13,
    }
    
    
    [Serializable]
    public class SoundGroup
    {
        [FormerlySerializedAs("soundEnum")] public SoundTypeEnum soundTypeEnum;
        public AudioClip audioClip;
        [FormerlySerializedAs("type")] public SoundDesc desc;
        [Description("오디오의 볼륨. 0~1 사이의 실수값으로, 볼륨 세팅과 곱셈적용")]
        [Range(0,1.0f)]public float volume=1;
        [Description("오디오의 피치. -3~3 사이의 실수값")]
        [Range(-3.0f,3.0f)]public float pitch=1;
    }
    public class SoundManager:CustomSingleton<SoundManager>
    {
        //[SerializeField]
        
        [SerializeField] private SoundGroup[] soundGroups;
        
        private Dictionary<SoundTypeEnum, Dictionary<SoundDesc, SoundGroup>> soundGroupDict = new Dictionary<SoundTypeEnum, Dictionary<SoundDesc, SoundGroup>>();

        //[slid(0.0f, 1.0f)]
        //[SerializeField] private float bgmVolume;
        
        //[MinMaxSlider(0.0f, 1.0f)]
        //[SerializeField] private float envVolume;
        //[MinMaxSlider(0.0f, 1.0f)]
        //[SerializeField] private float sfxVolume;


        [SerializeField] private AudioSource currentBgmQueue;
        private SoundGroup currentBgmSetting;
        [SerializeField] private AudioSource currentEnvQueue;
        private SoundGroup currentEnvSetting;
        [SerializeField] private AudioSource currentSfxQueue;
        private SoundGroup currentAloneSfxSetting;
        //private SoundGroup currentSfxSetting;
        
        protected override void Awake()
        {
            base.Awake();
            foreach (SoundGroup sg in soundGroups)
            {
                if(soundGroupDict.TryGetValue(sg.soundTypeEnum, out var dic))
                {
                    dic.Add(sg.desc, sg);
                }
                else
                {
                    var newDic = new Dictionary<SoundDesc, SoundGroup>();
                    newDic.Add(sg.desc, sg);
                    soundGroupDict.Add(sg.soundTypeEnum, newDic);
                }
            }

            currentBgmSetting = null;
            currentEnvSetting = null;
            currentAloneSfxSetting = null;
        }

        public void PlayBgm(SoundDesc soundEnum)
        {
            if (soundGroupDict.TryGetValue(SoundTypeEnum.BGM, out var dic)
                && dic.TryGetValue(soundEnum, out var soundGroup))
            {
                if(currentBgmQueue.clip!=null && currentBgmQueue.isPlaying)
                    currentBgmQueue.Stop();
                currentBgmSetting = soundGroup;
                currentBgmQueue.clip = soundGroup.audioClip;
                currentBgmQueue.volume = soundGroup.volume;
                currentBgmQueue.pitch = soundGroup.pitch;
                currentBgmQueue.Play();
            }
        }

        public void StopBgm()
        {
            if(currentBgmQueue.clip!=null && currentBgmQueue.isPlaying)
                currentBgmQueue.Stop();
            
            currentBgmSetting = null;
        }

        public void PlayEnv(SoundDesc soundEnum)
        {
            if (soundGroupDict.TryGetValue(SoundTypeEnum.ENVIRONMENT, out var dic)
                && dic.TryGetValue(soundEnum, out var soundGroup))
            {
                if(currentEnvQueue.clip!=null && currentEnvQueue.isPlaying)
                    currentEnvQueue.Stop();
                currentEnvSetting = soundGroup;
                currentEnvQueue.clip = soundGroup.audioClip;
                currentEnvQueue.volume = soundGroup.volume;
                currentEnvQueue.pitch = soundGroup.pitch;
                currentEnvQueue.Play();
            }
        }
        
        public void StopEnv()
        {
            if(currentEnvQueue.clip!=null && currentEnvQueue.isPlaying)
                currentEnvQueue.Stop();
            
            currentEnvSetting = null;
        }
        
        public void PlaySFX(SoundDesc soundEnum, bool randomPitch=false)
        {
            if (soundGroupDict.TryGetValue(SoundTypeEnum.SFX, out var dic)
                && dic.TryGetValue(soundEnum, out var soundGroup))
            {
                currentSfxQueue.volume = soundGroup.volume;
                currentSfxQueue.pitch = randomPitch? Random.Range(0.8f,1.2f): soundGroup.pitch;
                currentSfxQueue.PlayOneShot(soundGroup.audioClip);
            }
        }

        public void PlaySfxAlone(SoundDesc soundEnum, bool randomPitch = false)
        {
            if (soundGroupDict.TryGetValue(SoundTypeEnum.SFX, out var dic)
                && dic.TryGetValue(soundEnum, out var soundGroup))
            {
                currentSfxQueue.volume = soundGroup.volume;
                currentSfxQueue.pitch = randomPitch? Random.Range(0.8f,1.2f): soundGroup.pitch;
                currentSfxQueue.PlayOneShot(soundGroup.audioClip);
            }
        }

        public void PlaySfxRandom(SoundDesc[] soundEnums)
        {
            var sound = soundEnums[Random.Range(0, soundEnums.Length)];
            PlaySFX(sound);
        }
        
        public void StopSFX()
        {
            if(currentEnvQueue.isPlaying)
                currentEnvQueue.Stop();
            
            currentEnvSetting = null;
        }
        
    }
}