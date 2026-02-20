using System;
using UnityEngine;
using UnityEngine.UI;

public class AdButtonInteraction : ButtonInteraction
{
    [SerializeField] private Image buttonImage;
    [SerializeField] private Sprite inactiveSprite;
    [SerializeField] private GameObject activeContent, inActiveContent;
    private Sprite activeSprite;
    private float time;
    private float updateInterval = 0.3f;

    private void Awake()
    {
        activeSprite = buttonImage.sprite;
    }

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
        buttonImage.sprite = rewardAdReady ? activeSprite : inactiveSprite;
        activeContent.SetActive(rewardAdReady);
        inActiveContent.SetActive(!rewardAdReady);
    }
}