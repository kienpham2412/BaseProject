using System;
using UnityEngine;

[Serializable]
public class TutorialConfig
{
    [SerializeField] private int[] tutorialLevels;

    public bool IsTutorialAvailable(int level, out TutorialType tutorialType)
    {
        tutorialType = TutorialType.SelectionTutorial;
        for (int i = 0; i < tutorialLevels.Length; i++)
        {
            if (level == tutorialLevels[i])
            {
                tutorialType = (TutorialType)i;
                return true;
            }
        }

        return false;
    }

    public bool TutorialAvailable(int level, TutorialType type, out TutorialType tutorialType)
    {
        tutorialType = TutorialType.SelectionTutorial;
        for (int i = 0; i < tutorialLevels.Length; i++)
        {
            if (level == tutorialLevels[i] && type != (TutorialType)i)
            {
                tutorialType = (TutorialType)i;
                return true;
            }
        }

        return false;
    }
    
    public int GetLevel(TutorialType tutorialType)
    {
        var idx = Mathf.Clamp((int)tutorialType, 0, tutorialLevels.Length - 1);
        return tutorialLevels[idx];
    }
    
    public bool IsTutorial(int level)
    {
        foreach (var l in tutorialLevels)
        {
            if (l == level) return true;
        }

        return false;
    }
}