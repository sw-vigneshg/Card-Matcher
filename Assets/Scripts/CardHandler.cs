using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardHandler : MonoBehaviour
{
    [SerializeField] private SpriteRenderer CardBg;
    [SerializeField] private CardData MyCardData;

    private void Awake()
    {
        if (CardBg == null)
            CardBg = GetComponent<SpriteRenderer>();
    }
}
