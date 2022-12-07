using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI topText;
    [SerializeField] private TextMeshProUGUI blackScoreText;
    [SerializeField] private TextMeshProUGUI whiteScoreText;
    [SerializeField] private TextMeshProUGUI winnerText;
    [SerializeField] private Image blackOverlay;
    [SerializeField] private RectTransform playAgainButton;

    public void SetPlayerText(PlayerEnum currentPlayer)
    {
        if (currentPlayer == PlayerEnum.Black)
        {
            topText.text = "Au tour du joueur noir <sprite name=DiscBlackUp>";
        }
        else if (currentPlayer == PlayerEnum.White)
        {
            topText.text = "Au tour du joueur blanc <sprite name=DiscWhiteUp>";
        }
    }

    public void SetSkipText(PlayerEnum skippedPlayer)
    {
        if (skippedPlayer == PlayerEnum.Black)
        {
            topText.text = "Eh les noirs ces gros bouffons ils peuvent plus jouer <sprite name=DiscBlackUp>";
        }
        else if (skippedPlayer == PlayerEnum.White)
        {
            topText.text = "Eh les blancs ces gros bouffons ils peuvent plus joeur <sprite name=DiscWhiteUp>";
        }
    }

    public IEnumerator AnimateTopText()
    {
        topText.transform.LeanScale(Vector3.one * 1.2f, 0.25f).setLoopPingPong(4);
        yield return new WaitForSeconds(2);
    }

    public void SetTopText(string text)
    {
        topText.text = text;
    }

    private IEnumerator ScaleDown(RectTransform rect)
    {
        rect.LeanScale(Vector3.zero, 0.2f);
        yield return new WaitForSeconds(0.2f);
        rect.gameObject.SetActive(false);
    }

    private IEnumerator ScaleUp(RectTransform rect)
    {
        rect.gameObject.SetActive(true);
        rect.localScale = Vector3.zero;
        rect.LeanScale(Vector3.one, 0.2f);
        yield return new WaitForSeconds(0.2f);
    }

    public IEnumerator ShowScoreText()
    {
        yield return ScaleDown(topText.rectTransform);
        yield return ScaleUp(blackScoreText.rectTransform);
        yield return ScaleUp(whiteScoreText.rectTransform);
    }

    public void SetBlackScoreText(int score)
    {
        blackScoreText.text = $"<sprite name=DiscBlackUp> {score}";
    }
    
    public void SetWhiteScoreText(int score)
    {
        whiteScoreText.text = $"<sprite name=DiscWhiteUp> {score}";
    }

    private IEnumerator ShowOverlay()
    {
        blackOverlay.gameObject.SetActive(true);
        blackOverlay.color = Color.clear;
        blackOverlay.rectTransform.LeanAlpha(0.8f, 1);
        yield return new WaitForSeconds(1);
    }

    private IEnumerator HideOverlay()
    {
        blackOverlay.rectTransform.LeanAlpha(0, 1);
        yield return new WaitForSeconds(1);
        blackOverlay.gameObject.SetActive(false);
        
    }

    private IEnumerator MoveScoreDown()
    {
        blackScoreText.rectTransform.LeanMoveY(0, 0.5f);
        whiteScoreText.rectTransform.LeanMoveY(0, 0.5f);
        yield return new WaitForSeconds(0.5f);
    }

    public void SetWinnerText(PlayerEnum winner)
    {
        switch (winner)
        {
            case PlayerEnum.Black:
                winnerText.text = "Le joueur noir a gangé (haha le blanc ce bouffon)";
                break;
            case PlayerEnum.White:
                winnerText.text = "Le joueur blanc a gangé (haha le noirs ce bouffon)";
                break;
            case PlayerEnum.None:
                winnerText.text = "Srx vous êtes trop nuls y'a égalité";
                break;
        }
    }

    public IEnumerator ShowEndScreen()
    {
        yield return ShowOverlay();
        yield return MoveScoreDown();
        yield return ScaleUp(winnerText.rectTransform);
        yield return ScaleUp(playAgainButton);
    }

    public IEnumerator HideEndScreen()
    {
        StartCoroutine(ScaleDown(winnerText.rectTransform));
        StartCoroutine(ScaleDown(blackScoreText.rectTransform));
        StartCoroutine(ScaleDown(whiteScoreText.rectTransform));
        StartCoroutine(ScaleDown(playAgainButton));

        yield return new WaitForSeconds(0.5f);
        yield return HideOverlay();
    }
}
