using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public static UIController Instance;

    [SerializeField] private Button StartGame;
    [SerializeField] private Button ExitGame;
    [SerializeField] private Button NextLevel;
    [SerializeField] private TMP_Text ScoreText;
    [SerializeField] private TMP_Text LevelText;
    [SerializeField] private TMP_Text StreakText;
    [SerializeField] private TMP_Text ResultInfo;
    [SerializeField] private GameObject LobbyPanel;
    [SerializeField] private GameObject InGamePanel;
    [SerializeField] private GameObject CardsHoler;
    [SerializeField] private GameObject ResultPanel;

    [SerializeField] private GameController Controller;
    [SerializeField] private GameManager Manager;

    private void Awake()
    {
        Instance = this;

        if (Controller != null && Manager != null)
        {
            StartGame.onClick.AddListener(() => { OnStartGame(true, true); });
            ExitGame.onClick.AddListener(() => { Controller.OnExitGame(); });
            NextLevel.onClick.AddListener(() => { OnStartGame(true, true); });
        }

        OnStartGame(false);
    }

    public void OnStartGame(bool isOn, bool isGameStarts = false)
    {
        CardsHoler.SetActive(isOn);
        InGamePanel.SetActive(isOn);
        LobbyPanel.SetActive(!isOn);
        ResultPanel.SetActive(false);
        if (isGameStarts)
            Manager.StartGame();
    }

    public void ShowResultPanel(int level)
    {
        InGamePanel.SetActive(false);
        ResultPanel.SetActive(true);
        ResultInfo.text = $"\n CONGRATULATIONS!\n \nLevel {level} is Completed!\n";
    }

    public void UpdateLevelText(int level)
    {
        LevelText.text = $"LEVEL : {level}";
    }

    public void UpdateScoreText(int score)
    {
        ScoreText.text = $"SCORE : {score}";
    }

    public void UpdateStreakText(int count)
    {
        StreakText.text = $"STREAKS : {count}";
    }
}
