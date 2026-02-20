using System;
using UnityEngine;
using UnityEngine.UI;

public class AdButtonInteraction : ButtonInteraction
{
    [SerializeField] private GameObject activeContent, inActiveContent;
    private float time;
    private float updateInterval = 0.3f;

    private void Update()
    {
        if (Time.unscaledTime >= time)
        {
            time = Time.unscaledTime + updateInterval;
            UpdateButton();
        }
    }

    private void UpdateButton()
    {
        var rewardAdReady = AdController.Instance.IsVideoRewardAdsReady();
        activeContent.SetActive(rewardAdReady);
        inActiveContent.SetActive(!rewardAdReady);
    }
}