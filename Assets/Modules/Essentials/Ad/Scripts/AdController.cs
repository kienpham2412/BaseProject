using System;
using Pixelplacement;
using UnityEngine;
using UnityEngine.Events;

public interface IAdsInterface
{
    bool IsVideoRewardAdsReady();
    void ShowVideoAds(UnityAction onAdClosed = null);
    bool IsInterstitialAdsReady();
    void ShowInterstitial(UnityAction onAdClosed);
    void DestroyBanner();
}

public class AdController : Singleton<AdController>
{
    [SerializeField] private bool rewardAdReady = true;
    private IAdsInterface adsInterface;
    private AdConfig adConfig;
    private LevelData levelData;
    private Collectible removeAdCollectible;
    private float interstitialTime;

    private void Awake()
    {
        adsInterface = GetComponentInChildren<IAdsInterface>();
        adConfig = ConfigController.Instance.AdConfig;
        levelData = DataController.Instance.GameData.LevelData;

        var c = DataController.Instance.GameData.Collectibles;
        removeAdCollectible = c.GetCollectibleById(CollectibleType.RemoveAd);
    }

    private void UpdateInterstitialTime()
    {
        interstitialTime = Time.time + adConfig.interstitialCooldown;
    }

    public bool IsVideoRewardAdsReady()
    {
#if UNITY_EDITOR
        return rewardAdReady;
#else
        return adsInterface.IsVideoRewardAdsReady();
#endif
    }

    public void ShowVideoAds(UnityAction onAdClosed = null)
    {
#if UNITY_EDITOR
        onAdClosed?.Invoke();
#else
        UpdateInterstitialTime();
        adsInterface.ShowVideoAds(onAdClosed);
#endif
    }

    public bool IsInterstitialAdsReady()
    {
#if UNITY_EDITOR
        return true;
#else
        if (removeAdCollectible.Amount > 0) return false;
        if (levelData.lastestUnlockedLevel < adConfig.enableInterstitialFromLevel) return false;
        if (Time.time < interstitialTime) return false;
        return adsInterface.IsInterstitialAdsReady();
#endif
    }

    public void ShowInterstitial(UnityAction onAdClosed = null)
    {
#if UNITY_EDITOR
        onAdClosed?.Invoke();
#else
        UpdateInterstitialTime();
        adsInterface.ShowInterstitial(onAdClosed);
#endif
    }

    public void DestroyBanner()
    {
#if !UNITY_EDITOR
        adsInterface.DestroyBanner();
#endif
    }
}