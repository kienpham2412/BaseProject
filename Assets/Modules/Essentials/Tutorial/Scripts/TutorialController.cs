using UnityEngine;

public class TutorialController : MonoBehaviour
{
    [SerializeField] private Transform tutorialContainer;
    public Tutorial TutorialInstance { get; private set; }
    private TutorialData tutorialData;
    private TutorialConfig tutorialConfig;
    private LevelData levelData;
    private int Level => levelData.level;
    public Board Board { private get; set; }

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        tutorialData = DataController.Instance.GameData.TutorialData;
        levelData = DataController.Instance.GameData.LevelData;
        tutorialConfig = ConfigController.Instance.TutorialConfig;
    }

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        CheckAndDisplayTutorial();
    }

    private void CheckAndDisplayTutorial()
    {
        var isTutorialAvailable = tutorialConfig.IsTutorialAvailable(Level, out TutorialType tutorial);
        var isTutorialComplete = tutorialData.IsTutorialCompleted(tutorial);
        if (isTutorialAvailable && !isTutorialComplete)
        {
            TutorialInstance = Resources.Load<Tutorial>(tutorial.ToString());
            if (TutorialInstance == null) return;
            TutorialInstance = Instantiate(TutorialInstance, tutorialContainer);
        }
    }
}
