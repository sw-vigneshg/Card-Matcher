using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] private CardHandler CardPrefab;
    [SerializeField] private List<CardHandler> CardObjects = new();
    [SerializeField] private CardData[] AllCardData;

    [SerializeField] private int RowCount;
    [SerializeField] private int ColoumnCount;
    private Vector3 CardPosition;

    public Color CardBack;
    public Color CardFront;
    public Color WrongMatch;
    public Color CorrectMatch;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        GenerateCards();
    }

    private void GenerateCards()
    {
        if (CardObjects.Count > 0)
            CardObjects.Clear();

        float SpacingX = 2f;
        float SpacingY = 2f;

        for (int i = 0; i < RowCount; i++)
        {
            for (int j = 0; j < ColoumnCount; j++)
            {
                CardPosition.x = i * SpacingX;
                CardPosition.y = j * SpacingY;
                CardPosition.z = 0f;

                GameObject card = Instantiate(CardPrefab.gameObject, CardPosition, Quaternion.identity, this.transform);
                CardHandler cardHandler = card.GetComponent<CardHandler>();
                if (cardHandler != null && !CardObjects.Exists(x => x == cardHandler))
                {
                    CardObjects.Add(cardHandler);
                }
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