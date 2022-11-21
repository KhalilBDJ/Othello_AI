using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerPosition 
{
    public int Row { get;}
    public int Col { get;}

    public PlayerPosition(int row, int col)
    {
        Row = row;
        Col = col;
    }

    public override bool Equals(object o)
    {
        if (o is PlayerPosition other)
        {
            return Row == other.Row && Col == other.Col;
        }

        return false;
    }

    public override int GetHashCode()
    {
        return 8 * Row + Col;
    }
}
