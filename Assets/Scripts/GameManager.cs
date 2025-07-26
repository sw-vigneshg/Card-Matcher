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

    [SerializeField] private List<CardHandler> SelectedCards = new();

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        GenerateCards();
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

        if (!SelectedCards.Contains(card))
            SelectedCards.Add(card);

        if (SelectedCards.Count > 1)
        {
            bool isNotMatched = SelectedCards.Exists(x => x.MyCardData.CardIndex != card.MyCardData.CardIndex);
            foreach (CardHandler cards in SelectedCards)
            {
                cards.OnMatch(isNotMatched ? WrongMatch : CorrectMatch);
                if (isNotMatched)
                    cards.OnReset();
            }
        }
    }
}

[System.Serializable]
public class CardData
{
    public int CardIndex = -1;
    public string CardName = string.Empty;
    public bool IsFlipped = false;
}