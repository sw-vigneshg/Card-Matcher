using System.Collections.Generic;
using Unity.VisualScripting;
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

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        GenerateCards();
        PreviousCard = null;
        CurrentCard = null;
    }

    public void GenerateCards()
    {
        if (CardObjects.Count > 0)
        {
            CardObjects.Clear();
            Debug.LogError("Card Objects list is not Empty");
        }

        int cardIndex = 0;
        CardData tempData = new CardData();

        float spaceX = 1.3f;
        float spaceY = 1.5f;
        Vector3 position;

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
                        CardIndex = AllCardData[cardIndex].CardIndex,
                        CardName = AllCardData[cardIndex].CardName,
                        IsFlipped = false,
                    };
                    card.AssignCardData(tempData);
                    cardIndex++;
                    cardIndex = cardIndex <= AllCardData.Length - 1 ? cardIndex : 0;

                    CardObjects.Add(card);
                }
            }
        }
        ShuffleCards();
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
        AddToSelectedCards(card);
        HighlightCard();
    }

    private void HighlightCard()
    {
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

        if (SelectedCards.Count > 0)
            SelectedCards.Clear();

        CurrentCard = null;
        PreviousCard = null;
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