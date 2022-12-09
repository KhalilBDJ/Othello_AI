using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveInfo 
{
    public PlayerEnum Player { get; set; }
    public PlayerPosition NewPosition { get; set; }
    public PlayerPosition OldPosition { get; set; }
    public List<PlayerPosition> Taken { get; set; }
    public int euristicValue { get; set; }
}
