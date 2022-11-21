using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState
{
    public const int Rows = 8;
    public const int Cols = 8;
    
    public PlayerEnum[,] Board { get; } // 2D Array which is the game board
    public Dictionary<PlayerEnum, int> DiscCount { get; } // The number of discs that each player possesses
    public PlayerEnum CurrentPlayer { get; private set; } // Which player is currenlty playing
    public bool GameOver { get; private set; } // Bool to determine whether the game is over or not
    public PlayerEnum Winner { get; private set; } // Which player has won
    public Dictionary<PlayerPosition, List<PlayerPosition>> LegalMoves { get; private set; } // The move and a list of all the Taken Discs.

    public GameState()
    {
        Board = new PlayerEnum[Rows, Cols]; // Initialize the Board
        Board[3, 3] = PlayerEnum.White; // The 4 discs already positioned at the beginning of the game
        Board[3, 4] = PlayerEnum.Black;
        Board[4, 3] = PlayerEnum.Black;
        Board[4, 4] = PlayerEnum.White;

        DiscCount = new Dictionary<PlayerEnum, int>() // Each player has 2 discs on the board
        {
            {PlayerEnum.Black, 2},
            {PlayerEnum.White, 2}
        };

        CurrentPlayer = PlayerEnum.Black; // The first player to play, according to the rules, is the one using the black discs
    }

    private bool IsInsideBoard(int r, int c)
    {
        return r >= 0 && r < Rows && c >= 0 && c < Cols;
    }

    private List<PlayerPosition> TakenDiscsInDir(PlayerPosition pos, PlayerEnum player, int rDelta, int cDelta)
    {
        List<PlayerPosition> takenDiscs = new List<PlayerPosition>();
        int r = pos.Row + rDelta;
        int c = pos.Col + cDelta;

        while (IsInsideBoard(r,c) && Board[r,c] != PlayerEnum.None)
        {
            
        }
    }
}
