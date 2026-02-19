using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Setting
{
    public enum SFXType
    {
        MainMenuBG = 0,
        GameplayBG = 1,
        ButtonClick = 3,
        WaterSplash = 4,
        Jump = 5,
        Landing = 6,
        ObstacleCollide = 7,
        Confetti = 8, 
        Win = 9,
        Lose = 10,
        WhistleDown = 11
    }
    
    [Serializable]
    public class AudioClipData
    {
        public SFXType type;
        public AudioClip audioClip;
        [Range(0, 1)] public float volume = 0.5f;
    }

    [CreateAssetMenu(fileName = "SoundProfile", menuName = "SoundProfile")]
    public class SoundProfile : ScriptableObject
    {
        [SerializeField] private AudioClipData[] audioClipDatas;

        public AudioClipData GetClipData(SFXType sfxType)
        {
            foreach (var c in audioClipDatas)
                if (c.type == sfxType)
                    return c;

            return null;
        }
    }
}