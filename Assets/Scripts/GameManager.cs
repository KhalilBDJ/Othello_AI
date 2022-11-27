using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    [SerializeField] private Camera cam;
    [SerializeField] private LayerMask boardLayer;
    [SerializeField] private Disc discBlackUp;
    [SerializeField] private Disc discWhiteUp;

    private Dictionary<PlayerEnum, Disc> discPrefabs = new Dictionary<PlayerEnum, Disc>();
    private GameState _gameState = new GameState();
    private Disc[,] _discs = new Disc[8, 8];
    void Start()
    {
        discPrefabs[PlayerEnum.Black] = discBlackUp;
        discPrefabs[PlayerEnum.White] = discWhiteUp;
        
        AddStartDiscs();
    }

    // Update is called once per frame
    void Update()
    {
        
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
}
