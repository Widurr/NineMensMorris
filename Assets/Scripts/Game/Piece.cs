using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour
{
    public bool isWhite
    { get; set; }
    
    public GameObject gameController;
    public GameObject movePlate;

    public int xBoard
    { get; set; }
    public int yBoard
    { get; set; }

    public void Activate()
    {
        gameController = GameObject.FindGameObjectWithTag("GameController");
        SetCoords();

        if (isWhite)
            this.GetComponent<SpriteRenderer>().color = new Vector4(0.9f, 0.9f, 0.9f, 1f);
        else
            this.GetComponent<SpriteRenderer>().color = new Vector4(0.2f, 0.2f, 0.2f, 1f);
    }

    public void SetCoords()
    {
        float x, y;

        x = xBoard - 3;
        y = 3 - yBoard;

        this.transform.position = new Vector3(x, y, -1.0f);
    }

    internal void DestroyMovePlates()
    {
        GameObject[] movePlates = GameObject.FindGameObjectsWithTag("MovePlate");
        for (int i = 0; i < movePlates.Length; i++)
        {
            Destroy(movePlates[i]);
        }
    }

    void OnMouseUp()
    {
        Controller ctrlScript = gameController.GetComponent<Controller>();
        if (ctrlScript.getGame().gameState == Game.GameState.moving)
        {
            // checking if there's no deleting happening
            var mp = GameObject.FindGameObjectWithTag("MovePlate");
            if (!mp || !mp.GetComponent<MovePlate>().isKillerPlate)
            {
                DestroyMovePlates();
                if (ctrlScript.isWhiteTurn == isWhite)
                {
                    InitiateMovePlates();
                }
            }
        }
    }

    private void InitiateMovePlates()
    {
        var ctrlScript = gameController.GetComponent<Controller>();
        if (ctrlScript.piecesCount(isWhite) == 3)
        {
            for (int i = 0; i < 7; i++)
            {
                for (int j = 0; j < 7; j++)
                {
                    if (ctrlScript.PositionOnBoard(i, j))
                        MovePlateSpawn(i, j);
                }
            }
        }
        else
        {
            int xi = 1;
            int yi = 1;

            // outer ring
            if (xBoard == 0 || xBoard == 6)
                yi = 3;
            if (yBoard == 0 || yBoard == 6)
                xi = 3;

            // middle ring
            if (xBoard == 1 || xBoard == 5)
                yi = 2;
            if (yBoard == 1 || yBoard == 5)
                xi = 2;

            MovePlateSpawn(xBoard - xi, yBoard);
            MovePlateSpawn(xBoard + xi, yBoard);
            MovePlateSpawn(xBoard, yBoard - yi);
            MovePlateSpawn(xBoard, yBoard + yi);
        }
    }

    private void MovePlateSpawn(int x, int y)
    {
        Controller controller = gameController.GetComponent<Controller>();
        if(controller.PositionOnBoard(x,y))
        {
            GameObject p = controller.GetPosition(x, y);

            if(p == null)
            {
                float worldX = x - 3;
                float worldY = 3 - y;

                GameObject mp = Instantiate(movePlate, new Vector3(worldX, worldY, -3.0f), Quaternion.identity);
                MovePlate mpScript = mp.GetComponent<MovePlate>();
                mpScript.SetReference(gameObject);
                mpScript.SetCoords(x, y);

            }
        }
    }

    public bool IsInMill()
    {
        return gameController.GetComponent<Controller>().IsInMill(gameObject);
    }
}
