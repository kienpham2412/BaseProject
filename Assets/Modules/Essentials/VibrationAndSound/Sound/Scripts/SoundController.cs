using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using Pixelplacement;

namespace Setting
{
    public class SoundController : Singleton<SoundController>
    {
        public AudioMixer audioMixer;
        public SoundProfile soundProfile;
        public AudioSource bgMusicSource;
        public AudioSource winLoseSource;
        public AudioSource[] sfxSources;
        private SettingData settingData;

        private void Awake()
        {
            settingData = DataController.Instance.GameData.SettingData;
            UpdateBackgroundMusicGroup();
            UpdateSfxGroup();
        }

        public void PlayBackgroundMusic(SFXType sfxType, bool force = false)
        {
            var clip = soundProfile.GetClipData(sfxType).audioClip;
            if (!force && bgMusicSource.clip == clip) return;
            
            bgMusicSource.clip = clip;
            bgMusicSource.Play();
        }

        private AudioSource GetSFXSource(int priority)
        {
            var idx = Mathf.Clamp(priority, 0, sfxSources.Length - 1);
            return sfxSources[idx];
        }

        public void PlaySFX(SFXType sfxType, int priority = 0)
        {
            var clipData = soundProfile.GetClipData(sfxType);
            if (clipData == null || clipData.audioClip == null) return;

            PlaySFX(clipData, priority);
        }

        private void PlaySFX(AudioClipData clipData, int priority = 0)
        {
            var source = GetSFXSource(priority);
            source.PlayOneShot(clipData.audioClip, clipData.volume);
        }

        public void PlayWinLoseSFX(SFXType sfxType)
        {
            var clipData = soundProfile.GetClipData(sfxType);
            winLoseSource.PlayOneShot(clipData.audioClip, clipData.volume);
        }

        public void UpdateBackgroundMusicGroup()
        {
            SetMixerGroup("BgMusic", settingData.GetSettingByType(SettingType.Music).value);
        }

        public void UpdateSfxGroup()
        {
            SetMixerGroup("SFX", settingData.GetSettingByType(SettingType.Sound).value);
        }

        private void SetMixerGroup(string name, bool turnOn = true)
        {
            audioMixer.SetFloat(name, turnOn ? 1f : -80f);
        }
    }
}