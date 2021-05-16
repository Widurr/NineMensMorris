using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceData
{
    public float[] position;

    public PieceData(GameObject piece)
    {
        position = new float[3];
        position[0] = piece.transform.position.x;
        position[1] = piece.transform.position.y;
        position[2] = piece.transform.position.z;
    }
}
