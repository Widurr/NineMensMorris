using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePlate : MonoBehaviour
{
    public GameObject controller;

    GameObject reference = null;

    // board, not world positions
    int boardX, boardY;

    public void OnMouseUp()
    {
        controller = GameObject.FindGameObjectWithTag("GameController");
        Controller script = controller.GetComponent<Controller>();

        Piece refPiece = reference.GetComponent<Piece>();
        script.SetPositionEmpty(refPiece.xBoard, refPiece.yBoard);

        refPiece.xBoard = boardX;
        refPiece.yBoard = boardY;
        refPiece.SetCoords();

        script.SetPosition(reference);

        refPiece.DestroyMovePlates();
        script.isWhiteTurn = !script.isWhiteTurn;
    }

    public void SetCoords(int x, int y)
    {
        boardX = x;
        boardY = y;
    }

    public void SetReference(GameObject obj)
    {
        reference = obj;
    }

    public GameObject GetReference() { return reference; }
}
