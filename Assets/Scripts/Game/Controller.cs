using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
    public GameObject piece;

    private GameObject[,] positions = new GameObject[7, 7];
    private bool[,] positionsMask = new bool[7, 7]
    {
        {true, false, false, true, false, false, true },
        {false, true, false, true, false, true, false },
        {false, false, true, true, true, false, false },
        {true, true, true, false, true, true, true },
        {false, false, true, true, true, false, false },
        {false, true, false, true, false, true, false},
        {true, false, false, true, false, false, true }
    };

    public enum GameState
    {
        placing,
        moving,
        flying
    }
    public GameState gameState
    { get; private set; }

    public bool isWhiteTurn
    { get; set; }

    private int piecesPlaced = 0;
    private bool gameOver = false;

    // Start is called before the first frame update
    void Start()
    {
        gameState = GameState.placing;
        isWhiteTurn = true;

        /*
        //TESTING ONLY
        GameObject obj = CreatePiece(true, 0, 0);
        SetPosition(obj);
        */

        CtrlFirstStage firstStage = GetComponent<CtrlFirstStage>();
        while (piecesPlaced < 18)
        {
             firstStage.CreateMovePlates(isWhiteTurn, positions, positionsMask);
             isWhiteTurn = !isWhiteTurn;
             piecesPlaced++;
        }
        gameState = GameState.moving;
    }

    public GameObject CreatePiece(bool isWhite, int x, int y)
    {
        GameObject obj = Instantiate(piece, new Vector3(0f, 0f, -1f), Quaternion.identity);
        Piece p = obj.GetComponent<Piece>();
        p.isWhite = isWhite;
        p.xBoard = x;
        p.yBoard = y;

        p.Activate();
        return obj;
    }

    public void SetPosition(GameObject obj)
    {
        Piece p = obj.GetComponent<Piece>();

        positions[p.xBoard, p.yBoard] = obj;
    }
    public void SetPositionEmpty(int x, int y)
    {
        try
        {
            positions[x, y] = null;
        }
        catch(System.IndexOutOfRangeException e) { }
    }
    public GameObject GetPosition(int x, int y)
    {
        return positions[x, y];
    }
    public bool PositionOnBoard(int x, int y)
    {
        if (x >= 0 && y >= 0 && x < 7 && y < 7)
            if (positionsMask[x, y])
                return true;
        return false;
    }

}
