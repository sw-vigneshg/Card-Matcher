using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardHandler : MonoBehaviour
{
    public CardData MyCardData;
    [SerializeField] private SpriteRenderer CardBack;
    [SerializeField] private SpriteRenderer CardFront;
    [SerializeField] private TMP_Text CardNameText;
    private GameManager _GameManager;
    //[SerializeField] private Animator FlipAnimator;

    private void OnEnable()
    {
        _GameManager = GameManager.Instance;
        CardFront.gameObject.SetActive(true);
        CancelInvoke(nameof(OnReset));
        Invoke(nameof(OnReset), 1f);
        //FlipAnimator.SetBool("FlipFront", false);
        //FlipAnimator.SetBool("FlipBack", false);
    }

    public void AssignCardData(CardData data)
    {
        MyCardData = data;
    }

    public void AssignCardNameText()
    {
        CardNameText.text = MyCardData.CardName;
    }

    private void OnMouseDown()
    {
        if (MyCardData.IsFlipped)
            return;

        MyCardData.IsFlipped = true;

        //FlipAnimator.SetBool("FlipFront", true);

        CardFront.gameObject.SetActive(true);
        _GameManager.ValidateSelectedCards(this);
    }

    public void OnMatch(Color color)
    {
        CardFront.color = color;
    }

    public void OnReset()
    {
        MyCardData.IsFlipped = false;
        CardFront.gameObject.SetActive(false);
    }

    public void DisableCard(bool isMatched)
    {
        this.gameObject.SetActive(!isMatched);
    }
}
