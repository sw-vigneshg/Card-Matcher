using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] private List<CardHandler> CardObjects = new();
    [SerializeField] private List<CardData> AllCardData = new();

    [SerializeField] private Transform CardsHolder;
    [SerializeField] private int RowCount;
    [SerializeField] private int ColoumnCount;
    [SerializeField] private int StreakCount;

    [SerializeField] private Color CardFront;
    [SerializeField] private Color WrongMatch;
    [SerializeField] private Color CorrectMatch;

    [SerializeField] private PoolManager PoolManager;
    [SerializeField] private CardHandler PreviousCard;
    [SerializeField] private CardHandler CurrentCard;
    private List<CardData> TempList = new List<CardData>();

    [SerializeField] private Camera MainCamera;
    [SerializeField] private GameController Controller;

    public bool IsGameOver = false;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        GenerateCardsData();
        RowCount = 2;
        ColoumnCount = 2;
        Controller.UpdateLevel();
        Controller.UpdateScore(0);
    }

    public void StartGame()
    {
        GenerateCards();
        AlignCamera();
        PreviousCard = null;
        CurrentCard = null;
        IsGameOver = false;
    }

    private void AlignCamera()
    {
        float centerX = (RowCount / 2f) - 0.5f;
        float centerY = (ColoumnCount / 2f) - 0.5f;
        MainCamera.transform.position = new Vector3(centerX, centerY, -10f);

        float screenAspect = (float)Screen.width / Screen.height;
        float requiredVerticalSize = (centerX / 2f + 1.5f) / screenAspect;
        float requiredHorizontalSize = (centerY / 2f + 1.5f) / screenAspect;

        MainCamera.orthographicSize = Mathf.Max(requiredVerticalSize, requiredHorizontalSize);
    }

    public void GenerateCardsData()
    {
        if (AllCardData.Count > 0)
            AllCardData.Clear();

        CardData tempCard = new CardData();

        for (int i = 0; i < 100; i++)
        {
            tempCard = new()
            {
                CardIndex = i,
                CardName = i.ToString(),
                IsFlipped = false,
            };
            AllCardData.Add(tempCard);
        }
    }

    public void GenerateCards()
    {
        if (CardObjects.Count > 0)
        {
            foreach (CardHandler item in CardObjects)
            {
                PoolManager.ReturnCardToPool(item);
            }
            Debug.Log("Card Objects list is not Empty.So clearing CardsList");
        }

        int cardIndex = 0;
        Vector3 position;
        TempList = GetCardDetails();

        for (int i = 0; i < RowCount; i++)
        {
            for (int j = 0; j < ColoumnCount; j++)
            {
                position.x = i;
                position.y = j;
                position.z = 0f;

                CardHandler card = PoolManager.GetCards();
                if (card != null)
                {
                    card.transform.parent = CardsHolder.transform;
                    card.transform.localPosition = position;

                    CardData tempData = new()
                    {
                        CardIndex = TempList[cardIndex].CardIndex,
                        CardName = TempList[cardIndex].CardName,
                        IsFlipped = false,
                    };
                    card.AssignCardData(tempData);
                    cardIndex++;

                    CardObjects.Add(card);
                }
            }
        }
        ShuffleCards();
    }

    private List<CardData> GetCardDetails()
    {
        List<CardData> tempList = new List<CardData>();
        int totalCards = RowCount * ColoumnCount;

        int randomCount = 0;
        int randomIndex = 0;
        randomCount = 2;

        while (tempList.Count < totalCards)
        {
            randomIndex = Random.Range(0, AllCardData.Count);
            if (!tempList.Exists(x => x.CardIndex == AllCardData[randomIndex].CardIndex))
            {
                for (int i = 0; i < randomCount; i++)
                {
                    CardData card = new CardData()
                    {
                        CardIndex = AllCardData[randomIndex].CardIndex,
                        CardName = AllCardData[randomIndex].CardName,
                        IsFlipped = false
                    };
                    tempList.Add(card);
                    if (tempList.Count >= totalCards)
                        break;
                }
            }
        }
        return tempList;
    }

    private void ShuffleCards()
    {
        //int randomIndex;
        //for (int i = 0; i < CardObjects.Count; i++)
        //{
        //    randomIndex = Random.Range(0, CardObjects.Count);
        //    (CardObjects[i].MyCardData, CardObjects[randomIndex].MyCardData) = (CardObjects[randomIndex].MyCardData, CardObjects[i].MyCardData);
        //}

        foreach (CardHandler card in CardObjects)
        {
            card.AssignCardNameText();
        }
    }

    public void ValidateSelectedCards(CardHandler card)
    {
        if (card == null)
            return;

        if (PreviousCard == null)
        {
            PreviousCard = card;
            return;
        }
        CurrentCard = card;
        StartCoroutine(HighlightCard(PreviousCard, CurrentCard));
        PreviousCard = null;
        CurrentCard = null;
    }

    private IEnumerator HighlightCard(CardHandler card1, CardHandler card2)
    {
        yield return new WaitForSeconds(0.25f);
        if (card1 != null && card2 != null)
        {
            CardHandler tempCard1 = CardObjects.Find(x => x == card1 && x.MyCardData.IsFlipped);
            CardHandler tempCard2 = CardObjects.Find(x => x == card2 && x.MyCardData.IsFlipped);
            bool isMatched = tempCard1.MyCardData.CardIndex == tempCard2.MyCardData.CardIndex;
            bool isForceFlip = (RowCount == 3 && ColoumnCount == 3 && CardObjects.Where(x => x.gameObject.activeInHierarchy).Count() == 1);

            StreakCount = isMatched ? (StreakCount + 1) : 0;
            UIController.Instance.UpdateStreakText(StreakCount);

            Debug.Log($"Streak Count : {StreakCount}");

            if (tempCard1 != null && tempCard2 != null)
            {
                if (isMatched)
                {
                    if (isForceFlip)
                    {
                        CardHandler tempCard3 = CardObjects.Find(x => x.gameObject.activeInHierarchy);
                        if (tempCard3 != null)
                        {
                            tempCard3.FlipCard();
                            tempCard3.OnMatch(CorrectMatch);
                        }
                    }
                    AudioController.Instance.PlayAudio(AudioType.PerfectMatch);
                    tempCard1.OnMatch(CorrectMatch);
                    tempCard2.OnMatch(CorrectMatch);
                    yield return new WaitForSeconds(0.5f);
                    Controller.UpdateScore(StreakCount >= 2 ? (StreakCount * 10) + 2 : 2);
                    tempCard1.gameObject.SetActive(false);
                    tempCard2.gameObject.SetActive(false);
                    if (isForceFlip)
                    {
                        CardObjects.Find(x => !x.MyCardData.IsFlipped)?.gameObject.SetActive(false);
                        Controller.UpdateScore(1);
                    }
                    CheckGameOver();
                }
                else
                {
                    AudioController.Instance.PlayAudio(AudioType.WrongMatch);
                    tempCard1.OnMatch(CorrectMatch);
                    tempCard2.OnMatch(WrongMatch);
                    yield return new WaitForSeconds(0.5f);
                    tempCard1.OnMatch(CardFront);
                    tempCard2.OnMatch(CardFront);
                    tempCard1.OnReset();
                    tempCard2.OnReset();
                }
            }
        }
    }

    private void CheckGameOver()
    {
        if (!CardObjects.Exists(x => !x.MyCardData.IsFlipped))
        {
            IsGameOver = true;
            AudioController.Instance.PlayAudio(AudioType.GameOver);
            RowCount += 1;
            ColoumnCount += 1;
            foreach (CardHandler item in CardObjects)
            {
                item.OnReset();
            }
            CancelInvoke(nameof(RestartGame));
            Invoke(nameof(RestartGame), 1f);
        }
    }

    private void RestartGame()
    {
        Controller.UpdateLevel();
        StreakCount = 0;
    }
}

[System.Serializable]
public class CardData
{
    public int CardIndex = -1;
    public string CardName = string.Empty;
    public bool IsFlipped = false;
}