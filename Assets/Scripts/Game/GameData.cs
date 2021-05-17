using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameData: MonoBehaviour
{
    public bool turnInfo;
    public PieceData[] pieces; 
    
    public GameData(bool turnInfo, PieceData[] pieces)
    {
        this.turnInfo = turnInfo;
        this.pieces = pieces;
    }

    
}
