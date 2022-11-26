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
    public Dictionary<PlayerPosition, List<PlayerPosition>> LegalMoves { get; private set; } // Un ajout de pion et la liste des pions adverses que le pion va prendre
	
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
        LegalMoves = FindLegalMoves(CurrentPlayer);
    }

    private bool IsInsideBoard(int r, int c)
    {
        return r >= 0 && r < Rows && c >= 0 && c < Cols;
    }

    private List<PlayerPosition> TakenDiscsInDir(PlayerPosition pos, PlayerEnum player, int rDelta, int cDelta)
    // rDelta et cDelta correspondent à des directions, par exemple r = 1 et c = 0 correspond au Sud
    {
        List<PlayerPosition> takenDiscs = new List<PlayerPosition>();
        int r = pos.Row + rDelta; // Pour éviter de prendre le pion actuel, on ajoute nos direction (cad on prend le premier pion adverse dans la direction choisie)
        int c = pos.Col + cDelta;

        while (IsInsideBoard(r,c) && Board[r,c] != PlayerEnum.None)
        {
            if (Board[r, c] == player.Opponent())
            {
                takenDiscs.Add(new PlayerPosition(r,c));
                r += rDelta; // On continue d'une case dans la direction r
                c += cDelta; // Pareil pour c
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
        // On regarde cette fois-ci dans toutes les directions
        List<PlayerPosition> taken = new List<PlayerPosition>();

        for (int rDelta = -1; rDelta<=1; rDelta++)
        {
            for (int cDelta = -1; cDelta <= 1; cDelta++)
            {
                if (rDelta == 0 && cDelta == 0) // 0 et 0 signifie que c'est le pion actuel
                {
                    continue;
                }
                
                taken.AddRange(TakenDiscsInDir(pos, player, rDelta, cDelta));
            }
        }

        return taken;
    }


    private bool IsMoveLegal(PlayerEnum player, PlayerPosition pos, out List<PlayerPosition> taken)
    {
        if (Board[pos.Row, pos.Col] != PlayerEnum.None)
        {
            taken = null;
            return false;
        }

        taken = Taken(pos, player);
        return taken.Count > 0;
    }

    private Dictionary<PlayerPosition, List<PlayerPosition>> FindLegalMoves(PlayerEnum player)
    {
        Dictionary<PlayerPosition, List<PlayerPosition>> legalMoves = new Dictionary<PlayerPosition, List<PlayerPosition>>(); // On crée un dictionnaire de mouvements légaux

        for (int r = 0; r < Rows; r++)
        {
            for (int c = 0; c < Cols; c++)//on parcours chaque case
            {
                PlayerPosition position = new PlayerPosition(r, c);

                if (IsMoveLegal(player, position, out List<PlayerPosition> taken)) // on regarde si dans la position actuelle il y a des mouvements légaux
                {
                    legalMoves[position] = taken; // On ajoute au dictionnaire la liste des pions pris à cette position
                }
            }
        }
        return legalMoves;
    }

    public bool MakeMove(PlayerPosition pos, out MoveInfo moveInfo)
    {
        if (!LegalMoves.ContainsKey(pos)) // Si la position à laquelle on veut placer le pion n'est pas une position valide, alors on retourne faux
        {
            moveInfo = null;
            return false;
        }
        // Sinon, on récupère le joueur actuel
        PlayerEnum movePlayer = CurrentPlayer;
        List<PlayerPosition> taken = LegalMoves[pos]; // On récupère la liste des pions pris par ce mouvement

        Board[pos.Row, pos.Col] = movePlayer; // On déplace le joueur sur le plateau
        
        FlipDiscs(taken); 
        UpdateDiscCounts(movePlayer, taken.Count);
        //Tour passé

        moveInfo = new MoveInfo {Player = movePlayer, Position = pos, Taken = taken}; // On initialise les infos du mouvement
        return true;
    }

    private void FlipDiscs(List<PlayerPosition> positions)
    {
        // Transfert une liste de pion passé en paramètre au joueur adverse
        foreach (var position in positions)
        {
            Board[position.Row, position.Col] = Board[position.Row, position.Col].Opponent();
        }
    }

    private void UpdateDiscCounts(PlayerEnum player, int taken)
    {
        DiscCount[player] += taken + 1; // on ajoute le nombre de pions pris plus le pion placé
        DiscCount[player.Opponent()] -= taken; // on retire le nombre de pion pris à l'adversaire
    }

    private void ChangePlayer()
    {
        CurrentPlayer = CurrentPlayer.Opponent(); 
        LegalMoves = FindLegalMoves(CurrentPlayer); // On change la liste des déplacements possibles
    }
}
