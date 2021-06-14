using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System;
using System.Diagnostics;

using Random = System.Random;
using Debug = UnityEngine.Debug;

public class AI
{
    int depth;
    private Random random = new Random();

    private Stopwatch stopwatch = new Stopwatch();
    private int timeLimit = 30 * 1000;

    private TranspositionTable transpositionTable;

    private const byte EXACT = 0;
    private const byte LOWERBOUND = 1;
    private const byte UPPERBOUND = 2;

    public AI(int difficulty, TranspositionTable ts)
    {
        switch (difficulty)
        {
            case 1:
                depth = 2;
                break;
            case 2:
                depth = 3;
                break;
            case 3:
                depth = 4;
                break;
            default:
                depth = 2;
                break;
        }
        transpositionTable = ts;
    }

    public Move CalculateMove(BoardSimple board)
    {
        var locker = new object();

        BoardSimple[] nextStates = board.NextGameStates.ToArray();
        if (nextStates.Length == 0)
            return null;

        var bestResult = float.NegativeInfinity;
        Move bestMove = nextStates[0].LastMove;

        stopwatch.Start();

        /*
        for (int lookAhead = 3; lookAhead <= 6; lookAhead++)
        {
            Parallel.For(0, nextStates.Length, (i) =>
            {
                var result = AlphaBeta(nextStates[i], lookAhead, float.NegativeInfinity, float.PositiveInfinity, board.CurrentPlayer);

                lock (locker)
                {
                    if (result >= bestResult && stopwatch.ElapsedMilliseconds < timeLimit)
                    {
                        bestResult = result;
                        bestMove = nextStates[i].LastMove;
                    }
                }
            });
        }
        */
        Parallel.For(0, nextStates.Length, (i) =>
        {
            var result = AlphaBeta(nextStates[i], 2, int.MinValue, int.MaxValue, board.CurrentPlayer);


            lock (locker)
            {
                if (result >= bestResult)
                {
                    bestResult = result;
                    bestMove = nextStates[i].LastMove;
                }
            }
        });

        Debug.Log("Timer (ms): " + stopwatch.ElapsedMilliseconds);//
        return bestMove;
    }

    private int AlphaBeta(BoardSimple board, int depth, int alpha, int beta, int maximizingPlayer)
    {
        if (depth == 0 || board.IsGameOver || stopwatch.ElapsedMilliseconds >= timeLimit)
        {
            return CalculateRating(board, maximizingPlayer);
        }
        int alphaOrig = alpha;

        int val = 0;
        byte flag = 0;
        bool ts_gotten_key;
        lock (transpositionTable)
            ts_gotten_key = transpositionTable.get(board.getKey(), ref val, ref flag);
        if (ts_gotten_key)
        {
            if (flag == EXACT)
                return val;
            else if (flag == LOWERBOUND)
            {
                alpha = Math.Max(alpha, val);
            }
            else if (flag == UPPERBOUND)
                beta = Math.Min(beta, val);
        }
        if (alpha >= beta)
            return val;


        var value = int.MinValue;
        foreach (var child in board.NextGameStates)
        {
            value = Math.Max(value, AlphaBeta(child, depth - 1, -beta, -alpha, maximizingPlayer));

            alpha = Math.Max(alpha, value);
            if (alpha >= beta)
                break;
        }


        if (value <= alphaOrig)
            flag = UPPERBOUND;
        else if (value >= beta)
            flag = LOWERBOUND;
        else
            flag = EXACT;
        lock (transpositionTable)
            transpositionTable.put(board.getKey(), (byte)value, flag);
        return value;
    }

    /*
     private int AlphaBeta(BoardSimple board, int depth, int alpha, int beta, int maximizingPlayer)
    {
            //lock (locker2)//
                //count++;
        if (depth == 0 || board.IsGameOver || stopwatch.ElapsedMilliseconds >= timeLimit)
        {
            return CalculateRating(board, maximizingPlayer);
        }

        int val = 0;
        lock (transpositionTable)
        if (transpositionTable.get(board.getKey(), ref val))
        {
            return val;
        }

        if (board.CurrentPlayer == maximizingPlayer)
        {
            var value = int.MinValue;
            var maxValue = value;
            foreach (var child in board.NextGameStates)
            {
                value = AlphaBeta(child, depth - 1, alpha, beta, maximizingPlayer);
                maxValue = Math.Max(value, maxValue);

                alpha = Math.Max(alpha, value);
                if (alpha >= beta)
                    break;
            }
            //lock(transpositionTable)
                transpositionTable.put(board.getKey(), (byte)value);
            return value;
        }
        else
        {
            var value = int.MaxValue;
            var minValue = value;
            foreach (var child in board.NextGameStates)
            {
                value = AlphaBeta(child, depth - 1, alpha, beta, maximizingPlayer);
                minValue = Math.Min(value, minValue);

                beta = Math.Min(beta, value);
                if (alpha >= beta)
                    break;
            }
            //lock (transpositionTable)
                transpositionTable.put(board.getKey(), (byte)value);
            return value;
        }
    }
     */

    private int RandomInt()
    {
        lock (random)
        {
            return random.Next();
        }
    }

    private int CalculateRating(BoardSimple board, int player)
    {
        int otherPlayer = player == 2 ? 1 : 2;

        int p1 = player-1;
        int p2 = otherPlayer - 1;
        int playerStones = board.AvailableStones[p1] + board.StonesOnBoard[p1];
        int otherPlayerStones = board.AvailableStones[p2] + board.StonesOnBoard[p2];
        if(board.CurrentPlayer == player)
            return (playerStones - otherPlayerStones);
        else
            return -(playerStones - otherPlayerStones);
    }

    /*
     private int CalculateRating(BoardSimple board, int player)
    {
        int otherPlayer = player == 2 ? 1 : 2;

        int p1 = player-1;
        int p2 = otherPlayer - 1;
        int playerStones = board.AvailableStones[p1] + board.StonesOnBoard[p1];
        int otherPlayerStones = board.AvailableStones[p2] + board.StonesOnBoard[p2];
        return playerStones - otherPlayerStones;
    }
    */
}
