using System;
using System.Collections.Generic;
using UnityEngine;

public enum TutorialType
{
    SelectionTutorial,
    TurtleTutorial,
    JumpOverTutorial
}

[Serializable]
public class TutorialCompletion
{
    public TutorialType type;
    public bool isCompleted;
}

[Serializable]
public class TutorialData
{
    [SerializeField] private List<TutorialCompletion> tutorialCompletions;

    public TutorialData()
    {
        tutorialCompletions = new List<TutorialCompletion>();
    }

    private TutorialCompletion GetTutorialCompletion(TutorialType type)
    {
        foreach (var t in tutorialCompletions)
            if (t.type == type)
                return t;

        var newInfo = new TutorialCompletion
        {
            type = type,
            isCompleted = false
        };

        tutorialCompletions.Add(newInfo);
        return newInfo;
    }

    public bool IsTutorialCompleted(TutorialType type)
    {
        var tut = GetTutorialCompletion(type);
        return tut.isCompleted;
    }

    public void MarkTutorialAsCompleted(TutorialType type)
    {
        var tut = GetTutorialCompletion(type);
        tut.isCompleted = true;
    }
}