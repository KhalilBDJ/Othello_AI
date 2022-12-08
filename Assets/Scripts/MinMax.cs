using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinMax : MonoBehaviour
{
    public int[,] board = new int[8,8];
    
    public void InitialiseEuristic()
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
    }
}
