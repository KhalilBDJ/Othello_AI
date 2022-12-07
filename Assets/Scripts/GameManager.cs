using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    [SerializeField] private Camera cam;
    [SerializeField] private LayerMask boardLayer;
    [SerializeField] private Disc discBlackUp;
    [SerializeField] private Disc discWhiteUp;
    [SerializeField] private GameObject highlightPrefab;
    [SerializeField] private UIManager uiManager;

    private Dictionary<PlayerEnum, Disc> discPrefabs = new Dictionary<PlayerEnum, Disc>();
    private GameState _gameState = new GameState();
    private Disc[,] _discs = new Disc[8, 8];
    private List<GameObject> highlights = new List<GameObject>();
    void Start()
    {
        discPrefabs[PlayerEnum.Black] = discBlackUp;
        discPrefabs[PlayerEnum.White] = discWhiteUp;
        
        AddStartDiscs();
        ShowLegalMoves();
        uiManager.SetPlayerText(_gameState.CurrentPlayer);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hitInfo))
            {
                Vector3 impact = hitInfo.point;
                PlayerPosition boardPos = SceneToBoard(impact);
                OnBoardClicked(boardPos);
            }
        }
    }

    private void ShowLegalMoves()
    {
        foreach (var boardPos in _gameState.LegalMoves.Keys)
        {
            Vector3 scenePos = BoardToScenePos(boardPos) + Vector3.up * 0.01f;
            GameObject highlight = Instantiate(highlightPrefab, scenePos, Quaternion.identity);
            highlights.Add(highlight);
        }
    }

    private void HideLegalMoves()
    {
        highlights.ForEach(Destroy);
        highlights.Clear();
    }

    private void OnBoardClicked(PlayerPosition boardPos)
    {
        if (_gameState.MakeMove(boardPos, out MoveInfo moveInfo))
        {
            StartCoroutine(OnMoveMade(moveInfo));
        }
    }

    private IEnumerator OnMoveMade(MoveInfo moveInfo)
    {
        HideLegalMoves();
        yield return ShowMove(moveInfo);
        yield return ShowTurnOutcome(moveInfo);
        ShowLegalMoves();
    }

    private PlayerPosition SceneToBoard(Vector3 scenePos)
    {
        int col = (int) (scenePos.x - 0.25f);
        int row = 7 - (int) (scenePos.z - 0.25f);
        return new PlayerPosition(row,col);
    }

    private Vector3 BoardToScenePos(PlayerPosition boardPos)
    {
        return new Vector3(boardPos.Col + 0.75f, 0, 7 - boardPos.Row + 0.75f);
    }

    private void SpawnDiscs(Disc prefab, PlayerPosition boardPos)
    {
        Vector3 scenePos = BoardToScenePos(boardPos) + Vector3.up * 0.1f;
        _discs[boardPos.Row, boardPos.Col] = Instantiate(prefab, scenePos, Quaternion.identity);
    }

    private void AddStartDiscs()
    {
        foreach (var boardPos in _gameState.OccupiedPositions())
        {
            PlayerEnum player = _gameState.Board[boardPos.Row, boardPos.Col];
            SpawnDiscs(discPrefabs[player], boardPos);
        }
    }

    private void FlipDiscs(List<PlayerPosition> positions)
    {
        foreach (var position in positions)
        {
            _discs[position.Row, position.Col].Flip();
        }
    }

    private IEnumerator ShowMove(MoveInfo moveInfo)
    {
        SpawnDiscs(discPrefabs[moveInfo.Player], moveInfo.Position);
        yield return new WaitForSeconds(0.33f);
        FlipDiscs(moveInfo.Taken);
        yield return new WaitForSeconds(0.83f);
    }

    private IEnumerator ShowTurnSkipped(PlayerEnum skippedPlayer)
    {
        uiManager.SetSkipText(skippedPlayer);
        yield return uiManager.AnimateTopText();
    }

    private IEnumerator ShowTurnOutcome(MoveInfo moveInfo)
    {
        if (_gameState.GameOver)
        {
            yield return ShowGameOver(_gameState.Winner);
            yield break;
        }

        PlayerEnum currentPlayer = _gameState.CurrentPlayer;

        if (currentPlayer == moveInfo.Player)
        {
            yield return ShowTurnSkipped(currentPlayer.Opponent());
        }
        
        uiManager.SetPlayerText(currentPlayer);
    }

    private IEnumerator ShowGameOver(PlayerEnum winner)
    {
        uiManager.SetTopText("Personne peut jouer ah les bouffons");

        yield return uiManager.AnimateTopText();
        yield return uiManager.ShowScoreText();
    }
}
