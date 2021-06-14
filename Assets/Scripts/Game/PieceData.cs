using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PieceData
{
    public float[] position;
    public bool isWhite;

    public PieceData(Piece piece)
    {
        position = new float[3];
        position[0] = piece.transform.position.x;
        position[1] = piece.transform.position.y;
        position[2] = piece.transform.position.z;
        isWhite = piece.GetComponent<Piece>().isWhite;
    }
}
