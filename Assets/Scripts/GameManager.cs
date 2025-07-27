using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] private List<CardHandler> CardObjects = new();
    [SerializeField] private CardData[] AllCardData;

    [SerializeField] private int RowCount;
    [SerializeField] private int ColoumnCount;

    [SerializeField] private Color CardFront;
    [SerializeField] private Color WrongMatch;
    [SerializeField] private Color CorrectMatch;

    [SerializeField] private PoolManager PoolManager;
    [SerializeField] private CardHandler PreviousCard;
    [SerializeField] private CardHandler CurrentCard;
    [SerializeField] private List<CardHandler> SelectedCards = new();

    [SerializeField] private Camera MainCamera;

    public bool IsGameOver = false;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        GenerateCards();
        PreviousCard = null;
        CurrentCard = null;
        IsGameOver = false;
    }

    public List<CardData> TempList = new List<CardData>();

    public void GenerateCards()
    {
        if (CardObjects.Count > 0)
        {
            foreach (CardHandler item in CardObjects)
            {
                PoolManager.ReturnCardToPool(item);
            }
            Debug.LogError("Card Objects list is not Empty");
        }

        int cardIndex = 0;
        CardData tempData = new CardData();

        float spaceX = 1.3f;
        float spaceY = 1.5f;
        Vector3 position;

        TempList = GetCardDetails();

        for (int i = 0; i < RowCount; i++)
        {
            for (int j = 0; j < ColoumnCount; j++)
            {
                position.x = i * spaceX;
                position.y = j * spaceY;
                position.z = 0f;

                CardHandler card = PoolManager.GetCards();
                if (card != null)
                {
                    card.transform.parent = this.transform;
                    card.transform.localPosition = position;

                    tempData = new()
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
        bool isEven = (totalCards % 2 == 0);

        while (tempList.Count < totalCards)
        {
            randomIndex = Random.Range(0, AllCardData.Length);
            randomCount = isEven ? 2 : Random.Range(2, 4);

            if (tempList.Count <= 0 || !tempList.Exists(x => x.CardIndex == AllCardData[randomIndex].CardIndex))
            {
                for (int i = 0; i < randomCount; i++)
                {
                    tempList.Add(new CardData()
                    {
                        CardIndex = AllCardData[randomIndex].CardIndex,
                        CardName = AllCardData[randomIndex].CardName,
                        IsFlipped = false,
                    });
                }
            }
            else
            {
                randomCount = Random.Range(0, tempList.Count);
                tempList.Add(new CardData()
                {
                    CardIndex = tempList[randomIndex].CardIndex,
                    CardName = tempList[randomIndex].CardName,
                    IsFlipped = false,
                });
            }
        }

        return tempList;
    }

    private void ShuffleCards()
    {
        int randomIndex;
        for (int i = 0; i < CardObjects.Count; i++)
        {
            randomIndex = Random.Range(0, CardObjects.Count);
            (CardObjects[i].MyCardData, CardObjects[randomIndex].MyCardData) = (CardObjects[randomIndex].MyCardData, CardObjects[i].MyCardData);
        }

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
            AddToSelectedCards(card);
            return;
        }

        CurrentCard = card;
        StopCoroutine(nameof(HighlightCard));
        StartCoroutine(nameof(HighlightCard));
    }

    private IEnumerator HighlightCard()
    {
        yield return new WaitForSeconds(0.4f);
        AddToSelectedCards(CurrentCard);
        if (PreviousCard != null && CurrentCard != null)
        {
            if (CurrentCard.MyCardData.CardIndex == PreviousCard.MyCardData.CardIndex)
            {
                if (CardObjects.Exists(x => x == CurrentCard && x.MyCardData.IsFlipped))
                {
                    CardObjects.Find(x => x == CurrentCard && x.MyCardData.IsFlipped).OnMatch(CorrectMatch);
                }

                if (CardObjects.Exists(x => x == PreviousCard && x.MyCardData.IsFlipped))
                {
                    CardObjects.Find(x => x == PreviousCard && x.MyCardData.IsFlipped).OnMatch(CorrectMatch);
                }
                AudioController.Instance.PlayAudio(AudioType.PerfectMatch);
                CancelInvoke(nameof(DisableCards));
                Invoke(nameof(DisableCards), 2f);
            }
            else
            {
                foreach (CardHandler item in SelectedCards)
                {
                    if (CardObjects.Exists(x => x == item && x.MyCardData.IsFlipped))
                    {
                        CardObjects.Find(x => x == item && x.MyCardData.IsFlipped).OnMatch(WrongMatch);
                    }
                }
                AudioController.Instance.PlayAudio(AudioType.WrongMatch);
                CancelInvoke(nameof(DisableCards));
                CancelInvoke(nameof(OnResetCards));
                Invoke(nameof(OnResetCards), 2f);
            }
        }
    }

    private void DisableCards()
    {
        foreach (CardHandler item in SelectedCards)
        {
            item.DisableCard(true);
        }

        CurrentCard = null;
        PreviousCard = null;

        CheckGameOver();
    }

    private void CheckGameOver()
    {
        if (SelectedCards.Count >= CardObjects.Count)
        {
            IsGameOver = true;
            AudioController.Instance.PlayAudio(AudioType.GameOver);
        }
    }

    private void OnResetCards()
    {
        foreach (CardHandler item in SelectedCards)
        {
            if (CardObjects.Exists(x => x.MyCardData.CardIndex == item.MyCardData.CardIndex && x.MyCardData.IsFlipped))
            {
                CardHandler card = CardObjects.Find(x => x.MyCardData.CardIndex == item.MyCardData.CardIndex && x.MyCardData.IsFlipped);
                if (card != null)
                {
                    card.OnMatch(CardFront);
                    card.OnReset();
                }
            }
        }

        if (SelectedCards.Count > 0)
            SelectedCards.Clear();

        CurrentCard = null;
        PreviousCard = null;
    }

    private void AddToSelectedCards(CardHandler card)
    {
        if (!SelectedCards.Contains(card))
            SelectedCards.Add(card);
    }
}

[System.Serializable]
public class CardData
{
    public int CardIndex = -1;
    public string CardName = string.Empty;
    public bool IsFlipped = false;
}