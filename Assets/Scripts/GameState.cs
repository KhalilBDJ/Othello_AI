using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState
{
    public const int Rows = 8; // Nombre de ligne
    public const int Cols = 8; // Nombre de colonne 
    
    public PlayerEnum[,] Board { get; } // Un tableau à deux dimensions correspondant au plateau de jeu
    public Dictionary<PlayerEnum, int> DiscCount { get; } // Le nombre de pions que chaque joueur possède
    public PlayerEnum CurrentPlayer { get; private set; } // Le joueur actuellement en train de jouer
    public bool GameOver { get; private set; } // Permettant de déterminer si le jeu est finit ou non
    public PlayerEnum Winner { get; private set; } // Quel joueur a gagné
    public Dictionary<PlayerPosition, List<PlayerPosition>> LegalMoves { get; private set; } // Un ajout de pion et la liste des pions adverses que le pion va prednre
	
    public GameState()
    {
        Board = new PlayerEnum[Rows, Cols]; // Initialisation du plateau
        Board[3, 3] = PlayerEnum.White; // La position des pions au début du jeu
        Board[3, 4] = PlayerEnum.Black;
        Board[4, 3] = PlayerEnum.Black;
        Board[4, 4] = PlayerEnum.White;

        DiscCount = new Dictionary<PlayerEnum, int>() // Chaque joueur a deux pions sur le plateau au début
        {
            {PlayerEnum.Black, 2},
            {PlayerEnum.White, 2}
        };

        CurrentPlayer = PlayerEnum.Black; // Selon les règles, le premier joueur est le joueur possèdant les pions noirs
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
            if (Board[r, c] == player.Opponent())
            {
                takenDiscs.Add(new PlayerPosition(r,c));
                r += rDelta;
                c += cDelta;
            }
            else
            {
                return takenDiscs;
            }
        }
        return new List<PlayerPosition>();
    }

    private List<PlayerPosition> Taken(PlayerPosition pos, PlayerEnum player)
    {
        List<PlayerPosition> taken = new List<PlayerPosition>();

        for (int rDelta = -1; rDelta<=1; rDelta++)
        {
            for (int cDelta = -1; cDelta <= 1; cDelta++)
            {
                if (rDelta == 0 && cDelta == 0)
                {
                    continue;
                }
                
                taken.AddRange(TakenDiscsInDir(pos, player, rDelta, cDelta));
            }
        }

        return taken;
    }
}
