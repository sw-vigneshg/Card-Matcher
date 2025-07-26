using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    [SerializeField] private CardHandler CardPrefab;
    [SerializeField] private List<CardHandler> CardsList = new();
    [SerializeField] private int PoolCount;
    private CardHandler TempCard;

    private void Awake()
    {
        for (int i = 0; i < PoolCount; i++)
        {
            GameObject card = Instantiate(CardPrefab.gameObject, this.transform);
            card.gameObject.SetActive(false);
            CardsList.Add(card.GetComponent<CardHandler>());
        }
    }

    public CardHandler GetCards()
    {
        try
        {
            TempCard = CardsList[0];
            CardsList.RemoveAt(0);
        }
        catch
        {
            GameObject card = Instantiate(CardPrefab.gameObject, this.transform);
            TempCard = card.GetComponent<CardHandler>();
        }
        TempCard.gameObject.SetActive(true);
        return TempCard;
    }

    public void ReturnCardToPool(CardHandler card)
    {
        card.gameObject.SetActive(false);
        card.transform.parent = this.transform;
        card.transform.localPosition = Vector3.zero;
        CardsList.Add(card);
    }
}
