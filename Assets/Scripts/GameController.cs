using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    [SerializeField] private int CurrentLevel = 0;
    [SerializeField] private int Score = 0;
    [SerializeField] private string LevelKey = "LEVEL";
    [SerializeField] private string ScoreKey = "SCORE";
    [SerializeField] private string ToggleKey1 = "STORE";
    [SerializeField] private string ToggleKey2 = "CLEAR";
    [SerializeField] private bool NeedToStorePlayerData = false;
    [SerializeField] private bool ClearAllPlayerData = false;

    [SerializeField] private GameManager Manager;
    [SerializeField] private UIController UIController;
    [SerializeField] private Toggle ClearToggle;
    [SerializeField] private Toggle PlayerPrefToggle;

    private void Awake()
    {
        GetToggleValues();
        if (ClearAllPlayerData)
        {
            PlayerPrefs.DeleteAll();
        }
        OnInit();
    }

    private void GetToggleValues()
    {
        ClearToggle.isOn = PlayerPrefs.HasKey(ToggleKey2) ? bool.Parse(PlayerPrefs.GetString(ToggleKey2)) : false;
        PlayerPrefToggle.isOn = PlayerPrefs.HasKey(ToggleKey1) ? bool.Parse(PlayerPrefs.GetString(ToggleKey1)) : false;
        ClearToggle.onValueChanged.AddListener(delegate { ToggleValue(ClearToggle.isOn, true); });
        PlayerPrefToggle.onValueChanged.AddListener(delegate { ToggleValue(PlayerPrefToggle.isOn, false); });

        Debug.Log($"PlayerPrefToggle : {PlayerPrefToggle.isOn}, ClearToggle : {ClearToggle.isOn}");
    }

    private void ToggleValue(bool isOn, bool isClear)
    {
        if (isClear)
            ClearToggle.isOn = isOn;
        else
            PlayerPrefToggle.isOn = isOn;

        NeedToStorePlayerData = PlayerPrefToggle.isOn;
        ClearAllPlayerData = ClearToggle.isOn;
    }

    private void OnInit()
    {
        CurrentLevel = 1;
        UIController.UpdateLevelText(CurrentLevel);
        UpdateScore(0);
    }

    public void UpdateScore(int score)
    {
        Score += score;
        UIController.UpdateScoreText(Score);
    }

    public void UpdateLevel()
    {
        if (CurrentLevel > 0)
            UIController.ShowResultPanel(CurrentLevel);
        CurrentLevel += 1;
        if (CurrentLevel >= 10)
        {
            PlayerPrefs.DeleteKey(ScoreKey);
            PlayerPrefs.DeleteKey(LevelKey);
            PlayerPrefs.Save();
        }
        UIController.UpdateLevelText(CurrentLevel);
    }

    public void StorePlayerData()
    {
        if (NeedToStorePlayerData)
        {
            PlayerPrefs.SetInt(LevelKey, CurrentLevel);
            PlayerPrefs.SetInt(ScoreKey, Score);
        }
        PlayerPrefs.SetString(ToggleKey1, PlayerPrefToggle.isOn.ToString().ToLower());
        PlayerPrefs.SetString(ToggleKey2, ClearToggle.isOn.ToString().ToLower());
        PlayerPrefs.Save();
    }

    public void RetrrivePlayerData()
    {
        if (NeedToStorePlayerData && PlayerPrefs.HasKey(LevelKey) && PlayerPrefs.HasKey(ScoreKey))
        {
            GetPlayerPrefData();
        }
    }

    private void GetPlayerPrefData()
    {
        CurrentLevel = PlayerPrefs.GetInt(LevelKey);
        Score = PlayerPrefs.GetInt(ScoreKey);
        UIController.UpdateLevelText(CurrentLevel);
        UIController.UpdateScoreText(Score);
        Manager.OnPlayerDataLoad(CurrentLevel);
    }

    public void OnExitGame()
    {
#if UNITY_EDITOR
        StorePlayerData();
        if (EditorApplication.isPlaying)
            EditorApplication.isPlaying = false;
#else 
        Application.Quit();
#endif
    }

    private void OnApplicationQuit()
    {
        StorePlayerData();
    }
}
