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
    [SerializeField] private Animator FlipAnimator;

    private void OnEnable()
    {
        _GameManager = GameManager.Instance;
        
        StopCoroutine(nameof(OnStart));
        StartCoroutine(nameof(OnStart));
    }

    private IEnumerator OnStart()
    {
        FlipAnimator.SetBool("FlipFront", true);
        yield return new WaitForSeconds(2f);
        OnReset();
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

        CardFront.gameObject.SetActive(true);
        _GameManager.ValidateSelectedCards(this);
        FlipAnimator.SetBool("FlipFront", true);
        AudioController.Instance.PlayAudio(AudioType.Flip);
    }

    public void OnMatch(Color color)
    {
        CardFront.color = color;
    }

    public void OnReset()
    {
        MyCardData.IsFlipped = false;
        CardFront.gameObject.SetActive(false);
        FlipAnimator.SetBool("FlipFront", false);
        AudioController.Instance.PlayAudio(AudioType.Flip);
    }

    public void DisableCard(bool isMatched)
    {
        this.gameObject.SetActive(!isMatched);
    }
}
