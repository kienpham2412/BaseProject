using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Tutorial : MonoBehaviour
{
    [field: SerializeField] protected TutorialType tutorialType;
    [SerializeField] protected Canvas canvas;
    public Board Board => GameplayController.Board;
    public GameplayMenu GameplayMenu { protected get; set; }
    public GameplayController GameplayController { protected get; set; }
    public Image BlackCover { protected get; set; }
    protected Collectibles collectibles;
    protected TutorialData tutorialData;
    protected Camera mainCam;
    protected RectTransform Wrapper => (RectTransform)transform.GetChild(0);
    protected Queue<UnityAction> tutSteps = new Queue<UnityAction>();
    
    protected void Reset()
    {
        canvas = GetComponent<Canvas>();
    }

    protected virtual void Awake()
    {
        tutorialData = DataController.Instance.GameData.TutorialData;
        collectibles = DataController.Instance.GameData.Collectibles;
        mainCam = Camera.main;
    }

    protected virtual void Start()
    {
        GameplayMenu.SettingButton.interactable = false;
        // GameplayMenu.RestartButton.interactable = false;
    }

    /// <summary>
    /// Add canvas to specific ui element to override parent's canvas properties
    /// </summary>
    /// <param name="gameObject"></param>
    protected void AddCanvasOverride(GameObject gameObject)
    {
        gameObject.gameObject.AddComponent<GraphicRaycaster>();
        gameObject.gameObject.AddComponent<Canvas>();
        var canvas = gameObject.GetComponent<Canvas>();
        canvas.overrideSorting = true;
        canvas.sortingLayerID = SortingLayer.NameToID("UI");
        canvas.sortingOrder = 11;
    }

    /// <summary>
    /// Remove canvas override to specific ui element
    /// </summary>
    /// <param name="gameObject"></param>
    protected void RemoveCanvasOverride(GameObject gameObject)
    {
        Destroy(gameObject.GetComponent<GraphicRaycaster>());
        Destroy(gameObject.GetComponent<Canvas>());
    }

    public virtual void CompleteTutorial()
    {
        GameplayMenu.SettingButton.interactable = true;
        // GameplayMenu.RestartButton.interactable = true;
        tutorialData.MarkTutorialAsCompleted(tutorialType);
        DataController.Instance.SaveData(this);
        Destroy(gameObject);
    }
    
    public virtual void CheckAndChangeStep()
    {
        if (tutSteps.Count > 0)
        {
            var tutStep = tutSteps.Dequeue();
            tutStep?.Invoke();
            return;
        }

        CompleteTutorial();
    }
}
