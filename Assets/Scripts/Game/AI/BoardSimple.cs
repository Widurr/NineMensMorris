using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class BoardSimple
{
    private int[,] positions = null; // -1 - doesn't belong; 0 - empty; 1,2 - players
    public int CurrentPlayer { get; private set; } = 1;
    public int OtherPlayer => CurrentPlayer == 1 ? 2 : 1;

    public Move[] AvailableMoves { get; private set; }
    public int[] StonesOnBoard { get; } = new int[2] { 0, 0 };
    public int[] AvailableStones { get; } = new int[2] { 9, 9 };
    public Move LastMove { get; set; } = null;

    public bool IsGameOver => AvailableMoves.Length == 0 || (AvailableStones.Max() == 0 && StonesOnBoard.Min() < 3);

    public BoardSimple(int [,] pos, int whitePiecesPlaced, int blackPiecesPlaced, bool isWhiteTurn)
    {
        positions = pos;

        AvailableStones[0] = 9 - blackPiecesPlaced;
        AvailableStones[1] = 9 - whitePiecesPlaced;

        if(isWhiteTurn)
        {
            CurrentPlayer = 2;
        }

        for (int i = 0; i < 7; i++)
        {
            for (int j = 0; j < 7; j++)
            {
                if (positions[i, j] == 1)
                    StonesOnBoard[0]++;
                else if (positions[i, j] == 2)
                    StonesOnBoard[1]++;
            }
        }

        UpdateMoves();
    }
    private BoardSimple() { }

    /*
    private Move[] CalculateAvailableMoves()
    {
        LinkedList<Move> list = new LinkedList<Move>();
        for (int toX = 0; toX < 7; toX++)
        {
            for (int toY = 0; toY < 7; toY++)
            {
                if (positions[toX, toY] != 0)
                    continue;

                for (int fromX = 0; fromX < 7; fromX++)
                {
                    for (int fromY = -1; fromY < 7; fromY++)
                    {
                        if(fromY == -1)
                        {
                            if (AvailableStones[CurrentPlayer - 1] == 0) // gameState == moving
                                continue;
                        }
                        else
                        {
                            if (AvailableStones[CurrentPlayer - 1] != 0) // gameState == placing
                                continue;

                            if (positions[fromX, fromY] != CurrentPlayer)
                            {
                                continue;
                            }

                            if ((StonesOnBoard[CurrentPlayer - 1] + AvailableStones[CurrentPlayer - 1]) > 3 && !IsAdjacent(fromX, fromY, toX, toY))
                                continue;
                        }

                        if(WillHaveMill(fromX, fromY, toX, toY))
                        {
                            bool canRemoveStone = false;

                            for (int removeX = 0; removeX < 7; removeX++)
                            {
                                for (int removeY = 0; removeY < 7; removeY++)
                                {
                                    if (positions[removeX, removeY] != OtherPlayer)
                                        continue;

                                    if (isInMill(removeX, removeY))
                                        continue;

                                    canRemoveStone = true;
                                    list.AddFirst(new Move { toX = toX, toY = toY, fromX = fromX, fromY = fromY, removeX = removeX, removeY = removeY });
                                }
                            }

                            if (!canRemoveStone)
                            {
                                list.AddLast(new Move { toX = toX, toY = toY, fromX = fromX, fromY = fromY, removeX = -1, removeY = -1 });
                            }
                        }
                        else
                        {
                            //yield return new Move { toX = toX, toY = toY, fromX = fromX, fromY = fromY, removeX = -1, removeY = -1 };
                            list.AddLast(new Move { toX = toX, toY = toY, fromX = fromX, fromY = fromY, removeX = -1, removeY = -1 });
                        }
                    }
                }
            }
        }
        return list.ToArray();
    }
    */

    
     private IEnumerable<Move> CalculateAvailableMoves()
    {
        for (int toX = 0; toX < 7; toX++)
        {
            for (int toY = 0; toY < 7; toY++)
            {
                if (positions[toX, toY] != 0)
                    continue;

                for (int fromX = 0; fromX < 7; fromX++)
                {
                    for (int fromY = -1; fromY < 7; fromY++)
                    {
                        if(fromY == -1)
                        {
                            if (AvailableStones[CurrentPlayer - 1] == 0) // gameState == moving
                                continue;
                        }
                        else
                        {
                            if (AvailableStones[CurrentPlayer - 1] != 0) // gameState == placing
                                continue;

                            if (positions[fromX, fromY] != CurrentPlayer)
                            {
                                continue;
                            }

                            if ((StonesOnBoard[CurrentPlayer - 1] + AvailableStones[CurrentPlayer - 1]) > 3 && !IsAdjacent(fromX, fromY, toX, toY))
                                continue;
                        }

                        if(WillHaveMill(fromX, fromY, toX, toY))
                        {
                            bool canRemoveStone = false;

                            for (int removeX = 0; removeX < 7; removeX++)
                            {
                                for (int removeY = 0; removeY < 7; removeY++)
                                {
                                    if (positions[removeX, removeY] != OtherPlayer)
                                        continue;

                                    if (isInMill(removeX, removeY))
                                        continue;

                                    canRemoveStone = true;
                                    yield return new Move { toX = toX, toY = toY, fromX = fromX, fromY = fromY, removeX = removeX, removeY = removeY };
                                }
                            }

                            if (!canRemoveStone)
                            {
                                yield return new Move { toX = toX, toY = toY, fromX = fromX, fromY = fromY, removeX = -1, removeY = -1 };
                            }
                        }
                        else
                        {
                            yield return new Move { toX = toX, toY = toY, fromX = fromX, fromY = fromY, removeX = -1, removeY = -1 };
                        }
                    }
                }
            }
        }
    }
    

    private void UpdateMoves()
    {
        AvailableMoves = CalculateAvailableMoves().ToArray();
    }

    public bool WillHaveMill(int fromX, int fromY, int toX, int toY)
    {
        System.Diagnostics.Debug.Assert(fromX == -1 || fromY == -1 || positions[fromX, fromY] == CurrentPlayer);
        System.Diagnostics.Debug.Assert(positions[toX, toY] == 0);

        if (fromX != -1 && fromY != -1)
            positions[fromX, fromY] = 0;
        positions[toX, toY] = CurrentPlayer;

        var result = isInMill(toX, toY);

        if (fromX != -1 && fromY != -1)
            positions[fromX, fromY] = CurrentPlayer;
        positions[toX, toY] = 0;

        return result;
    }

    public IEnumerable<BoardSimple> NextGameStates
    {
        get
        {
            foreach (var move in AvailableMoves)
            {
                var newGame = Clone();
                newGame.Move(move);
                yield return newGame;
            }
        }
    }

    private int deltaX(int y)
    {
        int dx = 1;

        if (y == 0 || y == 6) // outer ring
            dx = 3;
        else if (y == 1 || y == 5) // middle ring
            dx = 2;

        return dx;
    }
    private int deltaY(int x)
    {
        int dy = 1;

        if (x == 0 || x == 6) // outer ring
            dy = 3;
        else if (x == 1 || x == 5) // middle ring
            dy = 2;

        return dy;
    }

    public bool isInMill(int x, int y)
    {
        if (!PositionOnBoard(x, y))
            return false;

        int p = positions[x, y];

        int dx = deltaX(y);
        int dy = deltaY(x);

        int n1, n2;
        // horizontal
        if (PositionOnBoard(x - dx, y))
            n1 = positions[x - dx, y];
        else
            n1 = positions[x + 2 * dx, y];

        if (PositionOnBoard(x + dx, y))
            n2 = positions[x + dx, y];
        else
            n2 = positions[x - 2 * dx, y];

        if (n1 == p && n2 == p)
        {
            return true;
        }

        // vertically
        if (PositionOnBoard(x, y - dy))
            n1 = positions[x, y - dy];
        else
            n1 = positions[x, y + 2 * dy];

        if (PositionOnBoard(x, y + dy))
            n2 = positions[x, y + dy];
        else
            n2 = positions[x, y - 2 * dy];

        if (n1 == p && n2 == p)
        {
            return true;
        }
        return false;
    }

    private bool PositionOnBoard(int x, int y)
    {
        if (x < 0 || x > 6 || y < 0 || y > 6)
            return false;
        if (positions[x, y] == -1)
            return false;
        return true;
    }

    public bool IsValidFromTo(Move move)
    {
        if (move.fromX == move.toX && move.fromY == move.toY)
            return false;
        if (move.toX < 0 || move.toX >= 7)
            return false;
        if (move.toY < 0 || move.toY >= 7)
            return false;
        if (move.fromX < -1 || move.fromX >= 7)
            return false;
        if (move.fromY < -1 || move.fromY >= 7)
            return false;

        if (positions[move.toX, move.toY] != 0)
            return false;
        if ((move.fromX == -1 || move.fromY == -1) && AvailableStones[CurrentPlayer - 1] == 0)
            return false;
        if ((move.fromX != -1 && move.fromY != -1) && AvailableStones[CurrentPlayer - 1] != 0)
            return false;
        if ((move.fromX != -1 && move.fromY != -1) && positions[move.fromX, move.fromY] != CurrentPlayer)
            return false;

        if ((move.fromX != -1 && move.fromY != -1)
            && StonesOnBoard[CurrentPlayer - 1] > 3
            && !IsAdjacent(move.fromX, move.fromY, move.toX, move.toY))
            return false;

        return true;
    }

    private bool IsAdjacent(int x1, int y1, int x2, int y2)
    {
        return ((x2 - x1 == deltaX(y1) || x2 - x1 == -deltaX(y1))
            && (y2 - y1 == deltaY(x1) || y2 - y1 == -deltaY(x1)));
    }

    public bool IsValid(Move move)
    {
        if (!IsValidFromTo(move))
            return false;

        if (move.removeX < -1 || move.removeX > 7)
            return false;
        if (move.removeY < -1 || move.removeY > 7)
            return false;

        if ((move.removeX != -1 && move.removeY != -1) && (positions[move.removeX, move.removeY] != OtherPlayer))
            return false;

        return true;
    }

    public void Move(Move move)
    {
        /*
        if (!IsValid(move))
        {
            Debug.Log("" + move.fromX + move.fromY + move.toX + move.toY + move.removeX + move.removeY);
            Debug.Log(AvailableStones[CurrentPlayer - 1]);
            throw new InvalidOperationException(nameof(move));
        }
        */

        positions[move.toX, move.toY] = CurrentPlayer;

        if (move.fromX != -1 && move.fromY != -1)
        {
            positions[move.fromX, move.fromY] = 0;
        }

        if (move.removeX != -1 && move.removeY != -1)
        {
            StonesOnBoard[positions[move.removeX, move.removeY] - 1]--;
            positions[move.removeX, move.removeY] = 0;
        }

        LastMove = move;
        CurrentPlayer = OtherPlayer;
        UpdateMoves();
    }

    public BoardSimple Clone()
    {
        BoardSimple clone = new BoardSimple();

        clone.CurrentPlayer = CurrentPlayer;
        //clone.LastMove = LastMove;
        clone.positions = positions.Clone() as int[,];
        Array.Copy(AvailableStones, clone.AvailableStones, AvailableStones.Length);
        Array.Copy(StonesOnBoard, clone.StonesOnBoard, StonesOnBoard.Length);

        clone.AvailableMoves = new Move[AvailableMoves.Length];
        Array.Copy(AvailableMoves, clone.AvailableMoves, AvailableMoves.Length);

        return clone;
    }

    public ulong getKey()
    {
        ulong key = (uint)AvailableStones[0];
        key <<= 4;
        key |= (uint)AvailableStones[1];
        for (int i = 0; i < 7; i++)
        {
            for (int j = 0; j < 7; j++)
            {
                if(positions[i,j] != -1)
                {
                    key <<= 2;
                    key |= (uint)positions[i, j];
                }
            }
        }

        return key;
    }
}
