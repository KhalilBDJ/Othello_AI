using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class GameState
{
    public const int Rows = 8; // Nombre de ligne
    public const int Cols = 8; // Nombre de colonne 

    private readonly int[,]
        _positionalBoard = new int[8, 8]; // L'interprétation 1 du tableau de stratégies dans le 4.2.2 du cours

    public Dictionary<PlayerEnum, int> positionalCount; // La valeur de chaque joueur selon le tableau positionel 

    public List<MoveInfo> previousMoves; // La liste des mouvements précédents

    public PlayerEnum[,] Board { get; } // Un tableau à deux dimensions correspondant au plateau de jeu
    public Dictionary<PlayerEnum, int> DiscCount { get; } // Le nombre de pions que chaque joueur possède
    public PlayerEnum CurrentPlayer { get; set; } // Le joueur actuellement en train de jouer
    public bool GameOver { get; set; } // Permettant de déterminer si le jeu est finit ou non
    public PlayerEnum Winner { get; set; } // Quel joueur a gagné

    public Dictionary<PlayerPosition, List<PlayerPosition>>
        LegalMoves { get; private set; } // Un ajout de pion et la liste des pions adverses que le pion va prendre

    public GameState()
    {
        previousMoves = new List<MoveInfo>();
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
        LegalMoves = FindAllLegalMoves(CurrentPlayer);
    }

    private bool IsInsideBoard(int r, int c)
    {
        return r >= 0 && r < Rows && c >= 0 && c < Cols;
    }

    private List<PlayerPosition> TakenDiscsInDir(PlayerPosition pos, PlayerEnum player, int rDelta, int cDelta)
        // rDelta et cDelta correspondent à des directions, par exemple r = 1 et c = 0 correspond au Sud
    {
        List<PlayerPosition> takenDiscs = new List<PlayerPosition>();
        int
            r = pos.Row +
                rDelta; // Pour éviter de prendre le pion actuel, on ajoute nos direction (cad on prend le premier pion adverse dans la direction choisie)
        int c = pos.Col + cDelta;

        while (IsInsideBoard(r, c) && Board[r, c] != PlayerEnum.None)
        {
            if (Board[r, c] == player.Opponent())
            {
                takenDiscs.Add(new PlayerPosition(r, c));
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

        for (int rDelta = -1; rDelta <= 1; rDelta++)
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


    private bool
        IsMoveLegal(PlayerEnum player, PlayerPosition pos,
            out List<PlayerPosition> taken) // Permet de vérifier si un mouvement est légal
    {
        if (Board[pos.Row, pos.Col] != PlayerEnum.None)
        {
            taken = null;
            return false;
        }

        taken = Taken(pos, player);
        return taken.Count > 0;
    }

    public Dictionary<PlayerPosition, List<PlayerPosition>> FindAllLegalMoves(PlayerEnum player)
    {
        Dictionary<PlayerPosition, List<PlayerPosition>> legalMoves =
            new Dictionary<PlayerPosition, List<PlayerPosition>>(); // On crée un dictionnaire de mouvements légaux

        for (int r = 0; r < Rows; r++)
        {
            for (int c = 0; c < Cols; c++) //on parcours chaque case
            {
                PlayerPosition position = new PlayerPosition(r, c);

                if (IsMoveLegal(player, position,
                        out List<PlayerPosition> taken)) // on regarde si dans la position actuelle il y a des mouvements légaux
                {
                    legalMoves[position] = taken; // On ajoute au dictionnaire la liste des pions pris à cette position
                }
            }
        }

        return legalMoves;
    }

    public MoveInfo MakeMove(PlayerPosition pos, out MoveInfo moveInfo, bool isAi)
    {
        if (!LegalMoves
                .ContainsKey(
                    pos)) // Si la position à laquelle on veut placer le pion n'est pas une position valide, alors on retourne faux
        {
            moveInfo = null;
            return null;
        }

        // Sinon, on récupère le joueur actuel
        PlayerEnum movePlayer = CurrentPlayer;
        List<PlayerPosition> taken = LegalMoves[pos]; // On récupère la liste des pions pris par ce mouvement


        Board[pos.Row, pos.Col] = movePlayer; // On déplace le joueur sur le plateau

        FlipDiscs(taken);
        UpdateDiscCounts(movePlayer, taken.Count);
        moveInfo = new MoveInfo 
            {Player = movePlayer, NewPosition = pos, Taken = taken}; // On initialise les infos du mouvement
        UpdatePositionalCount(movePlayer, moveInfo);
        previousMoves.Add(moveInfo);
        if (isAi)
        {
            ChangePlayer(); //Si le mouvement est effectué par l'IA est qu'elle rencontre en profondeur aucun mouvement, elle changeait deux fois de joueur, d'où l'importance de cette condition
        }
        else
        {
            PassTurn();
        }

        if (previousMoves != null)
        {
            moveInfo.OldMove = previousMoves[previousMoves.Count - 1];
        }

        return moveInfo;
    }

    public PlayerPosition RevertMove(MoveInfo previousMove) // Permet d'annuler un mouvement passé en paramètre
    {
        PlayerPosition previousPosition =
            new PlayerPosition(previousMove.NewPosition.Row, previousMove.NewPosition.Col);
        FlipDiscs(previousMove.Taken);
        Board[previousMove.NewPosition.Row, previousMove.NewPosition.Col] = PlayerEnum.None;
        previousMoves.Remove(previousMove);
        UpdateDiscCounts(CurrentPlayer,
            previousMove.Taken.Count()); // On met à jour le nombre de pions pour chaque joueur
        UpdatePositionalCount(CurrentPlayer,
            previousMove); // On met à jour les points d'heuristiques pour chaque joueur
        ChangePlayer();
        return previousPosition;
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


    public void ChangePlayer()
    {
        CurrentPlayer = CurrentPlayer.Opponent();
        LegalMoves = FindAllLegalMoves(CurrentPlayer); // On change la liste des déplacements possibles
    }

    public PlayerEnum FindWinner()
    {
        if (DiscCount[PlayerEnum.Black] > DiscCount[PlayerEnum.White])
        {
            return PlayerEnum.Black;
        }
        else if (DiscCount[PlayerEnum.Black] < DiscCount[PlayerEnum.White])
        {
            return PlayerEnum.White;
        }

        return PlayerEnum.None; // Match nul
    }

    public void PassTurn()
    {
        ChangePlayer();

        if (LegalMoves.Count > 0) // Si le joueur peut jouer, alors on continue de jouer
        {
            return;
        }

        ChangePlayer();

        if (LegalMoves.Count == 0) // Si aucun joueur ne peut jouer, alors on arrête le jeu
        {
            CurrentPlayer = PlayerEnum.None;
            GameOver = true;
            Winner = FindWinner();
        }
    }

    public IEnumerable<PlayerPosition> OccupiedPositions()
    {
        for (int r = 0; r < Rows; r++)
        {
            for (int c = 0; c < Cols; c++)
            {
                if (Board[r, c] != PlayerEnum.None)
                {
                    yield return new PlayerPosition(r, c);
                }
            }
        }
    }

    public MoveInfo MinMax(int depth, int maxDepth, MoveInfo chosenMove)
    {
        int bestScore = -100000;
        Dictionary<MoveInfo, MoveInfo> moves = new Dictionary<MoveInfo, MoveInfo>();
        MoveInfo currentMove = chosenMove;
        if (depth == maxDepth ||LegalMoves.Count == 0)
        {
            return chosenMove;
        }
        else
        {
            foreach (var legalMove in FindAllLegalMoves(CurrentPlayer).Keys)
            {
                MakeMove(legalMove, out currentMove, true);
                moves.Add(MinMax(depth + 1, maxDepth, currentMove), currentMove);
                RevertMove(currentMove);
            }
        }

        foreach (var move in moves.Keys)
        {
            if (move.heuristicValue > bestScore)
            {
                bestScore = move.heuristicValue;
                chosenMove = move;
            }
        }
        return moves[chosenMove];
    }

    public void InitializeEuristicValue() // Permet d'initialiser les valeurs heuristiques pour chaque joueur
    {
        InitialiseEuristic(_positionalBoard);
        positionalCount = new Dictionary<PlayerEnum, int>()
        {
            {PlayerEnum.White, _positionalBoard[3, 3] + _positionalBoard[4, 4]},
            {PlayerEnum.Black, _positionalBoard[4, 3] + _positionalBoard[3, 4]}
        };
    }

    public void UpdatePositionalCount(PlayerEnum player, MoveInfo move)
    {
        //Strat1(player, move);
        //Strat2(player, move);
        //Strat3(player, move);
        Strat4(player, move);

    }

    private void Strat1(PlayerEnum player, MoveInfo move)
    {
        positionalCount[player] += _positionalBoard[move.NewPosition.Row, move.NewPosition.Col]; //Première Stratégie

        move.heuristicValue = _positionalBoard[move.NewPosition.Row, move.NewPosition.Col];
    }
    
    private void Strat2(PlayerEnum player, MoveInfo move)
    {
        move.heuristicValue = 1; // Deuxième stratégie
        foreach (var pion in move.Taken)
        {
            move.heuristicValue += 1;
        }
    }
    
    private void Strat3(PlayerEnum player, MoveInfo move)
    {
        move.heuristicValue = -FindAllLegalMoves(CurrentPlayer.Opponent()).Count; // troisième stratégie
        if (_positionalBoard[move.NewPosition.Row, move.NewPosition.Col] == 500)
        {
            move.heuristicValue = 500;
        }
    }

    private void Strat4(PlayerEnum player, MoveInfo move)
    {
        if (previousMoves.Count<23)
        {
            Strat1(player, move);
        }
        else if (previousMoves.Count>= 46)
        {
            Strat3(player, move);
        }
        else
        {
            Strat2(player, move);
        }
        
    }
    

    public int[,] InitialiseEuristic(int[,] board)
    {
        // coins
        board[0, 0] = 500;
        board[0, 7] = 500;
        board[7, 0] = 500;
        board[7, 7] = 500;

        // centre
        board[3, 3] = 16;
        board[3, 4] = 16;
        board[4, 3] = 16;
        board[4, 4] = 16;

        // arrêtes
        board[0, 3] = 10;
        board[0, 4] = 10;
        board[0, 1] = -150;
        board[0, 6] = -150;
        board[0, 2] = 30;
        board[0, 5] = 30;

        board[3, 7] = 10;
        board[4, 7] = 10;
        board[1, 7] = -150;
        board[6, 7] = -150;
        board[2, 7] = 30;
        board[5, 7] = 30;

        board[7, 3] = 10;
        board[7, 4] = 10;
        board[7, 1] = -150;
        board[7, 6] = -150;
        board[7, 2] = 30;
        board[7, 5] = 30;

        board[3, 0] = 10;
        board[4, 0] = 10;
        board[1, 0] = -150;
        board[6, 0] = -150;
        board[2, 0] = 30;
        board[5, 0] = 30;

        // arrêtes intérieures 

        board[1, 3] = 0;
        board[1, 4] = 0;
        board[1, 2] = 0;
        board[1, 5] = 0;

        board[3, 1] = 0;
        board[4, 1] = 0;
        board[2, 1] = 0;
        board[5, 1] = 0;

        board[6, 3] = 0;
        board[6, 4] = 0;
        board[6, 2] = 0;
        board[6, 5] = 0;

        board[3, 6] = 0;
        board[4, 6] = 0;
        board[2, 6] = 0;
        board[5, 6] = 0;

        // coins intérieurs 1
        board[1, 1] = -250;
        board[1, 6] = -250;
        board[6, 1] = -250;
        board[6, 6] = -250;


        // arrêtes intérieures 2
        board[2, 3] = 2;
        board[2, 4] = 2;
        board[3, 5] = 2;
        board[4, 5] = 2;
        board[5, 3] = 2;
        board[5, 4] = 2;
        board[3, 2] = 2;
        board[4, 2] = 2;

        // coins intérieurs 2
        board[2, 2] = 1;
        board[2, 5] = 1;
        board[5, 2] = 1;
        board[5, 5] = 1;

        return board;
    } // crée un tableau remplie de la représentation 1 de l'euristique positionnel 
}