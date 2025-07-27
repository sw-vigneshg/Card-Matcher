using UnityEditor;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] private int CurrentLevel = 0;
    [SerializeField] private int Score = 0;

    [SerializeField] private string LevelKey = "LEVEL";
    [SerializeField] private string ScoreKey = "SCORE";

    [SerializeField] private bool NeedToStorePlayerData = false;
    [SerializeField] private bool ClearAllPlayerData = false;

    [SerializeField] private GameManager Manager;

    private void Awake()
    {
        if (ClearAllPlayerData)
        {
            PlayerPrefs.DeleteAll();
            return;
        }

        if (NeedToStorePlayerData && PlayerPrefs.HasKey(LevelKey) && PlayerPrefs.HasKey(ScoreKey))
        {
            RetrivePlayerData();
        }
    }

    public void UpdateScore(int score)
    {
        Score += score;
        UIController.Instance.UpdateScoreText(Score);
    }

    public void UpdateLevel()
    {
        if (CurrentLevel > 0)
            UIController.Instance.ShowResultPanel(CurrentLevel);
        CurrentLevel += 1;
        UIController.Instance.UpdateLevelText(CurrentLevel);
    }

    public void StorePlayerData()
    {
        PlayerPrefs.SetInt(LevelKey, CurrentLevel);
        PlayerPrefs.SetInt(ScoreKey, Score);
        PlayerPrefs.Save();
    }

    public void RetrivePlayerData()
    {
        CurrentLevel = PlayerPrefs.GetInt(LevelKey);
        Score = PlayerPrefs.GetInt(ScoreKey);
    }

    public void OnExitGame()
    {
        if (NeedToStorePlayerData)
            StorePlayerData();

#if UNITY_EDITOR
        if (EditorApplication.isPlaying)
            EditorApplication.isPlaying = false;
#else 
        Application.Quit();
#endif
    }
}
