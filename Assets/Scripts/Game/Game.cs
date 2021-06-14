using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Game
{
    private GameObject[,] positions = new GameObject[7, 7];
    private static bool[,] positionsMask { get; } = new bool[7, 7]
    {
        {true, false, false, true, false, false, true },
        {false, true, false, true, false, true, false },
        {false, false, true, true, true, false, false },
        {true, true, true, false, true, true, true },
        {false, false, true, true, true, false, false },
        {false, true, false, true, false, true, false},
        {true, false, false, true, false, false, true }
    };
    public GameObject pieceReference { get; set; } = null;

    public enum GameState
    {
        placing,
        moving
    }
    public GameState gameState
    { get; set; }

    public bool isWhiteTurn
    { get; set; }

    public int whitePiecesPlaced { get; set; } = 0;
    public int blackPiecesPlaced { get; set; } = 0;

    public bool IsInMill(GameObject piece)
    {
        if (!piece) // if null
            return false;

        var pieceScript = piece.GetComponent<Piece>();

        int dx = 1;
        int dy = 1;
        int xBoard = pieceScript.xBoard;
        int yBoard = pieceScript.yBoard;


        // outer ring
        if (xBoard == 0 || xBoard == 6)
            dy = 3;
        if (yBoard == 0 || yBoard == 6)
            dx = 3;

        // middle ring
        if (xBoard == 1 || xBoard == 5)
            dy = 2;
        if (yBoard == 1 || yBoard == 5)
            dx = 2;

        // checking neighbours
        GameObject n1, n2;
        // horizontal
        if (PositionOnBoard(xBoard - dx, yBoard))
            n1 = GetPosition(xBoard - dx, yBoard);
        else
            n1 = GetPosition(xBoard + 2 * dx, yBoard);

        if (PositionOnBoard(xBoard + dx, yBoard))
            n2 = GetPosition(xBoard + dx, yBoard);
        else
            n2 = GetPosition(xBoard - 2 * dx, yBoard);

        if (n1 && n2)
        {
            if (n1.GetComponent<Piece>().isWhite == pieceScript.isWhite    // checking colours
                    && n2.GetComponent<Piece>().isWhite == pieceScript.isWhite)
            {
                // horizontal = true
                return true;
            }
        }

        // vertically
        if (PositionOnBoard(xBoard, yBoard - dy))
            n1 = GetPosition(xBoard, yBoard - dy);
        else
            n1 = GetPosition(xBoard, yBoard + 2 * dy);

        if (PositionOnBoard(xBoard, yBoard + dy))
            n2 = GetPosition(xBoard, yBoard + dy);
        else
            n2 = GetPosition(xBoard, yBoard - 2 * dy);

        if (n1 && n2)
        {
            if (n1.GetComponent<Piece>().isWhite == pieceScript.isWhite    // checking colours
               && n2.GetComponent<Piece>().isWhite == pieceScript.isWhite)
            {
                // vertical = true
                return true;
            }
        }
        return false;
    }

    public int piecesCount(bool isWhite)
    {
        int counter = 0;
        GameObject piece;
        for (int i = 0; i < 7; i++)
        {
            for (int j = 0; j < 7; j++)
            {
                piece = GetPosition(i, j);
                if (piece && piece.GetComponent<Piece>().isWhite == isWhite)
                    counter++;
            }
        }
        return counter;
    }
    public int piecesCount()
    {
        int counter = 0;
        GameObject piece;
        for (int i = 0; i < 7; i++)
        {
            for (int j = 0; j < 7; j++)
            {
                piece = GetPosition(i, j);
                if (piece != null)
                    counter++;
            }
        }
        return counter;
    }

    public bool IsWinner()
    {
        if (piecesCount(isWhiteTurn) <= 2 && gameState == GameState.moving)
        {
            return true;
        }
        return false;
    }

    public void SetPosition(GameObject obj)
    {
        Piece p = obj.GetComponent<Piece>();

        positions[p.xBoard, p.yBoard] = obj;
    }

    public void SetPositionEmpty(int x, int y)
    {
        if (x >= 0 && y >= 0 && x < 7 && y < 7)
            if (positionsMask[x, y])
                positions[x, y] = null;
    }

    public bool PositionOnBoard(int x, int y)
    {
        if (x >= 0 && y >= 0 && x < 7 && y < 7)
            if (positionsMask[x, y])
                return true;
        return false;
    }

    public GameObject GetPosition(int x, int y)
    {
        return positions[x, y];
    }

    public BoardSimple Simplyfy()
    {
        var pos = new int[7, 7];
        for (int i = 0; i < 7; i++)
        {
            for (int j = 0; j < 7; j++)
            {
                if (positionsMask[i, j] == false)
                    pos[i, j] = -1;
                else if (positions[i, j] == null)
                    pos[i, j] = 0;
                else if (positions[i, j].GetComponent<Piece>().isWhite) // white
                    pos[i, j] = 2;
                else if (!positions[i, j].GetComponent<Piece>().isWhite) // black
                    pos[i, j] = 1;
            }
        }

        BoardSimple bs = new BoardSimple(pos, whitePiecesPlaced, blackPiecesPlaced, isWhiteTurn); //
        
        return bs;
    }

    public void ApplyMove(Move move)
    {
        // find piece
        Piece ps;
        GameObject p;
        if (move.fromX == -1 || move.fromY == -1)
        {
            p = pieceReference;
            pieceReference = null;
        }
        else
        {
            p = positions[move.fromX, move.fromY];
        }
        ps = p.GetComponent<Piece>();

        ps.xBoard = move.toX;
        ps.yBoard = move.toY;

        ps.SetCoords();

        SetPosition(p);
        SetPositionEmpty(move.fromX, move.fromY);

        // Destroy
        if(move.removeX != -1 && move.removeY != -1)
        {
            Object.Destroy(positions[move.removeX, move.removeY]);
            SetPositionEmpty(move.removeX, move.removeY);
        }
    }
}
