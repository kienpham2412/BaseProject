using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public enum PopupType
{
    Setting,
    Revive,
    Win,
    Lose,
    Shop,
    RemoveAd,
    Gift
}

public class PopupMenu : MonoBehaviour
{
    [field: SerializeField] public PopupType Type { get; private set; }
    [SerializeField] protected RectTransform wrapper;
    [SerializeField] protected Animator anim;
    [SerializeField] protected bool freezeTimeOnEnable = true;
    [SerializeField] protected bool unFreezeTimeOnDisable = true;
    public MenuBase Menu { get; private set; }
    public UnityAction CloseAction { get; set; }
    private Tween tweenAnimClose, tweenHandleSystem;
    private static int popupHide = Animator.StringToHash("Close");
    private static string closeClipName = "Close";
    private static string openClipName = "Open";
    public float CloseDuration { get; private set; }
    public float OpenDuration { get; private set; }

    private void Awake()
    {
        TryGetMenu();
        CloseDuration = GetDuration(closeClipName);
        OpenDuration = GetDuration(openClipName);
    }

    public void TryGetMenu()
    {
        if (Menu == null) Menu = wrapper.GetComponentInChildren<MenuBase>();
        if (Menu != null) Menu.Popup = this;
    }

    protected virtual void OnEnable()
    {
        if (freezeTimeOnEnable) Time.timeScale = 0;
    }

    public void Close()
    {
        anim.Play(popupHide);
        StartCoroutine(DelayInvoke(CloseDuration, OnClose));
    }

    private float GetDuration(string clipName)
    {
        RuntimeAnimatorController ac = anim.runtimeAnimatorController;

        foreach (var clip in ac.animationClips)
        {
            if (clip.name.CompareTo(closeClipName) == 0)
                return clip.length;
        }

        return 1;
    }

    private void OnClose()
    {
        gameObject.SetActive(false);
        CloseAction?.Invoke();
    }

    public void Display()
    {
        gameObject.SetActive(true);
    }

    protected virtual void OnDisable()
    {
        if (unFreezeTimeOnDisable) Time.timeScale = 1;
    }

    private IEnumerator DelayInvoke(float delay, UnityAction action)
    {
        yield return new WaitForSecondsRealtime(delay);
        action?.Invoke();
    }
}