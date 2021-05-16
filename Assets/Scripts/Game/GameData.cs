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

    public static PieceData[] GetPieces()
    {
        var objects = GameObject.FindGameObjectsWithTag("Piece");

        

        PieceData[] pieces = new PieceData[objects.Length];
        int i = 0;
        foreach (GameObject piece in objects)
        {
            pieces[i] = new PieceData(piece);
            i++;
        }
        return pieces;
    }
}
