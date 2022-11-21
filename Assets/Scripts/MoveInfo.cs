using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveInfo 
{
    public PlayerEnum Player { get; set; }
    public PlayerPosition Position { get; set; }
    public List<PlayerPosition> Taken { get; set; }
}
