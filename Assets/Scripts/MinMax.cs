using System.Collections;
using System.Collections.Generic;
using Unity.PlasticSCM.Editor.WebApi;
using UnityEngine;

public class MinMax
{
    private GameState _gameState;
    private int[,] _positionalBoard = new  int[8, 8];
    public Dictionary<PlayerEnum, int> positionalCount;

    public MinMax()
    {
        //_gameState = new GameState();
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

    public int makeAIMove( PlayerEnum player, int depth)
    {
        MoveInfo moveAI;
        Dictionary<PlayerPosition, List<PlayerPosition>> legalMoves = _gameState.FindAllLegalMoves(player);
        if (_gameState.CurrentPlayer == PlayerEnum.Black)
        {
            if (legalMoves == null)
            {
                if (_gameState.GameOver)
                {
                    return 0;
                }
            }
            else
            {
                List<int> score = new List<int>();
            
                foreach (var move in legalMoves.Keys)
                {
                    _gameState.MakeMove(new PlayerPosition(move.Col, move.Row), out moveAI);
                    score.Add(makeAIMove(player.Opponent(), depth +1));
                    _gameState.RevertMove(moveAI);
                }
            }

            if (depth >= 2)
            {
                
            }

            
        }
        return 0;
    }

    public void InitializeEuristicValue()
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
        InitializeEuristicValue();
        positionalCount[player] += _positionalBoard[move.NewPosition.Row, move.NewPosition.Col];
        foreach (var taken in move.Taken)
        {
            positionalCount[player] += _positionalBoard[taken.Row, taken.Col];
            positionalCount[player.Opponent()] -= _positionalBoard[taken.Row, taken.Col];
        }
    }

    
}
