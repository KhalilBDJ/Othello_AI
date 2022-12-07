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
}
